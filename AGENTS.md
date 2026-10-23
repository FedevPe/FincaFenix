# AGENTS.md — FincaFenix

> Contexto permanente para agentes de OpenCode. Refleja el estado **verificado** del repositorio a octubre 2026. No incluye historial de implementación ni planes completados.

---

## 1. Descripción del sistema

**FincaFenix** es un sistema de gestión agrícola para órdenes de trabajo, inventario, materiales, tareas y configuración de fincas.

- **Backend**: .NET 9, ASP.NET Core Web API, Clean Architecture, MediatR, AutoMapper 15, EF Core 9, SQL Server
- **Frontend**: Next.js 15 (App Router, SSR shell-only), React 19, TypeScript, TailwindCSS v4, shadcn/ui, lucide-react
- **Autenticación**: JWT + cookies httpOnly, policy-based authorization (roles: `admin`, `desarrollador`, `supervisor`, `operario`)
- **Cultura**: `es-AR` global (fechas, números, coma decimal)

---

## 2. Estructura de proyectos (verificada)

| Capa | Proyecto | Responsabilidad |
|------|----------|-----------------|
| Domain | `FincaFenix.Entities` | POCOs, DTOs, Enums, `UnitConverter`, `RendimientoCalculator` |
| Application | `FincaFenix.UsesCases` | MediatR Handlers, AutoMapper profiles, FluentValidation |
| Interface Adapters | `FincaFenix.Controllers` | Controllers HTTP |
| | `FincaFenix.Gateways` | Interfaces (Ports): Repositories, Query/Command Services, `IUnitOfWork` |
| Infrastructure | `FincaFenix.EFCore` | DbContext, configuraciones, migraciones, Query/Command Services, `EfCoreUnitOfWork`, interceptors |
| | `FincaFenix.InversionOfControl` | Composition Root DI (`AddServicesContainer`) |
| | `FincaFenix.WebAPI` | Host Web API (Program.cs, Swagger, middlewares, auth) |
| Libraries | `FincaFenix.PDF` | QuestPDF generation |
| Legacy (no tocar) | `FincaFenix.UIValidators`, `FincaFenix.Validators`, `FincaFenix.Validations`, `FincaFenix.UserInterface7.0` | Sin referencias activas / deprecados |

> **Eliminados en Fase 6**: `FincaFenix.UserInterface7.0` (Blazor) y `FincaFenix.ViewModels`. La UI actual es **Next.js en `FincaFenix.Frontend`**.

---

## 3. Arquitectura y flujo de dependencias

```
Clients (Next.js) → WebAPI/Controllers → InversionOfControl (DI) → Controllers → UsesCases (MediatR Handlers) → Gateways (Interfaces) → EFCore
                                                                                                                    ↕
                                                                                                               Entities
```

- **DI Scoping**: Controllers/Handlers → `Transient`; Gateways/Repositories/EFCore Services → `Scoped`
- **Unit of Work**: `IUnitOfWork` (Gateways) + `EfCoreUnitOfWork` (EFCore) sobre `Database.BeginTransactionAsync`, registrado **Scoped**. Orquestación en `CreateWorkOrderRepository`, `AddDetailWorkOrderRepository`, `UpdateWorkOrderCommand`.
- **MediatR + AutoMapper 15**: Reemplazan patrón Interactor/Presenter (Fase 3). `MappingProfile.cs` centraliza mapeos; `AssertConfigurationIsValid()` al startup.
- **Validación**: `ValidationBehavior` (MediatR pipeline) + FluentValidation → HTTP 422 via `ExceptionMiddleware`.

---

## 4. Convenciones transversales (verificadas en código)

