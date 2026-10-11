# AGENTS.md — FincaFenix

## Build & Run

```powershell
dotnet build FincaFenix.sln
dotnet run --project FincaFenix.WebApi             # http://localhost:5000 | https://localhost:5001 (Swagger)
```

Frontend (Next.js) — en `FincaFenix.Frontend`:

```powershell
npm install
npm run dev                                        # http://localhost:3000 (proxy /api → WebAPI)
npm run build                                      # typecheck + build SSR
```

> La UI Blazor (`FincaFenix.UserInterface7.0`) y `FincaFenix.ViewModels` fueron **eliminadas en Fase 6 (Oct 2026)**. La UI de front es ahora **Next.js 15 (App Router) + TailwindCSS v4 + shadcn/ui** en `FincaFenix.Frontend` (migrada desde la SPA Vite, Oct 2026). Ver `IA/DOCUMENTACION_UI.md`.

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
5. **Rename** `UpdateWorkOrderRepositor` → `UpdateWorkOrderRepository`. **`UpdateWorkOrder()` quedaba sin implementar a propósito** (futuro caso de edición de órdenes) — **implementado en Fase 5** (ver más abajo).
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

## Completed — Inventario Fase 1/2 + Divisas (Oct 2026)

1. **Fase 1 (modelo + migración):** 7 tablas nuevas (`UnidadMedida`, `StockPorFinca`, `ReservaMaterial`, `MovimientoInventario`, `CostoOrdenTrabajo`, `Consumo`, `Divisa`); `Material` + `CostoReferencia`/`IdUnidadMedida` (nullable) + `IdDivisa`. Una sola migración aditiva `20261009010827_AddInventoryModule` (aplicada).
2. **Divisas:** `CurrencyEntity` (tabla `Divisa`) con seed `HasData` (1=ARS, 2=USD, 3=EUR, 4=JPY, 5=GBP). `IdDivisa` **NOT NULL** en `Material`, `MovimientoInventario`, `CostoOrdenTrabajo` y `Consumo`; en `Material` con `default 1` para backfill de filas existentes. `GET api/inventory/currencies` (policy `STOCK_READ`). Conversión/cotizaciones **fuera de alcance v1** (futuro: tabla `CotizacionDivisa` + proveedor de cotización; ver `IA/INVENTARIO_MODELO_ACTUAL_VS_OBJETIVO.md`).
3. **Fase 2:** endpoints de stock (`stock/farm/{farmId}`, `stock/consolidated`, `stock/low`, `stock/zero`), `POST api/inventory/movement` transaccional (sin stock negativo; valida material/finca/divisa → 404, reglas → 422 vía `BusinessRuleException`). Actualiza `Material.CostoReferencia` + `Material.IdDivisa` cuando se informa `unitCost`.

## Completed — Inventario Fase 3 (reserva blanda + costo congelado — Oct 2026)

1. `IWorkOrderInventoryCommand` (`Gateways/Interfaces/CommandServices/WorkOrder/`) + impl `WorkOrderInventoryCommand` (`EFCore/Services/CommandServices/WorkOrder/`), registrado Transient.
2. Hook en `CreateWorkOrderRepository.CreateWorkOrder` (misma transacción, tras persistir la OT): si `workOrder.Recipe.DetailRecipeList` tiene ítems → `RegisterReservationsAndCostsAsync`. OT **sin receta → sin reservas** (compatibilidad total).
3. Por cada `DetailRecipe` con `EstimatedAmount > 0`: valida `StockDisponible = StockFisico − StockReservado` de la **Finca de la OT** (regla 27.21); insuficiente → `BusinessRuleException` → **422** y rollback total. Si alcanza: crea `ReservaMaterial` (Estado=Activa) e incrementa `StockReservado`.
4. Costo congelado: crea `CostoOrdenTrabajo` por material con `CostoUnitario = Material.CostoReferencia ?? 0`, `IdDivisa = Material.IdDivisa`, `CantidadPlanificada = EstimatedAmount`, `CostoTotal = CostoUnitario × CantidadPlanificada`, `FechaCongelado = now` (snapshot inmutable RN-COSTO-ORDEN).
5. **Concurrencia:** lock pesimista `StockPorFinca WITH (UPDLOCK, HOLDLOCK)` por Finca dentro de la transacción (serializa la creación de OTs de la misma Finca y re-valida con datos frescos). Se **omitió el retry** por `DbUpdateConcurrencyException` del plan: el lock evita la carrera; documentar si se agrega retry en el futuro.
6. La cantidad planificada de reserva/costo es `DetailRecipeEntity.EstimatedAmount` (plan total por OT provisto por el cliente).

Verificado (build 0 errores + smoke test con login real): divisa en ingreso `unitCost=15`/`currencyId=2` → movimiento/material actualizados; `currencyId=999→404`, `0/ausente→422`; OT con receta `estimatedAmount=2` (disponible 3) → reserva 2 + `StockReservado=2` + cost snapshot 15/30; OT `estimatedAmount=5` (disponible 1) → **422** sin filas parciales (rollback); OT sin receta → OK sin reservas. **Sin commits aún.**

## Completed — Inventario Fase 4 (consumo — Oct 2026)

