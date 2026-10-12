# Plan de Ejecución — % de área trabajada + fórmula de rendimiento

> Fecha: Oct 2026. Objetivo: (1) expresar el rendimiento como **`(real/teórico)×100`** y colorear
> **rojo si >100**, mostrando además la **desviación** al exceder el 100%; (2) permitir que el
> operario cargue el **% de área trabajada** por actividad, **acumulativo por (OT, sector) con tope 100%**,
> mostrando a cuántas ha equivale.

## Decisiones acordadas con el usuario

1. **Fórmula**: `(real/teórico)×100`. Aplica a los modos porcentuales (`MaterialEfficiency` + consumo).
2. **Color**: `>100` → **rojo** (`text-destructive`); `<=100` → verde.
3. **Desviación al exceder 100%**: mostrar el exceso (`rendimiento − 100`).
4. **% de área en todos los modos**, **obligatorio** (rango 1–100).
5. **Acumulativo por (OT, sector)**: no puede superar 100% entre actividades.
6. **MaterialEfficiency**: las maquinadas teóricas siguen usando el **área total del sector** (sin cambios).
7. Modos tasa (`AreaPerManHour`/`OutputPerManHour`/`ManHours`): **sin semáforo**.
8. **Sin renombrar** etiquetas existentes.
9. **Sí** mostrar el **% acumulado por sector** en la OT → ubicado en la pestaña **Actividades**.
10. Sin GPS/telemetría por ahora.

## Semántica

- El `%` de cada actividad es un **delta** (avance de esa jornada).
- **Acumulado (OT, sector)** = `Σ deltas` de ese sector en la OT. Debe ser `≤ 100`.
- Regla de alta: `acumulado + pct_nuevo ≤ 100`; si no → **422**.
- **Área efectivamente trabajada** de una actividad = `sector.Area × pct/100`.
- **Área efectivamente trabajada de la OT** = `Σ (sector.Area × pct/100) ≤ área asignada`.

## Parte A — Fórmula + color + desviación

- `FincaFenix.Entities/Rendimiento/RendimientoCalculator.cs`:
  - `MaterialEfficiency`: `teórico/real` → **`real/teórico`**.
  - `EfficiencyFromAmounts`: `teórico/real` → **`real/teórico`**.
- `FincaFenix.Frontend/src/features/work-orders/lib/deviation.ts`:
  - `efficiencyTone`: `>100 → text-destructive`, `<=100 → text-success`.
- Desviación = `rendimiento − 100` (= `deviationPercent` de `(real, teórico)`).
  - Mostrar en `PerformanceTab` (stat Rendimiento, solo MaterialEfficiency) y `ConsumptionTab`
    (card "Eficiencia de aplicación"); la columna Desviación ya existe.

## Parte B — % de área acumulativo

### Modelo / migración
- `DetailWorkOrderEntity`: `+ PercentageAreaWorked` (decimal 18,2).
- `DetailWorkOrderConfiguration`: col **`PorcentajeAreaTrabajada`**, `IsRequired`,
  `HasPrecision(18,2)`, `HasDefaultValue(100)` (backfill histórico).
- Migración: `AddActivityAreaPercentage`.

### DTOs
- `InfoDetailWorkOrderDTO`: `+ PercentageAreaWorked`.
- `ActivityWorkOrderDTO`: `+ PercentageAreaWorked`; `AreaWorked` **calculado** = `sector.Area × pct/100`.
- `ShowWorkOrderDTO`: `+ TotalAreaActuallyWorked` (Σ área real). `TotalAreaWorked` sigue = área asignada.

### Mapping
- `AddDetailWorkOrderDTO → DetailWorkOrderEntity`: mapear `PercentageAreaWorked`.
- `DetailWorkOrderEntity → ActivityWorkOrderDTO`: `AreaWorked = s.SectorWorked.Area * s.PercentageAreaWorked / 100`.

