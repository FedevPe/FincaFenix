import type { LucideIcon } from "lucide-react";
import { Ban, CheckCircle2, Hourglass, PlayCircle } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

interface StatusMeta {
  label: string;
  icon: LucideIcon;
  variant: "warning" | "info" | "success" | "destructive" | "default";
}

const STATUS_MAP: Record<string, StatusMeta> = {
  Pendiente: {
    label: "Pendiente",
    icon: Hourglass,
    variant: "warning",
  },
  Activo: {
    label: "Activo",
    icon: PlayCircle,
    variant: "info",
  },
  Cerrado: {
    label: "Cerrado",
    icon: CheckCircle2,
    variant: "success",
  },
  Cancelado: {
    label: "Cancelado",
    icon: Ban,
    variant: "destructive",
  },
};

const VARIANT_CLASSES: Record<StatusMeta["variant"], string> = {
  warning: "border-warning/40 bg-warning/20 text-warning",
  info: "border-info/40 bg-info/20 text-info",
  success: "border-success/40 bg-success/20 text-success",
  destructive: "border-destructive/40 bg-destructive/20 text-destructive",
  default: "border-border bg-muted text-muted-foreground",
};

export function StatusBadge({ status }: { status: string }) {
  const meta = STATUS_MAP[status] ?? {
    label: status || "Desconocido",
    icon: Ban,
    variant: "default",
  };
  const Icon: LucideIcon = meta.icon;

  return (
    <Badge variant="outline" className={cn("gap-1", VARIANT_CLASSES[meta.variant])}>
      <Icon className="size-3.5" aria-hidden />
      {meta.label}
    </Badge>
  );
}