1. `IWorkOrderConsumptionCommand` + `WorkOrderConsumptionCommand` (`EFCore/Services/CommandServices/WorkOrder/`, Transient): recálculo server-side del consumo por cada `addDetailWO`.
2. **Fórmulas corregidas** (bug del ViewModel histórico): `dosisNormalizada = AmountRequired/1000` si unidad `cc|gr`, si no `AmountRequired`; `ConsumoReal (acum) = (Σ Performance) × dosisNormalizada` sobre todas las `DetailWorkOrder` de la OT. *(Corregido nuevamente en "Unidades de medida base": la conversión a unidad base pasa por `UnitConverter`, ya sin hardcodear /1000.)*
3. Hook en `AddDetailWorkOrderRepository` (ahora transaccional con `IUnitOfWork`): guarda la actividad → recálculo por cada material de la receta → commit; rollback total si falla.
4. Por material: `delta = nuevoTotal − Consumo.CantidadConsumida`; primero contra `ReservaMaterial` activa (`CantidadConsumida += aplicado`, Estado→`Consumida` si se agota), luego contra `StockDisponible = StockFisico − StockReservado` (**lock pesimista `UPDLOCK, HOLDLOCK` por Finca**); exceso queda registrado sin aplicar → nunca stock negativo, nunca se rechaza la actividad. `MovimientoInventario` `SalidaConsumo` (Origen=OrdenTrabajo, costo congelado) solo si `aplicado > 0`. `ConsumptionEntity` (unique WorkOrderId+MaterialId) con CantidadConsumida/Resulting, CantidadAplicada acumulada, Origen `ConsumptionOriginEnum.Calculado`.
5. **Endpoint manual** `POST api/inventory/consumption` (policy `CONSUMPTION_CREATE`, `ConsumptionOriginEnum.Manual`, transacción propia): `RegisterConsumptionDTO` → handler `RegisterConsumptionCommand` → repo → command service. 404 si no existe OT/material; 422 por validación (`RegisterConsumptionCommandValidator`).
6. `GetWorkOrderByIdHandler` ahora puebla `DetailRecipeDTO.TotalAmountConsumed` desde `Consumo` (vía `GetConsumedAmountsByWorkOrderAsync` en el query service de WorkOrder).

Verificado (build 0 errores + smoke test OT 57 receta 33/mat 9, `AmountRequired=1 lts`, reserva 2/stock 3): addDetailWO Performance=1 → consume 1 contra reserva; 2º → reserva `Consumida` + stock 3→1; 3º → 1 contra stock disponible (stock 1→0); 4º → **exceso** (consumido 4, aplicado 3, sin movimiento, actividad aceptada); ingreso 5 → manual `amount=2` → aplicado 2 (stock 5→3, `SalidaManual`/`Manual`); `workOrderId/materialId` inexistentes → 404; `amount=0` → 422; `getCompleteInfoWorkOrder/57` → `TotalAmountConsumed=6`. **Sin commits aún.**

## Completed — Inventario Fase 5 (ciclo de vida de la OT — Oct 2026)

1. `WorkOrderStatusEnum` (`Pendiente`, `Activo`, `Cerrado`, `Cancelado`) + `AllowedTransitions` en `UpdateWorkOrderCommand` (EFCore). `Pendiente→{Activo,Cerrado,Cancelado}`, `Activo→{Cerrado,Cancelado}`, `Cerrado`/`Cancelado` terminales. Transición inválida o estado vacío → `BusinessRuleException` → **422**; OT inexistente → **404**.
2. **`Cerrado`**: setea `EndDate` y libera reservas activas sobrantes (regla 27.22) vía `ReleaseReservationsAsync` (nuevo en `IWorkOrderInventoryCommand`): por cada `ReservaMaterial` Activa decrementa `StockReservado` en `CantidadReservada − CantidadConsumida`, pasa a `Liberada` y setea `FechaLiberacion`. No altera `StockFisico`.
3. **`Cancelado`** (estado de negocio, ≠ borrado lógico `Eliminado`): rechaza (422) si la OT tiene `DetalleOrdenTrabajo` o consumos; libera reservas; sin movimiento físico.
4. **`UpdateWorkOrder`** implementado (antes `NotImplementedException`): `PUT api/workorder/updateworkorder` con `UpdateWorkOrderDTO` (edita tarea/descripción/fechas/área + receta). Ajusta reservas por material (§15): aumentar → valida `StockDisponible` de la Finca (insuficiente → 422 rollback); disminuir → libera sobrante. Si la OT no tenía receta, la crea y registra reserva+costo. Detalles re-mapeados con `GroupItems`. Costos congelados no se recalculan. Todo transaccional (`IUnitOfWork`).
5. Wiring: interfaces (`IUpdateWorkOrderCommand`/`IUpdateWorkOrderRepository`/`IUpdateWorkOrderController`), handler + `UpdateWorkOrderCommandValidator`, endpoint en `WorkOrderController`. Mapa `RecipeWorkOrderDTO → RecipeEntity` en `MappingProfile`.

Verificado (build 0 errores + smoke real): OT con receta 2 → reserva 2/stock 3; edición a 3 (+1) → reserva 3/`StockReservado` 3; a 5 (+2, disponible 0) → **422** rollback; a 1 (−2) → reserva 1/`StockReservado` 1; `Cerrado` → reserva `Liberada`, `StockReservado` 0, `FechaFin`; `Cancelado` de OT con actividades → **422**; transiciones `Cerrado→Activo` y `Cancelado→Activo` → **422**; OT inexistente → **404**; OT nueva con receta → `Cancelado` libera reserva. **Sin commits aún.**

## Completed — Unidades de medida base (Oct 2026)

