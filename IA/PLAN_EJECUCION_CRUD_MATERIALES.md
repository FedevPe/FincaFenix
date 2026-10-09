# Plan de Ejecución — CRUD de Materiales, Categorías y Unidades de Medida

> Estado: **COMPLETADO (Oct 2026)** — build 0 errores + smoke tests OK.
> Alcance acordado con el usuario: **Material + Categoría + UnidadMedida**.
> Divisa queda como maestro de solo lectura (no se hace CRUD).

---

## 1. Decisiones de negocio

| Tema | Decisión |
|---|---|
| Alcance | CRUD de **Material**, **Categoría de material** y **Unidad de medida**. |
| Borrado de material | **Lógico** (`Eliminado = true`). |
| Material eliminado en picker de receta | **No se muestra** al crear/elegir materiales para una receta (`getmateriallist` y `category/{id}/material` filtran `!IsDeleted`). |
| Material eliminado en histórico | **Sí se muestra** si ya estaba asignado a una receta (`recipe/{recipeId}/material` NO filtra) y en `GET material/{id}`. |
| `IdUnidadMedida` | **NOT NULL** en BD vía migración + requerido en validación. |
| Listado | **Paginado/filtrado** con `PagedResult<T>` + `GET material/paged`. |
| Divisa | Solo lectura (referencia en el material). No se hace CRUD. |

---

## 2. Políticas

- Material: `MATERIAL_CREATE / READ / UPDATE / DELETE` (ya existen).
- Categoría: `MATERIAL_CATEGORY_CREATE / READ / UPDATE / DELETE` (ya existen).
- **Unidad de medida**: se agregan `UNIT_OF_MEASURE_CREATE / READ / UPDATE / DELETE` en `PolicyMaster`.
  - `admin` y `desarrollador` las reciben automáticamente (seeding por reflexión de `PolicyMaster`).
  - Se agrega `UNIT_OF_MEASURE_READ` a la lista explícita de `supervisorPolicies` en `SeedDataBase`.
  - `operario` no las recibe.

---

## 3. DTOs — `FincaFenix.Entities/DTOs/InventoryDTOs/MaterialDTOs/` (una clase por archivo)

- `MaterialDTO` (respuesta): `Id, CodeSap, DescriptionSap, ArticleName, CommercialName, CategoryId, Category(MaterialCategoryDTO), Brand, Description, UnitOfMeasureId, UnitOfMeasure, ReferenceCost, CurrencyId, CurrencyCode, IsDeleted`.
- `CreateMaterialDTO`: `CodeSap?, DescriptionSap?, ArticleName(req), CommercialName(req), CategoryId(req), Brand?, Description?, UnitOfMeasureId(req int), ReferenceCost?, CurrencyId(req, default 1)`.
- `UpdateMaterialDTO`: igual a `CreateMaterialDTO` + `Id`.
- `MaterialFilterDTO`: `PageNumber, PageSize, CategoryId?, Search?, IncludeDeleted(bool, default false)`.
- `UnitOfMeasureDTO`: `Id, Description`.
- `SaveUnitOfMeasureDTO`: `Id?, Description`.
- `SaveMaterialCategoryDTO`: `Id?, Description`.
- Se reutiliza `MaterialCategoryDTO` (existente en `DTOs/WorkOrderDTOs`).

---

## 4. UseCases (MediatR)

- `UseCases/Material/MaterialCommands.cs`: `CreateMaterialCommand`, `UpdateMaterialCommand`, `DeleteMaterialCommand` + handlers.
- `UseCases/Material/MaterialQueries.cs`: agregar `GetMaterialByIdQuery`, `GetMaterialListPagedQuery`.
- `UseCases/MaterialCategory/MaterialCategoryCommands.cs` + `GetMaterialCategoryByIdQuery`.
- `UseCases/UnitOfMeasure/UnitOfMeasureQueries.cs` + `UnitOfMeasureCommands.cs`.
- Validators en `UsesCases/Validators/Material/`: `CreateMaterialCommandValidator`, `UpdateMaterialCommandValidator`, `SaveMaterialCategoryCommandValidator`, `SaveUnitOfMeasureCommandValidator`.
- Registrar validadores en `FincaFenix.UsesCases/DependencyContainer.cs`.

---

## 5. Gateways + EFCore

- `IMaterialRepository`: `GetMaterialById`, `GetMaterialListPaged`, `CreateMaterial`, `UpdateMaterial`, `DeleteMaterial`; eliminar `GetMaterialByOrderIdAsync` (código muerto).
- `IMaterialCategoryRepository`: `GetById`, `Add`, `Update`, `Delete`.
- Nuevo `IUnitOfMeasureRepository`.
- Nuevos command services: `IMaterialCommandService`, `IMaterialCategoryCommandService`, `IUnitOfMeasureCommandService` (Gateways) + impl en `EFCore/Services/CommandServices/Material/`.
- Query services: extender `MaterialQueryService` y `MaterialCategoryQueryService`; nuevo `UnitOfMeasureQueryService`.
- **Filtrado `!IsDeleted`** en `GetMaterialList` y `GetMaterialListByCategoryId`; **sin filtro** en `GetMaterialListByRecipeId`.

