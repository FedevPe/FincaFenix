export const RENDIMIENTO_MODES = [
  "ManHours",
  "MaterialEfficiency",
  "AreaPerManHour",
  "OutputPerManHour",
] as const;

export type RendimientoMode = (typeof RENDIMIENTO_MODES)[number];

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