| Convención | Detalle |
|------------|---------|
| **Nullability** | `Nullable: disable` en todos los `.csproj` **excepto** `FincaFenix.Entities` (`enable`). MVC mitigado con `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes` en `Program.cs`. |
| **ImplicitUsings** | `enable` + C# 12 (`LangVersion`) en todos los proyectos. |
| **Cultura** | `es-AR` en `Program.cs` (`CultureInfo.DefaultThreadCurrentCulture/UICulture`). |
| **Auth** | JWT con claims de policies. `[Authorize(Policy = PolicyMaster.XXX)]`. Cookies httpOnly para Next.js (proxy same-origin). |
| **DI Registration** | Cada capa expone `Add{Layer}Services()`; `ServicesDependencyContainer` las encadena. |
| **Excepciones de negocio** | `BusinessRuleException` → HTTP 422 via `ExceptionMiddleware`. `NotFoundException` → HTTP 404. |
| **Frontend** | Solo Tailwind + shadcn/ui primitives. Sin `*.module.css`. Iconos solo `lucide-react`. Formato es-AR vía `parseDecimal()`, `formatNumber()`, `formatCurrency()`. SSR auth en `(app)/layout.tsx` con `cookies()` + `/api/auth/me`. |
| **Proxy API** | `next.config.ts` reescribe `/api/*` → `API_PROXY_TARGET` (default `http://localhost:5000`). Cookie first-party, sin CORS browser. |

---

## 5. Invariantes de negocio y restricciones técnicas críticas

### 5.1 Inventario y transacciones
- **Stock**: `StockDisponible = StockFisico − StockReservado`. Validado con lock pesimista `WITH (UPDLOCK, HOLDLOCK)` sobre `StockPorFinca`.
- **Reservas**: Se crean al crear OT con receta (`RegisterReservationsAndCostsAsync`). Estado `Activa` → `Consumida` → `Liberada`. OT sin receta = sin reservas.
- **Costo congelado**: `CostoOrdenTrabajo` snapshot inmutable al crear OT (`CostoUnitario = Material.CostoReferencia`, `IdDivisa = Material.IdDivisa`). No se recalcula.
- **Consumo**: Recalculado server-side en cada `addDetailWO` (`WorkOrderConsumptionCommand`). Fórmula: `ConsumoReal = Σ MachinePasses × DosisNormalizada` (vía `UnitConverter`). Exceso se registra sin aplicar → nunca stock negativo, nunca rechaza actividad.
- **Consumo manual**: Endpoint `POST /api/inventory/consumption` (policy `CONSUMPTION_CREATE`), transacción propia.
- **Ciclo de vida OT**: `Pendiente → {Activo, Cerrado, Cancelado}`, `Activo → {Cerrado, Cancelado}`, `Cerrado/Cancelado` terminales. `Cerrado` libera reservas sobrantes; `Cancelado` rechaza si hay actividades/consumos.

### 5.2 Unidades de medida
- **Unidad base por material**: Única unidad de stock, reservas, movimientos, consumos, costos. Si se requiere otra unidad → material duplicado.
- **Conversión**: `UnitConverter` (familias: Masa kg/gr, Volumen lts/cc, Unidad, Metro, Paquete). Normaliza códigos (`lts/lt/litro→lts`, `cc/ml→cc`, `kg/kilo→kg`). Familia incompatible → `BusinessRuleException` (422).
- **Aplicado en**: Reservas+costos, ajuste de reservas (UpdateWorkOrder), consumo automático/manual. `Consumo.Unidad` guarda código base.

### 5.3 Rendimiento y % área trabajada
- **Calculado en consulta** (no persistido) vía `RendimientoCalculator`.
- **RendimientoModeEnum**: `ManHours=0`, `MaterialEfficiency=1`, `AreaPerManHour=2`, `OutputPerManHour=3`. **El modo de la tarea manda sobre la presencia de receta.**
- **Fórmula MaterialEfficiency**: `% = real/teórico × 100` (rojo si >100). Desviación = `rendimiento − 100` cuando supera 100%.
- **% Área trabajada**: Delta acumulativo por (OT, sector). Columna `PorcentajeAreaTrabajada` (decimal(18,2), default 100 backfill). Validación autoritativa en `AddDetailWorkOrderRepository` con `UPDLOCK/HOLDLOCK` → 422 si acumulado > 100%. `AreaPerManHour` usa área real; `MaterialEfficiency` teórico usa área total.

