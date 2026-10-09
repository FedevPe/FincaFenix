# Plan de Ejecución — Módulo de Inventario

**Proyecto:** Fincas Fénix
**Base funcional:** `IA/ESPECIFICACION_MODULO_INVENTARIO.md` (v0.5)
**Objetivo:** incorporar el módulo de Inventario sin romper lo existente y relacionándolo con Órdenes de Trabajo, Recetas y Consumos.
**Arquitectura:** Clean Architecture con MediatR + AutoMapper; cada fase respeta el patrón de capas existente (Controller → Handler → Gateway → EFCore).

> Estado: plan aprobado. Ejecución iniciada. Fases 0–7 completadas. Módulo de inventario completo; próxima etapa: Fase 8 (postergada) / migración React.
>
> **Nota (Oct 2026, previo a Fase 5):** incorporado el modelo de **unidad de medida base por material** (ver `AGENTS.md` → "Unidades de medida base"). Cada material define su unidad de stock/reservas/movimientos/consumos/costos; `UnitConverter` centraliza conversiones (cc→lts, gr→kg) y rechaza incompatibles (422). Pendiente: NOT NULL de `Material.IdUnidadMedida` cuando exista el CRUD de materiales; duplicar materiales con unidades distintas (no convertir masa↔volumen).

---

## Decisiones cerradas

| Tema | Decisión |
|---|---|
| Nombres de entidades | Clases en inglés; tablas/columnas en español (convención actual) |
| FK de reserva y snapshot de costo | `WorkOrderId` (+ `MaterialId`) |
| Movimiento de inventario | FK **opcional** a OT |
| `CostoReferencia` | Columna nullable en `Material`, actualizada solo en transacción de ingreso |
| Consumo | Entidad `ConsumptionEntity` explícita; `DetailWorkOrderEntity` **intocable** |
| Exceso de consumo | No rechazar la actividad: aplicar hasta reserva+stock, exceso registrado sin descontar stock |
| Endpoint manual | `POST api/inventory/consumption` (pruebas / alta directa) |
| Punto de cálculo | Recalcular en **cada** `addDetailWO` (transaccional) |
| Fórmula de consumo | Ver Fase 4 (corrige el bug del ViewModel histórico) |

---

## Estado actual relevante

- No existe nada de stock, reservas, movimientos, consumos, costos ni `UnidadMedida` (las unidades son strings sueltos).
- `WorkOrderEntity.FarmId` ya identifica la Finca (unidad productiva) de la OT.
- La creación de OT con receta ocurre en `CreateWorkOrderRepository.CreateWorkOrder` (transacción existente) — punto de hook natural.
- El cálculo de consumo vivía en la UI Blazor eliminada (Fase 6); sus fórmulas se replican server-side corregidas.
- `UpdateWorkOrderState` acepta cualquier string; `UpdateWorkOrder()` lanza `NotImplementedException`; no hay estado "Cancelado".

---

## Fase 0 — Análisis previo (§42)

Documento `IA/INVENTARIO_MODELO_ACTUAL_VS_OBJETIVO.md`: modelo actual vs. objetivo, entidades nuevas/modificadas, relaciones, índices, restricciones, reglas de negocio, estrategia de datos y riesgos de compatibilidad.

Estrategia de datos: las OT con receta existentes **no** reciben reservas retroactivas; el stock inicial se carga por ingreso/ajuste.

**Entregables:**
1. Modelo actual.
2. Modelo objetivo.
3. Comparación entre ambos.
4. Entidades nuevas.
5. Entidades modificadas.
6. Relaciones nuevas.
7. Índices.
8. Restricciones.
9. Reglas de negocio.
10. Estrategia de migración de datos.
11. Migraciones EF Core (a generar en Fase 1).
12. Riesgos de compatibilidad.

---

## Fase 1 — Modelo de datos + migración (aditiva)

**Entidades nuevas** (`FincaFenix.Entities/POCOEntities/`):

| Clase | Tabla | Campos clave |
|---|---|---|
| `UnitOfMeasureEntity` | `UnidadMedida` | Id, Descripcion, IsDeleted |
| `StockByFarmEntity` | `StockPorFinca` | StockFisico, StockReservado, StockMinimo, RowVersion; **unique (MaterialId, FincaId)**; StockDisponible calculado |
| `MaterialReservationEntity` | `ReservaMaterial` | **WorkOrderId**, MaterialId, FarmId, CantidadReservada, CantidadConsumida, Estado (Activa/Liberada/Cancelada), FechaCreacion, FechaLiberacion, RowVersion |
| `InventoryMovementEntity` | `MovimientoInventario` | TipoMovimiento (Ingreso/Consumo/Ajuste±/Transferencia±), Cantidad, CostoUnitario, CostoTotal, StockAnterior, StockResultante, Fecha, UsuarioId, Observaciones, Origen, **WorkOrderId?** |
| `WorkOrderCostEntity` | `CostoOrdenTrabajo` | **WorkOrderId**, MaterialId, CantidadPlanificada, CostoUnitario, CostoTotal (snapshot RN-COSTO-ORDEN) |
| `ConsumptionEntity` | `Consumo` | **WorkOrderId**, MaterialId, CantidadConsumida, CantidadAplicada, Unidad, Origen (Calculado/Manual), CostoUnitario, FechaCalculo, UsuarioId, RowVersion; **unique (WorkOrderId, MaterialId)** |

