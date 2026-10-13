namespace EmpLog.Domain.Common;

public sealed record ErroResultado(string Codigo, string Campo, string Mensagem);

public class Result
{
    public const string CodigoValidacao = "validacao";
    public const string CodigoConflito = "conflito";

    protected Result(bool sucesso, bool conflito, IReadOnlyList<ErroResultado> erros)
    {
        Sucesso = sucesso;
        Conflito = conflito;
        Erros = erros;
    }

    public bool Sucesso { get; }

    public bool Conflito { get; }

    public IReadOnlyList<ErroResultado> Erros { get; }

    public static Result Ok() => new(true, false, []);

    public static Result Falha(IEnumerable<ErroResultado> erros) =>
        new(false, false, erros.ToArray());

    public static Result ConflitoDe(string campo, string mensagem) =>
        new(false, true, [new ErroResultado(CodigoConflito, campo, mensagem)]);
}

public sealed class Result<T> : Result
{
    private Result(bool sucesso, bool conflito, T? valor, IReadOnlyList<ErroResultado> erros)
        : base(sucesso, conflito, erros)
    {
        Valor = valor;
    }

    public T? Valor { get; }

    public static Result<T> Ok(T valor) => new(true, false, valor, []);

    public static new Result<T> Falha(IEnumerable<ErroResultado> erros) =>
        new(false, false, default, erros.ToArray());

    public static Result<T> Falha(string campo, string mensagem) =>
        Falha([new ErroResultado(CodigoValidacao, campo, mensagem)]);

    public static new Result<T> ConflitoDe(string campo, string mensagem) =>
        new(false, true, default, [new ErroResultado(CodigoConflito, campo, mensagem)]);
}
