using EmpLog.Domain.Entities;

namespace EmpLog.Domain.Interfaces;

public interface IMotoristaRepository
{
    Task<bool> ExistePorCpfAsync(string cpf, CancellationToken cancellationToken);

    Task<bool> ExistePorCnhAsync(string numeroCnh, CancellationToken cancellationToken);

    void Adicionar(Motorista motorista);
}
