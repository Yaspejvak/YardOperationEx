using YardOperations.Application.DTOs;
using YardOperations.Application.Events;
using YardOperations.Application.Readers;
using YardOperations.Domain.Entities;
using YardOperations.Domain.Enums;
using YardOperations.Domain.Factories;
using YardOperations.Domain.Repositories;
using YardOperations.Domain.Strategies;

namespace YardOperations.Application.Services;

public class YardTaskService
{
    // public string Id;
    private readonly IYardTaskRepository _yardTaskRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentAvailabilityReader  _equipmentAvailabilityReader;
    private readonly IEquipmentAssignmentStrategy  _assignmentStrategy;
    private readonly IDomainEventDispatcher _eventDispatcher;
    public YardTaskService(
        IYardTaskRepository yardTaskRepository, 
        IEquipmentAvailabilityReader equipmentAvailabilityReader,
        IEquipmentRepository equipmentRepository,
        IEquipmentAssignmentStrategy assignmentStrategy,
        IDomainEventDispatcher eventDispatcher)
    {
        _yardTaskRepository = yardTaskRepository;
        _equipmentAvailabilityReader = equipmentAvailabilityReader;
        _equipmentRepository = equipmentRepository;
        _assignmentStrategy  = assignmentStrategy;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<YardTask> CreateTaskAsync(string id, CreateYardTaskRequest request,
        CancellationToken ct = default)
    {
        var yardTaskFactory = YardTaskFactory.Create(
            id, 
            request.ContainerId,
            request.TaskType,
            request.Priority);
        await _yardTaskRepository.AddAsync(yardTaskFactory, ct);
        await _yardTaskRepository.SaveChangesAsync(ct);
        return yardTaskFactory;
    }

    //AssignEquipmentAsync(string taskId, ...) → equipment //IEquipmentAvailabilityReader
    public async Task AssignEquipmentAsync(string taskId,CancellationToken ct)
    {
        var availableEquipment = await _equipmentAvailabilityReader.GetAvailableEquipmentAsync(ct); 
        var filteredEquipment = availableEquipment
            .Where(e => e.Status == EquipmentStatus.Available)
            .ToList();
        var yardTask = await _yardTaskRepository.GetByIdAsync(taskId, ct);
        if (yardTask == null)
        {
            throw new InvalidOperationException($"No YardTask found for task {taskId}");
        }

        var equipmemt = _assignmentStrategy.SelectEquipment(yardTask, filteredEquipment);
        if (equipmemt == null)
        {
            throw new InvalidOperationException($"No equipment available for task {taskId}");
        }
        yardTask.AssignEquipment(equipmemt);
        await _yardTaskRepository.SaveChangesAsync(ct);
    }   
    
    //CompleteTaskAsync(string taskId, ...) -> task start/complete - dispatch
    public async Task CompleteTaskAsync(
        string taskId,
        CancellationToken ct = default)
    {
        var yardTask = await _yardTaskRepository.GetByIdAsync(taskId, ct);

        if (yardTask == null)
        {
            throw new InvalidOperationException(
                $"No YardTask found for task {taskId}");
        }

        if (yardTask.AssignedEquipmentId == null)
        {
            throw new InvalidOperationException(
                $"No equipment assigned to task {taskId}");
        }

        var equipment = await _equipmentRepository.GetByIdAsync(
            yardTask.AssignedEquipmentId,
            ct);

        if (equipment == null)
        {
            throw new InvalidOperationException(
                $"Assigned equipment not found for task {taskId}");
        }

        if (yardTask.Status == YardTaskStatus.Pending)
        {
            yardTask.Start();
        }

        yardTask.Complete(equipment);

        await _yardTaskRepository.SaveChangesAsync(ct);
        await _equipmentRepository.SaveChangesAsync(ct);

        await _eventDispatcher.DispatchAsync(yardTask.DomainEvents, ct);

        yardTask.ClearDomainEvents();
    }
    public async Task<List<YardTaskDto>> GetAllAsync(
        CancellationToken ct = default)
    {
        var yardTasks = await _yardTaskRepository.GetAllAsync(ct);

        var result = yardTasks
            .OrderByDescending(task => task.Priority)
            .Select(task => new YardTaskDto(
                task.Id,
                task.ContainerId,
                task.TaskType,
                task.Priority,
                task.Status,
                task.AssignedEquipmentId))
            .ToList();

        return result;
    }
    //only orchestration / no if else
}