1. **Cada material tiene una unidad de medida base** = única unidad de stock, reservas, movimientos, consumos y costos. Si un mismo material debe medirse con otra unidad, se registra un material duplicado con la unidad cambiada (decisión de negocio). `Material.IdUnidadMedida` sigue **nullable** (se hará NOT NULL cuando exista el CRUD de materiales).
2. **Maestro `UnidadMedida`** seedeado vía `HasData` en `UnitOfMeasureConfiguration` (migración `AddUnitOfMeasureSeed`): 1=Kilogramo, 2=Litro, 3=Unidad, 4=Metro, 5=Bolsa, 6=Caja, 7=Gramo, 8=Centímetro cúbico.
3. **Backfill idempotente** `SeedDataBase.SeedInventoryUnitsAsync` (arranque WebAPI + `UseAsyncSeeding`): asigna `IdUnidadMedida` a los materiales sin unidad — por la **unidad de receta más frecuente** (`UnidadCantEstimada`, vía `UnitConverter`) y, si no hay receta, por categoría (Herbicidas/Insecticidas/Correctores/Coadyuvantes→Litro; Fertilizantes/Fungicidas/Enmiendas→Kilogramo; default→Unidad). 59/59 materiales asignados.
4. **`UnitConverter`** (`FincaFenix.Entities/Units`): conversión centralizada por familia (Masa kg/gr · Volumen lts/cc · Unidad · Metro · Paquete) con `Normalize` (lts/lt/litro→lts, cc/ml→cc, kg/kilo→kg, etc.). Unidad desconocida o **familia incompatible → `BusinessRuleException` → 422** con mensaje claro.
5. **Aplicado en:** reservas+costos (`WorkOrderInventoryCommand.ToBaseUnit`), ajuste de reservas (§15) en `UpdateWorkOrderCommand.AdjustReservationsAsync`, consumo automático y manual (`WorkOrderConsumptionCommand`, corrección del bug histórico: `dose = AmountRequired` sin hardcodear /1000; antes `NormalizeDose` asumía base lts|kg). `Consumo.Unidad` ahora guarda el **código base** (p.ej. `lts`).
6. **Expuesto en respuestas:** `MaterialRecipeDTO.UnitOfMeasure` (nullable, para no volverse `required` en requests), `ConsumptionResultDTO.UnitOfMeasure`, `StockDTO.UnitOfMeasure` (ya existía). `.Include(m => m.UnitOfMeasure)` en `MaterialQueryService`.
7. **Materiales con datos conflictivos** (usan volumen y masa en distintas recetas): **material 9** (base Litro; receta 30/NumReceta 18 en kg) y **material 58** (base Kilogramo; receta 14/NumReceta 2 en lts). Crear/consumir esas recetas lanzará 422 hasta curar los datos (crear el material duplicado).

Verificado (build 0 errores + smoke real): OT con receta `estimated=500 cc` (base lts) → reserva `0.5` lts + costo `0.5`; `addDetailWO` → consume 0.5 contra reserva, `Consumo.Unidad='lts'`; manual `1000 cc` → 1 lts aplicado, respuesta `unitOfMeasure:'lts'`; manual `1 kg` → **422** "incompatibles"; creación de OT con receta en kg para material 9 → **422**. **Sin commits aún.**

## Completed — Inventario Fase 6 (costos históricos — Oct 2026)

1. **Tres consultas de costos (solo lectura)** en `InventoryController` siguiendo el patrón Query/Handler/Repo passthrough (`InventoryQueryService`):
   - `GET api/inventory/costs/current` (policy `STOCK_READ`): **costo actual/referencia** por material desde `Material.CostoReferencia` + divisa (`CurrencyId/Code/Symbol` via `Currency`) + unidad base. Incluye materiales sin costo (`ReferenceCost` null).
   - `GET api/inventory/costs/material/{materialId}/history` (policy `MOVEMENT_READ`): **evolución de costos** desde `MovimientoInventario` con `CostoUnitario` no nulo, ordenada por fecha desc (muestra cada operación con monto, costo unitario, total, finca, fecha). Incluye el costo actual + divisa. Material inexistente → **404**.
   - `GET api/inventory/costs/workorder/{workOrderId}` (policy `WORKORDER_READ`): **costo congelado por OT** desde `CostoOrdenTrabajo` (material, `CantidadPlanificada`, `CostoUnitario`, `CostoTotal`, divisa, `FechaCongelado`) + `TotalCost` de la OT. OT inexistente → **404**; OT sin receta → items vacíos.
2. **DTOs** nuevos en `FincaFenix.Entities/DTOs/InventoryDTOs/CostDTOs/` (una clase por archivo, namespace `FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs`): `CurrentMaterialCostDTO`, `CostHistoryDTO` + `CostHistoryItemDTO`, `WorkOrderCostDTO` + `WorkOrderCostItemDTO`.
3. Se **reutilizaron policies existentes** (no se agregó `COST_READ`): `STOCK_READ`/`MOVEMENT_READ`/`WORKORDER_READ` cubren la lectura para desarrollador/admin/supervisor.

Verificado (build 0 errores + smoke real): `costs/current` 59 materiales; `costs/workorder/62` → orderNum 34, 1 item (material 9, 0.5 × 20 = 10 USD); `costs/material/9/history` → 8 operaciones con costo desc (último 20 USD); `material/999999/history` y `workorder/999999` → **404**. **Sin commits aún.**

## Completed — Inventario Fase 7 (verificación — Oct 2026)

