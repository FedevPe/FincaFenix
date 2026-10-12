# Plan de ejecución — Página "Configuración" (Settings)

Alcance: consolidar los CRUD de los datos de sistema (Fincas, Materiales, Tareas,
Empleados, Categorías de material, Unidades de medida y Usuarios) en una única
página **Configuración** con pestañas, accesible desde el fondo del menú lateral.
Cada pestaña lista registros activos, permite incluir eliminados/desactivados
para reactivar, filtrar (search server-side), crear, editar y borrar (lógico).

## Decisiones confirmadas (Oct 2026)

- **Empleados**: multi-asignación de fincas **sin** flag "finca principal":
  el form elige varias fincas y se sincronizan las filas de `Empleado_Finca`
  (tabla puente existente, sin columna nueva ni migración).
- **Usuarios**: desactivación vía nueva columna `Activo` (bool, default true)
  en `AspNetUsers` + migración EF. El login rechaza usuarios inactivos.
- **Nav**: ítem "Configuración" al **final** del sidebar, visible solo si el
  usuario tiene algún READ del conjunto de entidades de config.
- **`/tasks` standalone se elimina**: pasa a ser una pestaña de Configuración.
- **Paginación**: server-side para Fincas, Empleados, Materiales y Tareas
  (tienen muchos registros). Categorías y Unidades mantienen listas cortas
  (client-side). Usuarios: server-side.
- **Sin commits** hasta nueva indicación (convención del repo).

---

## Backend

### Fase 1 — CRUD de Fincas (hoy read-only)

- **DTOs** (`Entities/DTOs/FarmDTOs/`):
  - `FarmFilterDTO`: `PageNumber=1`, `PageSize=10`, `Search?`, `IncludeDeleted`.
  - `SaveFarmDTO`: `Id?`, `Name`.
  - `FarmDTO`: agregar `IsDeleted` (mapper automático, misma propiedad en la entidad).
- **`FarmQueryService.GetFarmListPagedAsync(filter)`**: Count + `OrderBy(Name)`
  + Skip/Take; excluye `IsDeleted` salvo `IncludeDeleted`.
- **`FarmRepository`**: passthrough.
- **`FarmCommandService`** (EFCore, `IUnitOfWork`): Create (reset `Id=0`,
  `IsDeleted=false`), Update, Delete (soft `Eliminado=true`), Reactivate
  (`IsDeleted=false`). Mismo patrón que Task/Material.
- **MediatR**: `GetFarmListPagedQuery`, `CreateFarmCommand`, `UpdateFarmCommand`,
  `DeleteFarmCommand`, `ReactivateFarmCommand` + handlers.
- **Validator**: `Name` obligatorio, ≤ 100 → 422 (`ValidationBehavior`).
- **`FarmController`**: de `[Authorize(FARM_READ)]` class-level a `[Authorize]`
  + policy por acción (patrón MaterialController):
  - `GET api/farm/paged` (FARM_READ)
  - `POST api/farm` (FARM_CREATE)
  - `PUT api/farm` (FARM_UPDATE)
  - `DELETE api/farm/{id}` (FARM_DELETE)
  - `PUT api/farm/ReactivateFarm/{id}` (FARM_UPDATE)
- Actualizar `IFarmController`.

### Fase 2 — CRUD de Empleados + multi-finca

- **DTOs** (`Entities/DTOs/EmployeeDTOs/`):
  - `EmployeeFilterDTO`: `FarmId?`, `Search?`, `IncludeDeleted`, paging.
  - `SaveEmployeeDTO`: `Id?`, `Name`, `LastName`, `List<int> FarmIds` (≥ 1).
  - `EmployeeDTO`: agregar `IsDeleted` + `FarmIds`
    (`MapFrom(x => x.FarmList.Select(ef => ef.FarmId))` en MappingProfile).
- **`EmployeeQueryService.GetEmployeeListPagedAsync`**: listado **global**
  (opcional filtro `FarmId` por join a `Empleado_Finca`), excluye
  `Employee.IsDeleted` salvo toggle.
- **`EmployeeCommandService`** (`IUnitOfWork`):
  - Create: inserta empleado + fila `Empleado_Finca` por cada `FarmId`.
  - Update: reemplaza las filas puente (delete + insert de la selección).
  - Delete: soft `IsDeleted=true`. Reactivate: `IsDeleted=false`.
  - El bridge usa `DeleteBehavior.NoAction` → manejar filas explícitamente, sin cascada.
