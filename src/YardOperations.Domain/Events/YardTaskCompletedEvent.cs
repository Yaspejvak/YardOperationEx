namespace YardOperations.Domain.Events;

public sealed class YardTaskCompletedEvent : IDomainEvent
{
    public string YardTaskId { get; }
    public string EquipmentId { get; }
    public DateTime OccurredOnUtc { get; }

    public YardTaskCompletedEvent(string yardTaskId, string equipmentId)
    {
        YardTaskId = yardTaskId;
        EquipmentId = equipmentId;
        OccurredOnUtc = DateTime.UtcNow;
    }
}