1. **Build** 0 errores.
2. **Concurrencia** (§16/§18): material 14 (5 lts disponibles), dos `createworkorder` simultáneas de 4 lts c/ú → una **200** (reserva 4, disponible 1) y otra **422** ("Disponible 1, requerido 4"); el perdedor **no dejó estado parcial** (solo 1 OT/reserva/costo). El lock `StockPorFinca WITH (UPDLOCK, HOLDLOCK)` serializa y la re-validación usa datos frescos. *Nota: se omitió el retry por `DbUpdateConcurrencyException` — el lock evita la carrera.*
3. **Prueba numérica de consumo** (OT 65: dosis 0.5 lts, plan 10, `AmountRequired=0.5`): `addDetailWO` perf 2 → consumo 1.0; perf 3 → `Σ Performance (5) × dosis (0.5) = 2.5`; `StockFisico` 51.5→49.0, `StockReservado` 10→7.5, 2 `MovimientoInventario` `SalidaConsumo` (1.0 + 1.5, costo congelado 20 USD); `getCompleteInfo/65` → `recipe.details[0].totalAmountConsumed=2.5`.
4. **Regresión OK:** login; create sin receta (OT 64) y con receta (OT 65, reserva 10); `addDetailWO`; `GET api/material/recipe/35/material`; paginada (`items`, `totalCount=34`, 7 páginas); `updatestateworkorder` (Activo→Cerrado `true` + `FechaFin`; Cerrado→Activo **422**); PDF (`application/pdf`, 62596 bytes, firma `%PDF-`).
5. **Checklist §39: 20/20 criterios satisfechos** (detalle en `IA/PLAN_EJECUCION_MODULO_INVENTARIO.md`).

**Módulo de Inventario completo (Fases 1–7 + unidades base).** Sin commits aún.

## Completed — CRUD de Materiales, Categorías y Unidades de Medida (Oct 2026)

Alcance: CRUD de **Material + Categoría de material + Unidad de medida**. Divisa queda de solo lectura. Plan: `IA/PLAN_EJECUCION_CRUD_MATERIALES.md`.

1. **Endpoints:**
   - `MaterialController` (`api/material`): `GET {id}`, `GET paged?pageNumber&pageSize&categoryId&search&includeDeleted`, `POST`, `PUT`, `DELETE {id}` (lógico), además de los 3 GET existentes. Clase pasó de `[Authorize(Policy=MATERIAL_READ)]` a `[Authorize]` + policy por acción.
   - `MaterialCategoryController` (`api/materialcategory`): `GET {id}`, `POST`, `PUT`, `DELETE {id}`.
   - `UnitOfMeasureController` (`api/unitofmeasure`, nuevo): CRUD completo.
2. **Borrado lógico del material** (`Eliminado=true`): no aparece al elegir material para una receta (`getmateriallist` y `category/{id}/material` filtran `!IsDeleted`), pero **sí** se devuelve en `GET material/{id}` y en `recipe/{recipeId}/material` (histórico). Categoría: DELETE **físico con guarda** → **422** si tiene materiales. Unidad: baja lógica (`IsDeleted=true`) y su listado la excluye.
3. **Policies:** se agregaron `UNIT_OF_MEASURE_CREATE/READ/UPDATE/DELETE` a `PolicyMaster` (admin/desarrollador las reciben por reflexión) y `UNIT_OF_MEASURE_READ` a `supervisorPolicies` en `SeedDataBase`.
4. **DTOs** (una clase por archivo en `FincaFenix.Entities/DTOs/InventoryDTOs/MaterialDTOs/`): `MaterialDTO`, `CreateMaterialDTO`, `UpdateMaterialDTO`, `MaterialFilterDTO`, `UnitOfMeasureDTO`, `SaveUnitOfMeasureDTO`, `SaveMaterialCategoryDTO`. Campos opcionales nullable (evita el `required` implícito de `[ApiController]` en query/body).
5. **Capas:** `IMaterialRepository`/`IMaterialCategoryRepository`/nuevo `IUnitOfMeasureRepository` (Gateways) + command services `IMaterialCommandService`/`IMaterialCategoryCommandService`/`IUnitOfMeasureCommandService` (EFCore, con `IUnitOfWork`). Núcleo de orquestación en `MaterialCommandService` (valida categoría/unidad/divisa → 404). Handlers MediatR + validadores FluentValidation (422). Mapas nuevos en `MappingProfile` (validados por `AssertConfigurationIsValid`).
6. **`IdUnidadMedida` NOT NULL:** migración `20261009042528_MakeMaterialUnitOfMeasureRequired` (con backfill defensivo `IdUnidadMedida = 3`). La propiedad CLR sigue `int?` para no romper las conversiones de unidades existentes; el required se valida en FluentValidation.
7. **Fix preexistente:** `GET api/material/category/{id}/material` devolvía **500** (`Include` tras `GroupBy`+`Select`). Reescrito con `Include` antes del `Where` + agrupación en memoria → 200.

Verificado (build 0 errores + smoke real): creación/edición/baja de material, categoría y unidad; paginado y filtros (`categoryId`, `search`, `includeDeleted`); `422` en validación de negocio (unidad 0, borrar categoría con materiales), `404` en referencias/material inexistente; baja lógica visible en `getmateriallist` (excluido) vs `GET {id}`/`recipe/{id}/material` (incluido); regresión OK (`getmateriallist`, `recipe/{id}/material`, `stock/consolidated`, `costs/current`, `currencies`, `workorderlistpaginated`). **Sin commits aún.**

## Completed — Rendimiento de OT (actividades + operarios — Oct 2026)

Plan: `IA/PLAN_EJECUCION_RENDIMIENTO_OT.md`.

