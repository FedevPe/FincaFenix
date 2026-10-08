# Plan de Refactorización del Backend — FincaFenix

> Generado: 2026-05-21
> Objetivo: Mejorar rendimiento, mantenibilidad y entendimiento del código sin cambiar el funcionamiento del sistema.
> Meta final: Backend limpio listo para servir una SPA React.

---

## Stack Objetivo Post-Refactorización

| Categoría | Tecnología | Versión |
|---|---|---|
| Runtime | .NET | 9.0 (STS actual) |
| ORM | Entity Framework Core | 9.x |
| Base de datos | SQL Server | — |
| CQRS + Mediador | MediatR | última |
| Mapeo | AutoMapper o Mapperly | — |
| Autenticación | ASP.NET Core Identity (store) + JWT (auth) | — |
| Validación | FluentValidation | — |
| Frontend | React (futuro, fuera de este plan) | — |

---

## Fases de Ejecución

### Fase 0 — Quick Wins (Bugs + Deuda Técnica Inmediata)

**Objetivo:** Corregir bugs existentes y código claramente incorrecto.

- [ ] Corregir `DetailWorkOrderController.GetActivitiesByOrderId` — descomentar llamada al interactor
- [ ] Corregir ruta de `MachineController`: `[Route("api/(controller)")]` → `[Route("api/[controller]")]`
- [ ] Corregir rutas ambiguas en `MaterialController`: `{categoryId}/material` y `{recipeId}/material`
- [ ] Corregir `MaterialController.GetListMaterialByRecipeId` — llama al método correcto del interactor
- [ ] Implementar `MaterialInteractor.GetMaterialListByRecipeId` (hoy lanza `NotImplementedException`)
- [ ] Corregir mensaje en `FarmQueryService.GetFarmById`: "Task with ID" → "Farm with ID"
- [ ] Eliminar `try/catch { throw; }` inútiles en:
  - `DetailWorkOrderInteractor.AddDetailWorkOrder`
  - `DetailWOCommandService.SaveDetailWorkOrderAsync`
  - `UpdateWorkOrderCommand.UpdateWorkOrderState`
- [ ] Eliminar registros duplicados en DI (`IAddDetailWorkOrderInputPort` e `IUpdateWorkOrderInputPort`)
- [ ] Unificar casing en rutas de controllers
- [ ] Implementar `UpdateWorkOrderCommand.UpdateWorkOrder()` (hoy `NotImplementedException`)
- [ ] Agregar `Microsoft.EntityFrameworkCore` usando donde falte

---

### Fase 1 — Upgrade a .NET 9

**Objetivo:** Actualizar target framework y paquetes a .NET 9.

**Razón:** .NET 9 es la versión STS estable más reciente. .NET 10 es preview (release LTS nov 2026). Migrar 7→9:
- Mejoras de performance en JIT, GC, y LINQ
- `TimeProvider`, `SearchValues`, `Regex` source gen, etc.
- EF Core 9 mejoras en consultas, traducción de GroupBy, ExecuteUpdate/Delete

**Tareas:**