### 5.4 Autorización
- Policies en `PolicyMaster` (40 policies). Seed idempotente al arranque (`SeedDataBase.SeedAddPoliciesAsync`).
- Matriz rol-policy: `desarrollador`/`admin` = todas; `supervisor` = READ todas + `DETAIL_WORKORDER_CREATE`; `operario` = `WORKORDER_READ`, `RECIPE_READ`, `DETAIL_WORKORDER_CREATE`, `DETAIL_WORKORDER_READ`.
- Frontend: `hasPolicy(user.policies, "POLICY")` para gating de UI.

---

## 6. Comandos de verificación confirmados

```powershell
# Backend (desde raíz del repo)
dotnet build FincaFenix.sln
dotnet run --project FincaFenix.WebAPI          # http://localhost:5000 | https://localhost:5001 (Swagger)

# EF Core Migraciones (desde raíz)
dotnet tool restore
dotnet ef migrations add <Name> --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI
dotnet ef database update --project FincaFenix.EFCore --startup-project FincaFenix.WebAPI

# Frontend (en FincaFenix.Frontend)
npm install
npm run dev                                     # http://localhost:3000 (proxy /api → WebAPI)
npm run build                                   # typecheck + build SSR
npx tsc --noEmit                                # solo typecheck
```

> **Secrets**: `appsettings.Development.json` (en `FincaFenix.WebAPI`) con `JwtSettings.SecretKey` (≥32 chars) es **gitignored**. Clones frescos deben crearlo. Connection strings en `appsettings.json` (tracked, local dev DB).

---

## 7. Documentación de referencia (archivos que existen y son vigentes)

| Archivo | Propósito |
|---------|-----------|
| `IA/DOCUMENTACION_UI.md` | **Documentación viva del frontend** (arquitectura, componentes, servicios, convenciones es-AR, límites) |
| `IA/STACK.md` | Análisis de stack tecnológico (algunas secciones desactualizadas: UI dice Blazor, real es Next.js) |
| `IA/PLAN_EJECUCION_MODULO_INVENTARIO.md` | Especificación detallada del módulo de inventario (reglas, fórmulas, endpoints) |
| `IA/PLAN_CONFIGURACION_SETTINGS.md` | Especificación de la página Configuración (backend + frontend) |
| `IA/PLAN_PORCENTAJE_AREA_Y_RENDIMIENTO.md` | Especificación de % área trabajada y fórmula de rendimiento |
| `IA/PLAN_EJECUCION_RENDIMIENTO_OT.md` | Especificación de rendimiento OT (maquinadas, modos de tarea) |
| `IA/PLAN_EJECUCION_CRUD_MATERIALES.md` | Especificación CRUD Materiales/Categorías/UOM |
| `IA/PLAN_EJECUCION_CRUD_FRUTAS_VARIEDADES.md` | Especificación CRUD Frutas/Variedades |
| `IA/PLAN_EJECUCION_CRUD_TAREAS.md` | Especificación CRUD Tareas |
| `IA/PLAN_MIGRACION_NEXT_TAILWIND.md` | Plan de migración UI (histórico, completado) |

> **Nota**: Los archivos `PLAN_*.md` son especificaciones de referencia. No son instrucciones para agentes salvo que el orquestador los cite explícitamente.

---

## 8. Reglas de seguridad y acciones sensibles

