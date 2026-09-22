using YardOperations.Domain.Entities;
using YardOperations.Domain.Repositories;

namespace YardOperations.Application.Readers;

public class EquipmentAvailabilityReader : IEquipmentAvailabilityReader
{
    private readonly IEquipmentRepository _repository;
    public EquipmentAvailabilityReader(IEquipmentRepository repository)
    {
        _repository = repository;
    }
    public Task<List<Equipment>> GetAvailableEquipmentAsync(CancellationToken ct = default)
    {
        return _repository.GetAvailableAsync(ct);
    }
}