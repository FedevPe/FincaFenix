"use client";

import { useCallback, useEffect, useState } from "react";
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
  getWorkOrdersPaginated,
  type GetWorkOrdersPaginatedParams,
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
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pendingChange, setPendingChange] = useState<PendingChange | null>(null);
  const [changing, setChanging] = useState(false);

  const loadWorkOrders = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const params: GetWorkOrdersPaginatedParams = {
        pageNumber: page,
        pageSize: PAGE_SIZE,
        status: filters.status || undefined,
        farmId: filters.farmId ? Number(filters.farmId) : undefined,
        taskId: filters.taskId ? Number(filters.taskId) : undefined,
        dateFrom: filters.dateFrom || undefined,
        dateTo: filters.dateTo || undefined,
      };
      const result = await getWorkOrdersPaginated(params);
      setWorkOrders(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las órdenes de trabajo.");
    } finally {
      setLoading(false);
    }
  }, [page, filters]);

  useEffect(() => {
    if (searchParams.get("created") === "1") {
      toast.success("La orden de trabajo se creó correctamente.");
      router.replace("/work-orders");
    }
  }, [searchParams, router]);

  useEffect(() => {
    void (async () => {
      try {
        const [farmList, taskList] = await Promise.all([getFarms(), getTasks()]);
        setFarms(farmList);
        setTasks(taskList);
      } catch {
        // Los filtros dependen de datos auxiliares; si fallan, la tabla sigue operativa.
      }
    })();
  }, []);

  useEffect(() => {
    setPage(1);
  }, [filters]);

  useEffect(() => {
    void loadWorkOrders();
  }, [loadWorkOrders]);

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
      ) : workOrders.length === 0 ? (
        <EmptyState
          title="No hay órdenes de trabajo"
          description={
            totalCount === 0
              ? "Todavía no se registraron órdenes de trabajo."
              : "Ninguna orden coincide con los filtros aplicados."
          }
        />
      ) : (
        <div>
          <WorkOrderTable
            workOrders={workOrders}
            canUpdate={canUpdate}
            onDownload={handleDownload}
            onChangeStatus={(workOrder, status) =>
              setPendingChange({ workOrder, status })
            }
          />
          <Pagination
            page={page}
            pageSize={PAGE_SIZE}
            totalCount={totalCount}
            onPageChange={setPage}
          />
        </div>
      )}

      <ConfirmDialog
        open={pendingChange !== null}
        title="Cambiar estado de la orden"
        message={
          pendingChange
            ? "Confirma cambiar el estado de la orden " + pendingChange.workOrder.orderNum + ' a "' + pendingChange.status + '"?'
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