1. **Modelo / migración** `20261009154605_AddWorkOrderRendimiento` (aplicada, datos preservados):
   - `DetailWorkOrderEntity.Performance` → **`MachinePasses`** (columna `DetalleOrdenTrabajo.Rendimiento` → `Maquinadas`, `decimal(18,5)`). Ya no es un rendimiento, es la **cantidad de maquinadas** (cargas de tanque), usada solo en la fórmula de consumo.
   - `DetailWorkOrderEntity.ProducedAmount` (`decimal?`, col `CantidadProducida`): kg cosechados.
   - `TaskEntity.RendimientoMode` (`RendimientoModeEnum`, col `Tarea.ModoRendimiento`, int, default 0).
2. **`RendimientoModeEnum`** (`FincaFenix.Entities/Enum`): `ManHours=0`, `MaterialEfficiency=1`, `AreaPerManHour=2`, `OutputPerManHour=3`. **El tipo de tarea manda sobre la presencia de receta.**
3. **El rendimiento se calcula en tiempo de consulta (NO se persiste)** vía `RendimientoCalculator` (`FincaFenix.Entities/Rendimiento`), expuesto por actividad y agregado por OT:
   - `MaterialEfficiency`: `TheoreticalMachinePasses = AreaTotal × TRV / VolumeMachine`; `RealMachinePasses = Σ MachinePasses`; `% Eficiencia = teóricas / reales × 100` (>100% = se usó menos, mejor). Por material: `theoreticalRaw = TheoreticalMachinePasses × AmountRequired` → **`UnitConverter` a la unidad base** del material; `EfficiencyFromAmounts(theoreticalBase, TotalAmountConsumed)`. `AreaTotal` = `Σ SectorFarm.Area`. División indeterminada (TRV/Volumen ≤ 0) → `null`.
   - `AreaPerManHour`: `ΣÁrea / ΣHoras` (ha/h). `OutputPerManHour`: `ΣProducedAmount / ΣHoras` (kg/h). `ManHours` (default): horas (h).
4. **Seed idempotente** `SeedDataBase.SeedTaskRendimientoModesAsync` (arranque WebAPI + `UseAsyncSeeding`, con `NormalizeTaskName` sin acentos): tareas **1/2/6/19 → `MaterialEfficiency`**, **3 (Cosecha) → `OutputPerManHour`**, resto `ManHours`. `TaskController` es read-only → los modos se setean por seed.
5. **Validación condicional por modo:** `WorkOrderValidator` (receta obligatoria para `MaterialEfficiency`) y `AddDetailWorkOrderCommandValidator` (inyecta `IGetWorkOrderInformationRepository`; `MachinePasses>0` para `MaterialEfficiency`, `ProducedAmount>0` para `OutputPerManHour`). Fallas → **422** vía `ValidationBehavior` + `ExceptionMiddleware`.
6. **DTOs/mapas:** `ActivityWorkOrderDTO` (+`MachinePasses`,`ProducedAmount`,`AreaWorked`,`Rendimiento`,`RendimientoUnit`,`RendimientoMode`), `RecipeWorkOrderDTO` (+`TheoreticalVolume`,`RealVolume`,`TheoreticalMachinePasses`,`RealMachinePasses`,`Rendimiento`), `DetailRecipeDTO` (+`TheoreticalAmount`,`Rendimiento`), `ShowWorkOrderDTO` (+`TotalManHours`,`TotalProducedAmount`,`Rendimiento`,`RendimientoUnit`,`RendimientoMode`), `TaskDTO.RendimientoMode` (string). `WorkOrderPDF` columna "Rendimiento" → **"Maquinadas"**. `DetailWOQueryService` proyecta `MachinePasses`/`ProducedAmount`/`Area`/`RendimientoMode`.

Verificado (build 0 errores + smoke real): OT 62 (`MaterialEfficiency`, receta Vol100/TRV1, área 2.6) → `theoreticalMP=0.026`, `realMP=1`, `rend=2.6%`; mat 9 `500cc→0.013 lts`, `0.013/1.5=0.87%`; crear OT de `MaterialEfficiency` sin receta → **422**; addDetailWO sin `machinePasses`/sin `producedAmount` → **422**; OT 68 (Cosecha, sin receta) `prod=500, h=5` → `mode=OutputPerManHour`, `rend=100 kg/h` (por OT y por actividad); PDF OT 62 → `application/pdf` (62299 bytes). **OT 68 se dejó como dato de prueba. Sin commits aún.**

Próxima etapa planificada (futuro): módulo de compras a proveedores y módulo dedicado de cosecha.

## Completed — CRUD de Tareas (Tarea — Oct 2026)

Alcance: CRUD completo de `TaskEntity` (tabla `Tarea`), incluyendo la edición del `RendimientoMode`. Plantilla: CRUD de MaterialCategory.

