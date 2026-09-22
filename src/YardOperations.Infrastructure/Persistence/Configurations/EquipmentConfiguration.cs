using YardOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YardOperations.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {   
        builder.HasKey(equipment => equipment.Id);
        builder.Property(equipment => equipment.Type)
            .HasConversion<string>();
        builder.Property(equipment => equipment.Status)
            .HasConversion<string>();
        
        }
}