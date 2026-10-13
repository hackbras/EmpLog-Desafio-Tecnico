using EmpLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmpLog.Infrastructure.Persistence.Context;

public sealed class EmpLogDbContext : DbContext
{
    public EmpLogDbContext(DbContextOptions<EmpLogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Motorista> Motoristas => Set<Motorista>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmpLogDbContext).Assembly);
    }
}
