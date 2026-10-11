"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { useParams, useRouter, useSearchParams } from "next/navigation";
import {
  AlertCircle,
  ArrowLeft,
  Boxes,
  Coins,
  FlaskConical,
  Gauge,
  Layers,
} from "lucide-react";
import { toast } from "sonner";
import { Tabs } from "@/components/common/Tabs";
import type { TabItem } from "@/components/common/Tabs";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { Button } from "@/components/common/Button";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { Modal } from "@/components/common/Modal";
import { useAuth } from "@/app/providers/AuthProvider";
import { hasPolicy } from "@/lib/utils";
import { AddActivityForm } from "../components/detail/AddActivityForm";
import { WorkOrderHeader } from "../components/detail/WorkOrderHeader";
import { ActivitiesTab } from "../components/detail/ActivitiesTab";
import { RecipeTab } from "../components/detail/RecipeTab";
import { ConsumptionTab } from "../components/detail/ConsumptionTab";
import { PerformanceTab } from "../components/detail/PerformanceTab";
import { CostsTab } from "../components/detail/CostsTab";
import { getProblemDetails } from "@/lib/api.client";
import type { ShowWorkOrderDTO, WorkOrderCostDTO } from "@/types/api";
import type { WorkOrderStatus } from "../constants";
import {
  downloadWorkOrderPdf,
  getWorkOrderById,
  getWorkOrderCosts,
  updateWorkOrderState,
} from "../services/workOrder.service";

type DetailTab = "actividades" | "receta" | "consumos" | "rendimiento" | "costos";

const DETAIL_TABS: TabItem<DetailTab>[] = [
  { id: "actividades", label: "Actividades", icon: Layers },
  { id: "receta", label: "Receta", icon: FlaskConical },
  { id: "consumos", label: "Consumos", icon: Boxes },
  { id: "rendimiento", label: "Rendimiento", icon: Gauge },
  { id: "costos", label: "Costos", icon: Coins },
];

function isDetailTab(value: string | null): value is DetailTab {
  return DETAIL_TABS.some((tab) => tab.id === value);
}

