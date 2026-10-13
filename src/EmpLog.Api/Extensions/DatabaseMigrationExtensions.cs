using EmpLog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EmpLog.Api.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task MigrarBancoAsync(this WebApplication app)
    {
        using var escopo = app.Services.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<EmpLogDbContext>();
        await contexto.Database.MigrateAsync();
    }
}