**Modificadas:** `MaterialEntity` + `UnidadMedidaId` (FK) + `CostoReferencia` (decimal?).

**EFCore:** DbSets en `FincaFenixContext`, `IEntityTypeConfiguration` en `Configurations/` (convención de carpetas), `RowVersion` en tablas mutables, índices unique/FK. **Una sola migración aditiva** (solo columnas nullable nuevas en tablas existentes → cero impacto en flujos actuales).

---

## Fase 2 — Stock y materiales (endpoints base)

- Consultas: stock por Finca, consolidado, stock bajo (mínimo), materiales sin stock.
- `POST` ingreso/ajuste → crea `InventoryMovementEntity` + actualiza `StockByFarmEntity`/`Material.CostoReferencia` (transaccional, sin stock negativo).
- Cadena completa por capas: DTOs → `UseCases/UseCases/Inventory/` → interfaces (`UsesCases/Repository/`, `Controllers/`) → `Gateways/Implementations/Inventory/` → `EFCore/Services/{Command,Query}Services/Inventory/` → controller + registro en los 4 `DependencyContainer.cs`.
- Policies nuevas en `PolicyMaster` (`STOCK_*`, `MOVEMENT_*`, `RESERVATION_*`, `CONSUMPTION_*`) — registración automática por reflexión; ajustar listas de `SeedDataBase` para `supervisor`/`operario`.

---

## Fase 3 — Reserva blanda al crear OT + costo congelado

Hook en `CreateWorkOrderRepository.CreateWorkOrder` (misma transacción): validar stock disponible **de `OT.FarmId`** (regla 27.21) → crear reservas por cada `DetailRecipe` → capturar `WorkOrderCostEntity` (último costo) → commit. OT sin receta → sin reservas (compatibilidad total).

**Concurrencia (§18/§34):** RowVersion + re-validación en transacción con lock pesimista (`UPDLOCK, HOLDLOCK`) + retry ante `DbUpdateConcurrencyException`; validación siempre en servidor. Falta de stock → 422 vía `ExceptionMiddleware`, sin estado parcial.

> **Implementado (Oct 2026):** lock pesimista `StockPorFinca WITH (UPDLOCK, HOLDLOCK)` por Finca; la re-validación usa datos frescos. **Retry omitido**: el lock serializa la creación de OTs de la misma Finca, por lo que no se produce la carrera. Cantidad planificada de reserva/costo = `DetailRecipeEntity.EstimatedAmount`.

---

## Fase 4 — Consumo (proceso actual preservado)

**Fórmulas server-side (corregidas):**

```
dosisNormalizada    = AmountRequired / 1000   (si unidad es cc|gr → lts|kg)   ; AmountRequired (si lts|kg)
volumenTotal        = TRV × área
maquinadasEstimadas = volumenTotal / VolumeMachine

ConsumoEstimado (plan)  = ((TRV × área) / VolumeMachine) × dosisNormalizada
ConsumoReal     (acum)  = (Σ Performance)                × dosisNormalizada
```

> Corrige el bug del `CalculateMaterialConsumedViewModel` histórico, que dividía de más por `VolumeMachine`. Coincide con la fórmula "Maquinadas" del PDF: `(TotalAreaWorked × TRV)/VolumeMachine`.

**Flujo automático** — `addDetailWO` (firma intacta, cálculo dentro de la transacción):

1. Registrar actividad.
2. Recalcular `Σ Performance` → nuevo total por cada material de la receta.
3. `delta = nuevoTotal − ConsumptionEntity.CantidadConsumida`.
4. Aplicar delta: dentro de reserva → siempre; excedente → hasta stock disponible de la Finca; lo que no entra queda como exceso sin aplicar (nunca stock negativo, nunca rechazar la actividad).
5. `ReservaMaterial.CantidadConsumida += aplicado`; `StockFisico`/`StockReservado` -= aplicado.
6. `InventoryMovementEntity` (Salida por consumo, Origen=OT, costo congelado) por el aplicado.
7. `ConsumptionEntity.CantidadConsumida = nuevoTotal`, `CantidadAplicada += aplicado` → commit.

