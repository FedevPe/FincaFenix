# Inventario — Modelo actual vs. Modelo objetivo (Fase 0)

**Proyecto:** Fincas Fénix
**Referencias:** `IA/ESPECIFICACION_MODULO_INVENTARIO.md` (v0.5) · `IA/PLAN_EJECUCION_MODULO_INVENTARIO.md`
**Propósito:** cumplir el punto §42 de la especificación (producir modelo actual, objetivo, comparación, entidades, relaciones, índices, restricciones, reglas, estrategia de datos y riesgos **antes** de generar migraciones).
**Estado:** análisis previo a la migración. Ninguna migración se genera en esta fase.

---

## 1. Modelo actual

### 1.1 Entidades de negocio (`FincaFenix.Entities/POCOEntities/`)

| Entidad | Tabla | Campos | Relaciones |
|---|---|---|---|
| `MaterialEntity` | `Material` | Id, CodeSap, DescriptionSap, ArticleName, CommercialName, CategoryId, Brand, Description, IsDeleted | N:1 `MaterialCategoryEntity`; 1:N `DiseasePlague_MaterialEntity` |
| `MaterialCategoryEntity` | `Categoria` | Id, Description | — |
| `FarmEntity` | `Finca` | Id, Name, IsDeleted | 1:N `Employee_FarmEntity` |
| `DetailSectorFarmEntity` | `DetalleSectorFinca` | Id, FarmId, SectorName, VarietyId, NumberPlants, Area, Age | N:1 `FarmEntity`, N:1 `FruitVarietyEntity` |
| `RecipeEntity` | `Receta` | Id, NumRecipe, MachineId, TRV, VolumeMachine, VolumeMachineUnit, State, IsDeleted, RowVersion | N:1 `MachineEntity`; 1:N `DetailRecipeEntity` |
| `DetailRecipeEntity` | `DetalleReceta` | Id, RecipeId, MaterialId, Brand, DiseasePlague, AmountRequired, AmountRequiredUnit, EstimatedAmount, EstimatedAmountUnit, RowVersion | N:1 `RecipeEntity`, N:1 `MaterialEntity` |
| `WorkOrderEntity` | `OrdenTrabajo` | Id, OrderNum, UserId, RecipeId, TaskId, FarmId, CreatedDate, StartDate, EndDate, Status, TotalAreaWorked, Description, IsDeleted, RowVersion | N:1 `RecipeEntity` (1:1, FK nullable, cascade), N:1 `TaskEntity`, N:1 `FarmEntity`; 1:N `DetailWorkOrderEntity`, `WorkOrderWorkedSectorEntity` |
| `DetailWorkOrderEntity` | `DetalleOrdenTrabajo` | Id, WorkOrderId, EmployeeId, SectorWorkedId, Performance, WorkedHours, Description, ActivityDate, RowVersion | N:1 `WorkOrderEntity` (NoAction), N:1 `EmployeeEntity`, N:1 `DetailSectorFarmEntity` |
| `WorkOrderWorkedSectorEntity` | `OrdenTrabajo_SectorTrabajado` | Id, WorkOrderId, SectorFarmId | N:1 `WorkOrderEntity`, N:1 `DetailSectorFarmEntity` |
| `MachineEntity` | `Maquinaria` | Id, Name, Capacity, CapacityUnit, IsDeleted | — |
| `EmployeeEntity` | `Empleado` | Id, Name, LastName, IsDeleted | 1:N `Employee_FarmEntity` |
| `Employee_FarmEntity` | `Empleado_Finca` | Id, EmployeeId, FarmId | N:1 `EmployeeEntity`, N:1 `FarmEntity` |
| `TaskEntity` | `Tarea` | Id, Description, IsDeleted | — |
| `CorrelativeNumberEntity` | `Correlativo` | Id, TypeDoc, LastNumber | — |
| `DiseasePlagueEntity` / `DiseasePlague_MaterialEntity` | `EnfermedadPlaga` / `EnfermedadPlaga_Material` | Descripción / puente | N:N Material↔EnfermedadPlaga |
| `FruitEntity` / `FruitVarietyEntity` | `Frutal` / `VariedadFrutal` | Descripción / FruitId, IsOrganic | N:1 |
| `ApplicationUser` | `AspNetUsers` | Identity | — |

