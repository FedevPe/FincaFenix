# WorkOrder PDF Export — Endpoint WebApi

> Generado: 2026-05-23
> Objetivo: Exponer un endpoint en la WebApi que permita descargar un PDF con la información completa de una Orden de Trabajo.

---

## Contexto

Actualmente la generación de PDF de órdenes de trabajo se realiza directamente desde los componentes Blazor:

```
Blazor Component → new WorkOrderPDF(dto) → PDFService (IJSRuntime) → download
```

Esto acopla la generación del PDF a la UI. Para desacoplar y preparar el backend para una futura SPA React, se migra esta funcionalidad a un endpoint de la WebApi.

---

## Endpoint nuevo

```
GET /api/workorder/{id}/pdf
```

- **Policy**: `WORKORDER_READ` (reutiliza la policy existente)
- **Response**: `Content-Type: application/pdf` con el archivo PDF
- **Flow**:
  1. Obtiene `ShowWorkOrderDTO` vía `GetWorkOrderByIdQuery` (MediatR) — endpoint existente
  2. Genera el PDF vía `IPdfGenerationService` (nueva abstracción)
  3. Retorna `FileContentResult` con los bytes del PDF

---

## Abstracción

Se introduce `IPdfGenerationService` para desacoplar la generación del PDF de su implementación concreta:

| Capa | Artefacto |
|---|---|
| `FincaFenix.PDF` | `IPdfGenerationService` (interfaz) |
| `FincaFenix.PDF` | `WorkOrderPdfService` (implementación con QuestPDF) |
| Vía DI | `services.AddTransient<IPdfGenerationService, WorkOrderPdfService>()` |

---

## Cambios por proyecto

### 1. `FincaFenix.Entities` — Sin cambios (reutiliza `WORKORDER_READ`)

### 2. `FincaFenix.PDF`

| Archivo | Cambio |
|---|---|
| `IPdfGenerationService.cs` | **Crear** — interfaz con `byte[] GenerateWorkOrderPdf(ShowWorkOrderDTO)` |
| `WorkOrderPdfService.cs` | **Crear** — implementación que wrappea `WorkOrderPDF` |
| `DependencyContainer.cs` | **Crear** — método `AddPDFServices()` registrando el servicio |

### 3. `FincaFenix.InversionOfControl`

| Archivo | Cambio |
|---|---|
| `ServicesDependencyContainer.cs` | Agregar `.AddPDFServices()` a la cadena de registro |

### 4. `FincaFenix.Controllers`

| Archivo | Cambio |
|---|---|
| `FincaFenixControllers.csproj` | Agregar `ProjectReference` a `FincaFenix.PDF` |
| `WorkOrderController.cs` | Inyectar `IPdfGenerationService` + agregar endpoint `GET {id}/pdf` |

### 5. `FincaFenix.WebApi`

| Archivo | Cambio |
|---|---|
| `Program.cs` | Setear `QuestPDF.Settings.License = LicenseType.Community` al inicio |

---

## Dependencias entre proyectos (post-cambio)

```
FincaFenix.Controllers ──→ FincaFenix.PDF (nueva)
FincaFenix.Controllers ──→ FincaFenix.UsesCases (existente)
FincaFenix.PDF ──→ FincaFenix.Entities (existente)
FincaFenix.InversionOfControl ──→ FincaFenix.PDF (existente)
```

---

## No tocar

- Blazor UI (`UserInterface7.0`) — sigue funcionando con el flujo actual
- No se migra el `PDFService` de Blazor (se reemplazará cuando React consuma el endpoint)
- No se agregan nuevas policies