- [x] Actualizar `TargetFramework` en todos los `.csproj`: `net7.0` → `net9.0`
- [x] Actualizar paquetes NuGet a sus versiones 9.x compatibles:
  - `Microsoft.EntityFrameworkCore` → 9.0.0
  - `Microsoft.EntityFrameworkCore.SqlServer` → 9.0.0
  - `Microsoft.EntityFrameworkCore.Tools` → 9.0.0
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` → 9.0.0
  - `Microsoft.AspNetCore.Identity.UI` → 9.0.0
  - `Microsoft.Extensions.Identity.Core` → 9.0.0
  - `Microsoft.Extensions.Configuration` → 9.0.0
  - `FluentValidation` → 11.11.0 (compatible)
  - `MudBlazor` → 7.16.0 (compatible)
  - `QuestPDF` → 2025.7.0 (compatible)
- [x] Verificar que `Microsoft.NET.Sdk.Web` compile correctamente
- [x] Revisar breaking changes: https://learn.microsoft.com/en-us/dotnet/core/compatibility/
- [x] Build completo y verificación de que la solución compila

**⚠️ Atención:** `FincaFenix.Repositories/UniversitarySystem.EFCore` apunta a .NET 8 y es un proyecto externo. Evaluar si se elimina del todo.

---

### Fase 2 — Optimización de Consultas (Performance)

**Objetivo:** Eliminar SELECT *, reducir round-trips a BD, consultas más eficientes.

#### 2.1 Reemplazar SELECT * por proyecciones

| Archivo | Acción | Estado |
|---|---|---|
| `TaskQueryService.cs` | Tabla pequeña (3 cols), overhead mínimo | Saltado |
| `MaterialQueryService.cs` | Se agregó `AsNoTracking()` | Hecho |
| `FarmQueryService.cs` | Tabla pequeña (3 cols), overhead mínimo | Saltado |
| `MachineQueryServices.cs` | Tabla pequeña (5 cols), overhead mínimo | Saltado |
| `MaterialCategoryQueryService.cs` | Tabla pequeña (2 cols), overhead mínimo | Saltado |
| `EmployeeQueryService.cs` | Ya usa `.Select()` en navegación | Sin cambios |
| `DetailWOQueryService.cs` | Reemplazado Include por `.Select()` con proyección exacta de columnas consumidas por el Presenter | Hecho |
| `DetailSectorQueryService.cs` | Reemplazado Include/ThenInclude por `.Select()` con proyección exacta | Hecho |

#### 2.2 Corregir anti-patrón GroupBy

- [x] `MaterialQueryService.GetMaterialListByCategoryId`: simplificado a `GroupBy().Select(g => g.First())`. EF Core 9 traduce esto a `ROW_NUMBER()` ventana SQL en servidor, eliminando la evaluación cliente. Se agregó `AsNoTracking()`.

#### 2.3 Optimizar GetWorkOrderInformationQuery

- [x] `GetAllWorkOrderList()` — se agregó `.AsSplitQuery()` (evita explosión cartesiana de 11+ JOINs) + `.AsNoTracking()` (evita cambio de tracking). Los comentarios inline se eliminaron para claridad.
- [x] `GetWorkOrderAndRecipeByIdWorkorder()` — mismo tratamiento: `.AsSplitQuery()` + `.AsNoTracking()`.
- [x] `GetWorkOrderListPaged`: unificado el `Where` en una sola variable `baseQuery` compartida por COUNT y SELECT (evita desincronización de criterios). Se decidió mantener 2 round-trips: unificar en 1 query con SQL crudo no compensa contra la proyección anidada de `SectorList`.
- [x] Eliminados comentarios inline redundantes en las cadenas de Include

#### 2.4 Optimizar CreateWorkOrderCommand

- [x] Reducido de 4 `SaveChangesAsync()` a 2: las operaciones de Recipe.Add + reNumber increment se hacen en un solo SaveChanges; WorkOrder.Add + woNumber increment en otro. También se cambió `FirstOrDefault` a `FirstOrDefaultAsync` para las queries iniciales.

---

### Fase 3 — Implementar MediatR + AutoMapper

**Objetivo:** Reemplazar la cadena `Controller → Interactor → Presenter` por handlers de MediatR, y eliminar mapeo manual con AutoMapper.

#### 3.1 MediatR — Estructura

```
FincaFenix.UsesCases/
├── WorkOrder/
│   ├── Queries/
│   │   └── GetWorkOrderList/
│   │       ├── GetWorkOrderListQuery.cs
│   │       ├── GetWorkOrderListQueryHandler.cs
│   │       └── GetWorkOrderListResponse.cs
│   └── Commands/
│       └── CreateWorkOrder/
│           ├── CreateWorkOrderCommand.cs
│           ├── CreateWorkOrderCommandHandler.cs
│           └── CreateWorkOrderResponse.cs
├── Farm/
│   ├── Queries/
│   │   ├── GetFarmListQuery.cs
│   │   └── GetFarmByIdQuery.cs
│   └── Commands/ (si aplica)
├── Material/
│   ├── Queries/
│   └── Commands/
└── ...
```

**Reglas:**
- **Queries** → solo lectura, retornan DTOs. Van directo al QueryService.
- **Commands** → modifican datos, usan validación + transacción. Van al CommandService.
- Handlers son inyectados directamente en los Controllers de la API.

#### 3.2 MediatR — Pipeline Behaviors

Agregar behaviors cross-cutting:

1. **ValidationBehavior** — ejecuta FluentValidation automáticamente antes del handler
2. **LoggingBehavior** — log de request/response (incluye log de excepciones + rethrow)
3. **ExceptionBehavior** — **cerrado como redundante (Oct 2026)**: LoggingBehavior ya loguea excepciones con contexto del request, y `ExceptionMiddleware` (FincaFenix.Controllers/Middleware) ya transforma `NotFoundException`→404, `ValidationException`→422, `UnauthorizedAccessException`→403, resto→500 con ProblemDetails. No se implementa.

#### 3.3 AutoMapper

- [ ] Instalar `AutoMapper` + `AutoMapper.Extensions.Microsoft.DependencyInjection`
- [ ] Crear perfiles de mapeo (`MappingProfile.cs`) por módulo:
  - `WorkOrderMappingProfile`
  - `FarmMappingProfile`
  - `MaterialMappingProfile`
  - etc.
- [ ] Eliminar `WorkOrderMapper.cs` (198 líneas) y `DetailWorkOrderMapper.cs`
- [ ] Reemplazar mapeo manual en Presenters por inyección de `IMapper`

**⚠️ Alternativa:** Evaluar **Mapperly** (source generator) si la performance del mapping es crítica. Cero reflection, genera código en tiempo de compilación.

---

### Fase 4 — Refactorizar Presenters y Gateways (Consolidación de Capas)

**Objetivo:** Mantener las capas pero darles responsabilidades reales.

**Decisión arquitectónica:** Se mantienen ambas capas por principio de Clean Architecture / SOLID, pero se redefine su rol.

#### 4.1 Presenters — Nuevo Rol

Ya no mapean entidades a DTOs (eso lo hace AutoMapper + las proyecciones). Ahora:

- Transforman resultados de handlers de MediatR para consumo del controller/API
- Manejan formateo de fechas (cultura `es-AR`)
- Enriquecen respuestas con metadata (ej: total de registros, página actual)
- Se inyectan en los handlers de MediatR o se usan como wrappers de respuesta

```csharp
// Ejemplo de nuevo rol
public class WorkOrderPresenter
{
    public PagedResponse<WorkOrderDTO> PresentPagedResult(
        IEnumerable<WorkOrderDTO> items, int totalCount, int page, int pageSize) 
        => new PagedResponse<WorkOrderDTO>
        {
            Data = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
}
```

#### 4.2 Gateways — Nuevo Rol

Ya no son proxies de un solo query/command service. Ahora:

- Orquestan múltiples operaciones (ej: crear orden + receta + correlativo en una transacción)
- Combinan resultados de múltiples queries
- Son el punto de entrada para operaciones complejas que involucran varias entidades

```csharp
// Ejemplo de nuevo rol
public class WorkOrderGateway(
    IWorkOrderQueryService queryService,
    IRecipeQueryService recipeQueryService,
    ICorrelativeNumberService correlativeService)
{
    public async Task<WorkOrderDetailDTO> GetWorkOrderWithRecipeDetailAsync(int workOrderId)
    {
        var workOrder = await queryService.GetByIdAsync(workOrderId);
        var recipe = await recipeQueryService.GetByWorkOrderIdAsync(workOrderId);
        var correlative = await correlativeService.GetCurrentNumberAsync("OrdenTrabajo");
        return new WorkOrderDetailDTO { WorkOrder = workOrder, Recipe = recipe, CorrelativeNumber = correlative.LastNumber };
    }
}
```

---

### Fase 5 — JWT + Identity (Autenticación y Autorización)

**Objetivo:** Identity se queda como store de usuarios (BD, password hashing, roles). JWT como mecanismo de autenticación para la API REST.

#### 5.1 Arquitectura

```
Login Request (user/pass) → /api/auth/login
  → UserManager<ApplicationUser>.FindByNameAsync()
  → UserManager.CheckPasswordAsync()
  → Generar JWT (claims: userId, roles, userName)
  → Devolver { accessToken, refreshToken?, expiresIn }

Siguientes requests:
  → Header: Authorization: Bearer <token>
  → Middleware JWT valida token
  → HttpContext.User.Identity.IsAuthenticated = true
```

#### 5.2 Tareas

- [x] Instalar paquetes:
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `System.IdentityModel.Tokens.Jwt`
- [x] Configurar JWT en `AuthDependencyContainer.cs` (llamado desde `ServicesDependencyContainer`):
  - `AddAuthentication().AddJwtBearer()`
  - Validar emisor, audiencia, expiry, signing key
- [x] Crear servicio `IAuthService` + `AuthService`:
  - `LoginAsync(LoginDTO)` → verifica credenciales contra Identity, genera JWT
- [x] Crear endpoint `/api/auth/login` (Controller de API)
- [x] Configurar `appsettings.json` con `JwtSettings:SecretKey`, `JwtSettings:Issuer`, `JwtSettings:Audience`, `JwtSettings:ExpirationInMinutes`
- [x] Migrar middleware: reemplazar cookies por JWT Bearer:
  - Mantener Identity como store (no se tocan tablas AspNetUsers)
  - JWT configurado como default en AuthDependencyContainer

**⚠️ No eliminar Identity** — solo se agrega JWT como capa de autenticación arriba de Identity.

---

### Fase 6 — Limpiar Capa Blazor

**Objetivo:** Eliminar todo lo específico de Blazor Server que no se usará con React.

- [ ] Eliminar `FincaFenix.ViewModels/` — son específicos de Blazor
- [ ] Eliminar Pages `.razor`, Components, `_Imports.razor`, `_Host.cshtml`
- [ ] Eliminar `Shared/MainLayout.razor`, `Shared/NavMenu.razor`
- [ ] Limpiar `DependencyContainer.cs` de UI (servicios y validadores específicos)
- [ ] Limpiar `Program.cs`:
  - Quitar `AddServerSideBlazor()`
  - Quitar `AddRazorPages()` si ya no se usan
  - Quitar `MapBlazorHub()`
  - Quitar `MapFallbackToPage("/_Host")`
- [ ] Verificar que los Controllers de API sigan funcionando sin Blazor

---

### Fase 7 — Middleware de Errores, Logging y Validación

**Objetivo:** Que el sistema sea robusto y observable.

#### 7.1 Error Handling

- [x] Implementar `ExceptionMiddleware` global:
  - `NotFoundException` → 404
  - `ValidationException` (FluentValidation) → 422 / 400
  - `UnauthorizedAccessException` → 403
  - `Exception` genérica → 500 (con log detallado)
- [x] Reemplazar `KeyNotFoundException` con `NotFoundException` en FarmQueryService + TaskQueryService
- [x] Usar `ProblemDetails` estándar para errores

#### 7.2 Logging

- [x] Log de requests/respuestas vía MediatR `LoggingBehavior` (ILogger<T>)
- [x] Configurar EF Core interceptor para log de queries lentas (>500ms) — `FincaFenix.EFCore/Interceptors/SlowQueryLogInterceptor.cs`, registrado en WebAPI (`DatabaseConfiguration.cs`). Umbral configurable: `EfCore:SlowQueryThresholdMs`. La UI Blazor no lo tiene (proyecto deprecado).

#### 7.3 Validación

- [x] FluentValidation ya es dependencia — crear validadores para LoginDTO, CreateWorkOrderCommand, AddDetailWorkOrderCommand
- [x] Conectar con MediatR `ValidationBehavior` para validación automática
- [x] Eliminar validación manual de `CreateWorkOrderHandler` (pipeline la reemplaza)

---

## Buenas Prácticas Transversales

### Async/Await
- Usar `ConfigureAwait(false)` en librerías (aunque en ASP.NET Core no es crítico)
- No mezclar `return Task<T>` directo con `async/await` en el mismo proyecto (elegir uno)
- Presenter methods síncronos deben ser `void` o devolver valores, no `Task.CompletedTask`

### DI
- Mantener `Presenter` y `Gateway` como `Scoped` (tienen estado)
- Handlers de MediatR como `Transient`
- QueryServices sin estado → `Transient`
- CommandServices → `Transient`

### Naming
- Unificar estilo de rutas API: preferir `kebab-case` (`/api/work-orders` en vez de `/api/[controller]`)
- Unificar sufijos de endpoints: `GET /api/work-orders`, `GET /api/work-orders/{id}`, `POST /api/work-orders`

### Seguridad
- Endpoints de API con `[Authorize(Roles = "...")]`
- Secrets (`Jwt:SecretKey`) fuera del código — usar User Secrets o variables de entorno
- HTTPS obligatorio en producción

---

## Glosario de Archivos a Crear/Modificar/Eliminar

| Archivo | Acción |
|---|---|
| `.csproj` de cada proyecto | Modificar target + paquetes |
| `FincaFenix.UsesCases/Interactors/*.cs` | Eliminar (reemplazados por MediatR handlers) |
| `FincaFenix.UsesCases/Mappers/*.cs` | Eliminar (reemplazados por AutoMapper) |
| `FincaFenix.Presenters/*.cs` | Refactorizar con nuevo rol |
| `FincaFenix.Gateways/*.cs` | Refactorizar con nuevo rol |
| `FincaFenix.EFCore/Services/QueryServices/*.cs` | Agregar proyecciones `.Select()` |
| `FincaFenix.EFCore/Services/CommandServices/*.cs` | Optimizar transacciones |
| `FincaFenix.ViewModels/*` | Eliminar |
| `FincaFenix.UserInterface7.0/Pages/*.razor` | Eliminar |
| `FincaFenix.UserInterface7.0/Components/*` | Eliminar |
| `FincaFenix.UserInterface7.0/Shared/*.razor` | Eliminar |
| `FincaFenix.Controllers/*.cs` | Refactorizar (inyectar handlers de MediatR) |
| `FincaFenix.InversionOfControl/ServicesDependencyContainer.cs` | Refactorizar DI |
| `Program.cs` | Limpiar Blazor, agregar JWT, MediatR, AutoMapper |

---

## Recomendaciones Finales

1. **No mezclar fases** — completar una antes de empezar otra. Especialmente Fase 3 (MediatR) depende de Fase 0 y 1.
2. **Branch por fase** — cada fase en su propia branch para PRs manejables.
3. **Prueba manual después de cada fase** — buildear, correr, verificar endpoints clave.
4. **La Fase 0 es prioritaria** — los bugs actuales pueden hacer que el sistema falle aunque el resto de la refactorización esté perfecta.
5. **Identity/JWT (Fase 5) se puede mover antes o después de MediatR** — no hay dependencia fuerte, pero es más fácil probar la API sin Blazor después de tener JWT.
