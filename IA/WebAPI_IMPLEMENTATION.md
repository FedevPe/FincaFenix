# FincaFenix — Plan de Implementación: Web API Standalone + Swagger

> Generado: 2026-05-22
> Objetivo: Crear proyecto `FincaFenix.WebApi` como interfaz de ejecución para probar todos los endpoints vía Swagger, sin modificar la arquitectura existente.

---

## Alcance

- Crear proyecto `FincaFenix.WebApi` (ASP.NET Core Web API)
- Integrar Swagger UI con autenticación JWT Bearer
- Configurar CORS para desarrollo
- Corregir el endpoint `GetWorkOrderListPaginated` que retorna ValueTuple → crear DTO `PagedResult<T>`
- NO se crean proyectos de testing en esta fase

---

## Stack

| Componente | Versión |
|---|---|
| .NET SDK | 9.0 |
| ASP.NET Core | 9.0 |
| Swashbuckle.AspNetCore | 7.2.0 |
| Microsoft.AspNetCore.OpenApi | 9.0.0 |

---

## Etapa 1 — Creación del Proyecto

### 1.1 Scaffolding

```powershell
dotnet new webapi -n FincaFenix.WebApi --no-openapi -o FincaFenix.WebApi
```

> `--no-openapi` evita que la plantilla genere el boilerplate de OpenAPI (lo configuraremos manualmente).

### 1.2 Archivo: `FincaFenix.WebApi/FincaFenix.WebApi.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>disable</Nullable>
    <LangVersion>12.0</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\FincaFenix.InversionOfControl\FincaFenix.InversionOfControl.csproj" />
    <ProjectReference Include="..\FincaFenix.EFCore\FincaFenix.EFCore.csproj" />
    <ProjectReference Include="..\FincaFenix.Entities\FincaFenix.Entities.csproj" />
  </ItemGroup>

</Project>
```

> No se requieren paquetes JWT ni Identity directamente — llegan transitivamente desde `FincaFenix.Controllers` (vía `InversionOfControl`).

### 1.3 Agregar a la solución

```powershell
dotnet sln add FincaFenix.WebApi/FincaFenix.WebApi.csproj --solution-folder "4. Frameworks and Drivers"
```

---

## Etapa 2 — Configuración de la Aplicación

### 2.1 Archivo: `FincaFenix.WebApi/Program.cs`

**Estrategia de organización:** El `Program.cs` se mantiene como punto de entrada principal, pero las configuraciones de servicios se delegan a métodos de extensión externos para mantenerlo legible:

- Configuración de OpenAPI/Swagger → método `AddSwaggerConfiguration()`
- Configuración de Identity + DbContext → método `AddIdentityAndDatabase()`

```csharp
using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.WebApi.Configurations;
using FincaFenixControllers.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var defaultCulture = "es-AR";
var culture = new CultureInfo(defaultCulture);
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddServicesContainer(builder.Configuration);
builder.Services.AddIdentityAndDatabase(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FincaFenix API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 2.2 Archivo: `FincaFenix.WebApi/Configurations/SwaggerConfiguration.cs`

```csharp
using Microsoft.OpenApi.Models;

namespace FincaFenix.WebApi.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FincaFenix API",
                Version = "v1",
                Description = "API REST de FincaFenix — Gestión de órdenes de trabajo agrícolas"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingrese el token JWT obtenido de POST /api/auth/login"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
```

### 2.3 Archivo: `FincaFenix.WebApi/Configurations/DatabaseConfiguration.cs`

```csharp
using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.WebApi.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddIdentityAndDatabase(
        this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FincaFenixContext>(conf => conf.UseSqlServer(conn));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.AllowedForNewUsers = false;
        })
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<FincaFenixContext>()
        .AddRoles<IdentityRole>();

        return services;
    }
}
```

### 2.4 Archivo: `FincaFenix.WebApi/Properties/launchSettings.json`

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### 2.5 Archivo: `FincaFenix.WebApi/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=FEDEPC\\SQLEXPRESS;Initial Catalog=FincaFenixDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "<minimo 32 caracteres — ver Local Secrets en AGENTS.md>",
    "Issuer": "FincaFenix",
    "Audience": "FincaFenixAPI",
    "ExpirationInMinutes": 60
  },
  "AllowedHosts": "*"
}
```

> ⚠️ Corregido el trailing comma de `ConnectionStrings` que existía en el `appsettings.json` del UI project. Sin `CircuitOptions.DetailedErrors` (específico de Blazor).

---

## Etapa 3 — Fix del ValueTuple (`PagedResult<T>`)

### 3.1 Crear DTO: `FincaFenix.Entities/DTOs/Common/PagedResult.cs`

Propósito: Reemplazar el retorno `(IEnumerable<ShowWorkOrderDTO>, int)` del endpoint paginado para que Swagger documente correctamente la respuesta.

```csharp
namespace FincaFenix.Entities.DTOs.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
```

### 3.2 Modificar handler: `FincaFenix.UsesCases/UseCases/WorkOrder/WorkOrderQueries.cs`

Buscar el handler de `GetWorkOrderListPaginatedQuery` y cambiar:

```csharp
// ANTES: retorna ValueTuple
public async Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> Handle(
    GetWorkOrderListPaginatedQuery request, CancellationToken cancellationToken)
{
    var result = await repository.GetWorkOrderListPaginated(
        request.PageNumber, request.PageSize, request.Status);
    return result;
}

