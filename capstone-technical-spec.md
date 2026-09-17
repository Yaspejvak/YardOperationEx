<div dir="rtl" align="right">

# مشخصات فنی دقیق — پروژه‌ی نهایی (Yard Operations API)

این سند **کد نیست** — مشخصات دقیق هر کلاس/اینترفیس/قانونیه که تیم باید از صفر پیاده‌ش کنه. هدف اینه که هیچ ابهامی درباره‌ی "دقیقاً چی باید ساخته بشه" نمونه، ولی خود پیاده‌سازی کاملاً کار خودشونه.

معماری: ۴ پروژه‌ی جدا در یک Solution، به سبک DDD/Clean Architecture:

```
YardOperationsCapstone.sln
├── src/YardOperations.Domain          (بدون وابستگی به هیچ پکیج بیرونی)
├── src/YardOperations.Application     (فقط وابسته به Domain)
├── src/YardOperations.Infrastructure  (وابسته به Domain + Application + EF Core/Npgsql)
└── src/YardOperations.Api             (وابسته به همه، ASP.NET Core Web API)
tests/YardOperations.Tests             (وابسته به Domain + Application، xUnit)
```

قانون کلی: **جهت وابستگی همیشه رو به داخل باشه** (Api → Infrastructure → Application → Domain). Domain هیچ‌وقت نباید بدونه EF Core یا PostgreSQL وجود داره.

---

## ۱. YardOperations.Domain

### Enums

```csharp
public enum YardTaskType { Lift, Move, Inspect }
public enum YardTaskStatus { Pending, InProgress, Completed, Cancelled }
public enum EquipmentType { Crane, Forklift, ReachStacker }
public enum EquipmentStatus { Available, InUse, UnderMaintenance }
```

### `Equipment` (Entity)

فیلدها: `Id` (string, read-only)، `Type` (EquipmentType, read-only)، `Status` (EquipmentStatus, setter خصوصی).

قوانین:
- سازنده‌ی عمومی `Equipment(string id, EquipmentType type)` → وضعیت اولیه همیشه `Available`
- `Reserve()`: فقط اگه `Status == Available` باشه اجازه بده، وگرنه exception. بعدش `Status = InUse`.
- `Release()`: فقط اگه `Status == InUse` باشه اجازه بده، وگرنه exception. بعدش `Status = Available`.
- باید یک سازنده‌ی بدون پارامتر (private/protected) برای EF Core هم داشته باشه.

### `YardTask` (Aggregate Root)

فیلدها: `Id`, `ContainerId`, `TaskType`, `Priority` (int، بین ۱ تا ۵)، `Status`, `AssignedEquipmentId` (nullable string)، و یک لیست فقط-خواندنی از `IDomainEvent` به نام `DomainEvents`.

قوانین (همه‌شون باید **داخل خود کلاس** enforce بشن، نه در Service/Controller):
- سازنده باید `internal` باشه (نه public) — تنها راه ساختنش از بیرون، از طریق `YardTaskFactory` (پایین) باشه.
- سازنده باید `Priority` خارج از بازه‌ی ۱ تا ۵ رو رد کنه (`ArgumentOutOfRangeException`).
- `AssignEquipment(Equipment equipment)`: فقط وقتی `Status == Pending` مجازه. equipment رو `Reserve()` می‌کنه و `AssignedEquipmentId` رو ست می‌کنه. اگه از قبل equipment داشته باشه، exception.
- `Start()`: فقط از `Pending` به `InProgress` مجازه، و فقط اگه equipment از قبل assign شده باشه.
- `Complete(Equipment equipment)`: فقط از `InProgress` به `Completed` مجازه. equipment باید همون equipment ای باشه که assign شده (چک با Id). equipment رو `Release()` می‌کنه، و یک `YardTaskCompletedEvent` به `DomainEvents` اضافه می‌کنه.
- `ClearDomainEvents()`: لیست رویدادها رو خالی می‌کنه (بعد از این‌که Application layer رویدادها رو dispatch کرد).

### `YardTaskFactory` (Factory Method — Design Pattern)

```csharp
public static class YardTaskFactory
{
    public static YardTask Create(string id, string containerId, YardTaskType taskType, int? priority = null);
}
```

قانون کسب‌وکار: اگه `priority` داده نشده بود، برای `Inspect` پیش‌فرض `5` و برای بقیه پیش‌فرض `3` باشه.

