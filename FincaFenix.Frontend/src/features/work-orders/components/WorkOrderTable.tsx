"use client";

import { useState } from "react";
import { Download, RotateCcw } from "lucide-react";
import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { StatusBadge } from "@/components/common/StatusBadge";
import type { ShowWorkOrderDTO } from "@/types/api";
import { allowedTransitions } from "../constants";
import type { WorkOrderStatus } from "../constants";
import { formatDateOnly } from "@/lib/utils";
import { Button } from "@/components/common/Button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Tooltip, TooltipTrigger, TooltipContent } from "@/components/ui/tooltip";

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
  const [statusModalOpen, setStatusModalOpen] = useState(false);
  const [selectedWorkOrder, setSelectedWorkOrder] = useState<ShowWorkOrderDTO | null>(null);

  const handleOpenStatusModal = (workOrder: ShowWorkOrderDTO) => {
    setSelectedWorkOrder(workOrder);
    setStatusModalOpen(true);
  };

  const handleStatusChange = (status: WorkOrderStatus) => {
    if (selectedWorkOrder) {
      onChangeStatus(selectedWorkOrder, status);
    }
    setStatusModalOpen(false);
    setSelectedWorkOrder(null);
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
      width: "6rem",
      render: (wo) => (
        <div className="flex items-center justify-end gap-1">
          <Tooltip>
            <TooltipTrigger asChild>
              <Button
                variant="ghost"
                size="sm"
                onClick={() => onDownload(wo)}
                aria-label={`Descargar PDF de la orden ${wo.orderNum}`}
              >
                <Download className="size-4" />
              </Button>
            </TooltipTrigger>
            <TooltipContent side="top">Descargar PDF</TooltipContent>
          </Tooltip>
          {canUpdate && allowedTransitions(wo.status).length > 0 && (
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => handleOpenStatusModal(wo)}
                  aria-label={`Cambiar estado de la orden ${wo.orderNum}`}
                >
                  <RotateCcw className="size-4" />
                </Button>
              </TooltipTrigger>
              <TooltipContent side="top">Cambiar estado</TooltipContent>
            </Tooltip>
          )}
        </div>
      ),
    },
  ];

  return (
    <>
      <DataTable
        columns={columns}
        rows={workOrders}
        rowKey={(wo) => wo.id}
        onRowClick={onView}
      />
      <Dialog open={statusModalOpen} onOpenChange={setStatusModalOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Cambiar estado de la orden</DialogTitle>
            <DialogDescription>
              Seleccione el nuevo estado para la orden <strong>{selectedWorkOrder?.orderNum}</strong>.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter showCloseButton>
            {selectedWorkOrder && allowedTransitions(selectedWorkOrder.status).map((status) => (
              <Button
                key={status}
                variant={status === "Cancelado" ? "danger" : "secondary"}
                onClick={() => handleStatusChange(status)}
              >
                {status}
              </Button>
            ))}
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