### Validación acumulativa (autoritativa, con lock)
- `IDetailWOCommandService`/`DetailWOCommandService`: nuevo
  `GetAccumulatedAreaPercentageAsync(int workOrderId, int sectorId, bool forUpdate)`.
  Con `forUpdate=true` usa SQL crudo `WITH (UPDLOCK, HOLDLOCK)` sobre `DetalleOrdenTrabajo`
  (patrón de inventario) para serializar altas concurrentes del mismo (OT, sector).
- `AddDetailWorkOrderRepository.CreateDetailWorkOrderAsync` (dentro de `IUnitOfWork`, antes de guardar):
  1. `acumulado = GetAccumulatedAreaPercentageAsync(ot, sector, forUpdate: true)`.
  2. Si `acumulado + pct > 100` → `BusinessRuleException` → **422** + rollback.
- `AddDetailWorkOrderCommandValidator`: rango `1–100` (422). La cota acumulada se valida en la transacción.

### Cálculo
- Actividad (`DetailWorkOrderQueries`): `PerManHour(sector.Area × pct/100, horas)`.
- OT (`WorkOrderQueries`):
  - `areaActual = Σ(sector.Area × pct/100)`.
  - `AreaPerManHour = PerManHour(areaActual, Σhoras)`.
  - `TotalAreaActuallyWorked = areaActual`.
  - `MaterialEfficiency`: **sin cambios** (teórico sobre `WorkedSectors`).

### Frontend
- `AddActivityForm.tsx`: campo **"% de área trabajada"** en todos los modos (obligatorio 1–100),
  helper `Ya trabajado: X% · Máx: (100−X)%`, label `"P% de A ha = R ha"` (es-AR), validación local.
- `types/api/workOrder.ts`: `+ percentageAreaWorked` (Activity/Info), `+ totalAreaActuallyWorked` (Show).
- `ActivitiesTab.tsx`: reincorporar columnas **"Área trabajada"** y **"% del sector"** + bloque
  **"Avance por sector"** (Sector · % acumulado · ha trabajadas).
- `PerformanceTab.tsx`: stat **"Área trabajada"** cuando aplique; desviación al exceder 100%.
- `ConsumptionTab.tsx`: ajuste de tono/label.

## Archivos afectados

| Capa | Archivos |
|---|---|
| Entities | `DetailWorkOrderEntity.cs`, `ActivityWorkOrderDTO.cs`, `InfoDetailWorkOrderDTO.cs`, `ShowWorkOrderDTO.cs` |
| EFCore | `DetailWorkOrderConfiguration.cs`, migración `AddActivityAreaPercentage`, `DetailWOCommandService.cs`, `DetailWOQueryService.cs` |
| UsesCases | `MappingProfile.cs`, `RendimientoCalculator.cs`, `WorkOrderQueries.cs`, `DetailWorkOrderQueries.cs`, `AddDetailWorkOrderCommandValidator.cs` |
| Gateways | `AddDetailWorkOrderRepository.cs`, `IDetailWOCommandService.cs` |
| Frontend | `AddActivityForm.tsx`, `ActivitiesTab.tsx`, `PerformanceTab.tsx`, `ConsumptionTab.tsx`, `deviation.ts`, `types/api/workOrder.ts` |

## Verificación

- Build `FincaFenix.sln` + `dotnet ef database update`.
- **Acumulativo**: 50% en sector X → OK; 60% → **422** ("máx 50%"); 50% → OK (100%).
- **Área**: sector 10 ha, 25% → `AreaWorked=2.5` (label "25% de 10 ha = 2,5 ha"); dos 25% → 5 ha (no 20).
- **Concurrencia**: dos altas simultáneas superando el tope → una OK, otra 422, sin estado parcial.
- **Fórmula**: OT 40 (Id 72) → rendimiento **138,9% rojo** + desviación **+38,9%**; consumo idem.
- `npx tsc --noEmit` + `npm run build`.
