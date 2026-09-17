using YardOperations.Domain.Enums;

namespace YardOperations.Domain.Entities;

public class Equipment
{
    public string Id { get; }
    public EquipmentType Type { get; }
    public EquipmentStatus Status { get; private set; }

    private Equipment()
    {
        Id = null!;
    }

    public Equipment(string id, EquipmentType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        Status = EquipmentStatus.Available;
        Id = id;
        Type = type;
    }

    public void Reserve()
    {
        if (Status == EquipmentStatus.Available)
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
        if (Status == EquipmentStatus.InUse)
        {
            Status = EquipmentStatus.Available;
        }
        else
        {
            throw new InvalidOperationException("Status must be In Use");
        }
    }
}
