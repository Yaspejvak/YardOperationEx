using Microsoft.EntityFrameworkCore;
using YardOperations.Domain.Entities;
using YardOperations.Domain.Enums;
using YardOperations.Domain.Repositories;
using YardOperations.Infrastructure.Persistence;

namespace YardOperations.Infrastructure.Repository;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly AppDbContext  _dbContext;
    public EquipmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Equipment?> GetByIdAsync(string id, CancellationToken ct = default)
    {
       return await _dbContext.Equipments.FindAsync([id],ct);
       //[id] -> seperating Key(s) from ct
    }

    public async Task<List<Equipment>> GetAvailableAsync(CancellationToken ct = default)
    {
        return await _dbContext.Equipments
            .Where(equipment => equipment.Status == EquipmentStatus.Available) // only returning available (not All)
            .ToListAsync(ct); 
    }

    public async Task AddAsync(Equipment equipment, CancellationToken ct = default)
    {   
        await _dbContext.Equipments.AddAsync(equipment,ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}