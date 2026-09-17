using YardOperations.Domain.Enums;
using YardOperations.Domain.Events;

namespace YardOperations.Domain.Entities;

public class YardTask
{


    public string Id { get; private set; }
    public string ContainerId { get; private set; }
    public YardTaskType TaskType { get; private set; }
    public int Priority { get; private set; }
    public YardTaskStatus Status { get; private set; }
    public string? AssignedEquipmentId  { get; private set; }
    private readonly List<IDomainEvent> _domainEvents = []; //can't be new - can Add or Clear
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly(); //Can't Add , Remove , Clear

    internal YardTask(string id, string containerId,YardTaskType taskType, int priority)
    {
        Status = YardTaskStatus.Pending;
        Id = id;
        ContainerId = containerId;
        TaskType = taskType;
        if (priority < 1 || priority > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(priority));
        }else{
            Priority = priority;
        }
    }

    public void AssignEquipment(Equipment equipment)
    {
        if (Status != YardTaskStatus.Pending)
        {
            throw new InvalidOperationException("Status Must be Pending");
        }else if (AssignedEquipmentId != null)
        {
            throw new InvalidOperationException("Assigned Equipment Id must be null");
        }
        equipment.Reserve(); //how does Yard Task have access to Equipment?
        AssignedEquipmentId  = equipment.Id;
    }
    
    public void Start()
    {
        if (Status != YardTaskStatus.Pending)
        {
            throw new InvalidOperationException("Status Must be Pending");
        }else if  (AssignedEquipmentId == null)
        {
            throw new InvalidOperationException("Assigned Equipment Id must not be null");
        }

        Status = YardTaskStatus.InProgress;
    }

    public void Complete(Equipment equipment)
    {
        if (Status != YardTaskStatus.InProgress)
        {
            throw new InvalidOperationException("Status Must be In Progress");
        }else if  (AssignedEquipmentId != equipment.Id)
        {
            throw new InvalidOperationException("Assigned Equipment Id is not equal to the correct Equipment Id");
        }
        equipment.Release();
        Status = YardTaskStatus.Completed;
        _domainEvents.Add(new YardTaskCompletedEvent(Id,equipment.Id));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
