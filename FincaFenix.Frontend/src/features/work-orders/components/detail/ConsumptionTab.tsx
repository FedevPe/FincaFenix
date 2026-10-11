import { Boxes, Info, Scale } from "lucide-react";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/Feedback";
import { StatCard } from "@/components/common/StatCard";
import type { DetailRecipeDTO, ShowWorkOrderDTO } from "@/types/api";
import { formatNumber } from "@/lib/utils";

interface ConsumptionTabProps {
  workOrder: ShowWorkOrderDTO;
}

export function ConsumptionTab({ workOrder }: ConsumptionTabProps) {
  const details = workOrder.recipe?.details ?? [];

  if (details.length === 0) {
    return (
      <EmptyState
        title="Sin consumos registrados"
        description="Esta orden no tiene una receta con materiales, por lo que no se registraron consumos."
      />
    );
  }

  const totalPlanned = details.reduce((sum, d) => sum + Number(d.estimatedAmount || 0), 0);
  const totalConsumed = details.reduce(
    (sum, d) => sum + Number(d.totalAmountConsumed || 0),
    0,
  );
  const consumedPercent = totalPlanned > 0 ? (totalConsumed / totalPlanned) * 100 : 0;

  const columns: Column<DetailRecipeDTO>[] = [
    {
      key: "material",
      header: "Material",
      render: (detail) => (
        <span>
          {detail.material?.articleName ?? `Material #${detail.materialId}`}
          {detail.material?.unitOfMeasure ? (
            <span className="text-muted-foreground"> · {detail.material.unitOfMeasure}</span>
          ) : null}
        </span>
      ),
    },
    {
      key: "planned",
      header: "Planificado",
      align: "right",
      render: (detail) =>
        `${formatNumber(detail.estimatedAmount, { maximumFractionDigits: 4 })} ${detail.estimatedAmountUnit ?? ""}`,
    },
    {
      key: "consumed",
      header: "Consumido",
      align: "right",
      render: (detail) =>
        `${formatNumber(detail.totalAmountConsumed, { maximumFractionDigits: 4 })} ${detail.material?.unitOfMeasure ?? ""}`,
    },
  ];

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
        <StatCard label="Planificado (Maquinadas)" value={formatNumber(totalPlanned, { maximumFractionDigits: 2 })} icon={Boxes} />
        <StatCard label="Consumido (Maquinadas)" value={formatNumber(totalConsumed, { maximumFractionDigits: 2 })} icon={Scale} />
        <StatCard label="Avance" value={`${formatNumber(consumedPercent, { maximumFractionDigits: 2 })}%`} />
      </div>

      <div className="flex items-center gap-2 rounded-md border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
        <Info className="size-4 shrink-0" aria-hidden />
        <span>
          La desagregación de consumos por sector no está disponible en la API
          actual; se muestra el consumo consolidado por material.
        </span>
      </div>

      <DataTable columns={columns} rows={details} rowKey={(d) => d.materialId} />
    </div>
  );
}