// DESPUÉS: retorna PagedResult<ShowWorkOrderDTO>
public async Task<PagedResult<ShowWorkOrderDTO>> Handle(
    GetWorkOrderListPaginatedQuery request, CancellationToken cancellationToken)
{
    var (workOrders, totalCount) = await repository.GetWorkOrderListPaginated(
        request.PageNumber, request.PageSize, request.Status);

    return new PagedResult<ShowWorkOrderDTO>
    {
        Items = workOrders,
        TotalCount = totalCount,
        Page = request.PageNumber,
        PageSize = request.PageSize
    };
}
```

### 3.3 Modificar controller: `FincaFenix.Controllers/Implementations/WorkOrder/WorkOrderController.cs`

```csharp
// ANTES:
[HttpGet("{pagenumber}/{pagesize}/getworkorderlistpaginated")]
public async Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalAcount)> 
    GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
{
    return await mediator.Send(new GetWorkOrderListPaginatedQuery(pageNumber, pageSize, status));
}

// DESPUÉS:
[HttpGet("{pagenumber}/{pagesize}/getworkorderlistpaginated")]
public async Task<PagedResult<ShowWorkOrderDTO>> 
    GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
{
    return await mediator.Send(new GetWorkOrderListPaginatedQuery(pageNumber, pageSize, status));
}
```

Agregar `using FincaFenix.Entities.DTOs.Common;` si no está presente.

---

## Etapa 4 — Build y Verificación

### 4.1 Compilar

```powershell
dotnet build FincaFenix.sln
```

Esperado: **0 errores, 0 warnings**.

### 4.2 Ejecutar

```powershell
dotnet run --project FincaFenix.WebApi
```

### 4.3 Probar en Swagger UI

| Paso | Acción |
|---|---|
| 1 | Navegar a `http://localhost:5000/swagger` |
| 2 | Verificar los 10 controladores y 21 endpoints listados |
| 3 | `POST /api/auth/login` con credenciales válidas |
| 4 | Copiar el token JWT |
| 5 | Click "Authorize" → pegar `Bearer <token>` |
| 6 | Probar endpoints GET (Farm, Task, Machine, etc.) |
| 7 | Verificar que `GET /api/workorder/{page}/{size}/getworkorderlistpaginated` ahora muestra `PagedResult` correctamente |

---

## Diagrama de Dependencias

```
FincaFenix.WebApi (Microsoft.NET.Sdk.Web)
├── Swashbuckle.AspNetCore 7.2        ← Swagger UI
├── Microsoft.AspNetCore.OpenApi 9.0  ← OpenAPI doc
├── FincaFenix.InversionOfControl     ← DI composition root
│   ├── FincaFenix.Controllers        ← 10 controllers (21 endpoints)
│   ├── FincaFenix.UsesCases          ← MediatR handlers + validators
│   ├── FincaFenix.Gateways           ← Repository implementations
│   ├── FincaFenix.EFCore             ← DbContext + queries/commands
│   └── FincaFenix.Entities           ← POCOs, DTOs, Enums
├── FincaFenix.EFCore                 ← DbContext (design-time)
└── FincaFenix.Entities               ← ApplicationUser, IdentityRole
```

---

## Resumen de Cambios

| Archivo | Acción |
|---|---|
| `FincaFenix.WebApi/FincaFenix.WebApi.csproj` | Crear |
| `FincaFenix.WebApi/Program.cs` | Crear |
| `FincaFenix.WebApi/Configurations/SwaggerConfiguration.cs` | Crear |
| `FincaFenix.WebApi/Configurations/DatabaseConfiguration.cs` | Crear |
| `FincaFenix.WebApi/appsettings.json` | Crear |
| `FincaFenix.WebApi/Properties/launchSettings.json` | Crear |
| `FincaFenix.Entities/DTOs/Common/PagedResult.cs` | Crear |
| `FincaFenix.UsesCases/UseCases/WorkOrder/WorkOrderQueries.cs` | Modificar (handler → PagedResult) |
| `FincaFenix.Controllers/Implementations/WorkOrder/WorkOrderController.cs` | Modificar (método → PagedResult) |
| `FincaFenix.sln` | Modificar (agregar proyecto) |

**Total: 7 archivos creados, 3 archivos modificados. Cero archivos eliminados.**

---

## Notas

- `FincaFenix.UserInterface7.0` no se modifica — sigue funcionando como Blazor Server con API embebida
- El endpoint `GET /api/material/recipe/{recipeId}/material` seguirá lanzando `NotImplementedException` (fuera del alcance de esta fase)
- La conexión a BD requiere SQL Server corriendo en `FEDEPC\SQLEXPRESS` con autenticación integrada de Windows
- CORS en modo `AllowAny` solo para desarrollo; en producción debe restringirse al origen del frontend
- `GetWorkOrderListPaginated` usa `[FromQuery]` para `status` (no va en la ruta) — Swagger lo muestra como parámetro query
