# FincaFenix — Technology Stack

> Generado el 2026-05-21. Actualizado post-Fase 1 (Upgrade .NET 9) y Fase 2 (Query Optimization).

---

## 1. Lenguaje y Runtime

| Tecnología | Versión |
|---|---|
| **.NET SDK** | 9.0 (todos los proyectos principales) |
| | 8.0 (solo `FincaFenix.Repositories/UniversitarySystem.EFCore` — legacy) |
| **Lenguaje** | C# 12 (LangVersion explícito) |
| **IDE** | Visual Studio 2022 |

---

## 2. Framework Principal

| Tecnología | Versión | Proyecto(s) |
|---|---|---|
| **ASP.NET Core** | 9.0 | `FincaFenix.UserInterface7.0` |
| **Blazor Server** | 9.0 | `FincaFenix.UserInterface7.0` |
| **Razor Pages** | 9.0 | `FincaFenix.UserInterface7.0` (Identity UI) |
| **ASP.NET Core MVC (Controllers)** | 9.0 | `FincaFenix.UserInterface7.0` + `FincaFenix.Controllers` |

---

## 3. Arquitectura

**Clean Architecture** con 4 capas + librerías auxiliares:

```
1. Enterprise Business Rules (Domain)
   └── FincaFenix.Entities

2. Application Business Rules
   └── FincaFenix.UsesCases

3. Interface Adapters
   ├── FincaFenix.Controllers
   ├── FincaFenix.Gateways
   ├── FincaFenix.Presenters
   └── FincaFenix.ViewModels

4. Frameworks & Drivers (Infrastructure)
   ├── FincaFenix.EFCore          (Data Access)
   ├── FincaFenix.InversionOfControl  (Composition Root / DI)
   └── FincaFenix.UserInterface7.0   (Blazor Server UI)

Libraries
   ├── FincaFenix.PDF
   └── FincaFenix.UIValidators

Standalone / Legacy
   ├── FincaFenix.Validators
   ├── FincaFenix.Validations
   └── FincaFenix.Repositories/UniversitarySystem.EFCore
```

**Flujo de dependencias:**  
UI → InversionOfControl → (Controllers, Gateways, Presenters, ViewModels, EFCore, PDF) → UsesCases → Entities

---

## 4. UI / Componentes

| Tecnología | Versión | Propósito |
|---|---|---|
| **Blazor Server** | 7.0 | Interfaz de usuario principal |
| **MudBlazor** | 7.16.0 | Librería de componentes UI (tablas, formularios, notificaciones) |
| **Bootstrap** | 5.x (aprox.) | CSS base (`wwwroot/css/bootstrap/`) |
| **Open Iconic** | — | Iconos base (`wwwroot/css/open-iconic/`) |
| **JavaScript (vanilla)** | — | Script auxiliar (`wwwroot/scripts/detailWorkOrderPage.js`) |

---

## 5. Acceso a Datos

| Tecnología | Versión | Propósito |
|---|---|---|
| **Entity Framework Core** | 9.0.0 | ORM principal |
| **EF Core SqlServer** | 9.0.0 | Provider para SQL Server |
| **EF Core Tools** | 9.0.0 | Migraciones y scaffolding (solo desarrollo) |

---

## 6. Base de Datos

| Componente | Detalle |
|---|---|
| **Motor** | Microsoft SQL Server |
| **Instancia** | `FEDEPC\SQLEXPRESS` |
| **Base de datos** | `FincaFenixDB` |
| **Autenticación** | Windows Integrated Security |
| **Conexión** | `Data Source=FEDEPC\\SQLEXPRESS;Initial Catalog=FincaFenixDB;Integrated Security=True;...` |

---

## 7. Autenticación y Seguridad

| Tecnología | Versión | Propósito |
|---|---|---|
| **ASP.NET Core Identity** | 9.0.0 | Gestión de usuarios, roles y autenticación |
| **Identity UI** | 9.0.0 | Páginas Razor de login/registro |
| **Identity.EntityFrameworkCore** | 9.0.0 | Store de Identity en EF Core |
| **Identity.Core** | 9.0.0 | Núcleo de Identity (usado en capa Domain) |

