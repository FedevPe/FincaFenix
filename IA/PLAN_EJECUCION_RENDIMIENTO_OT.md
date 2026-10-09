# Plan de Ejecución — Rendimiento de actividades de Orden de Trabajo

> Estado: **en ejecución** (Oct 2026). Módulo complementario al de Inventario.

## 1. Objetivo

Rediseñar el cálculo del **rendimiento de las actividades (detalles) de la OT** de los operarios,
separando dos conceptos que hoy están mezclados en `DetailWorkOrderEntity.Performance`:

1. **Cantidad de maquinadas** (input del operario). Hoy se llama `Performance` / columna `Rendimiento`,
   pero semánticamente es la cantidad de **cargas de tanque (maquinadas)** que usó el operario en una
   tarea con receta. Se **renombra**.
2. **Rendimiento** (métrica calculada, **no persistida**). El método de cálculo depende del **tipo de tarea**.

## 2. Decisiones de negocio

- El **modo de medición** lo define el **tipo de tarea** (`Tarea.RendimientoMode`) y **manda** sobre la
  presencia de receta.
- El rendimiento **se calcula al consultar** (no se guarda), a nivel **OT** y **por actividad**.
- En tareas con receta se expone además **total de receta** y **por material**.
- Fórmula con receta: **% Eficiencia = planificado / real**.
- Tareas sin receta: productividad por **horas hombre** (área/h o kg/h) o sólo horas hombre.
- Tareas de producción (cosecha): la cantidad producida se captura en **kg** (`ProducedAmount`).
  (Módulo específico de cosecha queda para el futuro.)

### Tareas que SIEMPRE requieren receta (`MaterialEfficiency`)

| Id | Nombre (BD) |
|----|-------------|
| 1  | Aplicación de herbicida mochila |
| 2  | Aplicación de herbicida maquina |
| 6  | Fertirrigación |
| 19 | Cura tratamiento fitosanitario |

`Cosecha` (Id 3) → `OutputPerManHour`. El resto → `ManHours` (default).

## 3. Lógica de cálculo (dominio)

### 3.1 Modo `MaterialEfficiency` (tareas con receta)

```
AreaTotal          = Σ área de los sectores de la OT (WorkedSectors → SectorFarm.Area)
MaquinadasTeoricas = AreaTotal × Receta.TRV / Receta.VolumeMachine       (precedente: WorkOrderPDF.cs:224-227)
MaquinadasReales   = Σ actividad.MachinePasses
Rendimiento %      = MaquinadasTeoricas / MaquinadasReales × 100         (plan / real)
```

Por material:

```
CantidadTeorica_i = MaquinadasTeoricas × DetailRecipe.AmountRequired_i   (convertida a unidad base)
CantidadReal_i    = TotalAmountConsumed_i                                 (unidad base, desde Consumo)
Eficiencia_i %    = CantidadTeorica_i / CantidadReal_i × 100
```

> **Nota:** con dosis *por carga de tanque* (`AmountRequired`), la eficiencia por material da el mismo %
> que el total (la dosis se cancela). Se expone igual porque el cliente lo pidió. Si a futuro la dosis
> se expresara por área, dejarían de coincidir.

### 3.2 Modos sin receta (productividad hombre)

| Modo | Fórmula | Unidad |
|------|---------|--------|
| `AreaPerManHour`   | Σ área trabajada / Σ horas hombre | ha/h |
| `OutputPerManHour` | Σ kg producidos / Σ horas hombre   | kg/h |
| `ManHours`         | Σ horas hombre                     | h    |

Por actividad se usa el sector y las horas de esa actividad; a nivel OT, los totales.

## 4. Modelo de datos y migración

- `DetailWorkOrderEntity`: `Performance` → **`MachinePasses`** (col. `Rendimiento` → `Maquinadas`, decimal(18,5)).
  Nuevo `ProducedAmount` `decimal?` (col. `CantidadProducida`, decimal(18,5), null).
- `TaskEntity`: nuevo `RendimientoMode` (col. `ModoRendimiento`, int NOT NULL, default `0` = `ManHours`).
- Nuevo enum `FincaFenix.Entities/Enum/RendimientoModeEnum.cs`:

```csharp
public enum RendimientoModeEnum
{
    ManHours = 0,
    MaterialEfficiency = 1,
    AreaPerManHour = 2,
    OutputPerManHour = 3
}
```

Migración: `RenameColumn` de `Rendimiento` (preserva datos), `AddColumn` de `CantidadProducida` y
`ModoRendimiento`, más seed idempotente de modos por tarea.

## 5. Cambios de aplicación

- **Consumo** (`WorkOrderConsumptionCommand`): sólo rename `Performance` → `MachinePasses`. Comportamiento idéntico.
- **DTOs**:
  - `InfoDetailWorkOrderDTO`: `Performance`→`MachinePasses`; `+ProducedAmount`.
  - `ActivityWorkOrderDTO`: `Performance`→`MachinePasses`; `+ProducedAmount`, `+AreaWorked`,
    `+Rendimiento`, `+RendimientoUnit`, `+RendimientoMode`.
  - `RecipeWorkOrderDTO`: `+TheoreticalVolume`, `+RealVolume`, `+TheoreticalMachinePasses`,
    `+RealMachinePasses`, `+Rendimiento`.
  - `DetailRecipeDTO`: `+TheoreticalAmount`, `+Rendimiento`.
  - `ShowWorkOrderDTO`: `+Rendimiento`, `+RendimientoUnit`, `+RendimientoMode`, `+TotalManHours`,
    `+TotalProducedAmount`.
  - `TaskDTO`: `+RendimientoMode`.
- **MappingProfile**: ajustar rename y nuevos campos; ignorar los calculados (los setea el handler).
- **Cálculo**: nueva clase pura `FincaFenix.Entities/Rendimiento/RendimientoCalculator.cs`
  (mismo estilo que `UnitConverter`). Se invoca en:
  - `GetWorkOrderByIdHandler` (total receta, por material y OT).
  - `GetActivitiesByOrderIdHandler` (por actividad).
- **Query de actividades** (`DetailWOQueryService`): proyectar `MachinePasses`, `ProducedAmount`,
  `SectorWorked.Area` y el `RendimientoMode` de la tarea de la OT.
- **Validación**:
  - `AddDetailWorkOrderCommandValidator` (condicional por modo): `WorkedHours > 0` siempre;
    `MachinePasses > 0` si `MaterialEfficiency`; `ProducedAmount > 0` si `OutputPerManHour`.
  - `WorkOrderValidator` (create): tarea `MaterialEfficiency` debe tener receta con materiales → **422**.
- **Seed**: `SeedDataBase.SeedTaskRendimientoModesAsync` idempotente (por nombre normalizado),
  invocado en `Program.cs` y en `PolicyContainer.UseAsyncSeeding`.
- **PDF** (`WorkOrderPDF`): opcional, mostrar rendimiento real vs teórico (ya muestra maquinadas teóricas).

## 6. Endpoints afectados (sin nuevos)

`getCompleteInfoWorkOrder/{id}`, `detailworkorder/order/{id}/getDetail`, `GetTaskList`, `GetTaskById`
devuelven los campos nuevos.

## 7. Verificación

- `dotnet build` 0 errores.
- Migración aplicada + seed de modos.
- Smoke test: OT con receta (maquinadas → % eficiencia), cosecha (kg/h), tarea default (horas hombre),
  validaciones 422 por modo, regresión de consumo / costos / stock / PDF.