---

## 6. Controllers

- `MaterialController`: `GET {id}` (READ), `POST` (CREATE), `PUT` (UPDATE), `DELETE {id}` (DELETE, lógico), `GET paged` (READ). Se mantienen los 3 GET existentes.
- `MaterialCategoryController`: `GET {id}`, `POST`, `PUT`, `DELETE`.
- Nuevo `UnitOfMeasureController` (`api/unitofmeasure`).

---

## 7. Mapping + DI

- `MappingProfile`: `MaterialEntity↔MaterialDTO`, `CreateMaterialDTO→MaterialEntity`, `UpdateMaterialDTO→MaterialEntity`, `UnitOfMeasureEntity↔UnitOfMeasureDTO`, `SaveMaterialCategoryDTO→MaterialCategoryEntity`, `SaveUnitOfMeasureDTO→UnitOfMeasureEntity`.
- DI: nuevos query/command services (`EFCore/DependencyContainer.cs`), `IUnitOfMeasureRepository` (`Gateways/DependencyContainer.cs`), `IUnitOfMeasureController` (`Controllers/DependencyContainer.cs`).

---

## 8. Migración

- `MaterialConfiguration`: `UnitOfMeasureId` → `IsRequired()`.
- Se **mantiene** la propiedad CLR `int?` para no romper las conversiones existentes
  (`WorkOrderConsumptionCommand.cs:118` `.HasValue`, `UpdateWorkOrderCommand.cs:191` ternario con `null`,
  `WorkOrderInventoryCommand.cs:87`).
- Migración `MakeMaterialUnitOfMeasureRequired` con `UPDATE Material SET IdUnidadMedida = 3 WHERE IdUnidadMedida IS NULL` antes del `AlterColumn` (defensivo; los 59 ya están backfilleados).

---

## 9. Verificación

- Build 0 errores (detener el server antes).
- Smoke: crear categoría/unidad/material (201); GET por id; listado paginado/filtrado; update; delete lógico → ausente en `getmateriallist` pero presente en `recipe/{id}/material` y `GET {id}`; 404/422; migración aplicada; regresión de inventario/OT/PDF.

---

## 10. Riesgos / notas

- Cambio de filtrado en `getmateriallist`/`category` afecta solo a los pickers de receta (intencional).
- `CodeSap` no tiene índice único hoy: **no** se agrega, para no romper datos existentes.
- `IdUnidadMedida` NOT NULL: la propiedad CLR sigue nullable; el required se valida en FluentValidation.

---

## 11. Resultado de verificación (Oct 2026)

**Build 0 errores.** Migración `20261009042528_MakeMaterialUnitOfMeasureRequired` aplicada (con backfill defensivo `IdUnidadMedida = 3`).

Smoke test real (login `root`, API en `http://localhost:5000`):

| Prueba | Resultado |
|---|---|
| `GET api/unitofmeasure` | 8 unidades |
| `GET api/materialcategory/getCategories` | 7 categorías |
| `POST api/material` | 201, devuelve `MaterialDTO` (category/unitOfMeasure/currencyCode poblados) |
| `GET api/material/{id}` | 200 con detalle |
| `GET api/material/paged?pageNumber=1&pageSize=5` | 200 (`totalCount=59`, 12 páginas) |
| `GET api/material/paged?...&categoryId=2` | 200 (20 materiales) |
| `GET api/material/paged?...&search=CRUD` | 200, filtra |
| `PUT api/material` | 200, actualiza (categoría/unidad/costo/divisa) |
| Validación `unitOfMeasureId=0` | **422** |
| `categoryId=999` / `unitOfMeasureId` inexistente (o eliminada) | **404** |
| `GET api/material/999999` | **404** |
| `DELETE api/materialcategory/1` (con materiales) | **422** (regla de negocio) |
| `DELETE api/material/{id}` | 200; `getmateriallist` ya no lo incluye; `GET {id}` sigue devolviéndolo (`isDeleted=true`); `recipe/{id}/material` no filtra |
| CRUD `unitofmeasure` (POST/PUT/DELETE) | OK; baja lógica; `GET` sigue devolviendo la unidad; el listado la excluye |
| `POST api/material` con unidad eliminada | **404** |

Regresión OK: `getmateriallist`, `recipe/{id}/material`, `stock/consolidated`, `costs/current`, `currencies`, `workorderlistpaginated`.

**Bug preexistente corregido de paso:** `GET api/material/category/{id}/material` devolvía **500** (`Include` aplicado después de `GroupBy`+`Select`). Reescrito: `Include` antes del `Where` + agrupación en memoria. Ahora responde 200 (cat 1 → 7, cat 2 → 16).

**Sin commits aún** (pendiente de indicación).