**Endpoint manual:** `POST api/inventory/consumption` (mismo camino transaccional, Origen=Manual).

**Comparación plan vs. real:** `DetailRecipeEntity.EstimatedAmount` + `ConsumptionEntity` → pobla `DetailRecipeDTO.TotalAmountConsumed` (hoy muerto). `DetailWorkOrderEntity` y la fórmula del PDF no se tocan.

> **Implementado (Oct 2026):**
> - `IWorkOrderConsumptionCommand` + `WorkOrderConsumptionCommand` (`EFCore/Services/CommandServices/WorkOrder/`): recálculo transaccional en cada `addDetailWO` (hook en `AddDetailWorkOrderRepository` con `IUnitOfWork`). `Σ Performance × dosisNormalizada` (dosis = `AmountRequired/1000` si unidad `cc|gr`, si no `AmountRequired`).
> - Aplicación incremental por material de la receta: `delta = nuevoTotal − Consumo.CantidadConsumida`; primero contra reserva activa (`ReservaMaterial.CantidadConsumida += aplicado`, pasa a `Consumida` si se agota), luego contra `StockDisponible = StockFisico − StockReservado`; exceso queda registrado sin aplicar (nunca stock negativo, nunca rechazar la actividad).
> - `MovimientoInventario` `SalidaConsumo`/`SalidaManual` (Origen `OrdenTrabajo`/`Manual`, `WorkOrderId`, costo congelado de `Material.CostoReferencia`) solo cuando `aplicado > 0`.
> - Endpoint `POST api/inventory/consumption` (`ConsumptionOriginEnum.Manual`, transacción propia, 404 si no existe OT/material, 422 por validación).
> - `GetWorkOrderByIdHandler` pobla `DetailRecipeDTO.TotalAmountConsumed` desde `Consumo`.
> - Registrado en `FincaFenix.EFCore/DependencyContainer.cs` (Transient) y validator en `FincaFenix.UsesCases`. Build 0 errores; smoke test paso a paso (reserva→stock→exceso→manual) + 404/422 + TotalAmountConsumed verificados.

---

## Fase 5 — Ciclo de vida de la OT

- Validar transiciones de estado en `UpdateWorkOrderState`.
- `"Cerrado"` → libera reservas sobrantes (regla 27.22, transaccional).
- **Estado `"Cancelado"`** (nuevo; ≠ borrado lógico `Eliminado`): solo si no hay actividades ni consumos; libera reservas; sin movimiento físico.
- **Implementar `UpdateWorkOrder()`** (hoy `NotImplementedException`): corrección de errores de creación editando OT + receta, con re-validación/ajuste de reservas (§15).

> **Implementado (Oct 2026):**
> - `WorkOrderStatusEnum` (`Pendiente`/`Activo`/`Cerrado`/`Cancelado`) + `AllowedTransitions` en `UpdateWorkOrderCommand` (EFCore). Transición inválida → `BusinessRuleException` (422); OT inexistente → 404.
> - **`Cerrado`:** setea `FechaFin` y libera reservas activas sobrantes (regla 27.22) decrementando `StockReservado` por `CantidadReservada − CantidadConsumida`; la reserva pasa a `Liberada` con `FechaLiberacion`. No altera stock físico.
> - **`Cancelado`:** rechaza (422) si hay `DetalleOrdenTrabajo` o consumos; libera reservas; sin movimiento físico. Es estado de negocio, distinto del borrado lógico `Eliminado`.
> - **`UpdateWorkOrder`:** nuevo `PUT api/workorder/updateworkorder` (`UpdateWorkOrderDTO`) edita metadatos (tarea, descripción, fechas, área) + receta (máquina/TRV/volumen/detalles, re-mapeados con `GroupItems`). Ajusta reservas por material (§15): al aumentar valida `StockDisponible` de la Finca (insuficiente → 422 rollback); al disminuir libera el sobrante. Si la OT no tenía receta, la crea y registra reserva/costo. Todo transaccional con `IUnitOfWork`.
> - `ReleaseReservationsAsync` agregado a `IWorkOrderInventoryCommand`. Costos congelados (`CostoOrdenTrabajo`) no se recalculan (RN-COSTO-ORDEN).
> - Build 0 errores; smoke test: cierre libera sobrante; cancelación libera y rechaza con actividades; transiciones inválidas 422; edición +1/-2/5(stock insuficiente) verificada.

---

## Fase 6 — Costos históricos

