using Microsoft.EntityFrameworkCore;
using YardOperations.Domain.Entities;
using YardOperations.Domain.Repositories;
using YardOperations.Infrastructure.Persistence;

namespace YardOperations.Infrastructure.Repository;

public class YardTaskRepository : IYardTaskRepository
{
    private readonly AppDbContext _dbContext;

    public YardTaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<YardTask?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        //we have async/await because EF might need I/O with DB for finding Entiies 
        return await _dbContext.YardTasks.FindAsync([id], ct);
        //we write the key(s) in [] to separate it from ct , that is the rule for EF 
    }

    public async Task<List<YardTask>> GetAllAsync(CancellationToken ct = default)
    {

        return await _dbContext.YardTasks.ToListAsync(ct); //return result as LIST

    }

    public async Task AddAsync(YardTask yardTask, CancellationToken ct = default)
    {
        await _dbContext.YardTasks.AddAsync(yardTask, ct);

    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);

    }
}