**No existe** en el modelo actual: `UnidadMedida`, stock, reservas, movimientos de inventario, consumos, costos, compras. Las unidades de medida son **strings sueltos** (`AmountRequiredUnit`, `EstimatedAmountUnit`, `VolumeMachineUnit`, `CapacityUnit`).

### 1.2 Índices únicos existentes
- `OrdenTrabajo.IdReceta` → índice único **con filtro** `[IdReceta] IS NOT NULL` (una receta solo puede pertenecer a una OT).
- Identity: `UserNameIndex`, `RoleNameIndex`.
- No hay unique en `NumOrden`, `NumReceta`, `CodigoSAP`.

### 1.3 Concurrencia existente
`RowVersion` (`[Timestamp]` / `IsRowVersion`) ya presente en `Receta`, `DetalleReceta`, `DetalleOrdenTrabajo`, `OrdenTrabajo`.

### 1.4 Migraciones existentes (14)
`FirstMigration` → … → `IdentityFirstMigration`, `VariedadesOrganicas`, `EdadCuadrosFinca`, `AddPolicySeeding`. Snapshot: `FincaFenixContextModelSnapshot.cs` (EF 9.0.0).

---

## 2. Modelo objetivo

Cadena funcional (spec §16):

```
Recipe ──plan──▶ WorkOrder ──reserve──▶ MaterialReservation ──consume──▶ Consumption
                                                                              │
                                                                              ▼
                                                                    InventoryMovement
                                                                              │
                                                                              ▼
                                                                      StockByFarm
```

### 2.1 Entidades nuevas

| Clase (inglés) | Tabla (español) | Campos | Notas |
|---|---|---|---|
| `UnitOfMeasureEntity` | `UnidadMedida` | Id, Descripcion, IsDeleted | Maestro de unidades; FK opcional desde `Material` |
| `StockByFarmEntity` | `StockPorFinca` | Id, MaterialId, FarmId, StockFisico, StockReservado, StockMinimo, RowVersion | Unique (MaterialId, FarmId); `StockDisponible` calculado |
| `MaterialReservationEntity` | `ReservaMaterial` | Id, WorkOrderId, MaterialId, FarmId, CantidadReservada, CantidadConsumida, Estado, FechaCreacion, FechaLiberacion, RowVersion | Estado: Activa/Liberada/Cancelada |
| `InventoryMovementEntity` | `MovimientoInventario` | Id, MaterialId, FarmId, TipoMovimiento, Cantidad, CostoUnitario, CostoTotal, StockAnterior, StockResultante, Fecha, UsuarioId, Observaciones, Origen, WorkOrderId? | Libro mayor de cambios físicos |
| `WorkOrderCostEntity` | `CostoOrdenTrabajo` | Id, WorkOrderId, MaterialId, CantidadPlanificada, CostoUnitario, CostoTotal, FechaCongelado | Snapshot RN-COSTO-ORDEN |
| `ConsumptionEntity` | `Consumo` | Id, WorkOrderId, MaterialId, CantidadConsumida, CantidadAplicada, Unidad, Origen, CostoUnitario, FechaCalculo, UsuarioId, RowVersion | Unique (WorkOrderId, MaterialId) |

### 2.2 Entidades modificadas

| Entidad | Cambio |
|---|---|
| `MaterialEntity` | + `UnidadMedidaId` (int?, FK opcional a `UnidadMedida`) + `CostoReferencia` (decimal?, columna `CostoReferencia`) |

### 2.3 Tablas futuras (solo reservadas en el modelo, no implementadas)
`Compra`, `DetalleCompra`, `Proveedor` (spec §30, §32). La FK `InventoryMovement.WorkOrderId?` es opcional; en el futuro se agregará una FK opcional análoga a `DetalleCompra`.

