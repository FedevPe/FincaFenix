import { Coins } from "lucide-react";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { StatCard } from "@/components/common/StatCard";
import type { WorkOrderCostDTO, WorkOrderCostItemDTO } from "@/types/api";
import { formatDateOnly, formatNumber } from "@/lib/utils";

interface CostsTabProps {
  cost: WorkOrderCostDTO | null;
  loading: boolean;
  error: string | null;
  onRetry: () => void;
}

export function CostsTab({ cost, loading, error, onRetry }: CostsTabProps) {
  if (loading) return <LoadingState label="Cargando costos..." />;
  if (error) return <ErrorState message={error} onRetry={onRetry} />;
  if (!cost) return null;

  const items = cost.items ?? [];
  const currency = items[0]?.currencyCode ?? "ARS";

  if (items.length === 0) {
    return (
      <EmptyState
        title="Sin costos registrados"
        description="Esta orden no tiene costos congelados."
      />
    );
  }

  const columns: Column<WorkOrderCostItemDTO>[] = [
    {
      key: "material",
      header: "Material",
      render: (item) => item.materialName,
    },
    {
      key: "planned",
      header: "Cantidad planificada",
      align: "right",
      render: (item) =>
        `${formatNumber(item.plannedAmount, { maximumFractionDigits: 4 })} ${item.unitOfMeasure}`,
    },
    {
      key: "consumed",
      header: "Cantidad real consumida",
      align: "right",
      render: (item) =>
        `${formatNumber(item.consumedAmount, { maximumFractionDigits: 4 })} ${item.unitOfMeasure}`,
    },
    {
      key: "unitCost",
      header: "Costo unitario",
      align: "right",
      render: (item) =>
        `${item.currencySymbol} ${formatNumber(item.unitCost, { maximumFractionDigits: 2 })}`,
    },
    {
      key: "totalCost",
      header: "Costo total",
      align: "right",
      render: (item) =>
        `${item.currencySymbol} ${formatNumber(item.totalCost, { maximumFractionDigits: 2 })}`,
    },
    {
      key: "realCost",
      header: "Costo real",
      align: "right",
      render: (item) =>
        `${item.currencySymbol} ${formatNumber(item.realCost, { maximumFractionDigits: 2 })}`,
    },
    {
      key: "frozenDate",
      header: "Fecha",
      render: (item) => formatDateOnly(item.frozenDate),
    },
  ];

  const currencySymbol = items[0]?.currencySymbol ?? "$";

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <StatCard
          label={`Costo total planificado (${cost.items[0]?.currencyCode ?? "ARS"})`}
          value={`${currencySymbol} ${formatNumber(cost.totalCost, { maximumFractionDigits: 2 })}`}
          icon={Coins}
        />
        <StatCard
          label={`Costo real (${cost.items[0]?.currencyCode ?? "ARS"})`}
          value={`${currencySymbol} ${formatNumber(cost.totalRealCost, { maximumFractionDigits: 2 })}`}
          icon={Coins}
        />
      </div>
      <DataTable columns={columns} rows={items} rowKey={(item) => item.id} />
    </div>
  );
}
