using YardOperations.Domain.Enums;

namespace YardOperations.Domain.Strategies;
using YardOperations.Domain.Entities;
public class FirstAvailableEquipmentStrategy : IEquipmentAssignmentStrategy
{
    // public EquipmentType requiredType { get; private set;}
    public Equipment? SelectEquipment(YardTask task, IReadOnlyList<Equipment> availableEquipment)
    {
        // switch (task.TaskType)
        // {
        //     case YardTaskType.Lift: requiredType = EquipmentType.Crane; break;
        //     case YardTaskType.Move: requiredType = EquipmentType.Forklift; break;
        //     case YardTaskType.Inspect: requiredType = EquipmentType.ReachStacker; break;
        // }
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
