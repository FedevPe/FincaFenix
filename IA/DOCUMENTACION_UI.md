# UI — Documentación del estado actual (Frontend Next.js)

> Estado: **en desarrollo activo**. Documento vivo que describe la UI implementada del frontend `FincaFenix.Frontend` (Next.js App Router), su arquitectura, convenciones, módulos y limitaciones conocidas.

## 1. Stack y herramientas

| Tecnología | Versión | Uso |
|---|---|---|
| Next.js | ^15.5 | Framework (App Router, SSR shell-only) |
| React | ^19.3 | Interfaz de usuario |
| TypeScript | ~6.0.2 | Tipado explícito |
| TailwindCSS | ^4.3 | Estilos (v4, CSS-first, `@theme`) |
| shadcn/ui | (radix-ui ^1.7 + cva) | Componentes base |
| lucide-react | ^1.55 | Única biblioteca de iconos |
| @tanstack/react-table | ^8.21.3 | Data Table (v8, usada por `DataTable`) |
| axios | ^1.20.0 | Cliente HTTP con `withCredentials` |
| Sonner | ^2.0.8 | Toasts/notificaciones |
| date-fns | ^4.4.0 | Formato de fechas (locale `es`) |
| react-hook-form + zod | 7.89 / 4.6 | Formularios (Login) |

> Se **eliminaron** Font Awesome, Vite, React Router, `next-themes` y los 29 `*.module.css`. La migración está documentada en `IA/PLAN_MIGRACION_NEXT_TAILWIND.md`.

## 2. Configuración y acceso a la API

- **Proxy same-origin**: `next.config.ts` reescribe `/api/:path*` → `API_PROXY_TARGET` (default `http://localhost:5000`). La cookie HttpOnly viaja first-party; el navegador no necesita CORS.
- `src/lib/api.client.ts`: `API_BASE_URL` (vacío = mismo origen) e instancia axios con `withCredentials: true`. Helper `getProblemDetails(error)`.
- Variables en `.env.local` (gitignored) / `.env.example`: `API_PROXY_TARGET`, `NEXT_PUBLIC_API_BASE_URL`.
- Scripts: `npm run dev`, `npm run build`, `npm run start`, `npm run typecheck` (`tsc --noEmit`).

## 3. Estructura

```
src/
├── app/                         # App Router
│   ├── layout.tsx               # <html lang="es">, fuente Inter, script de tema
│   ├── globals.css              # Tailwind v4 + tokens shadcn + tokens de chrome
│   ├── providers.tsx            # Theme + Auth + Tooltip + Toaster (Sonner)
│   ├── providers/{ThemeProvider,AuthProvider}.tsx
│   ├── (auth)/layout.tsx        # envuelve en <Providers>
│   ├── (auth)/login/page.tsx
│   ├── (app)/layout.tsx         # server: getCurrentUserServer() → redirect("/login")
│   ├── (app)/dashboard/page.tsx
│   ├── (app)/work-orders/{page,new/page,[id]/page}.tsx
│   ├── (app)/inventory/page.tsx
│   └── not-found.tsx
├── components/
│   ├── layout/{AppShell,Sidebar,AppBar}.tsx
│   ├── common/                  # componentes propios (API preservada)
│   └── ui/                      # 17 componentes shadcn generados
├── features/
│   ├── auth/pages/Login.tsx
│   ├── dashboard/pages/Dashboard.tsx
│   ├── not-found/pages/NotFound.tsx
│   ├── inventory/pages/Inventory.tsx + components/MovementForm.tsx + services + constants.ts
│   └── work-orders/             # pages/{List,Detail,Create} + components + services + constants.ts
├── lib/{api.client.ts,utils.ts,theme.ts,server/get-current-user.ts}
└── types/api/                   # auth, common, inventory, workOrder, index
```

Cada feature conserva `pages/`, `components/`, `services/`, `constants.ts` (ya sin `.module.css`).

## 4. Tema y tokens

- Tema por atributo `data-theme` en `<html>` (`light`/`dark`); `ThemeProvider` es SSR-safe (lee el DOM) y persiste en `localStorage["fincafenix:theme"]`. Script inline evita el flash.
- Tokens en `globals.css`: base shadcn (`--background`, `--card`, `--primary`, `--muted`, `--border`, `--ring`, `--radius`…) resueltos por `[data-theme]` y expuestos a Tailwind con `@theme inline`.
- **Chrome (Sidebar + AppBar) siempre oscuro**: tokens `--chrome-*` fijos (independientes del tema) expuestos como `bg-chrome`, `bg-chrome-surface`, `text-chrome-foreground`, `border-chrome-border`, `text-chrome-muted`, `text/bg-chrome-primary`.
- **Acento único** de contenido (no compite con el chrome).
- `@custom-variant dark (&:where([data-theme="dark"], [data-theme="dark"] *))` habilita utilidades `dark:`.

