using EmpLog.Domain.Entities;
using EmpLog.Domain.Interfaces;
using EmpLog.Domain.ValueObjects;
using EmpLog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EmpLog.Infrastructure.Persistence.Repositories;

public sealed class MotoristaRepository : IMotoristaRepository
{
    private readonly EmpLogDbContext _context;

    public MotoristaRepository(EmpLogDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistePorCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        var valor = Cpf.Create(cpf);
        return _context.Motoristas.AsNoTracking().AnyAsync(motorista => motorista.Cpf == valor, cancellationToken);
    }

    public Task<bool> ExistePorCnhAsync(string numeroCnh, CancellationToken cancellationToken) =>
        _context.Motoristas.AsNoTracking()
            .AnyAsync(motorista => motorista.Cnh.Numero == numeroCnh, cancellationToken);

    public void Adicionar(Motorista motorista) => _context.Motoristas.Add(motorista);
}
