import { Plus, Ruler } from "lucide-react";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/Feedback";
import { Button } from "@/components/common/Button";
import { StatCard } from "@/components/common/StatCard";
import type { ActivityWorkOrderDTO, ShowWorkOrderDTO } from "@/types/api";
import { formatDateOnly, formatNumber } from "@/lib/utils";

interface ActivitiesTabProps {
  workOrder: ShowWorkOrderDTO;
  onAddActivity?: () => void;
  canAddActivity?: boolean;
}

export function ActivitiesTab({
  workOrder,
  onAddActivity,
  canAddActivity,
}: ActivitiesTabProps) {
  const activities = workOrder.detailsWorkOrder ?? [];

  if (activities.length === 0 && !canAddActivity) {
    return (
      <EmptyState
        title="Sin actividades registradas"
        description="Todavía no se registraron actividades para esta orden de trabajo."
      />
    );
  }

  const totalHours = activities.reduce(
    (sum, a) => sum + Number(a.workedHours || 0),
    0,
  );

  const isMaterialEfficiency = workOrder.rendimientoMode === "MaterialEfficiency";

  const sectorProgress = (workOrder.sectorList ?? [])
    .map((sector) => {
      const sectorActivities = activities.filter(
        (activity) => activity.sector?.id === sector.id,
      );
      const percent = sectorActivities.reduce(
        (sum, activity) => sum + Number(activity.percentageAreaWorked || 0),
        0,
      );
      const area = sectorActivities.reduce(
        (sum, activity) => sum + Number(activity.areaWorked || 0),
        0,
      );
      return { sector, percent, area };
    })
    .filter((row) => row.percent > 0);

  const columns: Column<ActivityWorkOrderDTO>[] = [
    {
      key: "activityDate",
      header: "Fecha",
      render: (a) => formatDateOnly(a.activityDate),
    },
    {
      key: "employee",
      header: "Operario",
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
    {
      key: "percentageAreaWorked",
      header: "% del sector",
      align: "right",
      render: (a) =>
        `${formatNumber(a.percentageAreaWorked, { maximumFractionDigits: 2 })}%`,
    },
    {
      key: "areaWorked",
      header: "Área trabajada",
      align: "right",
      render: (a) =>
        a.areaWorked != null
          ? `${formatNumber(a.areaWorked, { maximumFractionDigits: 2 })} ha`
          : "-",
    },
    ...(isMaterialEfficiency
      ? [
          {
            key: "machinePasses",
            header: "Maquinadas",
            align: "right" as const,
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

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <p className="text-sm text-muted-foreground">
          {activities.length === 0
            ? "Todavía no hay actividades registradas."
            : `${activities.length} actividad${activities.length === 1 ? "" : "es"} registrada${activities.length === 1 ? "" : "s"}.`}
        </p>
        {canAddActivity && onAddActivity ? (
          <Button variant="primary" size="sm" icon={Plus} onClick={onAddActivity}>
            Registrar actividad
          </Button>
        ) : null}
      </div>

      {activities.length === 0 ? (
        <EmptyState
          title="Sin actividades registradas"
          description="Todavía no se registraron actividades para esta orden de trabajo."
        />
      ) : (
        <>
          <DataTable
            columns={columns}
            rows={activities}
            rowKey={(a) => a.id}
          />
          <div className="flex flex-wrap gap-x-6 gap-y-1 text-sm text-muted-foreground">
            <span>
              Horas totales:{" "}
              <span className="tabular-nums text-foreground">
                {formatNumber(totalHours, { maximumFractionDigits: 2 })} h
              </span>
            </span>
          </div>

          {sectorProgress.length > 0 ? (
            <div className="space-y-2">
              <h4 className="text-sm font-medium">Avance por sector</h4>
              <div className="overflow-hidden rounded-md border">
                <table className="w-full text-sm">
                  <thead className="bg-muted/50 text-muted-foreground">
                    <tr>
                      <th className="px-3 py-2 text-left font-medium">Sector</th>
                      <th className="px-3 py-2 text-right font-medium">
                        % acumulado
                      </th>
                      <th className="px-3 py-2 text-right font-medium">
                        Área trabajada
                      </th>
                      <th className="px-3 py-2 text-right font-medium">
                        Superficie
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {sectorProgress.map(({ sector, percent, area }) => (
                      <tr key={sector.id} className="border-t">
                        <td className="px-3 py-2">{sector.sectorName}</td>
                        <td className="px-3 py-2 text-right tabular-nums">
                          {formatNumber(percent, { maximumFractionDigits: 2 })}%
                        </td>
                        <td className="px-3 py-2 text-right tabular-nums">
                          {formatNumber(area, { maximumFractionDigits: 2 })} ha
                        </td>
                        <td className="px-3 py-2 text-right tabular-nums">
                          {sector.area != null
                            ? `${formatNumber(sector.area, { maximumFractionDigits: 2 })} ha`
                            : "-"}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          ) : null}
        </>
      )}
    </div>
  );
}
