using EmpLog.Infrastructure.Persistence.Context;
using EmpLog.Infrastructure.Persistence.Repositories;
using EmpLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmpLog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EmpLogDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IMotoristaRepository, MotoristaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
