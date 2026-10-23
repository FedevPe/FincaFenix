import type { ReactNode } from "react";
import type { LucideIcon } from "lucide-react";
import { AlertCircle, Inbox, Loader2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "./Button";
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert";
import { Empty, EmptyHeader, EmptyTitle, EmptyDescription, EmptyContent, EmptyMedia } from "@/components/ui/empty";
import { Skeleton } from "@/components/ui/skeleton";

export function LoadingState({
  label = "Cargando...",
  variant = "overlay",
}: { label?: string; variant?: "overlay" | "table" | "card" }) {
  if (variant === "overlay") {
    return (
      <div
        className={cn("flex flex-col items-center justify-center gap-3 py-12 text-muted-foreground", variant === "overlay" && "fixed inset-0 z-50 bg-background/80")}
        role="status"
        aria-live="polite"
      >
        <Loader2 className="size-6 animate-spin" aria-hidden />
        <p className="text-sm">{label}</p>
      </div>
    );
  }

  if (variant === "table") {
    return (
      <div className="space-y-3" role="status" aria-live="polite" aria-label={label}>
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
      </div>
    );
  }

  return (
    <div className="space-y-4 p-6" role="status" aria-live="polite" aria-label={label}>
      <Skeleton className="h-6 w-3/4" />
      <Skeleton className="h-4 w-1/2" />
      <Skeleton className="h-4 w-1/3" />
      <Skeleton className="h-4 w-full" />
      <Skeleton className="h-4 w-full" />
    </div>
  );
}

interface EmptyStateProps {
  title: string;
  description?: string;
  icon?: LucideIcon;
  action?: ReactNode;
}

export function EmptyState({ title, description, icon: Icon = Inbox, action }: EmptyStateProps) {
  return (
    <Empty>
      <EmptyHeader>
        <EmptyMedia variant="icon">
          <Icon className="size-6" aria-hidden />
        </EmptyMedia>
        <EmptyTitle>{title}</EmptyTitle>
        {description ? <EmptyDescription>{description}</EmptyDescription> : null}
      </EmptyHeader>
      {action ? <EmptyContent>{action}</EmptyContent> : null}
    </Empty>
  );
}

interface ErrorStateProps {
  message?: ReactNode;
  onRetry?: () => void;
  title?: string;
}

export function ErrorState({ message, onRetry, title = "Ocurrió un error" }: ErrorStateProps) {
  return (
    <Alert variant="destructive" role="alert">
      <AlertCircle className="size-4" aria-hidden />
      <AlertTitle>{title}</AlertTitle>
      <AlertDescription>{message ?? "No se pudo cargar la información"}</AlertDescription>
      {onRetry ? (
        <Button variant="secondary" size="sm" onClick={onRetry} className="mt-2">
          Reintentar
        </Button>
      ) : null}
    </Alert>
  );
}