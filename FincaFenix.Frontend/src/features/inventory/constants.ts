export const INVENTORY_MOVEMENT_TYPES = [
  "Ingreso",
  "Ajuste Positivo",
  "Ajuste Negativo",
] as const;

export type InventoryMovementType = (typeof INVENTORY_MOVEMENT_TYPES)[number];

const MOVEMENT_TYPE_LABELS: Record<string, string> = {
  Ingreso: "Ingreso",
  AjustePositivo: "Ajuste positivo",
  AjusteNegativo: "Ajuste negativo",
  SalidaConsumo: "Salida por consumo",
  SalidaManual: "Salida manual",
};

export function movementTypeLabel(type: string): string {
  return MOVEMENT_TYPE_LABELS[type] ?? type;
}

export const MOVEMENT_ORIGIN_LABELS: Record<string, string> = {
  OrdenTrabajo: "Orden de trabajo",
  Manual: "Manual",
  Ajuste: "Ajuste",
  Inventario: "Inventario",
};

export function movementOriginLabel(origin: string): string {
  return MOVEMENT_ORIGIN_LABELS[origin] ?? origin;
}
