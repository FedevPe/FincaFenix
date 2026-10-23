"use client";

import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import type { StockDTO } from "@/types/api";

type StockStatus = "ok" | "low" | "out";

const STATUS_CLASSES: Record<StockStatus, string> = {
  ok: "border-success/40 bg-success/20 text-success",
  low: "border-warning/40 bg-warning/20 text-warning",
  out: "border-destructive/40 bg-destructive/20 text-destructive",
};

const STATUS_LABELS: Record<StockStatus, string> = {
  ok: "OK",
  low: "Bajo",
  out: "Sin stock",
};

interface StockStatusBadgeProps {
  row: StockDTO;
}

export function StockStatusBadge({ row }: StockStatusBadgeProps) {
  let status: StockStatus;
  if (row.stockDisponible <= 0) {
    status = "out";
  } else if (row.isLow) {
    status = "low";
  } else {
    status = "ok";
  }

  return (
    <Badge variant="outline" className={cn(STATUS_CLASSES[status])}>
      {STATUS_LABELS[status]}
    </Badge>
  );
}