### Domain Events

```csharp
public interface IDomainEvent { DateTime OccurredOnUtc { get; } }

public sealed class YardTaskCompletedEvent : IDomainEvent
{
    public string YardTaskId { get; }
    public string EquipmentId { get; }
    public DateTime OccurredOnUtc { get; }
}
```

### Repository Interfaces (پیاده‌سازی واقعیشون توی Infrastructure میاد)

```csharp
public interface IYardTaskRepository
{
    Task<YardTask?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<YardTask>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(YardTask yardTask, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<Equipment>> GetAvailableAsync(CancellationToken ct = default);
    Task AddAsync(Equipment equipment, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
```

### `IEquipmentAssignmentStrategy` (Strategy — Design Pattern)

```csharp
public interface IEquipmentAssignmentStrategy
{
    Equipment? SelectEquipment(YardTask task, IReadOnlyList<Equipment> availableEquipment);
}
```

یک پیاده‌سازی پیش‌فرض به نام `FirstAvailableEquipmentStrategy`: بر اساس `TaskType` نوع equipment لازم رو تعیین کن (`Lift→Crane`, `Move→Forklift`, `Inspect→ReachStacker`) و اولین equipment با اون نوع رو از لیست برگردون (یا `null` اگه نبود).

---

## ۲. YardOperations.Application

### `ICacheService`

```csharp
public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl);
    void Remove(string key);
}
```

### `IEquipmentAvailabilityReader` + Decorator (Decorator — Design Pattern)

```csharp
public interface IEquipmentAvailabilityReader
{
    Task<List<Equipment>> GetAvailableEquipmentAsync(CancellationToken ct = default);
}
```

- `EquipmentAvailabilityReader`: مستقیم از `IEquipmentRepository.GetAvailableAsync` می‌خونه (بدون cache).
- `CachedEquipmentAvailabilityReader`: همون interface رو پیاده می‌کنه، یک `IEquipmentAvailabilityReader` دیگه (inner) و یک `ICacheService` می‌گیره؛ نتیجه رو با کلید ثابت (مثلاً `"equipment:available"`) و TTL حدود ۳۰ ثانیه کش می‌کنه.

### `IDomainEventDispatcher` + Observer (Observer — Design Pattern)

```csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}
```

پیاده‌سازی: وقتی به یک `YardTaskCompletedEvent` می‌رسه، باید کش equipment های در دسترس رو invalidate کنه (چون equipment ای که کارش تموم شده، دوباره available شده). یک abstraction ساده برای لاگ کردن هم لازمه (یک interface مثل `ILogger` با متد `Log(string message)` کافیه — نیازی به وابستگی به فریم‌ورک خاصی نیست).

### `YardTaskService`

متدها:
- `CreateTaskAsync(string id, CreateYardTaskRequest request, ...)` → از `YardTaskFactory` استفاده کن، ذخیره کن.
- `AssignEquipmentAsync(string taskId, ...)` → equipment های در دسترس رو (از طریق `IEquipmentAvailabilityReader`) بگیر، با LINQ فیلتر کن، با `IEquipmentAssignmentStrategy` انتخاب کن، assign کن، ذخیره کن.
- `CompleteTaskAsync(string taskId, ...)` → task رو start (اگه لازم بود) و complete کن، رویدادها رو dispatch کن.
- `GetAllAsync(...)` → همه‌ی task ها رو با LINQ بر اساس `Priority` نزولی مرتب کن و به DTO تبدیل کن.

**قانون مهم**: این کلاس فقط orchestration انجام می‌ده — هیچ `if` ای که تصمیم بگیره یک عملیات "مجاز" هست یا نه، نباید اینجا باشه (اون‌ها داخل `YardTask`/`Equipment` هستن).

### DTOs

```csharp
public record CreateYardTaskRequest(string ContainerId, YardTaskType TaskType, int? Priority);
public record YardTaskDto(string Id, string ContainerId, YardTaskType TaskType, int Priority, YardTaskStatus Status, string? AssignedEquipmentId);
```

---

## ۳. YardOperations.Infrastructure

- `AppDbContext : DbContext` با `DbSet<YardTask> YardTasks` و `DbSet<Equipment> Equipment`.
- پیکربندی با Fluent API (`IEntityTypeConfiguration<T>`):
  - Enum ها به‌صورت `string` ذخیره بشن (نه int) — خواناتره توی دیتابیس.
  - `YardTask.DomainEvents` باید `Ignore` بشه (persist نمی‌شه).
  - Primary key هرکدوم `Id` (string).
