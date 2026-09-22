using YardOperations.Domain.Entities;

namespace YardOperations.Domain.Repositories;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(
        string id,
        CancellationToken ct = default);

    Task<List<Equipment>> GetAvailableAsync(
        CancellationToken ct = default);

    Task AddAsync(
        Equipment equipment,
        CancellationToken ct = default);

    Task SaveChangesAsync(
        CancellationToken ct = default); 
}