Esquema: Cookies con sliding expiration de 1 hora, login requerido globalmente.

---

## 8. Validación

| Tecnología | Versión | Proyecto(s) |
|---|---|---|
| **FluentValidation** | 11.11.0 | `FincaFenix.UsesCases`, `FincaFenix.Validators`, `FincaFenix.Validations`, `FincaFenix.UserInterface7.0` |

> **Nota:** Existen dos proyectos duplicados (`Validators` y `Validations`) que probablemente deberían unificarse.

---

## 9. Generación de PDF

| Tecnología | Versión | Proyecto |
|---|---|---|
| **QuestPDF** | 2025.7.0 | `FincaFenix.PDF` |

---

## 10. Inyección de Dependencias

| Tecnología | Versión |
|---|---|
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 9.0.0 |
| Contenedor nativo de .NET (sin contenedores de terceros) | — |

---

## 11. Logging

| Tecnología | Propósito |
|---|---|
| `Microsoft.Extensions.Logging` (built-in) | Configurado en `appsettings.json` |
| **Nivel por defecto:** Information | `Microsoft.AspNetCore`: Warning |

---

## 12. Herramientas de Desarrollo

| Herramienta | Versión | Propósito |
|---|---|---|
| **Visual Studio 2022** | v17 | IDE principal |
| **dotnet-ef** | 9.0.8 | CLI para migraciones EF Core (definido en `dotnet-tools.json`) |
| **Microsoft.VisualStudio.Web.CodeGeneration.Design** | 9.0.0 | Scaffolding de ASP.NET Core |

---

## 13. CI/CD

| Plataforma | Estado |
|---|---|
| **GitHub Actions** | Carpeta `.github/` presente pero **vacía** (sin workflows definidos) |
| **Docker** | **No hay** Dockerfile ni docker-compose |

---

## 14. Proyectos Detectados como Legacy / Redundantes

| Proyecto | Problema |
|---|---|
| `FincaFenix.UserInterface` | Solo contiene `bin/` y `obj/` — fue reemplazado por `UserInterface7.0` |
| `FincaFenix.WebAPI` | Solo contiene `bin/` y `obj/` — parece abandonado |
| `FincaFenix.Validators` | Redundante con `FincaFenix.Validations` (ambos usan FluentValidation + Entities) |
| `FincaFenix.Validations` | Redundante con `FincaFenix.Validators` |
| `FincaFenix.Repositories/UniversitarySystem.EFCore` | Es un proyecto **.NET 8** aislado que referencia `UniversitarySystem.UsesCases` (no existe en esta solución) — probablemente un residuo de otro proyecto |

---

## 15. Testing

**No se detectaron proyectos de testing** en toda la solución.

---

## 16. Resumen Visual

```
┌──────────────────────────────────────────────────────────────┐
│                    Blazor Server UI                          │
│  MudBlazor 7.16 · Bootstrap 5 · JS · Identity UI            │
│  ┌────────────────────────────────────────────────────────┐  │
│  │           InversionOfControl (DI Composition)          │  │
│  └───────┬────────┬──────────┬──────────┬───────────────┘  │
│          │        │          │          │                    │
│     ┌────┴┐  ┌───┴───┐ ┌───┴────┐ ┌───┴─────┐              │
│     │Ctrl │  │Gateways│ │Present.│ │ViewMod.│              │
│     └──┬──┘  └───┬───┘ └───┬────┘ └───┬─────┘              │
│        │         │         │          │                    │
│        └─────────┴──┬──────┴──────────┘                    │
│                     │                                      │
│              ┌──────┴──────┐                               │
│              │  UsesCases  │  FluentValidation 11.11       │
│              └──────┬──────┘                               │
│                     │                                      │
│              ┌──────┴──────┐                               │
│              │   Entities  │  Identity.EntityFrameworkCore  │
│              └─────────────┘                               │
│                                                             │
│  ┌──────────────┐  ┌──────────────┐                        │
│  │  EFCore      │  │  PDF         │                        │
│  │  SQL Server  │  │  QuestPDF    │                        │
│  │  Identity    │  │  2025.7.0   │                        │
│  └──────────────┘  └──────────────┘                        │
└──────────────────────────────────────────────────────────────┘
```
