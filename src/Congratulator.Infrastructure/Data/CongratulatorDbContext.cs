using Congratulator.Infrastructure.Data.Mappings;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using DateOnlyConverter = Congratulator.Infrastructure.Data.ValueConverters.DateOnlyConverter;

namespace Congratulator.Infrastructure.Data;

public class CongratulatorDbContext(DbContextOptions<CongratulatorDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        configurationBuilder
            .Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>();
        
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfiguration(new PersonMap());
    }
}