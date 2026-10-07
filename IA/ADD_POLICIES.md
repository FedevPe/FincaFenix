# ADD_POLICIES — Plan de Implementación de Políticas de Autorización

## Objetivo

Implementar un sistema de autorización basado en políticas (policies) en FincaFenix para restringir el acceso a endpoints según claims de políticas asignados a roles. Actualmente todos los controladores usan `[Authorize]` bare (solo requiere autenticación, sin verificar roles ni policies), y `DetailWorkOrderController` ni siquiera tiene `[Authorize]`.

---

## Estado Actual (resumen del análisis)

### Qué existe
| Componente | Estado |
|---|---|
| `PolicyMaster.cs` | Define 40 constantes de políticas — **corregido** (Phase 1) |
| `CustomClaims.cs` | Define `POLICIES = "POLICIES"` como claim type |
| `SeedDataBase.cs` | `SeedAddPoliciesAsync` asigna claims policies a roles via `roleManager.AddClaimAsync()` |
| `PolicyContainer.cs` | Configura `UseAsyncSeeding` en el DbContext |
| `AuthService.cs` | Genera JWT con claims de rol — **NO incluye policy claims** |
| `AuthDependencyContainer.cs` | Configura JWT Bearer — **NO registra políticas de autorización** |
| Controladores | Usan `[Authorize]` bare o ninguno |

### Problemas detectados
1. ~~12 constantes en `PolicyMaster.cs` con valores incorrectos por copy-paste~~ **(Corregido)**
2. ~~Typo `WORKOREDER_CREATE` en vez de `WORKORDER_CREATE`~~ **(Corregido)**
3. `SeedDataBase.cs` crea `new FincaFenixContext()` innecesario
4. `AuthService.cs` no incluye policy claims en el JWT
5. No hay registro de `AuthorizationPolicy` via `AddPolicy()`
6. Controladores no usan `[Authorize(Policy = "...")]`
7. `DetailWorkOrderController` no tiene `[Authorize]`

---

## Plan de Implementación

### Fase 1: Corregir `PolicyMaster.cs` ✅ COMPLETADA

| Archivo | Cambio |
|---|---|
| `PolicyMaster.cs` | Corregir las 12 constantes con valores incorrectos (usar `nameof(X)` correcto) |
| `PolicyMaster.cs` | Corregir typo `WORKOREDER_CREATE` → `WORKORDER_CREATE` |

### Fase 2: Corregir `SeedDataBase.cs` ✅ COMPLETADA

| Archivo | Cambio |
|---|---|
| `SeedDataBase.cs` | Eliminar `new FincaFenixContext()` innecesario, usa `roleManager.FindByNameAsync()` |
| `SeedDataBase.cs` | Refactorizado a `AssignPoliciesToRoleAsync()` con dedup vía `HashSet` |

### Fase 3: Incluir policy claims en el JWT (`AuthService.cs`) ✅ COMPLETADA

| Archivo | Detalle |
|---|---|
| `AuthService.cs` | Inyecta `RoleManager<IdentityRole>`, agrega policy claims al JWT |

### Fase 4: Registrar políticas de autorización ✅ COMPLETADA

| Archivo | Detalle |
|---|---|
| `AuthDependencyContainer.cs` | `services.AddAuthorization()` con 40 policies vía `RequireClaim(CustomClaims.POLICIES, name)` |

### Fase 5: Aplicar `[Authorize(Policy = "…")]` en controladores ✅ COMPLETADA

| Archivo | Cambio |
|---|---|
| `WorkOrderController.cs` | Policies por método: WORKORDER_CREATE, WORKORDER_READ, WORKORDER_UPDATE |
| `DetailWorkOrderController.cs` | Agregado `[Authorize]` + DETAIL_WORKORDER_CREATE / DETAIL_WORKORDER_READ |
| `FarmController.cs` | `[Authorize(Policy = PolicyMaster.FARM_READ)]` |
| `DetailSectorController.cs` | `[Authorize(Policy = PolicyMaster.SECTOR_FARM_READ)]` |
| `EmployeeController.cs` | `[Authorize(Policy = PolicyMaster.EMPLOYEE_READ)]` |
| `MachineController.cs` | `[Authorize(Policy = PolicyMaster.MACHINE_READ)]` |
| `MaterialController.cs` | `[Authorize(Policy = PolicyMaster.MATERIAL_READ)]` |
| `MaterialCategoryController.cs` | `[Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_READ)]` |
| `TaskController.cs` | `[Authorize(Policy = PolicyMaster.TASK_READ)]` |
| `AuthController.cs` | Sin cambios (AllowAnonymous en login) |

### Fase 6: Migración + Seeding ✅ COMPLETADA

| Acción | Detalle |
|---|---|
| `dotnet ef migrations add AddPolicySeeding` | Checkpoint (sin cambios de esquema) |
| `dotnet ef database update` | Aplicada vía WebApi |
| Seeding en WebApi `Program.cs` | `SeedDataBase.SeedAddPoliciesAsync()` ejecutado al iniciar la app |
| Seeding via `PolicyContainer.ConfigureSeeding` | `UseAsyncSeeding` para cuando `SaveChangesAsync` se ejecute |

### Fase 7: Compilación y verificación ✅ COMPLETADA

| Acción |
|---|
| WebApi: 0 errores de compilación |
| Policies verificadas en BD via `INSERT INTO AspNetRoleClaims` logs |

---

## Políticas definidas (40 totales)

Cada entidad tiene 4 operaciones: `CREATE`, `READ`, `UPDATE`, `DELETE`.

- WORKORDER, RECIPE, DETAIL_WORKORDER, TASK, FARM, SECTOR_FARM, MATERIAL, MATERIAL_CATEGORY, MACHINE, FRUIT, FRUIT_VARIETY, EMPLOYEE, USER

## Claims por rol (SeedDataBase)

| Rol | Policies asignadas |
|---|---|
| `desarrollador` | Todas (40) — acceso total |
| `admin` | Todas (40) — acceso total |
| `supervisor` | Solo READ de todas las entidades + DETAIL_WORKORDER_CREATE |
| `operario` | WORKORDER_READ, RECIPE_READ, DETAIL_WORKORDER_CREATE, DETAIL_WORKORDER_READ |
