"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Plus } from "lucide-react";
import { toast } from "sonner";
import { PageHeader } from "@/components/common/PageHeader";
import { WorkOrderFilters, EMPTY_FILTERS } from "../components/WorkOrderFilters";
import type { WorkOrderFilterValues } from "../components/WorkOrderFilters";
import { WorkOrderTable } from "../components/WorkOrderTable";
import { Pagination } from "@/components/common/Pagination";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { Button } from "@/components/common/Button";
import { useAuth } from "@/app/providers/AuthProvider";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy } from "@/lib/utils";
import type { FarmDTO, ShowWorkOrderDTO, TaskDTO } from "@/types/api";
import type { WorkOrderStatus } from "../constants";
import {
  downloadWorkOrderPdf,
  getFarms,
  getTasks,
  getWorkOrders,
  updateWorkOrderState,
} from "../services/workOrder.service";

const PAGE_SIZE = 10;

interface PendingChange {
  workOrder: ShowWorkOrderDTO;
  status: WorkOrderStatus;
}

export default function WorkOrderList() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { user } = useAuth();
  const canUpdate = hasPolicy(user?.policies, "WORKORDER_UPDATE");
  const canCreate = hasPolicy(user?.policies, "WORKORDER_CREATE");

  const [workOrders, setWorkOrders] = useState<ShowWorkOrderDTO[]>([]);
  const [farms, setFarms] = useState<FarmDTO[]>([]);
  const [tasks, setTasks] = useState<TaskDTO[]>([]);
  const [filters, setFilters] = useState<WorkOrderFilterValues>(EMPTY_FILTERS);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pendingChange, setPendingChange] = useState<PendingChange | null>(null);
  const [changing, setChanging] = useState(false);

  const loadWorkOrders = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getWorkOrders();
      setWorkOrders(data);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las órdenes de trabajo.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (searchParams.get("created") === "1") {
      toast.success("La orden de trabajo se creó correctamente.");
      router.replace("/work-orders");
    }
  }, [searchParams, router]);

  useEffect(() => {
    void loadWorkOrders();
    void (async () => {
      try {
        const [farmList, taskList] = await Promise.all([getFarms(), getTasks()]);
        setFarms(farmList);
        setTasks(taskList);
      } catch {
        // Los filtros dependen de datos auxiliares; si fallan, la tabla sigue operativa.
      }
    })();
  }, [loadWorkOrders]);

  useEffect(() => {
    setPage(1);
  }, [filters]);

  const filtered = useMemo(() => {
    return workOrders.filter((wo) => {
      if (filters.status && wo.status !== filters.status) return false;
      if (filters.farmId && wo.farm?.id !== Number(filters.farmId)) return false;
      if (filters.taskId && wo.task?.id !== Number(filters.taskId)) return false;

      const start = wo.startDate ? wo.startDate.slice(0, 10) : "";
      if (filters.dateFrom && (!start || start < filters.dateFrom)) return false;
      if (filters.dateTo && (!start || start > filters.dateTo)) return false;
      return true;
    });
  }, [workOrders, filters]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  const currentPage = Math.min(page, totalPages);
  const paged = useMemo(
    () => filtered.slice((currentPage - 1) * PAGE_SIZE, currentPage * PAGE_SIZE),
    [filtered, currentPage],
  );

  const handleDownload = async (workOrder: ShowWorkOrderDTO) => {
    try {
      await downloadWorkOrderPdf(workOrder.id, workOrder.orderNum);
    } catch {
      toast.error("No se pudo descargar el PDF de la orden.");
    }
  };

  const confirmChange = async () => {
    if (!pendingChange) return;
    setChanging(true);
    try {
      await updateWorkOrderState(pendingChange.workOrder.id, pendingChange.status);
      toast.success(
        `Orden ${pendingChange.workOrder.orderNum} actualizada a "${pendingChange.status}".`,
      );
      setPendingChange(null);
      await loadWorkOrders();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(
        pd?.detail || "No se pudo actualizar el estado de la orden.",
      );
      setPendingChange(null);
    } finally {
      setChanging(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageHeader
        title="Órdenes de trabajo"
        subtitle="Resumen y gestión de las órdenes registradas."
        actions={
          <Button
            variant="primary"
            icon={Plus}
            disabled={!canCreate}
            onClick={() => router.push("/work-orders/new")}
          >
            Nueva orden
          </Button>
        }
      />

      <WorkOrderFilters
        filters={filters}
        farms={farms}
        tasks={tasks}
        onChange={setFilters}
        onReset={() => setFilters(EMPTY_FILTERS)}
      />

      {loading ? (
        <LoadingState label="Cargando órdenes de trabajo..." />
      ) : error ? (
        <ErrorState message={error} onRetry={loadWorkOrders} />
      ) : filtered.length === 0 ? (
        <EmptyState
          title="No hay órdenes de trabajo"
          description={
            workOrders.length === 0
              ? "Todavía no se registraron órdenes de trabajo."
              : "Ninguna orden coincide con los filtros aplicados."
          }
        />
      ) : (
        <>
          <WorkOrderTable
            workOrders={paged}
            canUpdate={canUpdate}
            onView={(wo) => router.push(`/work-orders/${wo.id}`)}
            onDownload={handleDownload}
            onChangeStatus={(workOrder, status) =>
              setPendingChange({ workOrder, status })
            }
          />
          <Pagination
            page={currentPage}
            pageSize={PAGE_SIZE}
            totalCount={filtered.length}
            onPageChange={setPage}
          />
        </>
      )}

      <ConfirmDialog
        open={pendingChange !== null}
        title="Cambiar estado de la orden"
        message={
          pendingChange
            ? `¿Confirma cambiar el estado de la orden ${pendingChange.workOrder.orderNum} a "${pendingChange.status}"?`
            : ""
        }
        confirmLabel="Confirmar"
        variant={pendingChange?.status === "Cancelado" ? "danger" : "primary"}
        loading={changing}
        onConfirm={confirmChange}
        onCancel={() => setPendingChange(null)}
      />
    </div>
  );
}
