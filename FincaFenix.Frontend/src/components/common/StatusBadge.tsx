import type { LucideIcon } from "lucide-react";
import { Ban, CheckCircle2, Hourglass, PlayCircle } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

interface StatusMeta {
  label: string;
  icon: LucideIcon;
  variant: string;
}

const STATUS_MAP: Record<string, StatusMeta> = {
  Pendiente: {
    label: "Pendiente",
    icon: Hourglass,
    variant:
      "border-amber-200 bg-amber-100 text-amber-800 dark:border-amber-500/30 dark:bg-amber-500/15 dark:text-amber-400",
  },
  Activo: {
    label: "Activo",
    icon: PlayCircle,
    variant:
      "border-blue-200 bg-blue-100 text-blue-800 dark:bg-blue-500/15 dark:text-blue-400 dark:border-blue-500/30",
  },
  Cerrado: {
    label: "Cerrado",
    icon: CheckCircle2,
    variant:
      "bg-emerald-100 text-emerald-800 border-emerald-200 dark:bg-emerald-500/15 dark:text-emerald-400 dark:border-emerald-500/30",
  },
  Cancelado: {
    label: "Cancelado",
    icon: Ban,
    variant:
      "bg-red-100 text-red-800 border-red-200 dark:bg-red-500/15 dark:text-red-400 dark:border-red-500/30",
  },
};

export function StatusBadge({ status }: { status: string }) {
  const meta = STATUS_MAP[status] ?? {
    label: status || "Desconocido",
    icon: Ban,
    variant: "bg-muted text-muted-foreground border-border",
  };
  const Icon: LucideIcon = meta.icon;

  return (
    <Badge variant="outline" className={cn("gap-1", meta.variant)}>
      <Icon className="size-3.5" aria-hidden />
      {meta.label}
    </Badge>
  );
}
