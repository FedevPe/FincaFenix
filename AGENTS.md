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
