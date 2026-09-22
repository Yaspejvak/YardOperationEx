using Microsoft.EntityFrameworkCore;
using YardOperations.Domain.Entities;

namespace YardOperations.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<YardTask> YardTasks { get; set; }
    public DbSet<Equipment> Equipments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}