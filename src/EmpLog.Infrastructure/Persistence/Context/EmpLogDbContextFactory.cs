using EmpLog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmpLog.Infrastructure.Persistence.Context;

public sealed class EmpLogDbContextFactory : IDesignTimeDbContextFactory<EmpLogDbContext>
{
    public EmpLogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EmpLogDbContext>()
            .UseSqlite("Data Source=emplog.db")
            .Options;

        return new EmpLogDbContext(options);
    }
}
