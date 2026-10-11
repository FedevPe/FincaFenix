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

export const RENDIMIENTO_MODE_LABELS: Record<string, string> = {
  ManHours: "Horas de trabajo",
  MaterialEfficiency: "Eficiencia de material",
  AreaPerManHour: "Área por hora hombre",
  OutputPerManHour: "Producción por hora hombre",
};

export function rendimientoModeLabel(mode?: string | null): string {
  if (!mode) return "-";
  return RENDIMIENTO_MODE_LABELS[mode] ?? mode;
}
