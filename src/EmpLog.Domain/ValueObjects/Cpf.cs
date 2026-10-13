using EmpLog.Domain.Common;
using EmpLog.Domain.Exceptions;

namespace EmpLog.Domain.ValueObjects;

public sealed class Cpf : ValueObject
{
    public string Valor { get; }

    private Cpf(string valor) => Valor = valor;

    public static bool TryCreate(string? texto, out Cpf? cpf, out string erro)
    {
        cpf = null;
        var digitos = Digitos.Apenas(texto);

        if (digitos.Length == 0)
        {
            erro = "CPF é obrigatório.";
            return false;
        }

        if (!EhValido(digitos))
        {
            erro = "CPF inválido.";
            return false;
        }

        cpf = new Cpf(digitos);
        erro = string.Empty;
        return true;
    }

    public static Cpf Create(string texto)
    {
        if (!TryCreate(texto, out var cpf, out var erro))
        {
            throw new DomainException(erro);
        }

        return cpf!;
    }

    public override string ToString() => Valor;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    private static bool EhValido(string digitos)
    {
        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
        {
            return false;
        }

        return digitos[9] - '0' == DigitoVerificador(digitos, 9)
            && digitos[10] - '0' == DigitoVerificador(digitos, 10);
    }

    private static int DigitoVerificador(string digitos, int tamanho)
    {
        var soma = 0;
        for (var i = 0; i < tamanho; i++)
        {
            soma += (digitos[i] - '0') * (tamanho + 1 - i);
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
