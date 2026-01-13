using Congratulator.Infrastructure.Data.ValueConverters;
using Congratulator.SharedKernel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Congratulator.Infrastructure.Data.Mappings;

public class PersonMap : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons").HasKey(p => p.Id);
        
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(p => p.LastName)
            .HasMaxLength(64);
        
        builder.Property(p => p.BirthDate)
            .HasColumnType("date")
            .IsRequired()
            .HasConversion<DateOnlyConverter>();
        
        builder.Property(p => p.RelationshipType)
            .HasConversion<string>()
            .IsRequired();
    }
}