---

## 3. Comparación actual → objetivo

| Concepto | Actual | Objetivo | Naturaleza del cambio |
|---|---|---|---|
| Material | `Material` sin unidad ni costo | `Material` + `UnidadMedidaId` + `CostoReferencia` | **Modificación aditiva** (columnas nullable) |
| Unidad de medida | strings sueltos | entidad `UnidadMedida` (maestro) | **Nueva entidad** + FK opcional |
| Stock | no existe | `StockPorFinca` | **Nueva entidad** |
| Reserva | no existe (la receta solo planifica) | `ReservaMaterial` | **Nueva entidad** |
| Movimiento | no existe | `MovimientoInventario` | **Nueva entidad** |
| Costo congelado OT | no existe | `CostoOrdenTrabajo` | **Nueva entidad** |
| Consumo | cálculo efímero en UI eliminada; `TotalAmountConsumed` DTO-only | `Consumo` + `MovimientoInventario` | **Nueva entidad** + cálculo server-side |
| Finca de la OT | `OrdenTrabajo.IdFinca` | se reutiliza como ubicación del stock | **Sin cambios** |
| Actividades | `DetalleOrdenTrabajo` | se mantiene igual; no registra materiales | **Sin cambios** |

**Cambios destructivos:** ninguno. Todas las tablas existentes se conservan; `Material` solo recibe dos columnas nuevas nullable.

---

## 4. Entidades nuevas (detalle de diseño)

### 4.1 `UnitOfMeasureEntity` → `UnidadMedida`
`Id` (PK), `Descripcion` (nvarchar(50), req.), `IsDeleted` (bit, default 0).
Semilla inicial sugerida: Kilogramo, Litro, Unidad, Metro, Bolsa, Caja.

### 4.2 `StockByFarmEntity` → `StockPorFinca`
`Id` (PK), `MaterialId` (FK `Material`, req.), `FarmId` (FK `Finca`, req.), `StockFisico` decimal(18,3) req. default 0, `StockReservado` decimal(18,3) req. default 0, `StockMinimo` decimal(18,3) req. default 0, `RowVersion`.
- `StockDisponible = StockFisico − StockReservado` → **calculado, no persistido**.
- Restricción de negocio: `StockFisico ≥ 0` y `StockReservado ≥ 0` (validadas en la capa transaccional).

### 4.3 `MaterialReservationEntity` → `ReservaMaterial`
`Id` (PK), `WorkOrderId` (FK `OrdenTrabajo`, req.), `MaterialId` (FK `Material`, req.), `FarmId` (FK `Finca`, req.), `CantidadReservada` decimal(18,3), `CantidadConsumida` decimal(18,3), `Estado` (nvarchar(20): `Activa|Liberada|Cancelada`), `FechaCreacion` datetime2, `FechaLiberacion` datetime2 null, `RowVersion`.
- `CantidadPendiente = CantidadReservada − CantidadConsumida` (calculado).

### 4.4 `InventoryMovementEntity` → `MovimientoInventario`
`Id` (PK), `MaterialId` (FK, req.), `FarmId` (FK, req.), `TipoMovimiento` (nvarchar(20): `Ingreso|Consumo|AjustePositivo|AjusteNegativo|TransferenciaSalida|TransferenciaIngreso`), `Cantidad` decimal(18,3), `CostoUnitario` decimal(18,4) null, `CostoTotal` decimal(18,4) null, `StockAnterior` decimal(18,3), `StockResultante` decimal(18,3), `Fecha` datetime2, `UsuarioId` int null, `Observaciones` nvarchar(500) null, `Origen` nvarchar(20): `Manual|WorkOrder|Compra|Transferencia|Ajuste`, `WorkOrderId` int **null** (FK opcional `OrdenTrabajo`).

