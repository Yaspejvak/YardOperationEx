namespace YardOperations.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