| Acción | Requiere autorización explícita |
|--------|--------------------------------|
| Ejecutar `dotnet ef database update` / `drop` | Sí |
| Aplicar migraciones sobre BD real | Sí |
| Scripts DDL/DML destructivos | Sí |
| Modificar `appsettings.Development.json` (secrets) | No versionar; solo local |
| Cambiar `PolicyMaster` o seed de policies | Sí (afecta autorización global) |
| Modificar `UnitConverter` / `RendimientoCalculator` | Sí (impacta cálculos críticos) |
| Alterar `IUnitOfWork` o transacciones de inventario | Sí (riesgo de inconsistencia) |
| Introducir nuevas dependencias NuGet/npm | Sí |

---

## 9. Pautas para mantener actualizada esta documentación

1. **Actualizá este archivo** cuando cambien: arquitectura, convenciones transversales, invariantes de negocio, comandos de build, estructura de proyectos, o se agreguen/eliminen proyectos.
2. **No agregués** historial de implementación, planes completados, resultados de smoke tests, o "sin commits aún".
3. **Derivá a documentación especializada** (`IA/DOCUMENTACION_UI.md`, `IA/PLAN_*.md`) para detalles de implementación, endpoints, DTOs, o reglas de módulos específicos.
4. **Verificá contra el código** antes de documentar una convención. Si el código no la sigue consistentemente, no la impongas como regla global.
5. **Eliminá referencias a proyectos eliminados** (`UserInterface7.0`, `ViewModels`) salvo en la tabla de Legacy.

---

## 10. Referencia rápida: Endpoints y policies clave

| Módulo | Endpoints representativos | Policies principales |
|--------|---------------------------|---------------------|
| WorkOrders | `GET/POST /api/workorder/*`, `PUT /api/workorder/updateworkorder`, `PUT /api/workorder/updatestateworkorder` | `WORKORDER_READ`, `WORKORDER_CREATE`, `WORKORDER_UPDATE` |
| DetailWorkOrder | `POST /api/detailworkorder/addDetailWO`, `GET /api/detailworkorder/getActivitiesByOrderId/{id}` | `DETAIL_WORKORDER_CREATE`, `DETAIL_WORKORDER_READ` |
| Inventory | `GET /api/inventory/stock/*`, `POST /api/inventory/movement`, `GET /api/inventory/costs/*`, `POST /api/inventory/consumption` | `STOCK_READ`, `MOVEMENT_CREATE`, `CONSUMPTION_CREATE` |
| Materials | `GET/POST/PUT/DELETE /api/material/*`, `GET /api/materialcategory/*`, `GET/POST/PUT/DELETE /api/unitofmeasure/*` | `MATERIAL_*`, `MATERIAL_CATEGORY_*`, `UNIT_OF_MEASURE_*` |
| Settings | `GET/POST/PUT/DELETE /api/farm/*`, `/api/employee/*`, `/api/user/*`, `/api/task/*`, `/api/fruit/*`, `/api/fruitvariety/*` | `FARM_*`, `EMPLOYEE_*`, `USER_*`, `TASK_*`, `FRUIT_*`, `FRUIT_VARIETY_*` |
| Auth | `POST /api/auth/login`, `GET /api/auth/me`, `POST /api/auth/logout` | — |

---

## 11. Notas técnicas (resumen)

- **AutoMapper 15.1.1**: Requiere `ILoggerFactory` en `MapperConfiguration` ctor. Licencia requerida para producción (dev/test gratis).
- **Concurrencia**: Lock pesimista `UPDLOCK/HOLDLOCK` en `StockPorFinca` y `DetalleOrdenTrabajo`. Sin retry por `DbUpdateConcurrencyException` (el lock evita la carrera).
- **Paginación**: COUNT + SELECT separados (decidido por proyecciones anidadas complejas).
- **Interceptors**: `SlowQueryLogInterceptor` loguea queries > `EfCore:SlowQueryThresholdMs` (default 500ms) en WebAPI.
- **Frontend**: `@tanstack/react-table` v8.21.3 (v9 API incompatible). `DataTable` wrapper preserva API propia.
- **No tests**: No hay proyectos de testing en la solución.

---

*Última actualización: octubre 2026 — Auditoría completa del repositorio.*