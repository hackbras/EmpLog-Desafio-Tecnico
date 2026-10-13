using System.Text.RegularExpressions;
using EmpLog.Domain.Common;
using EmpLog.Domain.Exceptions;

namespace EmpLog.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    public const int TamanhoMaximo = 254;

    private static readonly Regex Padrao = PadraoEmail();

    public string Valor { get; }

    private Email(string valor) => Valor = valor;

    public static bool TryCreate(string? texto, out Email? email, out string erro)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(texto))
        {
            erro = "E-mail é obrigatório.";
            return false;
        }

        var normalizado = texto.Trim().ToLowerInvariant();
        if (normalizado.Length > TamanhoMaximo || !Padrao.IsMatch(normalizado))
        {
            erro = "E-mail inválido.";
            return false;
        }

        email = new Email(normalizado);
        erro = string.Empty;
        return true;
    }

    public static Email Create(string texto)
    {
        if (!TryCreate(texto, out var email, out var erro))
        {
            throw new DomainException(erro);
        }

        return email!;
    }

    public override string ToString() => Valor;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex PadraoEmail();
}
