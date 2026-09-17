using YardOperations.Domain.Entities;

namespace YardOperations.Domain.Strategies;

public interface IEquipmentAssignmentStrategy
{
    Equipment? SelectEquipment(YardTask task, IReadOnlyList<Equipment> availableEquipment);
}
