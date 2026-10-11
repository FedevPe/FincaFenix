# Plan de migración — Next.js (SSR) + TailwindCSS + shadcn/ui

> Estado: **en ejecución**. Reescritura del frontend `FincaFenix.Frontend` desde la SPA Vite (CSS Modules) a **Next.js 15 (App Router, SSR shell-only)** con **TailwindCSS v4** y **shadcn/ui**, más las correcciones de UI pendientes.

## 1. Decisiones acordadas

| Tema | Decisión |
|---|---|
| Framework | **Next.js 15 + React 19**, App Router |
| Renderizado | **SSR shell-only** (el server renderiza layout/auth/tema; los datos de cada página se piden en el cliente) |
| Ubicación | **In-place** en `FincaFenix.Frontend` (reemplaza la config Vite) |
| Acceso a API | **Proxy same-origin** (`next.config` `rewrites` `/api/*` → WebAPI). Cookie HttpOnly first-party, sin CORS |
| Estilos | **TailwindCSS v4** (migración total desde CSS Modules) |
| Componentes | **shadcn/ui** (reemplaza los componentes propios) |
| Iconos | **lucide-react** (se elimina Font Awesome) |
| Tablas | **shadcn Data Table block** (`@tanstack/react-table`) |
| Feedback | **Sonner** para toasts |
| Tema | Acento **único** de contenido + **chrome siempre oscuro** (Sidebar/AppBar) |
| Orden | Base Next+Tailwind/shadcn → migración de componentes/estilos → 15 correcciones UI |

## 2. Fase 0 — Base Next.js + Tailwind + shadcn

1. **Next.js**: reemplazar Vite (`package.json`, `next.config.ts` con `rewrites`, `tsconfig` base Next, `next-env.d.ts`); borrar `index.html`, `src/main.tsx`, `src/App.tsx`, `src/app/router.tsx`; rutas en `src/app/*` con wrappers finos sobre `src/features/*/pages`; auth shell-only (`(app)/layout.tsx` server con `cookies()` → `/api/auth/me` → `<AppShell initialUser>`); migrar los 12 imports de `react-router-dom` a `next/navigation`; fuentes con `next/font` (Inter).
2. **Tailwind v4**: `tailwindcss` + `@tailwindcss/postcss`; `postcss.config.mjs`; `@import "tailwindcss"` en el CSS global; `@theme` con tokens; eliminar `reset.css` (preflight); dark mode vía `@custom-variant dark (&:where([data-theme=dark], [data-theme=dark] *))`.
3. **shadcn/ui**: `components.json`, `cn()` en `src/lib/utils.ts`; reconciliar tokens shadcn (`--background`, `--primary`, `--border`, `--ring`, `--radius`…) con los actuales bajo `[data-theme=dark]`; sumar tokens de chrome (chrome oscuro) y acento único.
4. **Checkpoint**: build de Next + un módulo (Dashboard) funcionando con el nuevo stack.

## 3. Fase 1 — Componentes a shadcn/ui + lucide

| Actual | Reemplazo |
|---|---|
| `Button` | `Button` (variants primary/secondary/danger/ghost) |
| `Dropdown` | `DropdownMenu` |
| `Modal` | `Dialog` |
| `ConfirmDialog` | `AlertDialog` |
| `Tabs` | `Tabs` |
| `StatusBadge` | `Badge` |
| `StatCard` | `Card` |
| `Pagination` | `Pagination` |
| `Feedback` | `Skeleton` / `Alert` / empty state |
| `FormControls` | `Form` + `Input`/`Label`/`Select`/`Checkbox` |
| `PageHeader` | Tailwind |
| `DataTable` | Data Table block (tanstack): `ColumnDef<T>[]` + `data`, sorting, `getRowId`, click de fila |
| Banner inline | **Sonner** |

**Iconos**: mapa Font Awesome → lucide-react en los 16 archivos que los usan
(`faGaugeHigh`→`Gauge`, `faListCheck`→`ClipboardList`, `faEllipsisVertical`→`MoreVertical`,
`faEye`→`Eye`, `faFilePdf`→`FileDown`, `faArrowRightArrowLeft`→`ArrowLeftRight`,
`faSpinner`→`Loader2`, `faCircleCheck`→`CheckCircle2`, `faCircleExclamation`→`AlertCircle`,
`faFilterCircleXmark`→`FilterX`, `faCoins`→`Coins`, `faFlaskVial`→`FlaskConical`,
`faChevronLeft/Right`→`ChevronLeft/Right`, `faXmark`→`X`, `faPlus`→`Plus`, etc.).

## 3.1 Call sites a actualizar por el cambio de `DataTable`/`Dropdown`
`WorkOrderTable`, `RecipeTab`, `ConsumptionTab`, `PerformanceTab`, `CostsTab`, `AppBar`, `WorkOrderDetail`, `WorkOrderList`.

## 4. Fase 2 — Migrar los 29 `*.module.css` a Tailwind (y borrar cada módulo)

Layouts (`AppLayout`, `AuthLayout`, `Sidebar`, `AppBar`); comunes (los de Fase 1) y features
(`Login`, `Dashboard`, `NotFound`, `WorkOrderList`, `WorkOrderDetail`, `CreateWorkOrder`,
`WorkOrderTable`, `WorkOrderFilters`, `detail/*` incl. `detail.module.css` compartido).

## 5. Fase 3 — 15 correcciones de UI

1. Sidebar colapsable (desktop) + drawer responsive (móvil).
2. Responsive general (layout, tablas, modal, formularios).
3. Chrome (Sidebar + AppBar) siempre oscuro, también en modo claro.
4. Acento único del contenido distinto del chrome (ambos temas).
5. Ícono de receta → químico (`FlaskConical`).
6. TRV = litros por hectárea (lts/ha): corrige label/hint.
7. Unidad de máquina no editable: fija en lts (convierte cc→lts).
8. Sectores por desplegable agrupado fruta→variedad + chips (reemplaza checkboxes).
9. Receta: primer select Categoría → segundo select Material filtrado.
10. Quitar el "modo de rendimiento" del select de tareas (lógica interna se mantiene).
11. Números es-AR: `parseDecimal()` (coma decimal) en formularios; unificar formatos.
12. Alineación de columnas numéricas (th/td coherentes).
13. Botón "Volver" con ancho de contenido (no full-width).
14. AppBar: dropdown de usuario con "Cambiar tema" + "Cerrar sesión".
15. Vite → Node+SSR: resuelto por esta migración.

## 6. Verificación

- `npm run build` (Next) → 0 errores; `npm run dev` para smoke.
- Smoke: login (cookie vía proxy), recarga directa a `/dashboard` (shell SSR), navegación,
  crear OT con/sin receta, registrar actividad, cambio de estado, logout.
- Formato es-AR (`1.234,56`) en todos los números.
- Chrome oscuro en modo claro; acento de contenido distinto.

## 7. Riesgos / notas

- Reescritura grande: se valida por etapas con `next build` en cada checkpoint.
- Tokens: reconciliar `variables.css` (propios) con los de shadcn; dark mode por `[data-theme="dark"]`.
- `@tanstack/react-table` pasa a usarse de verdad (Data Table block).
- Backend: `Program.cs` ya usa cookie HttpOnly; con proxy same-origin el CORS deja de ser necesario para el browser (se mantiene por compatibilidad).
