import { ArrowLeftRight, FileDown } from "lucide-react";
import { StatusBadge } from "@/components/common/StatusBadge";
import { Button } from "@/components/common/Button";
import { Dropdown } from "@/components/common/Dropdown";
import type { DropdownItem } from "@/components/common/Dropdown";
import { Badge } from "@/components/ui/badge";
import type { ShowWorkOrderDTO } from "@/types/api";
import { formatDateOnly } from "@/lib/utils";
import { allowedTransitions } from "../../constants";
import type { WorkOrderStatus } from "../../constants";

interface WorkOrderHeaderProps {
  workOrder: ShowWorkOrderDTO;
  onDownloadPdf: () => void;
  canUpdate?: boolean;
  onChangeStatus?: (status: WorkOrderStatus) => void;
}

function Field({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="space-y-0.5">
      <span className="text-xs font-medium text-muted-foreground">{label}</span>
      <span className="block text-sm">{value}</span>
    </div>
  );
}

export function WorkOrderHeader({
  workOrder,
  onDownloadPdf,
  canUpdate,
  onChangeStatus,
}: WorkOrderHeaderProps) {
  const sectors = workOrder.sectorList ?? [];
  const transitions = canUpdate ? allowedTransitions(workOrder.status) : [];
  const statusItems: DropdownItem[] = transitions.map((status) => ({
    label: `Marcar como ${status}`,
    icon: ArrowLeftRight,
    variant: status === "Cancelado" ? "danger" : "default",
    onClick: () => onChangeStatus?.(status),
  }));

  return (
    <section className="space-y-4 rounded-lg border bg-card p-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-3">
          <h1 className="text-xl font-semibold tracking-tight">
            Orden {workOrder.orderNum}
          </h1>
          <StatusBadge status={workOrder.status} />
        </div>
        <div className="flex items-center gap-2">
          {statusItems.length > 0 ? (
            <Dropdown
              align="right"
              trigger={
                <>
                  <ArrowLeftRight />
                  Cambiar estado
                </>
              }
              triggerLabel="Cambiar estado de la orden"
              items={statusItems}
              className="h-8 gap-1.5 rounded-md border bg-background px-3 text-sm font-medium text-foreground"
            />
          ) : null}
          <Button variant="secondary" size="sm" icon={FileDown} onClick={onDownloadPdf}>
            Descargar PDF
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-5">
        <Field label="Tarea" value={workOrder.task?.description ?? "-"} />
        <Field label="Finca" value={workOrder.farm?.name ?? "-"} />
        <Field label="Fecha de creación" value={formatDateOnly(workOrder.createdDate)} />
        <Field label="Fecha de inicio" value={formatDateOnly(workOrder.startDate)} />
        <Field label="Fecha de fin" value={formatDateOnly(workOrder.endDate)} />
      </div>

      {workOrder.description ? (
        <Field
          label="Descripción"
          value={<span className="text-muted-foreground">{workOrder.description}</span>}
        />
      ) : null}

      <div className="space-y-1.5">
        <span className="text-xs font-medium text-muted-foreground">
          Sectores de trabajo
        </span>
        {sectors.length === 0 ? (
          <span className="block text-sm text-muted-foreground">
            Sin sectores asignados
          </span>
        ) : (
          <div className="flex flex-wrap gap-2">
            {sectors.map((sector) => (
              <Badge key={sector.id} variant="secondary" className="font-normal">
                {sector.sectorName}
                {sector.area != null
                  ? ` · ${sector.area.toLocaleString("es-AR")} ha`
                  : ""}
              </Badge>
            ))}
          </div>
        )}
      </div>
    </section>
  );
}
