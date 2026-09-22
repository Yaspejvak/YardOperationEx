using YardOperations.Domain.Enums;

namespace YardOperations.Application.DTOs;

public record YardTaskDto(
    string Id,
    string ContainerId,
    YardTaskType TaskType,
    int Priority,
    YardTaskStatus Status,
    string? AssignedEquipmentId);