### 4.5 `WorkOrderCostEntity` → `CostoOrdenTrabajo`
`Id` (PK), `WorkOrderId` (FK, req.), `MaterialId` (FK, req.), `CantidadPlanificada` decimal(18,3), `CostoUnitario` decimal(18,4), `CostoTotal` decimal(18,4), `FechaCongelado` datetime2.
- Snapshot inmutable (RN-COSTO-ORDEN §20).

### 4.6 `ConsumptionEntity` → `Consumo`
`Id` (PK), `WorkOrderId` (FK, req.), `MaterialId` (FK, req.), `CantidadConsumida` decimal(18,3) (total calculado), `CantidadAplicada` decimal(18,3) (descontado efectivamente del stock), `Unidad` nvarchar(10), `Origen` nvarchar(20): `Calculado|Manual`, `CostoUnitario` decimal(18,4) null, `FechaCalculo` datetime2, `UsuarioId` int null, `RowVersion`.
- `ExcesoSinAplicar = CantidadConsumida − CantidadAplicada` (calculado).

---

## 5. Entidades modificadas

| Entidad | Campo nuevo | Columna | Tipo | Nulo |
|---|---|---|---|---|
| `MaterialEntity` | `UnidadMedidaId` | `IdUnidadMedida` | int | Sí (FK opcional) |
| `MaterialEntity` | `CostoReferencia` | `CostoReferencia` | decimal(18,4) | Sí |

`MaterialEntity` recibe navegación `UnitOfMeasureEntity? UnitOfMeasure`.

---

## 6. Relaciones nuevas

| Origen | Destino | Cardinalidad | Delete |
|---|---|---|---|
| `StockPorFinca` | `Material` | N:1 | Restrict |
| `StockPorFinca` | `Finca` | N:1 | Restrict |
| `ReservaMaterial` | `OrdenTrabajo` | N:1 | Restrict |
| `ReservaMaterial` | `Material` | N:1 | Restrict |
| `ReservaMaterial` | `Finca` | N:1 | Restrict |
| `MovimientoInventario` | `Material` | N:1 | Restrict |
| `MovimientoInventario` | `Finca` | N:1 | Restrict |
| `MovimientoInventario` | `OrdenTrabajo` | N:1 (FK **opcional**) | SetNull |
| `CostoOrdenTrabajo` | `OrdenTrabajo` | N:1 | Restrict |
| `CostoOrdenTrabajo` | `Material` | N:1 | Restrict |
| `Consumo` | `OrdenTrabajo` | N:1 | Restrict |
| `Consumo` | `Material` | N:1 | Restrict |
| `Material` | `UnidadMedida` | N:1 (FK opcional) | Restrict |

> Se usa `Restrict` (no `Cascade`) para preservar el historial (spec §36: los cambios actuales no destruyen información histórica). Las entidades existentes usan `Cascade` en varios FKs; las nuevas se alinean con el principio de conservación histórica salvo el FK opcional a OT, que usa `SetNull`.

---

## 7. Índices

| Tabla | Índice | Tipo | Motivo |
|---|---|---|---|
| `StockPorFinca` | `(MaterialId, FarmId)` | **Unique** | Restricción §6 / §27.3 (sin duplicados) |
| `StockPorFinca` | `FarmId` | No unique | Consultas por Finca |
| `StockPorFinca` | `MaterialId` | No unique | Consultas por material |
| `ReservaMaterial` | `WorkOrderId` | No unique | Reservas por OT |
| `ReservaMaterial` | `MaterialId` | No unique | Reservas por material |
| `ReservaMaterial` | `FarmId` | No unique | Stock/compromisos por Finca |
| `MovimientoInventario` | `MaterialId, Fecha` | No unique | Historial por material |
| `MovimientoInventario` | `FarmId, Fecha` | No unique | Historial por Finca |
| `MovimientoInventario` | `WorkOrderId` | No unique | Trazabilidad OT→consumo |
| `Consumo` | `(WorkOrderId, MaterialId)` | **Unique** | 1 registro acumulado por material/OT |
| `CostoOrdenTrabajo` | `(WorkOrderId, MaterialId)` | **Unique** | Un snapshot por material/OT |
| `UnidadMedida` | `Descripcion` | No unique | Búsquedas |
| `Material` | `IdUnidadMedida` | No unique | FK |