## 4.1 Correcciones de UI (Fase 3)
Implementadas: sidebar colapsable (desktop) + drawer móvil con overlay; chrome oscuro en ambos temas; acento de contenido propio; iconos lucide; **TRV = lts/ha** (etiqueta explícita); volumen de máquina de solo lectura en litros; sectores como `<select>` con `optgroup` (fruta→variedad) + chips; receta con select **Categoría → Material**; **sin** selector de "modo de rendimiento" en la tarea (el modo es property de la tarea en el backend); `parseDecimal()` (coma decimal) en todos los inputs numéricos; alineación a la derecha de columnas numéricas; botón "Volver" de ancho de contenido; dropdown de usuario en el AppBar (cambiar tema + cerrar sesión).

## 5. Autenticación (SSR shell-only)

```
POST /api/auth/login  { userName, password } → CurrentUserDTO { id, userName, email, roles, policies }
GET  /api/auth/me     → restaura la sesión
POST /api/auth/logout → borra la cookie
```

- El layout `(app)/layout.tsx` es **server**: `getCurrentUserServer()` (`src/lib/server/get-current-user.ts`) usa `cookies()` y llama a `API_PROXY_TARGET/api/auth/me` con `cache:"no-store"`; sin sesión `redirect("/login")`. El usuario se pasa a `<Providers initialUser>` para renderizar el shell en SSR.
- **`AuthProvider`**: expone `user`, `isAuthenticated`, `isChecking`, `login(user)`, `logout()`. `logout()` llama al endpoint y hace `router.replace("/login")`.
- **`Login`**: react-hook-form + zod; en éxito `router.replace("/dashboard") + router.refresh()`.
- **Policies** en el cliente: `hasPolicy(user?.policies, "POLICY")` (p. ej. `WORKORDER_CREATE`, `WORKORDER_UPDATE`, `DETAIL_WORKORDER_CREATE`, `STOCK_READ`, `MATERIAL_READ`+`MACHINE_READ`).

## 6. Rutas

| Ruta | Layout | Página |
|---|---|---|
| `/login` | `(auth)` | Login |
| `/` | `(app)` | Redirect a `/dashboard` |
| `/dashboard` | `(app)` | Dashboard |
| `/work-orders` | `(app)` | Listado de órdenes |
| `/work-orders/new` | `(app)` | Nueva orden |
| `/work-orders/[id]` | `(app)` | Detalle |
| `/inventory` | `(app)` | Inventario (stock + costos) |
| `*` | — | `not-found.tsx` |

- Los wrappers `/work-orders` y `/work-orders/[id]` usan `useSearchParams` → envueltos en `<Suspense fallback={<LoadingState/>}>` (requisito de Next 15).
- El shell (`AppShell`) es cliente: estado de sidebar (`collapsed`/`mobileOpen`) + `AppBar` (título por ruta).

## 7. Componentes compartidos (`components/common`)

| Componente | Descripción |
|---|---|
| `Button` | Variantes `primary/secondary/ghost/danger`, `size sm/md`, `icon?: LucideIcon`, `loading`. |
| `StatusBadge` | Estado de OT (Pendiente/Activo/Cerrado/Cancelado) con `Badge` + colores. |
| `Pagination`, `Tabs` | Paginación / pestañas. |
| `DataTable<T>` | Envuelve `@tanstack/react-table` **v8**; preserva API `Column<T>{key,header,render,align,width,sortable?,sortValue?}`, `rows`, `rowKey`, `onRowClick?`, `emptyMessage`. `align:"right"` alinea th/td. |
| `Dropdown`, `Modal`, `ConfirmDialog` | Envuelven `DropdownMenu` / `Dialog` / `AlertDialog` de shadcn. |
| `StatCard`, `PageHeader` | Métricas / encabezado de página. |
| `Feedback` | `LoadingState`, `EmptyState`, `ErrorState`. |
| `FormControls` | `Field`, `TextInput`, `SelectInput`, `CheckboxRow`. |

`components/ui/*`: 17 primitivas shadcn (button, dialog, alert-dialog, dropdown-menu, tabs, badge, card, input, label, select, checkbox, table, sonner, separator, skeleton, avatar, tooltip).

