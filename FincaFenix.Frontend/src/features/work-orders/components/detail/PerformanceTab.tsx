import { Clock, Gauge, Layers, Ruler, Weight } from "lucide-react";
import { StatCard } from "@/components/common/StatCard";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/Feedback";
import type { ActivityWorkOrderDTO, ShowWorkOrderDTO } from "@/types/api";
import { formatDateOnly, formatNumber } from "@/lib/utils";
import { rendimientoModeLabel } from "../../constants";

interface PerformanceTabProps {
  workOrder: ShowWorkOrderDTO;
}

export function PerformanceTab({ workOrder }: PerformanceTabProps) {
  const mode = workOrder.rendimientoMode;
  const activities = workOrder.detailsWorkOrder ?? [];

  const columns: Column<ActivityWorkOrderDTO>[] = [
    {
      key: "activityDate",
      header: "Fecha",
      render: (a) => formatDateOnly(a.activityDate),
    },
    {
      key: "employee",
      header: "Operario",
      render: (a) => (a.employee ? a.employee.name : "-"),
    },
    {
      key: "sector",
      header: "Sector",
      render: (a) => a.sector?.sectorName ?? "-",
    },
    {
      key: "machinePasses",
      header: "Maquinadas",
      align: "right",
      render: (a) => formatNumber(a.machinePasses),
    },
    {
      key: "workedHours",
      header: "Horas",
      align: "right",
      render: (a) => formatNumber(a.workedHours, { maximumFractionDigits: 2 }),
    },
    {
      key: "rendimiento",
      header: "Rendimiento",
      align: "right",
      render: (a) =>
        a.rendimiento != null
          ? `${formatNumber(a.rendimiento, { maximumFractionDigits: 2 })} ${a.rendimientoUnit ?? ""}`
          : "-",
    },
  ];

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
        <StatCard label="Modo de rendimiento" value={rendimientoModeLabel(mode)} icon={Gauge} />
        <StatCard
          label="Rendimiento"
          value={
            workOrder.rendimiento != null
              ? `${formatNumber(workOrder.rendimiento, { maximumFractionDigits: 2 })} ${workOrder.rendimientoUnit ?? ""}`
              : "-"
          }
        />
        <StatCard
          label="Área total"
          value={`${formatNumber(workOrder.totalAreaWorked, { maximumFractionDigits: 2 })} ha`}
          icon={Ruler}
        />
        <StatCard
          label="Horas totales"
          value={`${formatNumber(workOrder.totalManHours, { maximumFractionDigits: 2 })} h`}
          icon={Clock}
        />
        {workOrder.totalProducedAmount != null ? (
          <StatCard
            label="Producción total"
            value={`${formatNumber(workOrder.totalProducedAmount, { maximumFractionDigits: 2 })} kg`}
            icon={Weight}
          />
        ) : null}
        <StatCard
          label="Maquinadas reales"
          value={formatNumber(
            activities.reduce((sum, a) => sum + Number(a.machinePasses || 0), 0),
          )}
          icon={Layers}
        />
      </div>

      <DataTable columns={columns} rows={activities} rowKey={(a) => a.id} />
    </div>
  );
}
