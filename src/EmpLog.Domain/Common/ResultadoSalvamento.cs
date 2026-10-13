namespace EmpLog.Domain.Common;

public sealed record ResultadoSalvamento(bool Conflito, string? Campo)
{
    public static ResultadoSalvamento Salvo() => new(false, null);

    public static ResultadoSalvamento ConflitoDe(string campo) => new(true, campo);
}