## 8. Utilidades y cliente HTTP

**`src/lib/utils.ts`**:
- `cn(...)` / `cls(...)`: merge de clases (twMerge + clsx).
- `parseDecimal(value)`: parsea números tolerando **coma decimal** es-AR (`"1,5"` → `1.5`); devuelve `0` si inválido.
- `formatDate` / `formatDateOnly` (locale `es`), `toNumber`.
- `formatNumber(value, options?)` y `formatCurrency(value, currency="ARS")`: **`Intl.NumberFormat("es-AR")`** (miles `.`, decimal `,` → `1.234,56`). **Convención: toda cantidad visible pasa por estas funciones.**
- `hasPolicy(policies, policy)`.

**`src/lib/api.client.ts`**: `API_BASE_URL`, axios (`withCredentials`), `getProblemDetails`.

**Tipos** (`src/types/api/`): `auth`, `common`, `workOrder`, `inventory`, `index` (mismos DTOs que antes; `MachineRecipeDTO` expone `name`, no `description`).

## 8.1 Iconos (Font Awesome → lucide-react)
`faGaugeHigh`→`Gauge`, `faListCheck`→`ClipboardList`/`ListChecks`, `faEllipsisVertical`→`MoreVertical`, `faEye`→`Eye`, `faFilePdf`→`FileDown`, `faArrowRightArrowLeft`→`ArrowLeftRight`, `faSpinner`→`Loader2`, `faCircleCheck`→`CheckCircle2`, `faCircleExclamation`→`AlertCircle`, `faFilterCircleXmark`→`FilterX`, `faCoins`→`Coins`, `faFlaskVial`→`FlaskConical`, `faBoxesStacked`→`Boxes`, `faBalanceScale`→`Scale`, `faCircleInfo`→`Info`, `faLayerGroup`→`Layers`, `faWeightHanging`→`Weight`, `faRulerCombined`→`Ruler`, `faClock`→`Clock`, etc.

## 9. Módulos implementados

### 9.1 Dashboard (`/dashboard`)
- KPIs (total, pendientes, activas, cerradas, canceladas, fincas) desde `getallworkorderinfo`.
- "Órdenes recientes" (top 6 por id desc) → link al detalle.
- "Stock bajo" (`GET /api/inventory/stock/low`, gated por `STOCK_READ`).

### 9.2 Listado (`/work-orders`)
- Datos de `getallworkorderinfo` + filtros/paginación **en cliente** (`PAGE_SIZE = 10`). **No** se usa `getworkorderlistpaginated?status=` (semántica invertida en backend).
- Acciones por fila: ver, descargar PDF, cambiar estado (según `allowedTransitions`). Toasts Sonner.
- `?created=1` → toast de éxito al volver de la creación.

### 9.3 Nueva orden (`/work-orders/new`)
- Carga tareas, fincas, máquinas y materiales. Sectores por finca (`/api/detailsector/farm/{id}/sectors`) con `select` agrupado por fruta (optgroup) + `DataTable` de seleccionados (Sector, Fruta, Variedad, Plantas, Superficie, quitar) y área total.
- Categoría de material desde `GET /api/materialcategory/getCategories`; los materiales se filtran por categoría.
- **Receta opcional** (obligatoria si la tarea es `MaterialEfficiency`): máquina, volumen (solo lectura, lts), TRV (lts/ha), filas dinámicas de materiales con select **Categoría → Material**.
- **Cantidad requerida** + **unidad** (`lts`/`kg`/`gr`/`cc`) las carga el usuario. La **cantidad estimada es automática** (`Maquinadas teóricas × Cant. requerida`, en la **misma unidad de la dosis**; el backend convierte a unidad base al reservar) y se muestra de solo lectura.
- Mecanismo anti-400: MVC `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes` evita los `[Required]` implícitos por propiedades de navegación no enviadas (`Machine`, `NumRecipe`, `Farm`, `Material`).
- `GET /api/material/getmateriallist` ahora incluye la categoría (`.Include(m => m.Category)`) — antes el filtro por categoría mostraba "Sin categoría".
- `POST /api/workorder/createworkorder` (WorkOrderDTO). El backend deriva el estado. **La API no devuelve el id** → redirect a `/work-orders?created=1`.

