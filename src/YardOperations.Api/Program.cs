using Microsoft.EntityFrameworkCore;
using YardOperations.Application.Caching;
using YardOperations.Application.Events;
using YardOperations.Application.Readers;
using YardOperations.Application.Services;
using YardOperations.Domain.Repositories;
using YardOperations.Domain.Strategies;
using YardOperations.Infrastructure.Caching;
using YardOperations.Infrastructure.Logging;
using YardOperations.Infrastructure.Persistence;
using YardOperations.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Connection string 'Default' was not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();

builder.Services.AddScoped<IYardTaskRepository, YardTaskRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

builder.Services.AddScoped<EquipmentAvailabilityReader>();
builder.Services.AddScoped<IEquipmentAvailabilityReader>(serviceProvider =>
    new CachedEquipmentAvailabilityReader(
        serviceProvider.GetRequiredService<EquipmentAvailabilityReader>(),
        serviceProvider.GetRequiredService<ICacheService>()));

builder.Services.AddScoped<IEquipmentAssignmentStrategy,
    FirstAvailableEquipmentStrategy>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddSingleton<YardOperations.Application.Events.ILogger,
    ConsoleLogger>();
builder.Services.AddScoped<YardTaskService>();

var app = builder.Build();

app.Run();
