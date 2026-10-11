"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useSearchParams } from "next/navigation";
import {
  AlertTriangle,
  Boxes,
  Coins,
  History,
  Lock,
  PackageX,
  Plus,
  RefreshCw,
  Search,
} from "lucide-react";
import { toast } from "sonner";
import { PageHeader } from "@/components/common/PageHeader";
import { StatCard } from "@/components/common/StatCard";
import { Button } from "@/components/common/Button";
import { Modal } from "@/components/common/Modal";
import { Pagination } from "@/components/common/Pagination";
import { Tabs } from "@/components/common/Tabs";
import type { TabItem } from "@/components/common/Tabs";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { Field, SelectInput, TextInput } from "@/components/common/FormControls";
import { Badge } from "@/components/ui/badge";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { useAuth } from "@/app/providers/AuthProvider";
import { getProblemDetails } from "@/lib/api.client";
import { formatDateOnly, formatNumber, hasPolicy } from "@/lib/utils";
import type {
  CostHistoryDTO,
  CostHistoryItemDTO,
  CurrentMaterialCostDTO,
  MaterialCategoryDTO,
  MaterialReservationsDTO,
  PagedResult,
  ReservationItemDTO,
  StockDTO,
} from "@/types/api";
import {
  getConsolidatedStock,
  getCurrentCostsPaged,
  getLowStock,
  getMaterialCostHistory,
  getMaterialReservations,
  getStockByFarm,
  getZeroStock,
} from "../services/inventory.service";
import { MovementForm } from "../components/MovementForm";
import { movementTypeLabel } from "../constants";
import { getFarms } from "../../work-orders/services/workOrder.service";
import { getMaterialCategories } from "../../materials/services/material.service";

const COST_PAGE_SIZE = 10;

type InventoryTab = "stock" | "bajo" | "sin-stock" | "costos";

const TABS: TabItem<InventoryTab>[] = [
  { id: "stock", label: "Stock", icon: Boxes },
  { id: "bajo", label: "Stock bajo", icon: AlertTriangle },
  { id: "sin-stock", label: "Sin stock", icon: PackageX },
  { id: "costos", label: "Costos", icon: Coins },
];

function isInventoryTab(value: string | null): value is InventoryTab {
  return TABS.some((tab) => tab.id === value);
}

function money(value: number, symbol: string): string {
  return `${symbol} ${formatNumber(value, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })}`.trim();
}

function StockStatusBadge({ row }: { row: StockDTO }) {
  if (row.stockDisponible <= 0) {
    return (
      <Badge
        variant="outline"
        className="border-destructive/40 bg-destructive/10 text-destructive"
      >
        Sin stock
      </Badge>
    );
  }
  if (row.isLow) {
    return (
      <Badge
        variant="outline"
        className="border-amber-500/40 bg-amber-500/10 text-amber-600 dark:text-amber-400"
      >
        Bajo
      </Badge>
    );
  }
  return (
    <Badge
      variant="outline"
      className="border-emerald-500/40 bg-emerald-500/10 text-emerald-600 dark:text-emerald-400"
    >
      OK
    </Badge>
  );
}