### 9.4 Detalle (`/work-orders/[id]`)
Pestañas (sincronizadas con `?tab=`):
1. **Actividades**: tabla + totales; botón "Registrar actividad" (policy `DETAIL_WORKORDER_CREATE`, OT no terminal) → `Modal` con `AddActivityForm`.
2. **Receta**: máquina/volumen/TRV/dosis/materiales (teórico/real/rendimiento).
3. **Consumos**: consolidado por material (no hay endpoint por sector).
4. **Rendimiento**: métricas por actividad y por OT según el modo de la tarea.
5. **Costos**: `GET /api/inventory/costs/workorder/{id}` bajo demanda.

`AddActivityForm`: sector (de `workOrder.sectorList`), operario, horas, **maquinadas** (MaterialEfficiency) o **producción kg** (OutputPerManHour), descripción. La **fecha/hora es automática** (del momento de guardar, no editable por el operario). `POST /api/detailworkorder/addDetailWO`.

### 9.5 Inventario (`/inventory`)
Gated por `STOCK_READ` (ítem del Sidebar filtrado por policy). `PageHeader` + KPIs (materiales / con stock / stock bajo / sin stock, desde el consolidado).
Pestañas (sincronizadas con `?tab=`):
1. **Stock**: filtro de finca (Todas = consolidado / finca puntual vía `stock/farm/{id}`) + buscador de material; tabla con físico/reservado/disponible/mínimo y badge de estado. Cada fila tiene acción **Reservas** → `Modal` con `GET /api/inventory/reservations/material/{id}` (Orden, Estado OT, Finca, Reservado, Consumido, Pendiente, Reserva, Fecha).
2. **Stock bajo**: `GET /api/inventory/stock/low`.
3. **Sin stock**: `GET /api/inventory/stock/zero`.
4. **Costos**: server-side paging `GET /api/inventory/costs/current/paged` (`pageNumber`/`pageSize`/`search`/`categoryId`) con select de categoría + buscador y `Pagination`; acción "Historial" → `Modal` con `GET /api/inventory/costs/material/{id}/history`.

**Registrar movimiento** (botón, policy `MOVEMENT_CREATE`): `Modal` con `MovementForm` (**categoría → material** filtrado, finca, tipo `Ingreso`/`AjustePositivo`/`AjusteNegativo`, cantidad, costo unitario/divisa opcional, stock mínimo, observaciones) → `POST /api/inventory/movement` (es-AR, `parseDecimal`); los labels **Cantidad/Costo unitario/Stock mínimo** muestran la unidad base del material seleccionado (se guardan en esa unidad, sin conversión). Al guardar recarga el consolidado y muestra toast con el stock resultante.

Todos los `Modal`/`AlertDialog` difuminan el fondo (`backdrop-blur-sm` en los overlays de `ui/dialog.tsx` y `ui/alert-dialog.tsx`).

### 9.6 Servicios por módulo
| Servicio | Endpoint(s) |
|---|---|
| `auth.service` | login / me / logout |
| `workOrder.service` | getallworkorderinfo, getCompleteInfoWorkOrder, createworkorder, updatestateworkorder, pdf, getListFarm, GetTaskList, getmachinelist, sectors, costs/workorder |
| `detailWorkOrder.service` | addDetailWO (+ getDetail no usado) |
| `employee.service` | `/api/employee/{farmId}/employees` |
| `inventory.service` | stock/low, stock/consolidated, stock/farm/{farmId}, stock/zero, currencies, costs/current, costs/current/paged, costs/material/{id}/history, reservations/material/{id}, movement |
| `material.service` | getmateriallist, getCategories |

## 10. Convenciones de código
- Componentes de feature con `"use client"` cuando usan hooks/estado; layouts/tema en server.
- Estilos **solo Tailwind** (sin `*.module.css`).
- Iconos **solo lucide-react**.
- Fetch: servicio `Promise<T>` por feature; error vía `getProblemDetails`.
- Números/fechas: `formatNumber`/`formatCurrency`/`formatDate*` (es-AR).

## 11. Limitaciones y pendientes
1. **Crear OT no devuelve el id** → vuelve al listado con `?created=1`.
2. Endpoint paginado de OT con filtro status no usable (semántica invertida).
3. Consumos por sector sin endpoint → `ConsumptionTab` consolida por material.
4. `recharts` instalado pero sin consumir.
5. Sidebar: navegación Dashboard + Órdenes + Inventario (esta última gated por `STOCK_READ`); faltan Materiales/Tareas.
6. Sin smoke E2E automatizado; verificación manual + `npm run build`.

## 12. Próximos pasos
Módulos de **Materiales CRUD**, **Tareas CRUD** y edición de OT (`PUT /api/workorder/updateworkorder`) — endpoints ya disponibles en el backend.
