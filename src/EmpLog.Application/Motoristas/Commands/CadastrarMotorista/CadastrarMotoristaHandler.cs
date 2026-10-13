using EmpLog.Domain.Common;
using EmpLog.Domain.Entities;
using EmpLog.Domain.Interfaces;
using EmpLog.Domain.ValueObjects;
using FluentValidation.Results;

namespace EmpLog.Application.Motoristas.Commands.CadastrarMotorista;

public sealed class CadastrarMotoristaHandler
{
    private readonly IMotoristaRepository _motoristas;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CadastrarMotoristaCommandValidator _validator;
    private readonly TimeProvider _relogio;

    public CadastrarMotoristaHandler(
        IMotoristaRepository motoristas,
        IUnitOfWork unitOfWork,
        CadastrarMotoristaCommandValidator validator,
        TimeProvider relogio)
    {
        _motoristas = motoristas;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _relogio = relogio;
    }

    public async Task<Result<CadastrarMotoristaResponse>> Handle(
        CadastrarMotoristaCommand comando,
        CancellationToken cancellationToken)
    {
        var validacao = await _validator.ValidateAsync(comando, cancellationToken);
        if (!validacao.IsValid)
        {
            return Result<CadastrarMotoristaResponse>.Falha(validacao.Errors.Select(ParaErro));
        }

        var erros = new List<ErroResultado>();

        if (!Motorista.TryNormalizarNome(comando.NomeCompleto, out _, out var erroNome))
        {
            erros.Add(new ErroResultado(Result.CodigoValidacao, "nomeCompleto", erroNome));
        }

        Cpf? cpf = null;
        if (!Cpf.TryCreate(comando.Cpf, out var cpfCriado, out var erroCpf))
        {
            erros.Add(new ErroResultado(Result.CodigoValidacao, "cpf", erroCpf));
        }
        else
        {
            cpf = cpfCriado;
        }

        Email? email = null;
        if (!Email.TryCreate(comando.Email, out var emailCriado, out var erroEmail))
        {
            erros.Add(new ErroResultado(Result.CodigoValidacao, "email", erroEmail));
        }
        else
        {
            email = emailCriado;
        }

        Telefone? telefone = null;
        if (!Telefone.TryCreate(comando.Telefone, out var telefoneCriado, out var erroTelefone))
        {
            erros.Add(new ErroResultado(Result.CodigoValidacao, "telefone", erroTelefone));
        }
        else
        {
            telefone = telefoneCriado;
        }

        Cnh? cnh = null;
        if (!Cnh.TryCreate(
                comando.Cnh!.Numero,
                comando.Cnh.Categoria,
                comando.Cnh.Validade,
                _relogio,
                out var cnhCriada,
                out var errosCnh))
        {
            erros.AddRange(errosCnh);
        }
        else
        {
            cnh = cnhCriada;
        }

        if (erros.Count > 0 || cpf is null || email is null || telefone is null || cnh is null)
        {
            return Result<CadastrarMotoristaResponse>.Falha(erros);
        }

        if (await _motoristas.ExistePorCpfAsync(cpf.Valor, cancellationToken))
        {
            return Result<CadastrarMotoristaResponse>.ConflitoDe("cpf", "Já existe um motorista com este CPF.");
        }

        if (await _motoristas.ExistePorCnhAsync(cnh.Numero, cancellationToken))
        {
            return Result<CadastrarMotoristaResponse>.ConflitoDe(
                "cnh.numero",
                "Já existe um motorista com este número de CNH.");
        }

        var motorista = Motorista.Cadastrar(comando.NomeCompleto, cpf, email, telefone, cnh, _relogio);
        _motoristas.Adicionar(motorista);

        var salvamento = await _unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        if (salvamento.Conflito)
        {
            return ConflitoDePersistencia(salvamento.Campo);
        }

        return Result<CadastrarMotoristaResponse>.Ok(CadastrarMotoristaResponse.From(motorista));
    }

    private static ErroResultado ParaErro(ValidationFailure falha) =>
        new(Result.CodigoValidacao, falha.PropertyName, falha.ErrorMessage);

    private static Result<CadastrarMotoristaResponse> ConflitoDePersistencia(string? campo) =>
        campo switch
        {
            "cnh" => Result<CadastrarMotoristaResponse>.ConflitoDe(
                "cnh.numero",
                "Já existe um motorista com este número de CNH."),
            _ => Result<CadastrarMotoristaResponse>.ConflitoDe(
                "cpf",
                "Já existe um motorista com este CPF.")
        };
}
