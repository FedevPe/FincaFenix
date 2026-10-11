"use client";

import { ArrowLeftRight, Eye, FileDown, MoreVertical } from "lucide-react";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { Dropdown } from "@/components/common/Dropdown";
import type { DropdownItem } from "@/components/common/Dropdown";
import { StatusBadge } from "@/components/common/StatusBadge";
import type { ShowWorkOrderDTO } from "@/types/api";
import { allowedTransitions } from "../constants";
import type { WorkOrderStatus } from "../constants";
import { formatDateOnly } from "@/lib/utils";

interface WorkOrderTableProps {
  workOrders: ShowWorkOrderDTO[];
  canUpdate: boolean;
  onView: (workOrder: ShowWorkOrderDTO) => void;
  onDownload: (workOrder: ShowWorkOrderDTO) => void;
  onChangeStatus: (workOrder: ShowWorkOrderDTO, status: WorkOrderStatus) => void;
}

export function WorkOrderTable({
  workOrders,
  canUpdate,
  onView,
  onDownload,
  onChangeStatus,
}: WorkOrderTableProps) {
  const buildActions = (workOrder: ShowWorkOrderDTO): DropdownItem[] => {
    const items: DropdownItem[] = [
      { label: "Ver detalle", icon: Eye, onClick: () => onView(workOrder) },
      {
        label: "Descargar PDF",
        icon: FileDown,
        onClick: () => onDownload(workOrder),
      },
    ];

    if (canUpdate) {
      allowedTransitions(workOrder.status).forEach((status) => {
        items.push({
          label: `Marcar como ${status}`,
          icon: ArrowLeftRight,
          variant: status === "Cancelado" ? "danger" : "default",
          onClick: () => onChangeStatus(workOrder, status),
        });
      });
    }

    return items;
  };

  const columns: Column<ShowWorkOrderDTO>[] = [
    {
      key: "orderNum",
      header: "Nº de orden",
      sortable: true,
      sortValue: (wo) => wo.orderNum,
      render: (wo) => (
        <span className="font-medium tabular-nums">{wo.orderNum}</span>
      ),
    },
    {
      key: "farm",
      header: "Finca",
      render: (wo) => wo.farm?.name ?? "-",
    },
    {
      key: "task",
      header: "Tarea",
      render: (wo) => wo.task?.description ?? "-",
    },
    {
      key: "status",
      header: "Estado",
      render: (wo) => <StatusBadge status={wo.status} />,
    },
    {
      key: "startDate",
      header: "Inicio",
      sortable: true,
      sortValue: (wo) => wo.startDate ?? "",
      render: (wo) => formatDateOnly(wo.startDate),
    },
    {
      key: "sectors",
      header: "Sectores",
      render: (wo) => {
        const sectors = wo.sectorList ?? [];
        if (sectors.length === 0) return "-";
        return (
          <span className="text-muted-foreground">
            {sectors.slice(0, 3).map((s) => s.sectorName).join(", ")}
            {sectors.length > 3 ? ` (+${sectors.length - 3})` : ""}
          </span>
        );
      },
    },
    {
      key: "actions",
      header: "",
      align: "right",
      width: "3.5rem",
      render: (wo) => (
        <Dropdown
          trigger={<MoreVertical />}
          triggerLabel={`Acciones de la orden ${wo.orderNum}`}
          items={buildActions(wo)}
        />
      ),
    },
  ];

  return (
    <DataTable
      columns={columns}
      rows={workOrders}
      rowKey={(wo) => wo.id}
      onRowClick={onView}
    />
  );
}
