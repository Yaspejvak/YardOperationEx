using YardOperations.Application.Caching;
using YardOperations.Domain.Entities;


namespace YardOperations.Application.Readers;

public class CachedEquipmentAvailabilityReader : IEquipmentAvailabilityReader
{
    private readonly IEquipmentAvailabilityReader _inner;
    private readonly ICacheService _cacheService;

    public CachedEquipmentAvailabilityReader(IEquipmentAvailabilityReader  inner, ICacheService cacheService)
    {
        _inner = inner; 
        _cacheService = cacheService;
    }
    public Task<List<Equipment>> GetAvailableEquipmentAsync(CancellationToken ct = default)
    {
        //if cache exist -> get cache
        return _cacheService.GetOrCreateAsync("equipment:available", () => _inner.GetAvailableEquipmentAsync(ct), TimeSpan.FromSeconds(30));
        //if not -> call repository
       // await () => _inner.GetAvailableEquipmentAsync(ct);
    }
}