1. **Endpoints** (`TaskController`, `api/task`): `GET getTaskById/{id}` y `GET GetTaskList?includeDeleted` (existían), nuevos **`POST`**, **`PUT`**, **`DELETE {id}`**. Clase pasó de `[Authorize(Policy=TASK_READ)]` a `[Authorize]` + policy por acción (`TASK_CREATE/READ/UPDATE/DELETE` ya existían en `PolicyMaster`; admin/desarrollador las reciben por reflexión, supervisor tiene `TASK_READ`) — **sin cambios de seed**.
2. **Baja lógica** (`Eliminado=true`, igual que Material/Unidad): `GetTaskList` excluye eliminadas por defecto (`Where(includeDeleted || !IsDeleted)`); `includeDeleted=true` las incluye. `GET getTaskById/{id}` devuelve también eliminadas. **`TaskQueryService.Exists` filtra `!IsDeleted`** → crear una OT con tarea desactivada → **422** ("La tarea seleccionada no existe en la base de datos") vía `WorkOrderValidator`.
3. **`SaveTaskDTO`** (`FincaFenix.Entities/DTOs/WorkOrderDTOs/`): `Id?`, `Description`, `RendimientoMode` (string). Validadores Create/Update: descripción obligatoria ≤ 100 y modo obligatorio parseable a `RendimientoModeEnum` → **422**. Mapa `SaveTaskDTO → TaskEntity` con parse tolerante (fallback `ManHours`).
4. **Capas:** `ITaskCommandService`/`TaskCommandService` (`Gateways/Interfaces/CommandServices/Tasks` y `EFCore/Services/CommandServices/Tasks`, con `IUnitOfWork`; Create resetea `Id=0` + `IsDeleted=false`, Update toca Descripción/Modo, Delete soft con **404** si no existe). `ITaskRepository`/`TaskRepository` + `ITaskQueryService`/`TaskQueryService.GetTaskList(bool includeDeleted)`.
5. **Gotcha de naming:** el namespace `...CommandServices.Task` provoca **CS0118** (`'Task' es espacio de nombres pero se usa como tipo`) en todos los interfaces hermanos del proyecto — se usa **`Tasks`** (plural).

Verificado (build 0 errores + smoke real): listado 31 tareas; create → id 32 (`ManHours`); create modo inválido/descripción vacía → **422**; update → `MaterialEfficiency`; delete → `true` y `Eliminado=1` en DB; `GetTaskList` sin eliminadas (smoke ausente) vs `includeDeleted=true` (smoke presente); `getTaskById/32` (eliminada) → 200; delete/update inexistentes → **404**; regresión `getCompleteInfo/68` → `task(id=3 Cosecha)`; sin token → 401. **Task 32 quedó como dato de prueba (soft-deleted). Sin commits aún.**

## Completed — Migración UI a Next.js + Tailwind + shadcn/ui (Oct 2026)

Plan: `IA/PLAN_MIGRACION_NEXT_TAILWIND.md`. La SPA Vite + CSS Modules se reescribió **in-place** en `FincaFenix.Frontend` como **Next.js 15 (App Router, SSR shell-only)** + **TailwindCSS v4** + **shadcn/ui** + **lucide-react** + **Sonner**.

1. **Stack:** eliminados Vite, React Router, Font Awesome, `next-themes` y los 29 `*.module.css`. `next.config.ts` con `rewrites` `/api/*` → `API_PROXY_TARGET` (default `http://localhost:5000`; cookie first-party, sin CORS browser). `@tanstack/react-table` fijado en **v8.21.3** (la v9 tiene API incompatible con el Data Table block).
2. **SSR auth:** `(app)/layout.tsx` (server) usa `cookies()` + `GET /api/auth/me` (`src/lib/server/get-current-user.ts`, `cache:"no-store"`) → `redirect("/login")` o `<Providers initialUser>`. Tema por `[data-theme]` con script anti-flash.
3. **Componentes:** 12 comunes reescritos preservando API (incl. `DataTable` v8, `Button` con `icon: LucideIcon`); 17 primitivas shadcn en `src/components/ui`; chrome `AppShell`/`Sidebar`/`AppBar` (sidebar colapsable + drawer móvil; chrome siempre oscuro vía tokens `--chrome-*`).
4. **Features migradas:** Login, Dashboard, NotFound, WorkOrderList/Table/Filters, WorkOrderDetail/Header, CreateWorkOrder, AddActivityForm, y tabs Recipe/Activities/Consumption/Costs/Performance.
5. **Fase 3 (15 correcciones UI):** aplicadas (TRV=lts/ha, volumen máquina fijo lts, sectores select+chips, receta Categoría→Material, sin selector de modo en tarea, `parseDecimal()` es-AR, columnas numéricas a la derecha, botón Volver, dropdown de usuario, etc.).
6. Un artefacto del CLI de shadcn (`import { cn } from "cn"`) se corrigió a `@/lib/utils` en todos los `ui/*`; se agregó `src/types/globals.d.ts` (`declare module "*.css"`) por `noUncheckedSideEffectImports`.

Verificado: `npm run build` → **0 errores** (8 rutas generadas). Doc viva: `IA/DOCUMENTACION_UI.md`. **Sin commits aún.**

## Completed — UI Inventario (Next.js) (Oct 2026)

1. **Tipos** (`src/types/api/inventory.ts` + re-export `index.ts`): `CurrencyDTO`, `RegisterMovementDTO`, `MovementResultDTO`, `CurrentMaterialCostDTO`, `CostHistoryDTO`, `CostHistoryItemDTO`.
2. **`inventory.service.ts`** extendido: `getZeroStock`, `getCurrencies`, `getCurrentCosts`, `getMaterialCostHistory`, `registerMovement` (además de los 3 de stock existentes).
3. **`features/inventory/constants.ts`**: `INVENTORY_MOVEMENT_TYPES` (`Ingreso`/`AjustePositivo`/`AjusteNegativo` — los 3 que acepta el backend), `movementTypeLabel`, `movementOriginLabel`.
4. **`features/inventory/components/MovementForm.tsx`**: modal (material/finca de servicios existentes, tipo, cantidad, costo unitario/divisa opcional, stock mínimo, observaciones) → `POST /api/inventory/movement`; validaciones con `parseDecimal` (es-AR).
5. **`features/inventory/pages/Inventory.tsx`** + ruta `src/app/(app)/inventory/page.tsx` (con `<Suspense>`): KPIs + 4 tabs (`Stock` con filtro de finca consolidado/por finca + buscador; `Stock bajo`; `Sin stock`; `Costos` con modal de historial). Tab sincronizado con `?tab=` vía `history.replaceState`. Botón "Registrar movimiento" gated por `MOVEMENT_CREATE`; acceso a la página por `STOCK_READ`.
6. **Sidebar** ahora filtra por policy: se agregó el ítem "Inventario" (`Boxes`, policy `STOCK_READ`).