---

## 8. Restricciones

- **Unique** `StockPorFinca(MaterialId, FarmId)` — §27.3.
- **Unique** `Consumo(WorkOrderId, MaterialId)` y `CostoOrdenTrabajo(WorkOrderId, MaterialId)`.
- **Check (validación transaccional, no constraint de BD en v1):** `StockFisico ≥ 0`, `StockReservado ≥ 0`, `StockReservado ≤ StockFisico`.
- **Check de dominio (enum string):** `TipoMovimiento`, `Origen`, `Estado`, `Origen` de consumo — valores controlados por código.
- **RowVersion** en `StockPorFinca`, `ReservaMaterial`, `Consumo` (control de concurrencia) + los ya existentes.

---

## 9. Reglas de negocio (spec §27, §38)

Mapeo de las 22 reglas de integridad + RF-INV a su implementación prevista:

| Regla | Implementación |
|---|---|
| Material existe en N Fincas / Finca en N materiales | `StockPorFinca` N:N |
| Sin duplicados Material+Finca | Unique `(MaterialId, FarmId)` |
| Stock físico solo cambia por operaciones de inventario | Solo `InventoryMovement` (Ingreso/Consumo/Ajuste/Transferencia) |
| Reserva no modifica stock físico | `ReservaMaterial` solo afecta `StockReservado` |
| Reserva disminuye disponible | `StockDisponible = StockFisico − StockReservado` |
| Reserva vinculada a Material (+OT) | FKs de `ReservaMaterial` |
| Consumo reduce físico y reserva | Flujo transaccional Fase 4 |
| Reservas liberadas vuelven a disponible | `Estado = Liberada` + `StockReservado -=` |
| Cancelación/cierre libera reservas | Fase 5 |
| Transferencias afectan origen y destino | Fase 8 (modelo listo) |
| No reservar más que disponible | Validación transaccional Fase 3 |
| Concurrencia protege integridad | RowVersion + `UPDLOCK/HOLDLOCK` + retry |
| Movimientos históricos no se eliminan | `Restrict` + sin delete físico |
| Costos históricos inmutables | `WorkOrderCost` snapshot + movimientos con costo |
| OT congela costos al crearse | Fase 3 (snapshot) |
| Stock por Finca de la OT (no total) | Validación con `WorkOrder.FarmId` (regla 27.21) |
| Cierre libera sobrante | Fase 5 (regla 27.22) |
| Stock físico no negativo | Validación transaccional |
| Rendimiento fuera del módulo | No se crean entidades de productividad |

### Fórmulas de consumo (spec §9/§21, corregidas)
```
dosisNormalizada    = AmountRequired / 1000   (unidad cc|gr)   ; AmountRequired (unidad lts|kg)
volumenTotal        = TRV × área
maquinadasEstimadas = volumenTotal / VolumeMachine
ConsumoEstimado     = maquinadasEstimadas × dosisNormalizada
ConsumoReal (acum)  = (Σ Performance)     × dosisNormalizada
```

---

## 10. Estrategia de migración de datos

1. **Sin backfill de reservas:** las OT con receta existentes no reciben `ReservaMaterial` retroactivas (la reserva es un acto al crear la OT).
2. **Sin backfill de movimiento/consumo:** el histórico previo no tiene datos de stock físico ni de consumo persistido; no se inventan valores.
3. **Stock inicial:** se carga mediante `POST` de ingreso/ajuste (Fase 2), que genera el movimiento correspondiente. Decisión de negocio: el stock arranca en 0 hasta que se registren ingresos.
4. **`Material.UnidadMedidaId`:** nullable; se pobla gradualmente. `Material.CostoReferencia`: nullable.
5. **Seeds:** `UnidadMedida` (maestro básico) vía migración o seed idempotente.
6. **Policies:** nuevas constantes en `PolicyMaster` (registración automática por reflexión); `desarrollador`/`admin` las reciben solas; `SeedDataBase` se ajusta para `supervisor`/`operario`.