- **MediatR** + handlers + validators (`Name`/`LastName` ≤ 50, `FarmIds` ≥ 1 → 422).
- **`EmployeeController`**: `GET api/employee/paged`, `POST`, `PUT`,
  `DELETE {id}`, `PUT ReactivateEmployee/{id}` (policies `EMPLOYEE_*`).
  Actualizar `IEmployeeController`.

### Fase 3 — Materiales: solo reactivar

- `ReactivateAsync` en `MaterialCommandService`/`IMaterialCommandService` +
  `Reactivate` en `MaterialRepository`; `ReactivateMaterialCommand` + handler;
  `PUT api/material/ReactivateMaterial/{id}` (MATERIAL_UPDATE).
- Resto del CRUD ya existe (paged, create/update/delete soft).

### Fase 4 — Unidad de medida: includeDeleted + reactivar

- `UnitOfMeasureQueryService.GetAllAsync(bool includeDeleted)` (el actual
  `GetList` filtra `!IsDeleted`).
- `ReactivateAsync` en `UnitOfMeasureCommandService`;
  `ReactivateUnitOfMeasureCommand` + handler;
  `PUT api/unitofmeasure/ReactivateUnitOfMeasure/{id}` (UNIT_OF_MEASURE_UPDATE).
- `UnitOfMeasureDTO`: agregar `IsDeleted`.

### Fase 5 — Usuarios (lo más pesado)

- **Migración**: columna `Activo` (bool, default true) → `ApplicationUser.IsActive`.
- **Login**: `AuthService.LoginAsync` valida `IsActive` tras `CheckPasswordAsync`
  → 401 "Usuario inactivo".
- **Roles**: seeding **idempotente** en `SeedDataBase`
  (`roleManager.CreateAsync(new IdentityRole(...))` para admin/desarrollador/
  supervisor/operario — hoy solo se buscan, nunca se crean).
- **DTOs** (`Entities/DTOs/UserDTOs/`):
  - `UserDTO`: `Id`, `UserName`, `Email?`, `Name?`, `LastName?`, `IsActive`, `Roles`.
  - `CreateUserDTO`: `UserName`, `Email?`, `Name?`, `LastName?`, `Password`, `RoleName`.
  - `UpdateUserDTO`: `Id`, `Name?`, `LastName?`, `RoleName`.
  - `UserFilterDTO`: paging + `Search?` + `IncludeInactive`.
- **`UserQueryService`** (EFCore, inyecta `UserManager`/`RoleManager`):
  `GetUserListPagedAsync` (Search por UserName/Email/Name/LastName, toggle
  inactivos), `GetRolesAsync`.
- **`UserCommandService`**:
  - Create: `UserManager.CreateAsync` + `AddToRoleAsync` (password respeta la
    política de Identity: 6+ chars, dígito, mayúscula/minúscula, sin símbolo).
  - Update: Name/LastName + cambio de rol (`RemoveFromRoleAsync`/`AddToRoleAsync`).
  - Deactivate: `IsActive=false`. Reactivate: `IsActive=true`.
  - Validadores: campos obligatorios, rol válido y existente → 422.
- **`UserController`**:
  - `GET api/user/paged` (USER_READ)
  - `POST api/user` (USER_CREATE)
  - `PUT api/user` (USER_UPDATE)
  - `PUT api/user/DeactivateUser/{id}` (USER_UPDATE)
  - `PUT api/user/ReactivateUser/{id}` (USER_UPDATE)
- `IUserController` nuevo.
- **Nota**: `USER_READ` no está en `supervisorPolicies` → la pestaña Usuarios
  queda solo para admin/desarrollador (sin cambios de seed).

**DI**: registrar `IUserQueryService`/`IUserCommandService` y repos/query services
de Farm/Employee en los DependencyContainers de Gateways y EFCore.

---

## Frontend (FincaFenix.Frontend)

### Fase 6 — Shell

- **`app/(app)/settings/page.tsx`** con `<Suspense fallback={<LoadingState/>}>`
  → `features/settings/pages/Settings.tsx`.
- **Sidebar**: tras `</nav>` (line ~98), bloque inferior fijo `mt-auto` + `border-t`
  con ítem **"Configuración"** (icono `Settings`), visible si el usuario tiene algún
  READ del conjunto `[FARM_READ, MATERIAL_READ, TASK_READ, EMPLOYEE_READ,
  MATERIAL_CATEGORY_READ, UNIT_OF_MEASURE_READ, USER_READ]`. **Quitar** "Tareas"
  del nav principal.
- **AppBar** (`getTitle`): `if (pathname.startsWith("/settings")) return "Configuración";`.
- **`Settings.tsx`**: `Tabs` URL-sync `?tab=` (`history.replaceState`, patrón de
  `Inventory.tsx`). Tabs dinámicos (solo los que el usuario puede leer):
  `fincas · materiales · tareas · empleados · categorias · unidades · usuarios`.
  Si no hay ningún READ → `EmptyState` "Acceso restringido".
