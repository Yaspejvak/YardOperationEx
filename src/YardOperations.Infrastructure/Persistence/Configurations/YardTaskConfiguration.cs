using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YardOperations.Domain.Entities;
using YardOperations.Domain.Events;

namespace YardOperations.Infrastructure.Persistence.Configurations;

public class YardTaskConfiguration : IEntityTypeConfiguration<YardTask>
{
    public void Configure(EntityTypeBuilder<YardTask> builder)
    {
        builder.HasKey(yardTask => yardTask.Id); 
        builder.Property(yardTask => yardTask.ContainerId);
        builder.Property(yardTask => yardTask.TaskType).HasConversion<string>();
        builder.Property(yardTask => yardTask.Priority);
        builder.Property(yardTask => yardTask.Status).HasConversion<string>();
        builder.Property(yardTask => yardTask.AssignedEquipmentId);
        builder.Ignore(yardTask => yardTask.DomainEvents); //Domains are not saved in DB & will be ignored
        // we mention it so that EF automatic mapping doesn't happen for Domain Events
        
    }
}