- `YardTaskRepository` و `EquipmentRepository`: پیاده‌سازی مستقیم interface های Domain با EF Core.
- `InMemoryCacheService : ICacheService`: بر پایه‌ی `IMemoryCache` (از `Microsoft.Extensions.Caching.Memory`).
- پکیج‌های لازم: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.Extensions.Caching.Memory`.

---

## ۴. YardOperations.Api

### Endpoint ها

| Method | مسیر | کار |
|---|---|---|
| GET | `/api/equipment` | لیست equipment های در دسترس |
| POST | `/api/equipment` | ثبت equipment جدید |
| GET | `/api/yard-tasks` | لیست task ها (مرتب بر اساس اولویت) |
| POST | `/api/yard-tasks` | ایجاد task جدید |
| POST | `/api/yard-tasks/{id}/assign-equipment` | تخصیص equipment |
| POST | `/api/yard-tasks/{id}/complete` | تکمیل task |

### `Program.cs` باید:

1. `AppDbContext` رو با `UseNpgsql` و connection string از `appsettings.json` (کلید `ConnectionStrings:Default`) رجیستر کنه.
2. `IMemoryCache` + `ICacheService` رو رجیستر کنه.
3. Repository ها رو رجیستر کنه.
4. **ترکیب Decorator**: هم نسخه‌ی concrete `EquipmentAvailabilityReader` و هم `IEquipmentAvailabilityReader` (که به `CachedEquipmentAvailabilityReader` resolve بشه و دور نسخه‌ی concrete بپیچه) رو رجیستر کنه.
5. `IEquipmentAssignmentStrategy` رو به `FirstAvailableEquipmentStrategy` رجیستر کنه.
6. `IDomainEventDispatcher` و `YardTaskService` رو رجیستر کنه.
7. Swagger رو فعال کنه.
8. موقع startup، migration های EF Core رو خودش اجرا کنه (`db.Database.Migrate()`).

### Docker

- `Dockerfile` دو مرحله‌ای (SDK برای build/publish، `aspnet` برای runtime).
- `docker-compose.yml` با دو سرویس: `api` و `db` (Postgres 16). `api` باید منتظر `healthy` شدن `db` بمونه.
- Connection string داخل کانتینر باید `Host=db` باشه (نه `localhost`).

---

## ۵. tests/YardOperations.Tests (xUnit)

حداقل این سناریوها باید پوشش داده بشن (بدون نیاز به دیتابیس واقعی — فقط روی Domain/Application خالص):

- `YardTaskFactory` پیش‌فرض priority رو درست تنظیم می‌کنه (برای `Inspect` و برای بقیه).
- `Priority` خارج از بازه رد می‌شه.
- `AssignEquipment` روی equipment، وضعیتش رو `InUse` می‌کنه.
- assign کردن equipment دوم روی یک task که از قبل equipment داره، exception می‌ده.
- `Complete` بدون `Start` قبلی، exception می‌ده.
- یک چرخه‌ی کامل (`Assign → Start → Complete`) هم `YardTaskCompletedEvent` تولید می‌کنه و هم equipment رو آزاد می‌کنه.
- `FirstAvailableEquipmentStrategy` equipment با نوع درست رو انتخاب می‌کنه، و وقتی نوع مناسب نیست `null` برمی‌گردونه.
- `CachedEquipmentAvailabilityReader` روی فراخوانی دوم، دیگه سراغ inner reader نمی‌ره (تا وقتی کش invalidate نشده).

برای تست‌های آخر (که به `ICacheService`/`IEquipmentAvailabilityReader` نیاز دارن)، به‌جای دیتابیس واقعی از یک پیاده‌سازی fake دستی استفاده کنید (نیازی به Moq یا کتابخونه‌ی mock نیست).

---

## معیار پذیرش کلی این مرحله

- [ ] Solution با ۵ پروژه، دقیقاً با این ساختار و جهت وابستگی
- [ ] همه‌ی قوانین enforce‌شده داخل Domain هستن، نه جای دیگه
- [ ] `docker compose up --build` بدون خطا بالا میاد و migration خودکار اجرا می‌شه
- [ ] هر ۶ endpoint از طریق Swagger کار می‌کنن
- [ ] `dotnet test` سبزه

</div>
