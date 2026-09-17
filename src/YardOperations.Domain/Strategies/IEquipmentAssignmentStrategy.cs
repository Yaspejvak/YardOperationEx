namespace YardOperations.Domain.Strategies;
using YardOperations.Domain.Entities;
public interface IEquipmentAssignmentStrategy
{
    Equipment? SelectEquipment(YardTask task, IReadOnlyList<Equipment> availableEquipment);
}