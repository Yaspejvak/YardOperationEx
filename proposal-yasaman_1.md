# پروژه‌ی نهایی Mentorship — بخش شما (Yasaman)

## زمینه

این آخرین مرحله‌ی مسیر mentorship‌مونه. مثل مرحله‌ی پترن‌ها، **شما کل پروژه رو خودتون، از صفر، مستقل از بقیه‌ی تیم می‌سازید** — نه یک بخش ازش. موضوع (Yard Operations API) برای هر سه نفر یکیه، ولی کدها به هم وابسته نیستن و منتظر هم نمی‌مونید.

مرجع شما فقط `capstone-technical-spec.md` هست — دقیقاً می‌گه چی باید ساخته بشه (کلاس‌ها، امضاها، قوانین)، بدون این‌که خود کد رو بده.

---

## کار شما: کل پروژه، مستقل

طبق `capstone-technical-spec.md`، این ۵ بخش رو **خودتون** می‌سازید:

1. **`YardOperations.Domain`**: Entity ها (`YardTask`, `Equipment`)، `YardTaskFactory` (Factory Method)، `IEquipmentAssignmentStrategy` + `FirstAvailableEquipmentStrategy` (Strategy)، Domain Event ها، Repository interface ها
2. **`YardOperations.Application`**: `YardTaskService`، `CachedEquipmentAvailabilityReader` (Decorator)، `DomainEventDispatcher` (Observer)
3. **`YardOperations.Infrastructure`**: EF Core + PostgreSQL، پیاده‌سازی Repository ها، Caching
4. **`YardOperations.Api`**: Controller ها، `Program.cs` (DI wiring کامل)، Dockerfile/docker-compose
5. **`tests/YardOperations.Tests`**: تمام سناریوهای بخش ۵ سند فنی

هیچ‌کدوم از این بخش‌ها رو از کس دیگه‌ای قرض نمی‌گیرید یا کپی نمی‌کنید — همه‌چیز از صفر، توی پوشه‌ی خودتون.

---

## پوشه‌بندی

```
Capstone/Yasaman/YardOperationsCapstone/
```

فقط داخل همین پوشه کد بزنید. به پوشه‌ی Asal یا Elahe دست نزنید، حتی برای کمک.

---

## معیار پذیرش

- [ ] Solution با ساختار دقیق سند فنی (۵ پروژه، جهت وابستگی درست: Api→Infrastructure→Application→Domain)
- [ ] `YardOperations.Domain` بدون هیچ پکیج بیرونی کامپایل می‌شه
- [ ] همه‌ی قوانین کسب‌وکار enforce شدن (نه فقط comment)
- [ ] `docker compose up --build` (روی پوشه‌ی خودتون) بدون خطا بالا میاد
- [ ] هر ۶ endpoint از طریق Swagger کار می‌کنن
- [ ] `dotnet test` سبزه

## Workflow

- هر لایه → یک branch جدا: `feature/yasaman-domain`, `feature/yasaman-application`, `feature/yasaman-infrastructure`, `feature/yasaman-api-docker`, `feature/yasaman-tests`
- Commit message کامل و واضح
- PR به سمت `develop` با reviewer طبق جدول پایین — خودتون merge نکنید

### چرخش Reviewer برای شما

| مرحله | reviewer شما |
|---|---|
| Domain (روز ۱-۲) | Elahe |
| Application / Infrastructure (روز ۳) | Asal |
| Api / Docker / Tests (روز ۴-۵) | Asal و Elahe (هر دو) |

## برنامه‌ی زمانی

| روز | کار |
|:---:|---|
| ۱ | ساخت Solution + شروع Domain |
| ۲ | تکمیل Domain + PR + merge + شروع Application |
| ۳ | تکمیل Application + Infrastructure + PR + merge |
| ۴ | Api + Docker + PR + merge |
| ۵ | تست‌ها + رفع باگ + دمو نهایی |

سوالی بود همینجا بپرسید، قبل از این‌که حدس بزنید و اشتباه پیش برید.