---

## 11. Migraciones EF Core (a generar en Fase 1)

- Una única migración aditiva: `20261009010827_AddInventoryModule` (generada y aplicada).
- Contenido: 7 tablas nuevas (`UnidadMedida`, `StockPorFinca`, `ReservaMaterial`, `MovimientoInventario`, `CostoOrdenTrabajo`, `Consumo`, `Divisa`) + columnas en `Material` (`CostoReferencia` y `IdUnidadMedida` nullable; `IdDivisa` requerida con default 1=ARS para filas existentes) + índices/relaciones.
- `Divisa`: catálogo maestro (ARS, USD, EUR, JPY, GBP) sembrado vía `HasData` en `CurrencyConfiguration`. `IdDivisa` es **obligatoria** en `Material`, `MovimientoInventario`, `CostoOrdenTrabajo` y `Consumo`.
- Conversión de divisas: **fuera de alcance v1**. Solo se persiste la divisa de cada costo. Diseño futuro: tabla de cotizaciones (ej. `CotizacionDivisa`: divisa origen/destino, tasa, fecha vigencia, fuente) + servicio/proveedor de cotización actualizable para recalcular valores; ver pendientes fuera de v1 (fin del documento).
- Comando (raíz del repo):
  ```
  dotnet tool restore
  dotnet ef migrations add AddInventoryModule --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
  dotnet ef database update --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
  ```
- No se generan migraciones definitivas hasta revisión de este documento.

---

## 12. Riesgos de compatibilidad

| Riesgo | Impacto | Mitigación |
|---|---|---|
| Nuevo endpoint/flujo de consumo toca `addDetailWO` | Alto (flujo clave del operario) | Añadir lógica dentro de la transacción existente sin cambiar la firma; pruebas de regresión obligatorias |
| `Material` gana columnas | Bajo | Columnas nullable; sin cambio de comportamiento |
| FKs nuevas con `Restrict` | Bajo | No afecta datos existentes; evita borrados en cascada del historial |
| Concurrencia en reservas | Alto (sobre-reserva) | RowVersion + lock pesimista + revalidación en transacción + test dedicado |
| Cálculo de consumo corregido difiere del histórico (bug) | Medio (valores distintos a la UI vieja) | Documentado; validado con ejemplo numérico acordado; la UI vieja ya no existe |
| Seed de policies idempotente no revoca | Bajo | `supervisor`/`operario` requieren alta explícita de las nuevas policies |
| Estado "Cancelado" nuevo | Medio | Regla: solo sin actividades ni consumos; no es borrado lógico |

---

## Decisiones de §40 resueltas en este documento

| # | Decisión |
|---|---|
| 1 | `CostoReferencia` permanece en `Material` (columna nullable) |
| 2 | Historial de costos = `MovimientoInventario.CostoUnitario` + snapshot `CostoOrdenTrabajo` |
| 4 | Transferencias modeladas como movimientos (sin endpoint en v1) |
| 5 | Ajustes = movimientos `AjustePositivo`/`AjusteNegativo` |
| 6 | `MovimientoInventario → Compra`: FK opcional futura |
| 7 | `MovimientoInventario → Consumo`: consumo registrado en `Consumo` + movimiento `Origen=WorkOrder` |
| 9 | `ReservaMaterial`: WorkOrderId + MaterialId + FarmId + cantidades + estado |
| 10 | Consumo > reserva: se aplica hasta reserva+stock; exceso registrado sin descontar |
| 11 | Modificación OT: re-validación/ajuste de reservas (Fase 5) |
| 12 | Cierre OT: libera reservas sobrantes (Fase 5) |
| 13 | Concurrencia: RowVersion + `UPDLOCK/HOLDLOCK` + retry |
| 14 | Snapshot de costo: tabla `CostoOrdenTrabajo` |

**Pendientes (fuera de v1):** #3 método de valuación del inventario; #8 si una compra se distribuye entre múltiples Fincas.
