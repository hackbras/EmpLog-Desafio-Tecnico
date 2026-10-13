namespace EmpLog.Domain.Common;

internal static class Digitos
{
    public static string Apenas(string? texto) =>
        string.IsNullOrWhiteSpace(texto)
            ? string.Empty
            : new string(texto.Where(char.IsDigit).ToArray());
}