Verificado: `npx tsc --noEmit` y `npm run build` → **0 errores** (9 rutas, incl. `/inventory`). **Sin commits aún.**

## Completed — Fixes UI creación de OT + cantidades automáticas + fecha de actividad (Oct 2026)

1. **Fix 400 al crear OT** ("The Machine/NumRecipe/Farm/Material field is required."): causa raíz `FincaFenix.Entities` tiene `<Nullable>enable</Nullable>` (único proyecto así; AGENTS decía mal que todos eran `disable`) → MVC agrega `[Required]` implícito a props de navegación no-nullable que el front no envía. Fix en `FincaFenix.WebAPI/Program.cs`: `AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)`.
2. **Fix "Sin categoría"**: `MaterialQueryService.GetMaterialList()` ahora hace `.Include(m => m.Category)` (antes faltaba; el filtro por categoría del `CreateWorkOrder` mostraba todo como "Sin categoría").
3. **Cantidad estimada automática** (`CreateWorkOrder.tsx`): el usuario carga solo **Cant. requerida** + **unidad** (`lts`/`kg`/`gr`/`cc`); la **Cant. estimada** = `Maquinadas teóricas × Cant. requerida` (`áreaTotal × TRV / volumenMáquina` × dosis), **en la misma unidad de la dosis** (sin conversión local; el backend convierte a unidad base al reservar). Se muestra de solo lectura; en el submit se envía `estimatedAmount` + `estimatedAmountUnit`.
4. **Unidad del material**: `getMaterialCategories()` en `material.service.ts`; categorías del combo desde `GET /api/materialcategory/getCategories` (+ `MaterialCategoryDTO`).
5. **Sectores como tabla** (`CreateWorkOrder.tsx`): se quitó el chip `Badge`; los seleccionados van a `DataTable` (Sector, Fruta, Variedad, Plantas, Superficie, quitar) con total de área.
6. **`MovementForm.tsx`**: nuevo select **Categoría** → el select **Material** queda filtrado (y deshabilitado hasta elegir categoría).
7. **Fecha de actividad automática** (`AddActivityForm.tsx`): se eliminó el campo "Fecha y hora"; `activityDate = new Date().toISOString()` al registrar. Helper `nowLocal()` borrado.
8. **Limpieza**: eliminada `getUnitsMesure()` (rota, `TS2304 UnitMesureDTO`) de `workOrder.service.ts`.

Verificado: `npx tsc --noEmit` + `npm run build` → **0 errores** (9 rutas) y `dotnet build FincaFenix.WebAPI` → 0 errores. **Sin commits aún.**

## Completed — UI inventario: reservas, costos paginados, unidades en movimiento (Oct 2026)

1. **Unidad estimada = unidad de la dosis** (`CreateWorkOrder.tsx`): `computeEstimatedAmount(row, passes)` ya no convierte a la unidad base del material; devuelve `dose × passes` en `row.amountRequiredUnit`. Se eliminaron `UNIT_FAMILIES`/`convertToBaseUnit`. La "Cant. estimada" y su "Unidad" (read-only) reflejan la unidad de la dosis.
2. **Unidad del material en el movimiento** (`MovementForm.tsx`): los labels **Cantidad**, **Costo unitario** y **Stock mínimo** muestran dinámicamente la unidad base del material seleccionado (p.ej. `Cantidad (lts)`, `Costo unitario (lts)`), con hint "En la unidad base del material". El backend guarda `Amount`/`UnitCost` en unidad base (sin conversión).
2.C **Reservas por material** (nuevo dialog en tab **Stock** de Inventory):
   - Backend: DTOs `ReservationItemDTO` / `MaterialReservationsDTO` (`DTOs/InventoryDTOs/ReservationDTOs/`); `IInventoryQueryService.GetMaterialReservationsAsync` + repo passthrough + handler MediatR `GetMaterialReservationsQuery`; endpoint `GET api/inventory/reservations/material/{materialId}` (policy `STOCK_READ`; material inexistente → **404**). Proyecta desde `MaterialReservation` con OT (`OrderNum`,`Status`) y Finca, más `PendingAmount`.
   - Frontend: `getMaterialReservations()` + botón **Reservas** en cada fila de la pestaña Stock → `Modal` con tabla (Orden, Estado OT, Finca, Reservado, Consumido, Pendiente, Reserva, Fecha).
