using System.Text.RegularExpressions;
using EmpLog.Domain.Common;
using EmpLog.Domain.Enums;
using EmpLog.Domain.Exceptions;
using EmpLog.Domain.ValueObjects;

namespace EmpLog.Domain.Entities;

public sealed partial class Motorista : BaseEntity, IAggregateRoot
{
    public const int TamanhoMinimoNome = 5;
    public const int TamanhoMaximoNome = 150;

    private static readonly Regex PadraoNome = PadraoNomeCompleto();

    public string NomeCompleto { get; private set; } = null!;

    public Cpf Cpf { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    public Telefone Telefone { get; private set; } = null!;

    public Cnh Cnh { get; private set; } = null!;

    public StatusMotorista Status { get; private set; }

    private Motorista()
    {
    }

    public static Motorista Cadastrar(
        string? nomeCompleto,
        Cpf cpf,
        Email email,
        Telefone telefone,
        Cnh cnh,
        TimeProvider relogio)
    {
        if (!TryNormalizarNome(nomeCompleto, out var nome, out var erro))
        {
            throw new DomainException(erro);
        }

        return new Motorista
        {
            Id = Guid.NewGuid(),
            NomeCompleto = nome,
            Cpf = cpf,
            Email = email,
            Telefone = telefone,
            Cnh = cnh,
            Status = StatusMotorista.Ativo,
            CriadoEmUtc = relogio.GetUtcNow().UtcDateTime
        };
    }

    public static bool TryNormalizarNome(string? nomeCompleto, out string nome, out string erro)
    {
        nome = string.Empty;
        if (string.IsNullOrWhiteSpace(nomeCompleto))
        {
            erro = "Nome completo é obrigatório.";
            return false;
        }

        nome = string.Join(' ', nomeCompleto.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        if (nome.Length is < TamanhoMinimoNome or > TamanhoMaximoNome)
        {
            erro = "Nome completo deve ter entre 5 e 150 caracteres.";
            nome = string.Empty;
            return false;
        }

        if (!PadraoNome.IsMatch(nome))
        {
            erro = "Informe nome e sobrenome, usando apenas letras.";
            nome = string.Empty;
            return false;
        }

        erro = string.Empty;
        return true;
    }

    [GeneratedRegex(@"^[\p{L}][\p{L}\p{M}'\.\-]*(?: [\p{L}][\p{L}\p{M}'\.\-]*)+$", RegexOptions.CultureInvariant)]
    private static partial Regex PadraoNomeCompleto();
}
