using YardOperations.Domain.Entities;

namespace YardOperations.Domain.Repositories;

public interface IYardTaskRepository
{
    Task<YardTask?> GetByIdAsync(
        string id,
        CancellationToken ct = default);

    Task<List<YardTask>> GetAllAsync(
        CancellationToken ct = default);

    Task AddAsync(
        YardTask yardTask,
        CancellationToken ct = default);

    Task SaveChangesAsync(
        CancellationToken ct = default);
}