- **Eliminar** `/tasks` standalone: borrar `app/(app)/tasks/page.tsx` y
  `features/tasks/pages/TaskList.tsx`; el contenido pasa a
  `features/settings/tabs/TareasTab.tsx` reutilizando `task.service.ts`,
  `TaskForm` y `features/tasks/constants.ts`.

### Fase 7 — Tabs (patrón TaskList: buscador con debounce, toggle "incluir
eliminadas/desactivadas", DataTable + Pagination server-side, Modal-form,
ConfirmDialog y dropdown "Reactivar" cuando aplica)

- **FincasTab** (`FARM_READ/CREATE/UPDATE/DELETE`): tabla Id/Nombre/Estado;
  form solo Nombre (`SaveFarmDTO`).
- **MaterialesTab** (`MATERIAL_READ/CREATE/UPDATE/DELETE`): `MaterialDTO` (tipo
  nuevo); filtro `categoryId` + search; form con categoría (`getMaterialCategories`),
  unidad de medida, divisa (`getCurrencies`, fallback ARS), costo referencial.
- **TareasTab** (`TASK_*`): reutiliza `TaskForm`.
- **EmpleadosTab** (`EMPLOYEE_*`): form Nombre/Apellido + multi-select de fincas
  (checkboxes de `getFarms` vía `CheckboxRow`), mínimo 1 finca; la tabla muestra
  las fincas asignadas.
- **CategoriasTab** (`MATERIAL_CATEGORY_*`): lista + form (Description); delete
  físico con guarda (422 si tiene materiales) → **sin toggle/reactivar** (el
  modelo no tiene IsDeleted).
- **UnidadesTab** (`UNIT_OF_MEASURE_*`): form Description; toggle eliminadas + reactivar.
- **UsuariosTab** (`USER_READ/CREATE/UPDATE`): tabla UserName/Email/Nombre/Rol/
  Estado; form Create (password + dropdown de roles) y edit (nombre/rol, sin
  password en v1); acciones deactivar/reactivar.

### Fase 8 — Tipos y servicios

- **Tipos** (`types/api/`): extender `FarmDTO`/`EmployeeDTO` (`isDeleted`,
  `farmIds`); nuevos `MaterialDTO`, `UnitOfMeasureDTO`, `SaveFarmDTO`,
  `SaveEmployeeDTO`, `SaveMaterialDTO`/`UpdateMaterialDTO`, `SaveUnitOfMeasureDTO`,
  `SaveMaterialCategoryDTO`, `UserDTO`, `CreateUserDTO`, `UpdateUserDTO`; re-exports en barrel.
- **Servicios**: nuevos `farm.service.ts` y `user.service.ts`; extender
  `material.service.ts` (paged CRUD + reactivar + categorías + unidades) y
  `employee.service.ts`; reutilizar `task.service.ts` y `workOrder.service.ts`
  (`getFarms`).
- Policies inline (convención del repo: no hay archivo de constantes).

---

## Verificación final

- Matar WebApi vieja si está corriendo con binarios viejos; `dotnet build
  FincaFenix.sln` → 0 errores; `dotnet ef migrations add AddUserActivo` +
  `database update`.
- Smoke: login; por entidad create/update/delete/reactivate + paged; login de
  usuario inactivado → 401; cambio de rol; regresión endpoints existentes
  (workorders, inventario, PDF).
- `npx tsc --noEmit` + `npm run build` → 0 errores (rutas: `/settings` nueva, sin `/tasks`).

---

## Fuera de alcance v1

- Edición de password de usuario en el tab.
- Flag "finca principal" del empleado.
- CRUD de divisas (solo lectura).
- Paged server-side para Categorías y Unidades (maestros pequeños).

---

## Progreso de ejecución (Oct 2026)

### Backend — COMPLETO (Fases 1–5)

- **Fase 1 (Fincas)** ✅: DTOs `FarmFilterDTO`/`SaveFarmDTO`, `FarmDTO.IsDeleted`;
  `FarmQueryService.GetFarmListPaged`, `FarmCommandService` (Create/Update/Delete/
  Reactivate con `IUnitOfWork`), handlers `FarmQueries`/`FarmCommands`, validadores,
  `FarmController` con policy por acción, DI. **Fix**: agregado
  `.ForMember(d => d.FarmList, o => o.Ignore())` en `SaveFarmDTO → FarmEntity`
  (el `AssertConfigurationIsValid` de AutoMapper reventaba el arranque).
