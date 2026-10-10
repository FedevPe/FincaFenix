# Plan de Ejecución — CRUD de Tareas (Tarea)

> Estado: **COMPLETADO (Oct 2026)** — build 0 errores + smoke tests OK.
> Alcance acordado con el usuario: CRUD completo de `TaskEntity` (tabla `Tarea`), incluida la edición del `RendimientoMode`.
> Plantilla de implementación: CRUD de MaterialCategory (`IA/PLAN_EJECUCION_CRUD_MATERIALES.md`).

---

## 1. Decisiones de negocio

| Tema | Decisión |
|---|---|
| Alcance | CRUD de **Tarea** (`TaskEntity`): `Descripción` + `RendimientoMode`. |
| Borrado | **Lógico** (`Eliminado = true`), igual que Material/Unidad. |
| Tarea eliminada en picker de OT | **No se muestra** en `GetTaskList` (filtra `!IsDeleted`); `includeDeleted=true` las incluye. |
| Tarea eliminada en histórico | `GET getTaskById/{id}` **sí** la devuelve (permite ver/editar/restaurar). |
| Tarea eliminada al crear OT | **Rechazada** → `TaskQueryService.Exists` filtra `!IsDeleted` → **422** (ver §5). |
| `RendimientoMode` | **Editable** vía CRUD (string en el DTO, validado contra `RendimientoModeEnum`). |
| Listado | Sin paginar (dominio chico: ~31 tareas). `GetTaskList?includeDeleted`. |

---

## 2. Políticas

- `TASK_CREATE / READ / UPDATE / DELETE` **ya existían** en `PolicyMaster` → **sin cambios de seed**.
- `admin` y `desarrollador` las reciben automáticamente (seeding por reflexión de `PolicyMaster`).
- `supervisor` ya tenía `TASK_READ` en la lista explícita de `SeedDataBase`. `operario` no las recibe.
- `TaskController` pasó de `[Authorize(Policy = TASK_READ)]` a nivel clase → `[Authorize]` + policy por acción.

---

## 3. DTOs

- `SaveTaskDTO` (`FincaFenix.Entities/DTOs/WorkOrderDTOs/SaveTaskDTO.cs`): `Id?`, `Description`, `RendimientoMode` (string).
- Se reutiliza `TaskDTO` existente (respuesta): `Id, Description, RendimientoMode` (no se modificó, porque también se embebe en `ShowWorkOrderDTO.Task` / `InfoWorkOrderDTO.TaskOrder`).

---

## 4. UseCases (MediatR)

- `UseCases/Task/TaskQueries.cs`: `GetTaskListQuery(bool IncludeDeleted = false)` (antes sin parámetro).
- `UseCases/Task/TaskCommands.cs` (nuevo): `CreateTaskCommand`, `UpdateTaskCommand`, `DeleteTaskCommand` + handlers.
- Validators en `UsesCases/Validators/Task/`: `CreateTaskCommandValidator`, `UpdateTaskCommandValidator`
  (Descripción obligatoria ≤ 100; `RendimientoMode` obligatorio y parseable a `RendimientoModeEnum`).
- Registro de validadores en `FincaFenix.UsesCases/DependencyContainer.cs`.

---

## 5. Gateways + EFCore

- `ITaskRepository`/`TaskRepository`: se agregan `Add`, `Update`, `Delete`; `GetAllTasks(bool includeDeleted = false)`.
- `ITaskQueryService`/`TaskQueryService`:
  - `GetTaskList(bool includeDeleted = false)` → `Where(t => includeDeleted || !t.IsDeleted)`.
  - `Exists(int id)` → `Where(t => t.Id == id && !t.IsDeleted)` (**guarda** para no asignar tareas desactivadas a nuevas OT).
- Nuevo `ITaskCommandService`/`TaskCommandService` (`Gateways/Interfaces/CommandServices/Tasks` y `EFCore/Services/CommandServices/Tasks`), con `IUnitOfWork`:
  - Create: `Id=0`, `IsDeleted=false`.
  - Update: toca `Description` + `RendimientoMode`; **404** si no existe.
  - Delete: **soft** (`IsDeleted=true`); **404** si no existe.

---

## 6. Controllers

`TaskController` (`api/task`):

| Método | Ruta | Policy | Descripción |
|---|---|---|---|
| GET | `getTaskById/{id}` | `TASK_READ` | Devuelve también eliminadas. |
| GET | `GetTaskList?includeDeleted=false` | `TASK_READ` | Excluye eliminadas por defecto. |
| POST | *(base)* | `TASK_CREATE` | Crea tarea. |
| PUT | *(base)* | `TASK_UPDATE` | Edita descripción/modo. |
| DELETE | `{id}` | `TASK_DELETE` | Baja lógica. |

---

## 7. Mapping + DI

- `MappingProfile`: `SaveTaskDTO → TaskEntity` (parse tolerante de `RendimientoMode`, fallback `ManHours`; `IsDeleted` ignorado).
- DI: `ITaskCommandService` (`EFCore/DependencyContainer.cs`), validadores (`UsesCases/DependencyContainer.cs`). Repositorios/controllers ya registrados.

---

## 8. Gotcha de naming (CS0118)

El namespace `FincaFenix.*.Services/Interfaces.CommandServices.**Task**` provoca **CS0118**
(`'Task' es espacio de nombres pero se usa como tipo`) en **todos** los interfaces hermanos del proyecto
(porque dentro de `...CommandServices` el namespace `Task` tapa al tipo `System.Threading.Tasks.Task`).
→ Se usó **`Tasks`** (plural) para el namespace/carpeta.

---

## 9. Verificación (Oct 2026)

**Build 0 errores.** Sin migración (columnas `Nombre` / `ModoRendimiento` / `Eliminado` ya existían desde `AddWorkOrderRendimiento`).

Smoke test real (login `root`, `http://localhost:5199`):

| Prueba | Resultado |
|---|---|
| `GET GetTaskList` | 31 tareas, modos correctos (1/2/6/19=`MaterialEfficiency`, 3=`OutputPerManHour`) |
| `POST /api/task` (ManHours) | 200, id 32 |
| `POST` modo inválido (`NoExiste`) | **422** ("El modo de rendimiento no es válido") |
| `POST` descripción vacía | **422** ("La descripción de la tarea es obligatoria") |
| `PUT` → `MaterialEfficiency` | 200, actualiza descripción + modo |
| `DELETE /api/task/32` | 200 `true`; `Eliminado=1` en DB |
| `GET GetTaskList` post-delete | tarea 32 **ausente** |
| `GET GetTaskList?includeDeleted=true` | tarea 32 **presente** |
| `GET getTaskById/32` (eliminada) | 200 |
| `DELETE /api/task/999999` | **404** |
| `PUT` id inexistente | **404** |
| `POST createworkorder` con tarea 32 (eliminada) | **422** ("La tarea seleccionada no existe") |
| `POST createworkorder` con tarea 3 (activa) | 200 |
| Regresión `getCompleteInfo/68` | 200, `task(id=3 Cosecha)`, `mode=OutputPerManHour` |
| Sin token | **401** |

**Task 32** quedó como dato de prueba (soft-deleted). **Sin commits aún** (pendiente de indicación).
