import { Boxes, Gauge, Info, Scale } from "lucide-react";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/Feedback";
import { StatCard } from "@/components/common/StatCard";
import type { DetailRecipeDTO, ShowWorkOrderDTO } from "@/types/api";
import { formatNumber } from "@/lib/utils";
import { deviationPercent, efficiencyTone, formatDeviation, overrunTone } from "../../lib/deviation";

interface ConsumptionTabProps {
  workOrder: ShowWorkOrderDTO;
}

function plannedBase(detail: DetailRecipeDTO): number {
  return detail.theoreticalAmount ?? detail.estimatedAmount;
}

function baseUnit(detail: DetailRecipeDTO): string {
  return detail.material?.unitOfMeasure ?? detail.estimatedAmountUnit ?? "";
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

  const totalPlanned = details.reduce((sum, d) => sum + plannedBase(d), 0);
  const totalConsumed = details.reduce(
    (sum, d) => sum + Number(d.totalAmountConsumed || 0),
    0,
  );

  const efficiencyDeviation =
    workOrder.rendimiento != null ? Number(workOrder.rendimiento) - 100 : null;

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
      header: "Planificado (base)",
      align: "right",
      render: (detail) =>
        `${formatNumber(plannedBase(detail), { maximumFractionDigits: 4 })} ${baseUnit(detail)}`,
    },
    {
      key: "consumed",
      header: "Consumido",
      align: "right",
      render: (detail) => (
        <span className={overrunTone(detail.totalAmountConsumed, plannedBase(detail))}>
          {`${formatNumber(detail.totalAmountConsumed, { maximumFractionDigits: 4 })} ${baseUnit(detail)}`}
        </span>
      ),
    },
    {
      key: "deviation",
      header: "Desviación",
      align: "right",
      render: (detail) => (
        <span className={overrunTone(detail.totalAmountConsumed, plannedBase(detail))}>
          {formatDeviation(deviationPercent(detail.totalAmountConsumed, plannedBase(detail)))}
        </span>
      ),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
        <StatCard
          label="Eficiencia de aplicación"
          value={
            <span className={efficiencyTone(workOrder.rendimiento)}>
              {workOrder.rendimiento != null
                ? `${formatNumber(workOrder.rendimiento, { maximumFractionDigits: 2 })} ${workOrder.rendimientoUnit ?? "%"}`
                : "-"}
            </span>
          }
          icon={Gauge}
          hint={
            efficiencyDeviation != null && efficiencyDeviation > 0 ? (
              <span className="text-destructive">
                +{formatNumber(efficiencyDeviation, { maximumFractionDigits: 1 })}% de desviación
              </span>
            ) : undefined
          }
        />
        <StatCard
          label="Planificado (unidad base)"
          value={formatNumber(totalPlanned, { maximumFractionDigits: 2 })}
          icon={Boxes}
        />
        <StatCard
          label="Consumido (unidad base)"
          value={
            <span className={overrunTone(totalConsumed, totalPlanned)}>
              {formatNumber(totalConsumed, { maximumFractionDigits: 2 })}
            </span>
          }
          icon={Scale}
        />
      </div>

      <div className="flex items-center gap-2 rounded-md border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
        <Info className="size-4 shrink-0" aria-hidden />
        <span>
          El consumo se muestra en la unidad base de cada material. La desviación compara lo
          consumido contra la cantidad teórica (planificada). La desagregación por sector no está
          disponible en la API actual.
        </span>
      </div>

      <DataTable columns={columns} rows={details} rowKey={(d) => d.materialId} />
    </div>
  );
}
