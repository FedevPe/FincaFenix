# AGENTS.md — FincaFenix

## Build & Run

```powershell
dotnet build FincaFenix.sln
dotnet run --project FincaFenix.UserInterface7.0  # http://localhost:8080 | https://localhost:8060
dotnet run --project FincaFenix.WebApi             # http://localhost:5000 | https://localhost:5001 (Swagger)
```

## Local Secrets (gitignored)

`appsettings.Development.json` (both `FincaFenix.WebAPI` and `FincaFenix.UserInterface7.0`) holds the JWT `JwtSettings` block including `SecretKey` and is **ignored by git** — never commit it. Fresh clones must create it locally:

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

Tool manifest (`FincaFenix.UserInterface7.0/.config/dotnet-tools.json`) installs `dotnet-ef` v9.0.8. Run from repo root:

```powershell
dotnet tool restore
dotnet ef migrations add <Name> --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
dotnet ef database update --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
```

`FincaFenixContextFactory` in `FincaFenix.EFCore` reads connection string from the UI project's `appsettings.json` at design time.

## Architecture (Clean — strict layers)

```
UI (Blazor Server) → InversionOfControl (DI orchestrator) → Controllers → UsesCases (MediatR Handlers) → Gateways (Repo interfaces) → EFCore
                                                                                                                       ↕
                                                                                                                  Entities
```

- Every layer has a `DependencyContainer.cs` registering its services as `Add{Layer}Services()` extension methods.
- `FincaFenix.InversionOfControl/ServicesDependencyContainer.cs` chains them all.
- ViewModels are registered in `FincaFenix.ViewModels/DependencyContainer.cs` as `Transient`.
- **Interactors + Presenters pattern was removed in Phase 3** — replaced by MediatR handlers + AutoMapper.

## Key Conventions

- **`Nullable: disable`** in all `.csproj` — do not add null checks.
- **`ImplicitUsings: enable`** + **C# 12** (`LangVersion`).
- **Culture**: `es-AR` set globally in `Program.cs` — format dates/numbers for Argentina.
- **Auth**: Global `RequireAuthorization()` on all Razor Pages, Blazor hub, and fallback. Roles: `admin`, `desarrollador`, `supervisor`, `operario`. API controllers use policy-based `[Authorize(Policy = PolicyMaster.XXX)]` via JWT with policy claims.
- **DI scoping**: Controllers/Handlers/ViewModels → `Transient`. Gateways/Repositories → `Scoped`.
- **Blazor pattern**: Pages inject ViewModels (`[Inject]`) that encapsulate state + orchestration. Razor code-behind is minimal.

## Project Structure

| Layer | Project | Purpose |
|---|---|---|
| Domain | `FincaFenix.Entities` | POCOs, DTOs, Enums |
| Application | `FincaFenix.UsesCases` | MediatR Handlers + AutoMapper profiles |
| Interface Adapters | `FincaFenix.Controllers` | Controller implementations |
| | `FincaFenix.Gateways` | Repository interfaces |
| | `FincaFenix.ViewModels` | ViewModels for Blazor |
| Infrastructure | `FincaFenix.EFCore` | DbContext, migrations, query/command services |
| | `FincaFenix.InversionOfControl` | DI composition root |
| | `FincaFenix.UserInterface7.0` | Blazor Server UI |
| | `FincaFenix.WebApi` | Web API standalone (Swagger, testing) |
| Libraries | `FincaFenix.PDF` | QuestPDF generation |
| | `FincaFenix.UIValidators` | UI validation helpers |

## Key Routes (Blazor)

| Route | Page | Roles |
|---|---|---|
| `/` | Index — Menu | admin, desarrollador, supervisor |
| `/ordenestrabajo` | WorkOrders | todos (operario ve solo cards) |
| `/usuarios` | Users | admin, desarrollador |
| `/login` | Login (Razor Page Identity UI) | — |
| `/accesodenegado` | Access Denied | — |

## API Controllers — Notable Quirks

- `MaterialController` uses explicit route segments: `category/{categoryId}/material`, `recipe/{recipeId}/material` (not the standard `[controller]` pattern).
- `MachineController` route fixed from `api/(controller)` to `api/[controller]`.
- `WorkOrderController` unifies all WorkOrder endpoints (create, queries, state update) in a single controller.
- `DetailWorkOrderController` unifies both add and get-activities endpoints.
- `GetMaterialListByRecipeId` still throws `NotImplementedException` (no repository method exists yet).

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

- `FincaFenix.UserInterface/` — only build artifacts, replaced by `UserInterface7.0`
- `FincaFenix.Validators/` — redundant with `FincaFenix.Validations/` (keep Validations)
- `FincaFenix.Repositories/UniversitarySystem.EFCore` — .NET 8, references missing project, unrelated

## IA/ Directory

| File | Purpose |
|---|---|
| `IA/STACK.md` | Full technology stack analysis |
| `IA/BACKEND_REFACTORING_PLAN.md` | Phased refactoring plan (.NET 9, MediatR, AutoMapper, JWT, React migration) |
| `IA/WebAPI_IMPLEMENTATION.md` | Web API standalone + Swagger implementation plan |
| `IA/ADD_POLICIES.md` | Policy-based authorization implementation plan |