Consultas: costo congelado por OT (`WorkOrderCostEntity`), evolución de costos (desde `InventoryMovementEntity.CostoUnitario`), costo actual (`Material.CostoReferencia`).

> **Completada (implementada + smoke test, Oct 2026):**
> - 3 endpoints de solo lectura en `InventoryController`: `GET costs/current` (policy `STOCK_READ`), `GET costs/material/{materialId}/history` (policy `MOVEMENT_READ`), `GET costs/workorder/{workOrderId}` (policy `WORKORDER_READ`). Patrón Query/Handler/Repo passthrough; DTOs en `Entities/DTOs/InventoryDTOs/CostDTOs/` (una clase por archivo); `NotFoundException` → 404.
> - Build 0 errores; smoke test: 59 materiales en `current`, OT 62 congelado (0.5 × 20 = 10 USD), historial mat 9 con 8 operaciones (último 20 USD), 404 en material/OT inexistentes.

---

## Fase 7 — Verificación

- `dotnet build` 0 errores.
- Checklist de los 20 criterios de §39.
- **Test de concurrencia** (2 OTs simultáneas que sumadas superen el disponible → solo una gana).
- Prueba numérica de las fórmulas de consumo contra el ejemplo acordado.
- **Regresión**: login, create con/sin receta, addDetailWO, materiales por receta, paginada, updatestate, PDF.
- Actualizar `AGENTS.md`.

> **Verificado (Oct 2026):** build 0 errores. **Concurrencia:** material 14 (5 lts disponibles), dos `createworkorder` simultáneas de 4 lts c/ú → una **200** (reserva 4, disponible 1) y otra **422** ("Disponible 1, requerido 4") sin estado parcial (solo 1 OT/reserva/costo persistidos). **Consumo numérico** (OT 65: dosis 0.5 lts, plan 10): `addDetailWO` perf 2 → consumo 1.0; perf 3 → Σ5 × 0.5 = **2.5** consumido/aplicado, `StockFisico` 51.5→49.0, `StockReservado` 10→7.5, 2 `SalidaConsumo` (1.0 + 1.5), `getCompleteInfo.recipe.details[0].totalAmountConsumed=2.5`. **Regresión OK:** login, create sin/con receta (OT 64/65), `addDetailWO`, `material/recipe/35/material`, paginada (`items`/`totalCount=34`), `updatestateworkorder` (Activo→Cerrado `true` + `FechaFin`; Cerrado→Activo **422**), PDF (`application/pdf`, 62596 bytes, `%PDF-`).

### Checklist §39 — 20/20

| # | Criterio | Evidencia |
|---|---|---|
| 1 | Material en múltiples Fincas | `StockPorFinca` (IdMaterial, IdFinca) |
| 2 | Finca con múltiples materiales | filas N:1 en `StockPorFinca` |
| 3 | Físico/Reservado/Disponible | `StockByFarmEntity` + `Disponible = Físico − Reservado` |
| 4 | Reserva no reduce físico | `RegisterReservationsAndCostsAsync` solo suma `StockReservado` |
| 5 | Reserva reduce disponible | cálculo de `available` por diferencia |
| 6 | No sobre-reservar | `plannedAmount > available → 422` |
| 7 | OT con receta genera reservas | hook en `CreateWorkOrderRepository` (OT 65) |
| 8 | Reserva ligada a OT | `MaterialReservationEntity.WorkOrderId` |
| 9 | Consumo reduce físico | `WorkOrderConsumptionCommand` (49.0) |
| 10 | Consumo reduce reserva | `ConsumedAmount += aplicado` |
| 11 | Cancelación libera reservas | `ReleaseReservationsAsync` (Fase 5) |
| 12 | Edición ajusta reservas | `AdjustReservationsAsync` (Fase 5) |
| 13 | Movimientos históricos | `MovimientoInventario` append-only |
| 14 | Costos históricos inalterables | `WorkOrderCostEntity` congelado (RN-COSTO-ORDEN) |
| 15 | Operaciones transaccionales | `IUnitOfWork` Begin/Commit/Rollback |
| 16 | Concurrencia sin sobre-reserva | `UPDLOCK, HOLDLOCK` + re-validación (test OK) |
| 17 | Físico nunca negativo | consumo/ajuste topan en disponible |
| 18 | Evoluciona a Compras | modelo `MovimientoInventario` + divisa (Fase 8) |
| 19 | No depende de productividad | módulo aislado |
| 20 | Permite rendimiento futuro | `DetalleOrdenTrabajo` (Performance/WorkedHours) + `Consumo` |

---

## Fase 8 — Postergado (solo preparar modelo)

Transferencias entre Fincas, Compras/Proveedores, informes completos, SignalR (stock bajo), política de consumo > reserva con ampliación explícita.