3. **Costos paginados + filtros** (`Inventory.tsx` pestaña Costos): `CurrentMaterialCostDTO` + `CategoryId`/`CategoryName`; `GetCurrentMaterialCostsPagedAsync` (filtros `pageNumber/pageSize/search/categoryId`, reusa `MaterialFilterDTO`) + endpoint `GET api/inventory/costs/current/paged` (policy `STOCK_READ`) devolviendo `PagedResult<CurrentMaterialCostDTO>`; UI con select de categoría, buscador, `DataTable` y `Pagination` (página 20).
4. **Backdrop blur**: `backdrop-blur-sm` en `DialogOverlay` (`ui/dialog.tsx`) y `AlertDialogOverlay` (`ui/alert-dialog.tsx`) → todos los `Modal`/alertas difuminan el fondo.

Verificado: `dotnet build` (EFCore + Controllers) 0 errores; `npx tsc --noEmit` + `npm run build` → **0 errores** (9 rutas). **Sin commits aún.**

## Completed — Ajustes UI módulo de OT (5 puntos — Oct 2026)

1. **Unidad de dosis acotada a la familia del material** (`CreateWorkOrder`): nuevo `FincaFenix.Frontend/src/features/materials/material.utils.ts` (`unitCodeFor`, `unitFamilyOf`, `materialLabel`, tipo `UnitFamily`); `DOSE_UNITS_BY_FAMILY` (volumen→`lts`,`cc`; masa→`kg`,`gr`; unidad→`unidad`; longitud→`metro`; paquete→`bolsa`,`caja`; material sin unidad base→todas). El `<SelectInput>` de dosis sólo lista `rowDoseUnits`; el backend ya valida la familia vía `UnitConverter` (**422** si es incompatible).
2. **Desplegables de material enriquecidos**: `materialLabel(m)` = `CommercialName · ArticleName (unidad)` (tolera faltantes); aplicado en la receta de `CreateWorkOrder` (donde `ArticleName` es el **principio activo**) y en `MovementForm`.
3. **Área trabajada**: se eliminó la columna y el total "Área trabajada" de `ActivitiesTab` (el backend no persiste área por actividad → siempre 0). Fix del "Área total = 0" en Rendimiento: `CreateWorkOrder` enviaba `totalArea` vacío → ahora `totalArea: areaTotal`. `AddActivityForm` muestra la info de cada sector (Fruta/Variedad/Plantas/Superficie) al seleccionarlo.
4. **Costo real**: `WorkOrderCostItemDTO` + `ConsumedAmount`/`RealCost`; `WorkOrderCostDTO` + `TotalRealCost`; `InventoryQueryService.GetWorkOrderCostsAsync` cruza `Consumptions` (costo real = cantidad consumida × costo unitario congelado); `CostsTab` con columnas "Cantidad real consumida"/"Costo real" + tarjetas "Costo total planificado"/"Costo real".
5. **Cambio/cierre de estado**: fix del dropdown de acciones del listado (el clic se propagaba a la fila y navegaba al detalle) → el trigger de `Dropdown` hace `stopPropagation`. Nueva acción **Cambiar estado** en la cabecera del detalle (`WorkOrderHeader`) con las transiciones permitidas (`allowedTransitions`), confirmación con `ConfirmDialog` y `updateWorkOrderState` **sin navegar** (recarga la orden).

Verificado: `dotnet build` (WebApi) 0 errores; `npx tsc --noEmit` + `npm run build` → **0 errores** (9 rutas). **Sin commits aún.**

## Completed — Dashboard: últimos movimientos de inventario (Oct 2026)

1. **Backend** — nuevo `GET api/inventory/movements/recent?take=10` (policy `STOCK_READ`; default 10, clamp `<1 → 10`). DTO `InventoryMovementDTO` (`DTOs/InventoryDTOs/`); `IInventoryQueryService.GetRecentMovementsAsync` + impl (`InventoryQueryService`: ordena por `Date`/`Id` desc + `Take`); passthrough en `IInventoryRepository`/`InventoryRepository`; `GetRecentMovementsQuery` + handler; endpoint en `InventoryController`/`IInventoryController`. Proyecta material (+unidad base), finca, tipo/origen, cantidad, costo/divisa, stock resultante, fecha y OT (`OrderNum`).
2. **Frontend** — tipo `InventoryMovementDTO` (`types/api/inventory.ts` + re-export), `getRecentMovements(take)` en `inventory.service.ts`.
3. **Dashboard** (`Dashboard.tsx`) — nueva sección "Últimos movimientos" (visible con `STOCK_READ`), cargada junto a `getLowStock` en el mismo `Promise.all`; `DataTable` con Fecha, Material (· comercial), Finca, Movimiento, Origen (muestra `Orden {orderNum}` si es OT), Cantidad (+ unidad) y Stock resultante.

Verificado (build 0 errores + smoke real): login; `movements/recent?take=5` → 5 movimientos con material/divisa/finca/OT correctos (SalidaConsumo OrdenTrabajo OT 40, AjustePositivo Manual, Ingreso Manual). **Sin commits aún.**

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

- `FincaFenix.UserInterface/` — only build artifacts; la UI Blazor viva (`UserInterface7.0`) fue **eliminada en Fase 6** — la UI nueva es Next.js (`FincaFenix.Frontend`)
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
| `IA/PLAN_EJECUCION_RENDIMIENTO_OT.md` | Rendimiento de OT (maquinadas + modos de tarea) plan |
| `IA/PLAN_EJECUCION_CRUD_TAREAS.md` | CRUD de Tareas (Tarea) plan |
| `IA/PLAN_MIGRACION_NEXT_TAILWIND.md` | Migración UI a Next.js + Tailwind + shadcn/ui (plan) |
| `IA/DOCUMENTACION_UI.md` | Documentación viva del frontend Next.js (arquitectura, componentes, servicios, convenciones es-AR, límites) |
