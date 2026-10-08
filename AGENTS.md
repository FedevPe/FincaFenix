# AGENTS.md — FincaFenix

## Build & Run

```powershell
dotnet build FincaFenix.sln
dotnet run --project FincaFenix.WebApi             # http://localhost:5000 | https://localhost:5001 (Swagger)
```

> La UI Blazor (`FincaFenix.UserInterface7.0`) y `FincaFenix.ViewModels` fueron **eliminadas en Fase 6 (Oct 2026)** — la UI de front será React (migración pendiente).

## Local Secrets (gitignored)

`appsettings.Development.json` (in `FincaFenix.WebAPI`) holds the JWT `JwtSettings` block including `SecretKey` and is **ignored by git** — never commit it. Fresh clones must create it locally:

```json
{
  "JwtSettings": {
    "SecretKey": "<at least 32 characters>",
    "Issuer": "FincaFenix",
    "Audience": "FincaFenixAPI",
    "ExpirationInMinutes": 60
  }
}
```

Connection strings stay in the tracked `appsettings.json` (local dev DB).

## EF Core Migrations

Tool manifest (`/.config/dotnet-tools.json` en la raíz del repo — movido desde la UI en Fase 6) installs `dotnet-ef` v9.0.8. Run from repo root:

```powershell
dotnet tool restore
dotnet ef migrations add <Name> --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
dotnet ef database update --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
```

`FincaFenixContextFactory` in `FincaFenix.EFCore` reads connection string from `FincaFenix.WebAPI/appsettings.json` at design time (re-apuntado en Fase 6; antes usaba la UI).

## Architecture (Clean — strict layers)

```
Clients (React — futuro) → WebAPI/Controllers → InversionOfControl (DI orchestrator) → Controllers → UsesCases (MediatR Handlers) → Gateways (Repo interfaces) → EFCore
                                                                                                                        ↕
                                                                                                                   Entities
```

- Every layer has a `DependencyContainer.cs` registering its services as `Add{Layer}Services()` extension methods.
- `FincaFenix.InversionOfControl/ServicesDependencyContainer.cs` chains them all (ya no encadena `.AddViewModelServices()` — proyecto eliminado).
- **Interactors + Presenters pattern was removed in Phase 3** — replaced by MediatR handlers + AutoMapper.

## Key Conventions

- **`Nullable: disable`** in all `.csproj` — do not add null checks.
- **`ImplicitUsings: enable`** + **C# 12** (`LangVersion`).
- **Culture**: `es-AR` set globally in `Program.cs` — format dates/numbers for Argentina.
- **Auth**: API endpoints secured via JWT with policy-based `[Authorize(Policy = PolicyMaster.XXX)]`. Roles: `admin`, `desarrollador`, `supervisor`, `operario`.
- **DI scoping**: Controllers/Handlers → `Transient`. Gateways/Repositories → `Scoped`.
- ~~Blazor pattern~~ — **obsoleto desde Fase 6**: la UI Blazor fue eliminada; la UI nueva será React.

## Project Structure

| Layer | Project | Purpose |
|---|---|---|
| Domain | `FincaFenix.Entities` | POCOs, DTOs, Enums |
| Application | `FincaFenix.UsesCases` | MediatR Handlers + AutoMapper profiles |
| Interface Adapters | `FincaFenix.Controllers` | Controller implementations |
| | `FincaFenix.Gateways` | Repository interfaces |
| Infrastructure | `FincaFenix.EFCore` | DbContext, migrations, query/command services |
| | `FincaFenix.InversionOfControl` | DI composition root |
| | `FincaFenix.WebApi` | Web API standalone (Swagger, testing) |
| Libraries | `FincaFenix.PDF` | QuestPDF generation |
| | `FincaFenix.UIValidators` | UI validation helpers (hoy sin referencias — ver Legacy) |

> Proyectos eliminados en Fase 6: `FincaFenix.UserInterface7.0` (Blazor) y `FincaFenix.ViewModels`. El sln quedó con 9 proyectos.

## API Controllers — Notable Quirks

- `MaterialController` uses explicit route segments: `category/{categoryId}/material`, `recipe/{recipeId}/material` (not the standard `[controller]` pattern).
- `MachineController` route fixed from `api/(controller)` to `api/[controller]`.
- `WorkOrderController` unifies all WorkOrder endpoints (create, queries, state update) in a single controller.
- `DetailWorkOrderController` unifies both add and get-activities endpoints.
- `GetMaterialListByRecipeId` fue implementado en Fase 4 (consulta `DetalleReceta → Material`); ya no lanza `NotImplementedException`.

## Known Fixes — Phase 0 (May 2026)

Fixed bugs + removed dead `DetailWorkOrderController`, cleaned duplicate DI registrations, removed useless `try/catch { throw; }` blocks, and fixed `FarmQueryService` error message copy-paste bug.

## Completed — Phase 1 (Upgrade .NET 9)

