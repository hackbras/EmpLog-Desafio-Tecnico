using EmpLog.Application.Motoristas.Commands.CadastrarMotorista;
using EmpLog.Domain.Common;
using EmpLog.Domain.Entities;
using EmpLog.Domain.Enums;
using EmpLog.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace EmpLog.Tests.Application.Motoristas;

public class CadastrarMotoristaCommandHandlerTests
{
    private static readonly TimeProvider Relogio = new FixedTimeProvider(new DateTimeOffset(2026, 10, 12, 15, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task Cadastra_quando_os_dados_sao_validos()
    {
        var repositorio = Substitute.For<IMotoristaRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        repositorio.ExistePorCpfAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        repositorio.ExistePorCnhAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        unitOfWork.SalvarAlteracoesAsync(Arg.Any<CancellationToken>()).Returns(ResultadoSalvamento.Salvo());
        var handler = CriarHandler(repositorio, unitOfWork);

        var resultado = await handler.Handle(ComandoValido(), CancellationToken.None);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.NomeCompleto.Should().Be("Ana Silva");
        resultado.Valor.Cpf.Should().Be("52998224725");
        resultado.Valor.Status.Should().Be(nameof(StatusMotorista.Ativo));
        resultado.Valor.Cnh.Numero.Should().Be("12345678900");
        repositorio.Received(1).Adicionar(Arg.Any<Motorista>());
        await unitOfWork.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Recusa_cpf_ja_cadastrado()
    {
        var repositorio = Substitute.For<IMotoristaRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        repositorio.ExistePorCpfAsync("52998224725", Arg.Any<CancellationToken>()).Returns(true);
        var handler = CriarHandler(repositorio, unitOfWork);

        var resultado = await handler.Handle(ComandoValido(), CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Conflito.Should().BeTrue();
        resultado.Erros.Should().Contain(erro => erro.Campo == "cpf");
        repositorio.DidNotReceive().Adicionar(Arg.Any<Motorista>());
    }

    [Fact]
    public async Task Recusa_cnh_ja_cadastrada()
    {
        var repositorio = Substitute.For<IMotoristaRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        repositorio.ExistePorCpfAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        repositorio.ExistePorCnhAsync("12345678900", Arg.Any<CancellationToken>()).Returns(true);
        var handler = CriarHandler(repositorio, unitOfWork);

        var resultado = await handler.Handle(ComandoValido(), CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Conflito.Should().BeTrue();
        resultado.Erros.Should().Contain(erro => erro.Campo == "cnh.numero");
        repositorio.DidNotReceive().Adicionar(Arg.Any<Motorista>());
    }

    [Fact]
    public async Task Recusa_cnh_vencida()
    {
        var repositorio = Substitute.For<IMotoristaRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var handler = CriarHandler(repositorio, unitOfWork);
        var comando = ComandoValido() with
        {
            Cnh = new CnhCadastro
            {
                Numero = "12345678900",
                Categoria = "B",
                Validade = new DateOnly(2026, 10, 11)
            }
        };

        var resultado = await handler.Handle(comando, CancellationToken.None);

        resultado.Sucesso.Should().BeFalse();
        resultado.Conflito.Should().BeFalse();
        resultado.Erros.Should().Contain(erro => erro.Campo == "cnh.validade");
        await repositorio.DidNotReceive().ExistePorCpfAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static CadastrarMotoristaHandler CriarHandler(IMotoristaRepository repositorio, IUnitOfWork unitOfWork) =>
        new(repositorio, unitOfWork, new CadastrarMotoristaCommandValidator(), Relogio);

    private static CadastrarMotoristaCommand ComandoValido() =>
        new()
        {
            NomeCompleto = "Ana Silva",
            Cpf = "52998224725",
            Email = "ana.silva@email.com",
            Telefone = "11988887777",
            Cnh = new CnhCadastro
            {
                Numero = "12345678900",
                Categoria = "B",
                Validade = new DateOnly(2030, 6, 15)
            }
        };
}
