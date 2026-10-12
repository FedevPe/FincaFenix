export type WorkOrderStatus = "Pendiente" | "Activo" | "Cerrado" | "Cancelado";

export const WORK_ORDER_STATUSES: WorkOrderStatus[] = [
  "Pendiente",
  "Activo",
  "Cerrado",
  "Cancelado",
];

export const ALLOWED_TRANSITIONS: Record<WorkOrderStatus, WorkOrderStatus[]> = {
  Pendiente: ["Activo", "Cerrado", "Cancelado"],
  Activo: ["Cerrado", "Cancelado"],
  Cerrado: [],
  Cancelado: [],
};

export function isWorkOrderStatus(value: string): value is WorkOrderStatus {
  return (WORK_ORDER_STATUSES as string[]).includes(value);
}

export function allowedTransitions(status: string): WorkOrderStatus[] {
  return isWorkOrderStatus(status) ? ALLOWED_TRANSITIONS[status] : [];
}

export {
  RENDIMIENTO_MODE_LABELS,
  rendimientoModeLabel,
} from "@/features/tasks/constants";
