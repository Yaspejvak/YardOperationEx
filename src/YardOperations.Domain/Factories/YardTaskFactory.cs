using YardOperations.Domain.Enums;
using YardOperations.Domain.Entities;

namespace YardOperations.Domain.Factories;

public static class YardTaskFactory
{
    public static YardTask Create(
        string id,
        string containerId,
        YardTaskType taskType,
        int? priority = null)
    {
        // if (!priority.HasValue)
        // {
        //     switch (taskType)
        //     {
        //         case YardTaskType.Inspect:
        //             priority = 5;
        //             break;
        //         default:
        //             priority = 3;
        //             break;
        //     }
        // }
        var resolvedPriority = priority ?? (taskType == YardTaskType.Inspect ? 5 : 3);
        return new YardTask(id, containerId, taskType, resolvedPriority);
    }
}
