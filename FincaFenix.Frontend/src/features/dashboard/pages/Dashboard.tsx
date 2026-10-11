"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import {
  Boxes,
  Building2,
  CheckCircle2,
  ClipboardList,
  Clock,
  History,
  PlayCircle,
  XCircle,
} from "lucide-react";
import { PageHeader } from "@/components/common/PageHeader";
import { StatCard } from "@/components/common/StatCard";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { StatusBadge } from "@/components/common/StatusBadge";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { useAuth } from "@/app/providers/AuthProvider";
import { getProblemDetails } from "@/lib/api.client";
import { formatDate, formatDateOnly, formatNumber, hasPolicy } from "@/lib/utils";
import type {
  InventoryMovementDTO,
  ShowWorkOrderDTO,
  StockDTO,
} from "@/types/api";
import {
  getFarms,
  getWorkOrders,
} from "../../work-orders/services/workOrder.service";
import {
  getLowStock,
  getRecentMovements,
} from "../../inventory/services/inventory.service";
import {
  movementOriginLabel,
  movementTypeLabel,
} from "../../inventory/constants";

export default function Dashboard() {
  const router = useRouter();
  const { user } = useAuth();
  const canViewStock = hasPolicy(user?.policies, "STOCK_READ");

  const [workOrders, setWorkOrders] = useState<ShowWorkOrderDTO[]>([]);
  const [farmsCount, setFarmsCount] = useState<number | null>(null);
  const [lowStock, setLowStock] = useState<StockDTO[] | null>(null);
  const [movements, setMovements] = useState<InventoryMovementDTO[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    void (async () => {
      setLoading(true);
      setError(null);
      try {
        const [orders, farms] = await Promise.all([getWorkOrders(), getFarms()]);
        if (!active) return;
        setWorkOrders(orders);
        setFarmsCount(farms.length);

        if (canViewStock) {
          const [low, recent] = await Promise.all([
            getLowStock(),
            getRecentMovements(8),
          ]);
          if (!active) return;
          setLowStock(low);
          setMovements(recent);
        }
      } catch (err) {
        if (!active) return;
        const pd = getProblemDetails(err);
        setError(pd?.detail || "No se pudieron cargar los datos del dashboard.");
      } finally {
        if (active) setLoading(false);
      }
    })();

    return () => {
      active = false;
    };
  }, [canViewStock]);

  const counts = useMemo(
    () => ({
      total: workOrders.length,
      pendiente: workOrders.filter((wo) => wo.status === "Pendiente").length,
      activo: workOrders.filter((wo) => wo.status === "Activo").length,
      cerrado: workOrders.filter((wo) => wo.status === "Cerrado").length,
      cancelado: workOrders.filter((wo) => wo.status === "Cancelado").length,
    }),
    [workOrders],
  );

  const recent = useMemo(
    () => [...workOrders].sort((a, b) => b.id - a.id).slice(0, 6),
    [workOrders],
  );

  const recentColumns: Column<ShowWorkOrderDTO>[] = [
    {
      key: "orderNum",
      header: "Nº de orden",
      render: (wo) => (
        <span className="font-medium tabular-nums">{wo.orderNum}</span>
      ),
    },
    {
      key: "farm",
      header: "Finca",
      render: (wo) => wo.farm?.name ?? "-",
    },
    {
      key: "task",
      header: "Tarea",
      render: (wo) => wo.task?.description ?? "-",
    },
    {
      key: "status",
      header: "Estado",
      render: (wo) => <StatusBadge status={wo.status} />,
    },
    {
      key: "startDate",
      header: "Inicio",
      render: (wo) => formatDateOnly(wo.startDate),
    },
  ];

  const movementColumns: Column<InventoryMovementDTO>[] = [
    {
      key: "date",
      header: "Fecha",
      render: (mv) => formatDate(mv.date),
    },
    {
      key: "material",
      header: "Material",
      render: (mv) => (
        <span>
          {mv.articleName}
          {mv.commercialName ? (
            <span className="text-muted-foreground"> · {mv.commercialName}</span>
          ) : null}
        </span>
      ),
    },
    {
      key: "farm",
      header: "Finca",
      render: (mv) => mv.farmName ?? "-",
    },
    {
      key: "type",
      header: "Movimiento",
      render: (mv) => movementTypeLabel(mv.movementType),
    },
    {
      key: "origin",
      header: "Origen",
      render: (mv) =>
        mv.orderNum ? `Orden ${mv.orderNum}` : movementOriginLabel(mv.origin),
    },
    {
      key: "amount",
      header: "Cantidad",
      align: "right",
      render: (mv) =>
        `${formatNumber(mv.amount, { maximumFractionDigits: 4 })}${
          mv.unitOfMeasure ? ` ${mv.unitOfMeasure}` : ""
        }`,
    },
    {
      key: "resulting",
      header: "Stock resultante",
      align: "right",
      render: (mv) => formatNumber(mv.resultingStock, { maximumFractionDigits: 4 }),
    },
  ];

  const lowStockItems = lowStock ?? [];

  return (
    <div className="space-y-6">
      <PageHeader
        title="Dashboard"
        subtitle="Resumen general de la actividad del sistema."
      />

      {loading ? (
        <LoadingState label="Cargando indicadores..." />
      ) : error ? (
        <ErrorState message={error} onRetry={() => window.location.reload()} />
      ) : (
        <>
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
            <StatCard label="Órdenes totales" value={counts.total} icon={ClipboardList} />
            <StatCard label="Pendientes" value={counts.pendiente} icon={Clock} />
            <StatCard label="Activas" value={counts.activo} icon={PlayCircle} />
            <StatCard label="Cerradas" value={counts.cerrado} icon={CheckCircle2} />
            <StatCard label="Canceladas" value={counts.cancelado} icon={XCircle} />
            {farmsCount != null ? (
              <StatCard label="Fincas" value={farmsCount} icon={Building2} />
            ) : null}
          </div>

          <div className="grid gap-4 lg:grid-cols-3">
            <section className="rounded-lg border bg-card p-4 lg:col-span-2">
              <h2 className="mb-3 text-base font-semibold">Órdenes recientes</h2>
              {recent.length === 0 ? (
                <EmptyState
                  title="Sin órdenes"
                  description="Todavía no hay órdenes de trabajo."
                />
              ) : (
                <DataTable
                  columns={recentColumns}
                  rows={recent}
                  rowKey={(wo) => wo.id}
                  onRowClick={(wo) => router.push(`/work-orders/${wo.id}`)}
                />
              )}
            </section>

            <aside className="space-y-4">
              {canViewStock ? (
                <section className="rounded-lg border bg-card p-4">
                  <h2 className="mb-3 text-base font-semibold">Stock bajo</h2>
                  {lowStockItems.length === 0 ? (
                    <EmptyState
                      title="Sin alertas"
                      description="No hay materiales con stock bajo."
                      icon={Boxes}
                    />
                  ) : (
                    <ul className="divide-y">
                      {lowStockItems.slice(0, 8).map((item) => (
                        <li
                          key={`${item.materialId}-${item.farmId}`}
                          className="flex items-center justify-between gap-3 py-2"
                        >
                          <span className="min-w-0">
                            <span className="block truncate text-sm">
                              {item.articleName}
                            </span>
                            <span className="block text-xs text-muted-foreground">
                              {item.farmName}
                            </span>
                          </span>
                          <span className="shrink-0 text-sm tabular-nums text-muted-foreground">
                            {formatNumber(item.stockDisponible, {
                              maximumFractionDigits: 2,
                            })}{" "}
                            {item.unitOfMeasure}
                          </span>
                        </li>
                      ))}
                    </ul>
                  )}
                </section>
              ) : (
                <section className="rounded-lg border bg-card p-4">
                  <h2 className="mb-3 text-base font-semibold">Accesos rápidos</h2>
                  <button
                    type="button"
                    className="flex w-full items-center gap-3 rounded-md border p-3 text-left transition-colors hover:bg-accent"
                    onClick={() => router.push("/work-orders")}
                  >
                    <span className="flex size-9 items-center justify-center rounded-md bg-muted">
                      <ClipboardList className="size-4" aria-hidden />
                    </span>
                    <span className="text-sm font-medium">
                      Órdenes de trabajo
                    </span>
                  </button>
                </section>
              )}
            </aside>
          </div>

          {canViewStock ? (
            <section className="rounded-lg border bg-card p-4">
              <div className="mb-3 flex items-center gap-2">
                <History className="size-4 text-muted-foreground" aria-hidden />
                <h2 className="text-base font-semibold">Últimos movimientos</h2>
              </div>
              {movements === null ? (
                <LoadingState label="Cargando movimientos..." />
              ) : movements.length === 0 ? (
                <EmptyState
                  title="Sin movimientos"
                  description="Todavía no hay movimientos de inventario registrados."
                  icon={Boxes}
                />
              ) : (
                <DataTable
                  columns={movementColumns}
                  rows={movements}
                  rowKey={(mv) => mv.id}
                />
              )}
            </section>
          ) : null}
        </>
      )}
    </div>
  );
}
