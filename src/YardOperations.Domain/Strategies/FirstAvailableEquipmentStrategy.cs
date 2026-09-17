using YardOperations.Domain.Enums;
using YardOperations.Domain.Entities;

namespace YardOperations.Domain.Strategies;

public class FirstAvailableEquipmentStrategy : IEquipmentAssignmentStrategy
{
    public Equipment? SelectEquipment(YardTask task, IReadOnlyList<Equipment> availableEquipment)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(availableEquipment);

        var requiredEquipmentType = task.TaskType switch
        {
            YardTaskType.Lift => EquipmentType.Crane,
            YardTaskType.Move => EquipmentType.Forklift,
            YardTaskType.Inspect => EquipmentType.ReachStacker,
            _ => throw new ArgumentOutOfRangeException(nameof(task.TaskType))
        };

        return availableEquipment.FirstOrDefault(equipment => equipment.Type == requiredEquipmentType);
    }
}
