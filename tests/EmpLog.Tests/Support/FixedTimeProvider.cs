namespace EmpLog.Tests;

internal sealed class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _agora;

    public FixedTimeProvider(DateTimeOffset agora) => _agora = agora;

    public override DateTimeOffset GetUtcNow() => _agora;
}
