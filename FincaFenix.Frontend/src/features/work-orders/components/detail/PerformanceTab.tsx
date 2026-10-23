import { Clock, Gauge, Layers, Ruler, Scale, Weight } from "lucide-react";
import { StatCard } from "@/components/common/StatCard";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/Feedback";
import type { ActivityWorkOrderDTO, ShowWorkOrderDTO } from "@/types/api";
import { formatDateOnly, formatNumber, toNumber } from "@/lib/utils";
import { rendimientoModeLabel } from "../../constants";
import { efficiencyTone, overrunTone } from "../../lib/deviation";

interface PerformanceTabProps {
  workOrder: ShowWorkOrderDTO;
}

export function PerformanceTab({ workOrder }: PerformanceTabProps) {
  const mode = workOrder.rendimientoMode;
  const isMaterialEfficiency = mode === "MaterialEfficiency";
  const activities = workOrder.detailsWorkOrder ?? [];

  const totalArea = toNumber(workOrder.totalAreaWorked);
  const totalAreaActual = toNumber(workOrder.totalAreaActuallyWorked);
  const totalHours = toNumber(workOrder.totalManHours);
  const totalProduced = workOrder.totalProducedAmount;
  const areaPerHour = totalHours > 0 ? totalAreaActual / totalHours : null;
  const outputPerHour =
    totalHours > 0 && totalProduced != null ? toNumber(totalProduced) / totalHours : null;

  const theoreticalMachinePasses = workOrder.recipe?.theoreticalMachinePasses ?? null;
  const realMachinePasses = activities.reduce(
    (sum, a) => sum + toNumber(a.machinePasses),
    0,
  );
  const machinePassesTone =
    theoreticalMachinePasses != null
      ? overrunTone(realMachinePasses, theoreticalMachinePasses)
      : undefined;

  const tableColumns: Column<ActivityWorkOrderDTO>[] = [
    {
      key: "activityDate",
      header: "Fecha",
      render: (a) => formatDateOnly(a.activityDate),
    },
    {
      key: "employee",
      header: "Operario",
      hidden: "md",
      render: (a) =>
        a.employee
          ? `${a.employee.name}${a.employee.lastName ? ` ${a.employee.lastName}` : ""}`
          : "-",
    },
    {
      key: "sector",
      header: "Sector",
      render: (a) => a.sector?.sectorName ?? "-",
    },
    ...(isMaterialEfficiency
      ? [
          {
            key: "machinePasses",
            header: "Maquinadas",
            align: "right" as const,
            hidden: "md" as const,
            render: (a: ActivityWorkOrderDTO) => formatNumber(a.machinePasses),
          },
        ]
      : []),
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

  const rendimientoValue =
    workOrder.rendimiento != null
      ? `${formatNumber(workOrder.rendimiento, { maximumFractionDigits: 2 })} ${workOrder.rendimientoUnit ?? ""}`
      : "-";

  const rendimientoDeviation =
    workOrder.rendimiento != null ? toNumber(workOrder.rendimiento) - 100 : null;
  const showDeviation =
    isMaterialEfficiency && rendimientoDeviation != null && rendimientoDeviation > 0;

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-4">
        <StatCard label="Modo de rendimiento" value={rendimientoModeLabel(mode)} icon={Gauge} />
        <StatCard
          label="Rendimiento"
          value={
            <span
              className={isMaterialEfficiency ? efficiencyTone(workOrder.rendimiento) : undefined}
            >
              {rendimientoValue}
            </span>
          }
          hint={
            showDeviation ? (
              <span className="text-destructive">
                +{formatNumber(rendimientoDeviation, { maximumFractionDigits: 1 })}% de desviación
              </span>
            ) : undefined
          }
        />
        <StatCard
          label="Área total"
          value={`${formatNumber(totalArea, { maximumFractionDigits: 2 })} ha`}
          icon={Ruler}
        />
        <StatCard
          label="Área trabajada"
          value={`${formatNumber(totalAreaActual, { maximumFractionDigits: 2 })} ha`}
          icon={Ruler}
        />
        <StatCard
          label="Horas totales"
          value={`${formatNumber(totalHours, { maximumFractionDigits: 2 })} h`}
          icon={Clock}
        />
        {areaPerHour != null ? (
          <StatCard
            label="Área por hora"
            value={`${formatNumber(areaPerHour, { maximumFractionDigits: 2 })} ha/h`}
            icon={Ruler}
          />
        ) : null}
        {totalProduced != null ? (
          <>
            <StatCard
              label="Producción total"
              value={`${formatNumber(totalProduced, { maximumFractionDigits: 2 })} kg`}
              icon={Weight}
            />
            {outputPerHour != null ? (
              <StatCard
                label="Producción por hora"
                value={`${formatNumber(outputPerHour, { maximumFractionDigits: 2 })} kg/h`}
                icon={Weight}
              />
            ) : null}
          </>
        ) : null}
        {isMaterialEfficiency ? (
          <>
            <StatCard
              label="Maquinadas teóricas"
              value={formatNumber(theoreticalMachinePasses, { maximumFractionDigits: 2 })}
              icon={Layers}
            />
            <StatCard
              label="Maquinadas reales"
              value={<span className={machinePassesTone}>{formatNumber(realMachinePasses)}</span>}
              icon={Scale}
            />
          </>
        ) : null}
      </div>

      {activities.length === 0 ? (
        <EmptyState
          title="Sin actividades registradas"
          description="Todavía no se registraron actividades para esta orden de trabajo."
        />
      ) : (
        <DataTable columns={tableColumns} rows={activities} rowKey={(a) => a.id} />
      )}
    </div>
  );
}