function buildStockColumns(
  showFarm: boolean,
  onReservations?: (materialId: number) => void,
): Column<StockDTO>[] {
  const columns: Column<StockDTO>[] = [
    {
      key: "material",
      header: "Material",
      render: (row) => (
        <span>
          {row.articleName}
          {row.commercialName ? (
            <span className="text-muted-foreground"> · {row.commercialName}</span>
          ) : null}
        </span>
      ),
    },
  ];

  if (showFarm) {
    columns.push({
      key: "farm",
      header: "Finca",
      render: (row) => row.farmName ?? "Consolidado",
    });
  }

  columns.push(
    {
      key: "unit",
      header: "Unidad",
      render: (row) => row.unitOfMeasure,
    },
    {
      key: "fisico",
      header: "Físico",
      align: "right",
      render: (row) => formatNumber(row.stockFisico, { maximumFractionDigits: 4 }),
    },
    {
      key: "reservado",
      header: "Reservado",
      align: "right",
      render: (row) => formatNumber(row.stockReservado, { maximumFractionDigits: 4 }),
    },
    {
      key: "disponible",
      header: "Disponible",
      align: "right",
      render: (row) => formatNumber(row.stockDisponible, { maximumFractionDigits: 4 }),
    },
    {
      key: "minimo",
      header: "Mínimo",
      align: "right",
      render: (row) =>
        row.stockMinimo > 0
          ? formatNumber(row.stockMinimo, { maximumFractionDigits: 4 })
          : "-",
    },
    {
      key: "estado",
      header: "Estado",
      render: (row) => <StockStatusBadge row={row} />,
    },
  );

  if (onReservations) {
    columns.push({
      key: "actions",
      header: "",
      align: "right",
      render: (row) => (
        <Button
          variant="ghost"
          size="sm"
          icon={Lock}
          onClick={() => onReservations(row.materialId)}
        >
          Reservas
        </Button>
      ),
    });
  }

  return columns;
}

const COST_COLUMNS = (onHistory: (materialId: number) => void): Column<CurrentMaterialCostDTO>[] => [
  {
    key: "material",
    header: "Material",
    render: (row) => (
      <span>
        {row.articleName}
        {row.commercialName ? (
          <span className="text-muted-foreground"> · {row.commercialName}</span>
        ) : null}
      </span>
    ),
  },
  {
    key: "category",
    header: "Categoría",
    render: (row) => row.categoryName ?? "-",
  },
  {
    key: "unit",
    header: "Unidad",
    render: (row) => row.unitOfMeasure ?? "-",
  },
  {
    key: "cost",
    header: "Costo de referencia",
    align: "right",
    render: (row) =>
      row.referenceCost != null
        ? `${money(row.referenceCost, row.currencySymbol)} ${row.currencyCode}`.trim()
        : "Sin costo",
  },
  {
    key: "actions",
    header: "",
    align: "right",
    render: (row) => (
      <Button variant="ghost" size="sm" icon={History} onClick={() => onHistory(row.materialId)}>
        Historial
      </Button>
    ),
  },
];