- **Fase 2 (Empleados)** ✅: `EmployeeFilterDTO`/`SaveEmployeeDTO`, `EmployeeDTO`
  (`IsDeleted`+`FarmIds`); `EmployeeQueryService.GetEmployeeListPaged` (filtro
  `FarmId`, search, `Include(EmployeeList)`); `EmployeeCommandService` (Create con
  filas puente, Update reemplaza filas, Delete soft, Reactivate); handlers,
  validadores, `EmployeeController` per-action, DI.
- **Fase 3 (Material reactivar)** ✅: `ReactivateMaterialCommand` +
  `PUT api/material/ReactivateMaterial/{id}`.
- **Fase 4 (Unidad includeDeleted + reactivar)** ✅:
  `GetUnitOfMeasureListQuery(bool IncludeDeleted)`, `ReactivateUnitOfMeasureCommand`
  + `PUT api/unitofmeasure/ReactivateUnitOfMeasure/{id}`; `UnitOfMeasureDTO.IsDeleted`.
- **Fase 5 (Usuarios)** ✅: `ApplicationUser.IsActive` + `ApplicationUserConfiguration`
  (`HasDefaultValue(true)`) + migración `20261011052044_AddUserActivo` (aplicada);
  guard en `AuthService.LoginAsync`; seeding idempotente de roles (`EnsureRoleAsync`)
  + root idempotente en `SeedDataBase`; DTOs `UserDTO`/`CreateUserDTO`/`UpdateUserDTO`/
  `UserFilterDTO`; `UserQueryService`/`UserCommandService` (UserManager/RoleManager);
  handlers `UserQueries`/`UserCommands`; validadores (password vs política Identity,
  rol existente, username único); `UserController` + `IUserController`; DI.
- **Fix transversal**: los validadores de **Farm y Employee** no estaban
  registrados en `AddUseCasesServices` (por eso `farmIds` vacío no daba 422);
  agregados junto con los de User.

### Smoke test (build 0 errores + perfil https)

login root (desarrollador); `user/paged` y `user/roles` OK; create usuario
(id real, rol `operario`) → weak password/invalid role/duplicado = **422**;
update rol (`supervisor`); getById; deactivate → login del usuario = **401**;
reactivate; Farm create + paged; Employee create (multi-finca) + paged, sin
fincas = **422**; Material create/delete/paged(deleted)/reactivate; UOM
create/delete/list(includeDeleted)/reactivate; Task list. Todo OK.

> Detalle de entorno: el perfil por defecto de `dotnet run` es **http** y la
> cookie de auth es `Secure=true`/`SameSite=None`, por lo que el smoke test se
> corrió con `--launch-profile https` + bypass de certificado.

### Frontend — COMPLETO (Fases 6–8)

- **Fase 6 (Shell)** ✅: `app/(app)/settings/page.tsx` (`<Suspense>`);
  `features/settings/pages/Settings.tsx` (Tabs URL-sync `?tab=`, tabs filtrados
  por `hasPolicy`, `EmptyState` de acceso restringido); Sidebar con ítem
  **Configuración** al fondo (`border-t`, visible por READ de config) y
  **sin** el ítem "Tareas"; AppBar `/settings` → "Configuración".
  **Eliminados** `app/(app)/tasks/page.tsx` y
  `features/tasks/pages/TaskList.tsx` (contenido → `TareasTab`).
- **Fase 7 (Tabs)** ✅: `FincasTab`, `MaterialesTab`, `TareasTab`,
  `EmpleadosTab`, `CategoriasTab`, `UnidadesTab`, `UsuariosTab`
  (patrón TaskList: debounce, toggle de eliminados/desactivados, DataTable +
  Pagination server-side, Modal-form, ConfirmDialog, reactivar). Forms:
  `FarmForm`, `MaterialForm`, `EmployeeForm`, `CategoryForm`, `UnitForm`,
  `UserForm`; `TareasTab` reutiliza `TaskForm`.
- **Fase 8 (Tipos y servicios)** ✅: `types/api/settings.ts` (+ re-export);
  `FarmDTO`/`EmployeeDTO` extendidos; servicios `farm`/`employee`/`user` +
  `material.service.ts` extendido (paged CRUD, reactivar, categorías, unidades).

**Verificado**: `npx tsc --noEmit` + `npm run build` → **0 errores** (10 rutas;
`/settings` presente, `/tasks` ausente).

### Pendiente

- Regresión backend de endpoints existentes (workorders, inventario, PDF).
- Smoke E2E de la UI de Configuración si se desea (no automatizado).