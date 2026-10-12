# Plan de ejecución — UI del módulo de Tareas (Oct 2026)

## Objetivo

Construir la pantalla CRUD de **Tareas** (`TaskEntity` / tabla `Tarea`) en el frontend
Next.js (`FincaFenix.Frontend`), con listado **paginado del lado del servidor**, buscador
y toggle de eliminadas. Requiere agregar un endpoint paginado en el backend respetando la
arquitectura (Clean + MediatR + Query/Command Services).

La tarea solo tiene dos campos de negocio: `Description` (nombre, máx. 100) y
`RendimientoMode` (`ManHours` | `MaterialEfficiency` | `AreaPerManHour` | `OutputPerManHour`).

## Alcance

- CRUD completo: listar (paginado), crear, editar y baja **lógica**.
- Filtros: `search` (descripción) e `includeDeleted`; paginación server-side (`pageNumber`/`pageSize`).
- Policies: `TASK_READ` (ver), `TASK_CREATE` (crear), `TASK_UPDATE` (editar), `TASK_DELETE` (baja).
  `admin`/`desarrollador` tienen todas; `supervisor` solo `TASK_READ`; `operario` sin acceso.

## Backend — endpoint paginado

Se **mantiene** el endpoint existente `GET api/Task/GetTaskList` (lo usan las pantallas de
Órdenes de Trabajo para poblar combos). Se agrega uno nuevo, replicando el patrón de
`MaterialQueryService.GetMaterialListPaged`. Sin cambios de DI (MediatR auto-escanea) ni migración.

1. `FincaFenix.Entities/DTOs/TaskDTO/TaskFilterDTO.cs` (nuevo): `PageNumber=1`, `PageSize=10`,
   `string? Search`, `bool IncludeDeleted`.
2. `FincaFenix.Gateways/Interfaces/QueryServices/ITaskQueryService.cs`: `GetTaskListPaged(TaskFilterDTO)`.
3. `FincaFenix.EFCore/Services/QueryServices/TaskQueryService.cs`: `CountAsync()` +
   `OrderBy(t => t.Description)` + `Skip`/`Take` (filtra `!IsDeleted` salvo `IncludeDeleted`).
4. `FincaFenix.UsesCases/Repository/ITaskRepository.cs` + `FincaFenix.Gateways/Implementations/TaskRepository.cs`: pass-through.
5. `FincaFenix.UsesCases/UseCases/Task/TaskQueries.cs`: `GetTaskListPagedQuery` + handler que arma `PagedResult<TaskDTO>` con AutoMapper.
6. `FincaFenix.Controllers/Implementations/TaskController.cs` + `FincaFenix.UsesCases/Controllers/ITaskController.cs`:
   `[HttpGet("paged")]` + `[Authorize(Policy = PolicyMaster.TASK_READ)]` → `GET api/Task/paged`.

## Frontend

1. `src/types/api/task.ts` (nuevo): `TaskDTO` (movido desde `workOrder.ts`) + `SaveTaskDTO`.
2. `src/types/api/workOrder.ts`: importa `TaskDTO` desde `./task`.
3. `src/types/api/index.ts`: re-exporta `TaskDTO`/`SaveTaskDTO` desde `./task`.
4. `src/features/tasks/constants.ts` (nuevo): `RENDIMIENTO_MODES` + labels (dueño canónico).
5. `src/features/work-orders/constants.ts`: re-exporta los labels desde `tasks/constants`.
6. `src/features/tasks/services/task.service.ts` (nuevo): `getTasksPaged`, `createTask`, `updateTask`, `deleteTask`.
7. `src/features/tasks/components/TaskForm.tsx` (nuevo): form en modal (descripción + modo).
8. `src/features/tasks/pages/TaskList.tsx` (nuevo): buscador server-side + toggle eliminadas +
   `DataTable` + `Pagination` server-side + `Modal` + `ConfirmDialog` + gates de policy + toasts.
9. `src/app/(app)/tasks/page.tsx` (nuevo): wrapper `Suspense`.
10. `src/components/layout/Sidebar.tsx` + `AppBar.tsx`: ítem "Tareas" y título de ruta.

## Verificación

- `dotnet build FincaFenix.sln` → 0 errores.
- `npx tsc --noEmit` + `npm run build` → 0 errores.
- Smoke backend: `GET /api/task/paged?pageNumber=1&pageSize=10&search=&includeDeleted=false`.

## Decisiones / límites

- Baja lógica (`Eliminado`); el toggle `includeDeleted` decide si se listan.
- `GetTaskList` se conserva para no tocar Órdenes de Trabajo.
- Nombres/formatos en es-AR; sin conversión de datos.

## Ampliación — Reactivación de tareas eliminadas

Las tareas dadas de baja lógica (`Eliminado=true`) pueden **reactivarse** por usuarios con
`TASK_UPDATE` (admin/desarrollador). `TaskDTO` ahora expone `IsDeleted` para distinguirlas en el front.

- Backend: `ReactivateTaskCommand` + handler, `ITaskCommandService.ReactivateAsync` /
  `TaskCommandService.ReactivateAsync` (transaccional, 404 si no existe, `IsDeleted=false`),
  `ITaskRepository.Reactivate` / pass-through, endpoint `PUT api/Task/ReactivateTask/{id}`
  (policy `TASK_UPDATE`).
- Frontend: badge "Eliminada" en la fila; para tareas eliminadas el dropdown muestra solo
  **Reactivar** (con confirmación); el resto mantiene Editar/Eliminar.
