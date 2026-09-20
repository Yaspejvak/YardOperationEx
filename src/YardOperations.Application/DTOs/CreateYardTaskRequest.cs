using YardOperations.Domain.Enums;

namespace YardOperations.Application.DTOs;

public record CreateYardTaskRequest(
    string ContainerId,
    YardTaskType TaskType,
    int? Priority);
