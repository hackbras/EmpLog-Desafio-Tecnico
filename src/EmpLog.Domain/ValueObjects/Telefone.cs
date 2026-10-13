using EmpLog.Domain.Common;
using EmpLog.Domain.Exceptions;

namespace EmpLog.Domain.ValueObjects;

public sealed class Telefone : ValueObject
{
    public string Valor { get; }

    private Telefone(string valor) => Valor = valor;

    public static bool TryCreate(string? texto, out Telefone? telefone, out string erro)
    {
        telefone = null;
        var digitos = Digitos.Apenas(texto);

        if (digitos.Length == 0)
        {
            erro = "Telefone é obrigatório.";
            return false;
        }

        if (digitos.StartsWith("55") && digitos.Length is 12 or 13)
        {
            digitos = digitos[2..];
        }

        if (!EhValido(digitos))
        {
            erro = "Telefone inválido. Informe DDD e número, com 10 ou 11 dígitos.";
            return false;
        }

        telefone = new Telefone(digitos);
        erro = string.Empty;
        return true;
    }

    public static Telefone Create(string texto)
    {
        if (!TryCreate(texto, out var telefone, out var erro))
        {
            throw new DomainException(erro);
        }

        return telefone!;
    }

    public override string ToString() => Valor;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    private static bool EhValido(string digitos)
    {
        if (digitos.Length is not (10 or 11))
        {
            return false;
        }

        if (!int.TryParse(digitos[..2], out var ddd) || ddd is < 11 or > 99)
        {
            return false;
        }

        var primeiro = digitos[2] - '0';
        return digitos.Length == 11
            ? primeiro == 9
            : primeiro is >= 2 and <= 5;
    }
}
