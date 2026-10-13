namespace EmpLog.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    public DateTime CriadoEmUtc { get; protected set; }
}