13 projects upgraded from `net7.0` to `net9.0`. All Microsoft packages updated to 9.0.0 (EF Core, Identity, Configuration, DI Abstractions). Third-party packages (MudBlazor 7.16, FluentValidation 11.11, QuestPDF 2025.7) left unchanged — compatible. Build: 0 errors.

## Completed — Phase 3 (MediatR + AutoMapper)

MediatR 14.1.0 + AutoMapper 15.1.1 replaces old Interactor/Presenter pattern.

> **AutoMapper 15 (security upgrade, GHSA-rvv3-g6hj-g44x / CVE-2026-32933):** 15.1.1 is the minimum patched version (13.x is vulnerable). It requires `ILoggerFactory` in the `MapperConfiguration` ctor (see `ServicesDependencyContainer.AddServicesContainer`) and **requires a license key for production** (free dev/test use without one; set `cfg.LicenseKey` via `AddAutoMapper`/config when licensed — https://luckypennysoftware.com).

### Files created

| File | Purpose |
|---|---|---|
| `IA/ADD_POLICIES.md` | Execution plan documentation |
| `FincaFenix.EFCore/Migrations/20260523031021_AddPolicySeeding.cs` | Migration checkpoint for policy seeding |

### Migration

```
dotnet ef migrations add AddPolicySeeding --project FincaFenix.EFCore --startup-project FincaFenix.WebApi
dotnet ef database update --project FincaFenix.EFCore --startup-project FincaFenix.WebApi
```

La migración es un checkpoint (sin cambios de esquema). El seeding de claims se ejecuta al inicio de la WebApi via `SeedDataBase.SeedAddPoliciesAsync()` en `Program.cs` + via `UseAsyncSeeding` en `PolicyContainer.cs` (para cuando `SaveChangesAsync` se ejecute en cualquier DbContext). Es idempotente: no duplica claims ya existentes.

### Auth flow (updated)

```
POST /api/auth/login { "userName": "...", "password": "..." }
  → UserManager.FindByNameAsync()
  → UserManager.CheckPasswordAsync()
  → GetRolesAsync() → claims (name, email, roles)
  → RoleManager.GetClaimsAsync(role) for each role → policy claims
  → SymmetricSecurityKey + HmacSha256 → JWT (with policies)
  → Endpoint: [Authorize(Policy = "WORKORDER_READ")] checks Claim("POLICIES", "WORKORDER_READ")
```

### Policy-role matrix (as seeded)

| Rol | Access |
|---|---|
| `desarrollador` | Todas las 40 policies |
| `admin` | Todas las 40 policies |
| `supervisor` | READ de todas las entidades + DETAIL_WORKORDER_CREATE |
| `operario` | WORKORDER_READ, RECIPE_READ, DETAIL_WORKORDER_CREATE, DETAIL_WORKORDER_READ |

## Completed — Debt Cleanup (Fases 2/3/7 — Oct 2026)

1. **Fase 2** — `GetWorkOrderListPaged` (`GetWorkOrderInformationQuery.cs`): el `Where` duplicado se unificó en `baseQuery` compartido por COUNT y SELECT. Decidido: se mantienen 2 round-trips (SQL crudo no compensa con la proyección anidada de `SectorList`).
2. **Fase 3** — `ExceptionBehavior` **cerrado como redundante**: `LoggingBehavior` ya loguea excepciones con contexto y `ExceptionMiddleware` ya mapea a ProblemDetails (404/422/403/500).
3. **Fase 7.2** — `FincaFenix.EFCore/Interceptors/SlowQueryLogInterceptor.cs`: loguea queries que superan `EfCore:SlowQueryThresholdMs` (default 500ms, en `appsettings.json` de WebAPI). Registrado solo en WebAPI — la UI Blazor no lo tiene (proyecto deprecado).

## Completed — Phase 4 (Gateways Orchestration — Oct 2026)

1. **`IUnitOfWork`** (`FincaFenix.Gateways/Interfaces/IUnitOfWork.cs`): `Begin/SaveChanges/Commit/Rollback`. Impl `EfCoreUnitOfWork` (`FincaFenix.EFCore/Services/EfCoreUnitOfWork.cs`) sobre `Database.BeginTransactionAsync`, registrado **Scoped** (comparte el `DbContext`).
2. **`ICreateWorkOrderCommand` eliminado y dividido en primitivas sin transacción propia**: `ICorrelativeNumberService.GetByTypeDoc` (entidad **tracked** — sin `AsNoTracking` para que el `LastNumber++` en memoria se persista), `IRecipeCommand.AddRecipe`, `IWorkOrderCommand.AddWorkOrder` (solo `context.Add`).
3. **`CreateWorkOrderRepository` (Gateway) orquesta**: Begin → leer correlativos → receta (`NumRecipe`, Add, +1, SaveChanges) → orden (`OrderNum`, Add, +1, SaveChanges) → Commit; catch → Rollback. Los correlativos +1 solo persisten si todo sale bien. El handler de MediatR no cambia.
4. **Fix** `GET api/material/recipe/{recipeId}/material` (antes 500 por `NotImplementedException`): `MaterialQueryService.GetMaterialListByRecipeId` + repo + handler.
5. **Rename** `UpdateWorkOrderRepositor` → `UpdateWorkOrderRepository`. **`UpdateWorkOrder()` queda sin implementar a propósito** (futuro caso de edición de órdenes).
6. Los 14 proxies pass-through simples se **mantienen** como abstracción de repositorio de los handlers (decisión explícita).
7. **4.1 Presenters** del plan: aclarado que la estructura Interactor/Presenter fue reemplazada por Handlers de MediatR en Fase 3.

Verificado: build 0 errores + pruebas manuales (login, crear orden con/sin receta con incremento correcto de correlativos, FK inválida → rollback total, endpoint materiales por receta, query paginada, updatestate).

Próxima etapa planificada: **Fase 8 — congelar contrato API** (postergada) / migración React.

## Completed — Fase 3.3 + Fase 6 (AutoMapper + Borrado de UI — Oct 2026)

**Fase 3.3 — Eliminación de mappers manuales:**
1. `FincaFenix.UsesCases/Mappers/` (con `WorkOrderMapper.cs` y `DetailWorkOrderMapper.cs`) **eliminada**. Los 7 mapas resultantes viven en `FincaFenix.UsesCases/Mappings/MappingProfile.cs` (lectura: `WorkOrderEntity→ShowWorkOrderDTO`, `RecipeEntity→RecipeWorkOrderDTO`, `DetailRecipeEntity→DetailRecipeDTO`, `DetailWorkOrderEntity→ActivityWorkOrderDTO`; escritura: `WorkOrderDTO→WorkOrderEntity` (con helpers privados `GroupItems`/`MapRecipeToEntity`), `DetailRecipeDTO→DetailRecipeEntity`, `AddDetailWorkOrderDTO→DetailWorkOrderEntity`).
2. Los 4 handlers usan `IMapper`: `GetAllWorkOrdersHandler`, `GetWorkOrderByIdHandler`, `CreateWorkOrderHandler`, `AddDetailWorkOrderHandler` + `GetActivitiesByOrderIdHandler` (mapping inline eliminado).
3. `AssertConfigurationIsValid()` (ya existente) valida todo al startup — todos los miembros de destino mapeados o ignorados explícitamente preservan 1:1 el comportamiento del mapper manual (p.ej. `EndDate` ignorado → null, `Recipe.Status`/`TotalAplications` ignorados → defaults).
4. **AutoMapper 15**: la sobrecarga con `ResolutionContext` en `ForMember().MapFrom(...)` requiere **4 parámetros** `(source, dest, member, context)`.

**Fase 6 — Borrado definitivo de la UI Blazor:**
1. Carpetas eliminadas: `FincaFenix.UserInterface7.0/` (31 `.razor` + Program.cs + appsettings) y `FincaFenix.ViewModels/`.
2. `FincaFenix.InversionOfControl`: sin ProjectReference a ViewModels y sin `.AddViewModelServices()`.
3. Tool manifest `dotnet-ef` movido a `/.config/dotnet-tools.json` (raíz); `FincaFenixContextFactory` re-apuntado a `FincaFenix.WebAPI/appsettings.json`.
4. `FincaFenix.sln`: 11 → 9 proyectos (fuera UI + ViewModels y sus carpetas de solución).

Verificado: build 0 errores; `dotnet tool restore` + `dotnet ef migrations list`; prueba manual WebAPI completa (login, getall, getCompleteInfo con/sin receta, create sin receta → orden 28, create con receta → orden 29 + receta 19 con `GroupItems` fusionando 10+3→13, addDetailWO con `Description.ToUpper()` persistida, material/recipe/{id}, paginada). **Sin commits aún (pendiente de indicación).**

## No Tests

## NuGet Versions (key)

| Package | Version |
|---|---|
| MudBlazor | 7.16.0 |
| FluentValidation | 11.11.0 |
| QuestPDF | 2025.7.0 |
| MediatR | 14.1.0 |
| AutoMapper | 15.1.1 |
| EF Core + SqlServer | 9.0.0 |
| ASP.NET Identity + UI | 9.0.0 |

## Legacy / Dead Projects (do not touch)

- `FincaFenix.UserInterface/` — only build artifacts; la UI Blazor viva (`UserInterface7.0`) fue **eliminada en Fase 6** — la UI nueva será React
- `FincaFenix.UIValidators/` — no referenciado por ningún proyecto; **se conservó** (no estaba confirmado para borrar)
- `FincaFenix.Validators/` — redundant with `FincaFenix.Validations/` (keep Validations)
- `FincaFenix.Repositories/UniversitarySystem.EFCore` — .NET 8, references missing project, unrelated

## IA/ Directory

| File | Purpose |
|---|---|
| `IA/STACK.md` | Full technology stack analysis |
| `IA/BACKEND_REFACTORING_PLAN.md` | Phased refactoring plan (.NET 9, MediatR, AutoMapper, JWT, React migration) |
| `IA/WebAPI_IMPLEMENTATION.md` | Web API standalone + Swagger implementation plan |
| `IA/ADD_POLICIES.md` | Policy-based authorization implementation plan |