export default function Inventory() {
  const searchParams = useSearchParams();
  const { user } = useAuth();
  const canRead = hasPolicy(user?.policies, "STOCK_READ");
  const canManage = hasPolicy(user?.policies, "MOVEMENT_CREATE");

  const tabValue = searchParams.get("tab");
  const [activeTab, setActiveTab] = useState<InventoryTab>(
    isInventoryTab(tabValue) ? tabValue : "stock",
  );

  const [consolidated, setConsolidated] = useState<StockDTO[]>([]);
  const [farms, setFarms] = useState<{ id: number; name: string }[]>([]);
  const [farmFilter, setFarmFilter] = useState("");
  const [search, setSearch] = useState("");
  const [stockRows, setStockRows] = useState<StockDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [lowRows, setLowRows] = useState<StockDTO[] | null>(null);
  const [zeroRows, setZeroRows] = useState<StockDTO[] | null>(null);
  const [tabLoading, setTabLoading] = useState(false);
  const [tabError, setTabError] = useState<string | null>(null);

  const [costs, setCosts] = useState<PagedResult<CurrentMaterialCostDTO> | null>(null);
  const [costCategories, setCostCategories] = useState<MaterialCategoryDTO[]>([]);
  const [costPage, setCostPage] = useState(1);
  const [costSearch, setCostSearch] = useState("");
  const [costCategoryId, setCostCategoryId] = useState("");
  const [costsLoading, setCostsLoading] = useState(false);
  const [costsError, setCostsError] = useState<string | null>(null);
  const [costRefresh, setCostRefresh] = useState(0);

  const [movementOpen, setMovementOpen] = useState(false);

  const [history, setHistory] = useState<CostHistoryDTO | null>(null);
  const [historyOpen, setHistoryOpen] = useState(false);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [historyError, setHistoryError] = useState<string | null>(null);

  const [reservations, setReservations] = useState<MaterialReservationsDTO | null>(null);
  const [reservationsOpen, setReservationsOpen] = useState(false);
  const [reservationsLoading, setReservationsLoading] = useState(false);
  const [reservationsError, setReservationsError] = useState<string | null>(null);

  const loadBase = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [stock, farmList, categoryList] = await Promise.all([
        getConsolidatedStock(),
        getFarms().catch(() => [] as { id: number; name: string }[]),
        getMaterialCategories().catch(() => [] as MaterialCategoryDTO[]),
      ]);
      setConsolidated(stock);
      setFarms(farmList);
      setCostCategories(categoryList);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudo cargar el inventario.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (canRead) void loadBase();
  }, [canRead, loadBase]);

  useEffect(() => {
    if (!canRead) return;
    if (!farmFilter) {
      setStockRows(consolidated);
      return;
    }
    let active = true;
    void (async () => {
      try {
        const rows = await getStockByFarm(Number(farmFilter));
        if (active) setStockRows(rows);
      } catch (err) {
        if (!active) return;
        const pd = getProblemDetails(err);
        setError(pd?.detail || "No se pudo cargar el stock de la finca.");
      }
    })();
    return () => {
      active = false;
    };
  }, [farmFilter, consolidated, canRead]);

  useEffect(() => {
    if (!canRead) return;
    if (activeTab !== "bajo" && activeTab !== "sin-stock") return;
    if (
      (activeTab === "bajo" && lowRows !== null) ||
      (activeTab === "sin-stock" && zeroRows !== null)
    ) {
      return;
    }
    let active = true;
    setTabLoading(true);
    setTabError(null);
    void (async () => {
      try {
        if (activeTab === "bajo") setLowRows(await getLowStock());
        else if (activeTab === "sin-stock") setZeroRows(await getZeroStock());
      } catch (err) {
        const pd = getProblemDetails(err);
        if (active) setTabError(pd?.detail || "No se pudo cargar la información.");
      } finally {
        if (active) setTabLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, [activeTab, canRead, lowRows, zeroRows]);

  useEffect(() => {
    if (!canRead || activeTab !== "costos") return;
    let active = true;
    setCostsLoading(true);
    setCostsError(null);
    void (async () => {
      try {
        const result = await getCurrentCostsPaged({
          pageNumber: costPage,
          pageSize: COST_PAGE_SIZE,
          search: costSearch.trim() || undefined,
          categoryId: costCategoryId ? Number(costCategoryId) : undefined,
        });
        if (active) setCosts(result);
      } catch (err) {
        if (!active) return;
        const pd = getProblemDetails(err);
        setCostsError(pd?.detail || "No se pudo cargar la información de costos.");
      } finally {
        if (active) setCostsLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, [activeTab, canRead, costPage, costSearch, costCategoryId, costRefresh]);

  useEffect(() => {
    setCostPage(1);
  }, [costSearch, costCategoryId]);

  useEffect(() => {
    if (typeof window === "undefined") return;
    const url = activeTab === "stock" ? "/inventory" : `/inventory?tab=${activeTab}`;
    window.history.replaceState(null, "", url);
  }, [activeTab]);

  const openHistory = useCallback(async (materialId: number) => {
    setHistoryOpen(true);
    setHistoryLoading(true);
    setHistoryError(null);
    setHistory(null);
    try {
      setHistory(await getMaterialCostHistory(materialId));
    } catch (err) {
      const pd = getProblemDetails(err);
      setHistoryError(pd?.detail || "No se pudo cargar el historial de costos.");
    } finally {
      setHistoryLoading(false);
    }
  }, []);

  const openReservations = useCallback(async (materialId: number) => {
    setReservationsOpen(true);
    setReservationsLoading(true);
    setReservationsError(null);
    setReservations(null);
    try {
      setReservations(await getMaterialReservations(materialId));
    } catch (err) {
      const pd = getProblemDetails(err);
      setReservationsError(pd?.detail || "No se pudieron cargar las reservas.");
    } finally {
      setReservationsLoading(false);
    }
  }, []);

  const filteredStock = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return stockRows;
    return stockRows.filter((row) => {
      const haystack = `${row.articleName} ${row.commercialName ?? ""}`.toLowerCase();
      return haystack.includes(term);
    });
  }, [stockRows, search]);

  const sortedCostCategories = useMemo(
    () => [...costCategories].sort((a, b) => a.description.localeCompare(b.description)),
    [costCategories],
  );

  const historyColumns: Column<CostHistoryItemDTO>[] = [
    { key: "date", header: "Fecha", render: (row) => formatDateOnly(row.date) },
    {
      key: "movementType",
      header: "Movimiento",
      render: (row) => movementTypeLabel(row.movementType),
    },
    { key: "origin", header: "Origen", render: (row) => row.origin },
    {
      key: "amount",
      header: "Cantidad",
      align: "right",
      render: (row) => formatNumber(row.amount, { maximumFractionDigits: 4 }),
    },
    {
      key: "unitCost",
      header: "Costo unitario",
      align: "right",
      render: (row) =>
        row.unitCost != null ? money(row.unitCost, history?.currencySymbol ?? "$") : "-",
    },
    {
      key: "totalCost",
      header: "Total",
      align: "right",
      render: (row) =>
        row.totalCost != null ? money(row.totalCost, history?.currencySymbol ?? "$") : "-",
    },
    { key: "farm", header: "Finca", render: (row) => row.farmName },
  ];

  const reservationColumns: Column<ReservationItemDTO>[] = [
    { key: "order", header: "Orden", render: (row) => row.orderNum || `#${row.workOrderId}` },
    {
      key: "status",
      header: "Estado OT",
      render: (row) => row.workOrderStatus ?? "-",
    },
    { key: "farm", header: "Finca", render: (row) => row.farmName },
    {
      key: "reserved",
      header: "Reservado",
      align: "right",
      render: (row) => formatNumber(row.reservedAmount, { maximumFractionDigits: 4 }),
    },
    {
      key: "consumed",
      header: "Consumido",
      align: "right",
      render: (row) => formatNumber(row.consumedAmount, { maximumFractionDigits: 4 }),
    },
    {
      key: "pending",
      header: "Pendiente",
      align: "right",
      render: (row) => formatNumber(row.pendingAmount, { maximumFractionDigits: 4 }),
    },
    { key: "state", header: "Reserva", render: (row) => row.state ?? "-" },
    {
      key: "created",
      header: "Fecha",
      render: (row) => formatDateOnly(row.createdDate),
    },
  ];

  if (!canRead) {
    return (
      <div className="space-y-6">
        <PageHeader title="Inventario" subtitle="Stock y costos de materiales." />
        <EmptyState
          title="Acceso restringido"
          description="No tiene permisos para ver el inventario."
          icon={Boxes}
        />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Inventario"
        subtitle="Stock consolidado, alertas y costos de materiales."
        actions={
          <>
            <Button variant="secondary" icon={RefreshCw} onClick={() => void loadBase()}>
              Actualizar
            </Button>
            <Button
              variant="primary"
              icon={Plus}
              disabled={!canManage}
              onClick={() => setMovementOpen(true)}
            >
              Registrar movimiento
            </Button>
          </>
        }
      />

      {!canRead ? null : loading && consolidated.length === 0 ? (
        <LoadingState label="Cargando inventario..." />
      ) : error ? (
        <ErrorState message={error} onRetry={() => void loadBase()} />
      ) : (
        <>
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
            <StatCard label="Materiales" value={consolidated.length} icon={Boxes} />
            <StatCard
              label="Con stock"
              value={consolidated.filter((r) => r.stockDisponible > 0).length}
            />
            <StatCard
              label="Stock bajo"
              value={consolidated.filter((r) => r.stockDisponible > 0 && r.isLow).length}
              icon={AlertTriangle}
            />
            <StatCard
              label="Sin stock"
              value={consolidated.filter((r) => r.stockDisponible <= 0).length}
              icon={PackageX}
            />
          </div>

          <Tabs tabs={TABS} active={activeTab} onChange={(tab) => setActiveTab(tab)} />

          {activeTab === "stock" ? (
            <div className="space-y-3">
              <div className="grid gap-3 sm:grid-cols-2 lg:max-w-xl">
                <Field label="Finca" htmlFor="farmFilter">
                  <SelectInput
                    id="farmFilter"
                    value={farmFilter}
                    onChange={(event) => setFarmFilter(event.target.value)}
                  >
                    <option value="">Todas</option>
                    {farms.map((farm) => (
                      <option key={farm.id} value={farm.id}>
                        {farm.name}
                      </option>
                    ))}
                  </SelectInput>
                </Field>
                <Field label="Buscar material" htmlFor="stockSearch">
                  <div className="relative">
                    <Search className="pointer-events-none absolute top-1/2 left-2.5 size-4 -translate-y-1/2 text-muted-foreground" />
                    <TextInput
                      id="stockSearch"
                      value={search}
                      className="pl-8"
                      placeholder="Nombre o nombre comercial"
                      onChange={(event) => setSearch(event.target.value)}
                    />
                  </div>
                </Field>
              </div>

              {filteredStock.length === 0 ? (
                <EmptyState
                  title="Sin resultados"
                  description="No hay materiales que coincidan con el filtro."
                  icon={Boxes}
                />
              ) : (
                <DataTable
                  columns={buildStockColumns(
                    !farmFilter,
                    (materialId) => void openReservations(materialId),
                  )}
                  rows={filteredStock}
                  rowKey={(row) => `${row.materialId}-${row.farmId}`}
                />
              )}
            </div>
          ) : (activeTab === "bajo" || activeTab === "sin-stock") && tabLoading ? (
            <LoadingState label="Cargando..." />
          ) : (activeTab === "bajo" || activeTab === "sin-stock") && tabError ? (
            <ErrorState
              message={tabError}
              onRetry={() => {
                setLowRows(null);
                setZeroRows(null);
                setCosts(null);
              }}
            />
          ) : activeTab === "bajo" ? (
            (lowRows ?? []).length === 0 ? (
              <EmptyState
                title="Sin alertas"
                description="No hay materiales con stock bajo."
                icon={AlertTriangle}
              />
            ) : (
              <DataTable
                columns={buildStockColumns(true)}
                rows={lowRows ?? []}
                rowKey={(row) => `${row.materialId}-${row.farmId}`}
              />
            )
          ) : activeTab === "sin-stock" ? (
            (zeroRows ?? []).length === 0 ? (
              <EmptyState
                title="Sin quiebres"
                description="No hay materiales sin stock."
                icon={PackageX}
              />
            ) : (
              <DataTable
                columns={buildStockColumns(true)}
                rows={zeroRows ?? []}
                rowKey={(row) => `${row.materialId}-${row.farmId}`}
              />
            )
          ) : (
            <div className="space-y-3">
              <div className="grid gap-3 sm:grid-cols-2 lg:max-w-xl">
                <Field label="Categoría" htmlFor="costCategory">
                  <SelectInput
                    id="costCategory"
                    value={costCategoryId}
                    onChange={(event) => {
                      setCostCategoryId(event.target.value);
                      setCostPage(1);
                    }}
                  >
                    <option value="">Todas</option>
                    {sortedCostCategories.map((category) => (
                      <option key={category.id} value={category.id}>
                        {category.description}
                      </option>
                    ))}
                  </SelectInput>
                </Field>
                <Field label="Buscar material" htmlFor="costSearch">
                  <div className="relative">
                    <Search className="pointer-events-none absolute top-1/2 left-2.5 size-4 -translate-y-1/2 text-muted-foreground" />
                    <TextInput
                      id="costSearch"
                      value={costSearch}
                      className="pl-8"
                      placeholder="Nombre o nombre comercial"
                      onChange={(event) => {
                        setCostSearch(event.target.value);
                        setCostPage(1);
                      }}
                    />
                  </div>
                </Field>
              </div>

              {costsLoading ? (
                <LoadingState label="Cargando costos..." />
              ) : costsError ? (
                <ErrorState
                  message={costsError}
                  onRetry={() => setCostRefresh((key) => key + 1)}
                />
              ) : !costs || costs.items.length === 0 ? (
                <EmptyState
                  title="Sin materiales"
                  description="No hay materiales que coincidan con el filtro."
                  icon={Coins}
                />
              ) : (
                <div className="space-y-3">
                  <DataTable
                    columns={COST_COLUMNS((materialId) => void openHistory(materialId))}
                    rows={costs.items}
                    rowKey={(row) => row.materialId}
                  />
                  <Pagination
                    page={costs.page}
                    pageSize={costs.pageSize}
                    totalCount={costs.totalCount}
                    onPageChange={setCostPage}
                  />
                </div>
              )}
            </div>
          )}
        </>
      )}

      <Modal
        open={movementOpen}
        title="Registrar movimiento de inventario"
        onClose={() => setMovementOpen(false)}
      >
        <MovementForm
          onClose={() => setMovementOpen(false)}
          onSaved={(resultingStock) => {
            setMovementOpen(false);
            toast.success(
              `Movimiento registrado. Stock resultante: ${formatNumber(resultingStock, {
                maximumFractionDigits: 2,
              })}.`,
            );
            setLowRows(null);
            setZeroRows(null);
            setCostRefresh((key) => key + 1);
            void loadBase();
          }}
        />
      </Modal>

      <Modal
        open={historyOpen}
        title={history ? `Historial de costos · ${history.articleName}` : "Historial de costos"}
        onClose={() => {
          setHistoryOpen(false);
          setHistory(null);
          setHistoryError(null);
        }}
        size="lg"
      >
        {historyLoading ? (
          <LoadingState label="Cargando historial..." />
        ) : historyError ? (
          <ErrorState message={historyError} />
        ) : history ? (
          history.items.length === 0 ? (
            <EmptyState
              title="Sin registros"
              description="Este material no tiene movimientos con costo registrado."
              icon={Coins}
            />
          ) : (
            <DataTable
              columns={historyColumns}
              rows={history.items}
              rowKey={(row) => row.movementId}
            />
          )
        ) : null}
      </Modal>

      <Modal
        open={reservationsOpen}
        title={
          reservations
            ? `Reservas · ${reservations.articleName}`
            : "Reservas del material"
        }
        onClose={() => {
          setReservationsOpen(false);
          setReservations(null);
          setReservationsError(null);
        }}
        size="lg"
      >
        {reservationsLoading ? (
          <LoadingState label="Cargando reservas..." />
        ) : reservationsError ? (
          <ErrorState message={reservationsError} />
        ) : reservations ? (
          reservations.items.length === 0 ? (
            <EmptyState
              title="Sin reservas"
              description="Este material no tiene reservas registradas por órdenes de trabajo."
              icon={Lock}
            />
          ) : (
            <DataTable
              columns={reservationColumns}
              rows={reservations.items}
              rowKey={(row) => row.reservationId}
            />
          )
        ) : null}
      </Modal>
    </div>
  );
}
