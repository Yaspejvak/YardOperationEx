using YardOperations.Domain.Enums;

namespace YardOperations.Domain.Entities;

public class Equipment
{
    public string Id { get; }
    public EquipmentType Type { get; }
    public EquipmentStatus Status { get; private set; }

    private Equipment()
    {
        //For EF
        Id = null!; //only to remove warning for now
    }
    public Equipment(string id, EquipmentType type)
    {
        Status = EquipmentStatus.Available;
        Id = id;
        Type = type;
    }

    public void Reserve()
    {
        if(Status == EquipmentStatus.Available)
        {
            Status = EquipmentStatus.InUse;
        }
        else
        {
            throw new InvalidOperationException("Status must be available");
        }
    }
    public void Release()
    {
        if(Status == EquipmentStatus.InUse)
        {
            Status = EquipmentStatus.Available;
        }
        else
        {
            throw new InvalidOperationException("Status must be In Use");
        }
    }
}