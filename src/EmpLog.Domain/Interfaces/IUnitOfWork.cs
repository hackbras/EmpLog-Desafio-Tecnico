using EmpLog.Domain.Common;

namespace EmpLog.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<ResultadoSalvamento> SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
