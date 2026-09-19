using YardOperations.Domain.Events;

namespace YardOperations.Application.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct = default);
}