export default function WorkOrderDetail() {
  const params = useParams<{ id: string }>();
  const workOrderId = Number(params?.id);
  const router = useRouter();
  const searchParams = useSearchParams();

  const tabValue = searchParams.get("tab");
  const [activeTab, setActiveTab] = useState<DetailTab>(
    isDetailTab(tabValue) ? tabValue : "actividades",
  );

  const [workOrder, setWorkOrder] = useState<ShowWorkOrderDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [notFound, setNotFound] = useState(false);

  const [cost, setCost] = useState<WorkOrderCostDTO | null>(null);
  const [costLoading, setCostLoading] = useState(false);
  const [costError, setCostError] = useState<string | null>(null);

  const [activityOpen, setActivityOpen] = useState(false);
  const [pendingStatus, setPendingStatus] = useState<WorkOrderStatus | null>(null);
  const [changingStatus, setChangingStatus] = useState(false);

  const { user } = useAuth();
  const canUpdateStatus = hasPolicy(user?.policies, "WORKORDER_UPDATE");
  const canAddActivity =
    hasPolicy(user?.policies, "DETAIL_WORKORDER_CREATE") &&
    workOrder?.status !== "Cerrado" &&
    workOrder?.status !== "Cancelado";

  const loadWorkOrder = useCallback(async () => {
    setLoading(true);
    setError(null);
    setNotFound(false);
    try {
      const data = await getWorkOrderById(workOrderId);
      setWorkOrder(data);
    } catch (err) {
      const pd = getProblemDetails(err);
      if (pd?.status === 404) {
        setNotFound(true);
      } else {
        setError(pd?.detail || "No se pudo cargar la orden de trabajo.");
      }
    } finally {
      setLoading(false);
    }
  }, [workOrderId]);

  const loadCosts = useCallback(async () => {
    if (cost) return;
    setCostLoading(true);
    setCostError(null);
    try {
      setCost(await getWorkOrderCosts(workOrderId));
    } catch (err) {
      const pd = getProblemDetails(err);
      setCostError(pd?.detail || "No se pudieron cargar los costos de la orden.");
    } finally {
      setCostLoading(false);
    }
  }, [cost, workOrderId]);

  useEffect(() => {
    void loadWorkOrder();
  }, [loadWorkOrder]);

  useEffect(() => {
    if (activeTab === "costos") void loadCosts();
  }, [activeTab, loadCosts]);

  const handleTabChange = (tab: DetailTab) => {
    setActiveTab(tab);
    if (typeof window !== "undefined") {
      const url =
        tab === "actividades"
          ? `/work-orders/${workOrderId}`
          : `/work-orders/${workOrderId}?tab=${tab}`;
      window.history.replaceState(null, "", url);
    }
  };

  const confirmStatusChange = useCallback(async () => {
    if (!pendingStatus) return;
    setChangingStatus(true);
    try {
      await updateWorkOrderState(workOrderId, pendingStatus);
      toast.success(`Orden ${workOrder?.orderNum ?? workOrderId} actualizada a "${pendingStatus}".`);
      setPendingStatus(null);
      await loadWorkOrder();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo actualizar el estado de la orden.");
      setPendingStatus(null);
    } finally {
      setChangingStatus(false);
    }
  }, [pendingStatus, workOrderId, workOrder?.orderNum, loadWorkOrder]);

  const handleDownloadPdf = async () => {
    try {
      await downloadWorkOrderPdf(workOrderId, workOrder?.orderNum ?? String(workOrderId));
    } catch {
      setError("No se pudo descargar el PDF de la orden.");
    }
  };

  const tabContent = useMemo(() => {
    if (!workOrder) return null;
    switch (activeTab) {
      case "receta":
        return <RecipeTab workOrder={workOrder} />;
      case "consumos":
        return <ConsumptionTab workOrder={workOrder} />;
      case "rendimiento":
        return <PerformanceTab workOrder={workOrder} />;
      case "costos":
        return (
          <CostsTab
            cost={cost}
            loading={costLoading}
            error={costError}
            onRetry={loadCosts}
          />
        );
      default:
        return (
          <ActivitiesTab
            workOrder={workOrder}
            onAddActivity={() => setActivityOpen(true)}
            canAddActivity={canAddActivity}
          />
        );
    }
  }, [workOrder, activeTab, cost, costLoading, costError, loadCosts, canAddActivity]);

  return (
    <div className="space-y-6">
      <Button
        variant="ghost"
        icon={ArrowLeft}
        onClick={() => router.push("/work-orders")}
        className="self-start"
      >
        Volver al listado
      </Button>

      {loading ? (
        <LoadingState label="Cargando orden de trabajo..." />
      ) : error && !workOrder ? (
        <ErrorState message={error} onRetry={loadWorkOrder} />
      ) : notFound || !workOrder ? (
        <EmptyState
          title="Orden no encontrada"
          description="La orden de trabajo solicitada no existe o fue eliminada."
        />
      ) : (
        <>
          {error ? (
            <div className="flex items-center gap-2 rounded-md border border-destructive/40 bg-destructive/10 px-3 py-2 text-sm text-destructive">
              <AlertCircle className="size-4 shrink-0" aria-hidden />
              <span>{error}</span>
            </div>
          ) : null}

          <WorkOrderHeader
            workOrder={workOrder}
            onDownloadPdf={handleDownloadPdf}
            canUpdate={canUpdateStatus}
            onChangeStatus={setPendingStatus}
          />

          <Tabs tabs={DETAIL_TABS} active={activeTab} onChange={handleTabChange} />

          <div>{tabContent}</div>
        </>
      )}

      <Modal
        open={activityOpen}
        title="Registrar actividad"
        onClose={() => setActivityOpen(false)}
      >
        {workOrder ? (
          <AddActivityForm
            workOrder={workOrder}
            onClose={() => setActivityOpen(false)}
            onSaved={() => {
              setActivityOpen(false);
              void loadWorkOrder();
            }}
          />
        ) : null}
      </Modal>

      <ConfirmDialog
        open={pendingStatus !== null}
        title="Cambiar estado de la orden"
        message={
          pendingStatus
            ? `¿Confirma cambiar el estado de la orden ${workOrder?.orderNum ?? workOrderId} a "${pendingStatus}"?`
            : ""
        }
        confirmLabel="Confirmar"
        variant={pendingStatus === "Cancelado" ? "danger" : "primary"}
        loading={changingStatus}
        onConfirm={confirmStatusChange}
        onCancel={() => setPendingStatus(null)}
      />
    </div>
  );
}
