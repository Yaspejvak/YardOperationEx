using YardOperations.Domain.Entities;

namespace YardOperations.Application.Readers;

public interface IEquipmentAvailabilityReader
{
    Task<List<Equipment>> GetAvailableEquipmentAsync(
        CancellationToken ct = default);
}
