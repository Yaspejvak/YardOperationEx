using YardOperations.Application.Caching;
using YardOperations.Domain.Events;

namespace YardOperations.Application.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;
    public DomainEventDispatcher(ICacheService cacheService, ILogger logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
       var yardTaskCompletedEvent = events.FirstOrDefault(domainEvent  => domainEvent  is YardTaskCompletedEvent);
       if (yardTaskCompletedEvent == null)
       {
           //don't invalidate cache just return TASK
           return Task.CompletedTask;
       }
       //invalidate cache then return
       _cacheService.Remove("equipment:available");
       var message = "Equipment availability cache invalidated.";
       _logger.Log(message);
       return Task.CompletedTask;
    }
}