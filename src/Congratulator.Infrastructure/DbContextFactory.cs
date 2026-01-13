using Congratulator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Congratulator.Infrastructure;

public class DbContextFactory : IDesignTimeDbContextFactory<CongratulatorDbContext>
{
    public CongratulatorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CongratulatorDbContext>();
        optionsBuilder.UseNpgsql();

        return new CongratulatorDbContext(optionsBuilder.Options);
    }
}