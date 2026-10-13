using EmpLog.Domain.Common;
using EmpLog.Domain.Enums;
using EmpLog.Domain.Exceptions;

namespace EmpLog.Domain.ValueObjects;

public sealed class Cnh : ValueObject
{
    public string Numero { get; private set; } = null!;

    public CategoriaCnh Categoria { get; private set; }

    public DateOnly Validade { get; private set; }

    private Cnh()
    {
    }

    private Cnh(string numero, CategoriaCnh categoria, DateOnly validade)
    {
        Numero = numero;
        Categoria = categoria;
        Validade = validade;
    }

    public static bool TryCreate(
        string? numero,
        string? categoria,
        DateOnly? validade,
        TimeProvider relogio,
        out Cnh? cnh,
        out IReadOnlyList<ErroResultado> erros)
    {
        var encontrados = new List<ErroResultado>();

        if (!TryNumero(numero, out var digitos, out var erroNumero))
        {
            encontrados.Add(new ErroResultado(Result.CodigoValidacao, "cnh.numero", erroNumero));
        }

        if (!TryCategoria(categoria, out var categoriaNormalizada, out var erroCategoria))
        {
            encontrados.Add(new ErroResultado(Result.CodigoValidacao, "cnh.categoria", erroCategoria));
        }

        if (validade is null)
        {
            encontrados.Add(new ErroResultado(Result.CodigoValidacao, "cnh.validade", "Validade da CNH é obrigatória."));
        }
        else if (!EhFutura(validade.Value, relogio))
        {
            encontrados.Add(new ErroResultado(
                Result.CodigoValidacao,
                "cnh.validade",
                "A validade da CNH deve ser posterior à data de hoje."));
        }

        if (encontrados.Count > 0 || categoriaNormalizada is null || validade is null)
        {
            cnh = null;
            erros = encontrados;
            return false;
        }

        cnh = new Cnh(digitos, categoriaNormalizada.Value, validade.Value);
        erros = [];
        return true;
    }

    public static Cnh Create(string numero, string categoria, DateOnly validade, TimeProvider relogio)
    {
        if (!TryCreate(numero, categoria, validade, relogio, out var cnh, out var erros))
        {
            throw new DomainException(erros[0].Mensagem);
        }

        return cnh!;
    }

    public override string ToString() => Numero;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Numero;
        yield return Categoria;
        yield return Validade;
    }

    private static bool TryNumero(string? texto, out string digitos, out string erro)
    {
        digitos = Digitos.Apenas(texto);
        if (digitos.Length == 0)
        {
            erro = "Número da CNH é obrigatório.";
            return false;
        }

        if (!NumeroEhValido(digitos))
        {
            erro = "Número da CNH inválido.";
            return false;
        }

        erro = string.Empty;
        return true;
    }

    private static bool TryCategoria(string? texto, out CategoriaCnh? categoria, out string erro)
    {
        categoria = null;
        if (string.IsNullOrWhiteSpace(texto))
        {
            erro = "Categoria da CNH é obrigatória.";
            return false;
        }

        var informado = texto.Trim();
        if (informado.All(char.IsDigit)
            || !Enum.TryParse<CategoriaCnh>(informado, ignoreCase: true, out var valor)
            || !Enum.IsDefined(valor))
        {
            erro = "Categoria de CNH inválida. Aceitas: A, B, C, D, E, AB, AC, AD e AE.";
            return false;
        }

        categoria = valor;
        erro = string.Empty;
        return true;
    }

    private static bool EhFutura(DateOnly validade, TimeProvider relogio)
    {
        var hoje = DateOnly.FromDateTime(relogio.GetUtcNow().UtcDateTime);
        return validade > hoje;
    }

    private static bool NumeroEhValido(string cnh)
    {
        if (cnh.Length != 11 || cnh.Distinct().Count() == 1)
        {
            return false;
        }

        var decremento = 0;
        var primeiraSoma = 0;
        for (var indice = 0; indice < 9; indice++)
        {
            primeiraSoma += (cnh[indice] - '0') * (9 - indice);
        }

        var primeiroDigito = primeiraSoma % 11;
        if (primeiroDigito >= 10)
        {
            primeiroDigito = 0;
            decremento = 2;
        }

        var segundaSoma = 0;
        for (var indice = 0; indice < 9; indice++)
        {
            segundaSoma += (cnh[indice] - '0') * (indice + 1);
        }

        var resto = segundaSoma % 11;
        var segundoDigito = resto >= 10 ? 0 : resto - decremento;
        if (segundoDigito < 0)
        {
            return false;
        }

        return cnh[9] - '0' == primeiroDigito && cnh[10] - '0' == segundoDigito;
    }
}
