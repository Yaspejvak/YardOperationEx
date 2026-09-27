using Microsoft.EntityFrameworkCore;
using YardOperations.Domain.Entities;
using YardOperations.Infrastructure.Persistence.Configurations;

namespace YardOperations.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<YardTask> YardTasks { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //this method is called when EF wants to build DB -> Introduce Configurations just like entities
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration<Equipment>(new EquipmentConfiguration());
        modelBuilder.ApplyConfiguration<YardTask>(new YardTaskConfiguration());
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}