import type { MaterialRecipeDTO } from "@/types/api";

export type UnitFamily =
  | "volume"
  | "mass"
  | "count"
  | "length"
  | "package"
  | "unknown";

const UNIT_TOKENS: Array<[string, string]> = [
  ["kg", "kg"],
  ["kgs", "kg"],
  ["kilo", "kg"],
  ["kilos", "kg"],
  ["kilogramo", "kg"],
  ["kilogramos", "kg"],
  ["gr", "gr"],
  ["grs", "gr"],
  ["gramo", "gr"],
  ["gramos", "gr"],
  ["lts", "lts"],
  ["lt", "lts"],
  ["l", "lts"],
  ["litro", "lts"],
  ["litros", "lts"],
  ["cc", "cc"],
  ["ml", "cc"],
  ["cm3", "cc"],
  ["centimetrocubico", "cc"],
  ["unidad", "unidad"],
  ["unidades", "unidad"],
  ["un", "unidad"],
  ["u", "unidad"],
  ["uni", "unidad"],
  ["metro", "metro"],
  ["metros", "metro"],
  ["m", "metro"],
  ["bolsa", "bolsa"],
  ["bolsas", "bolsa"],
  ["caja", "caja"],
  ["cajas", "caja"],
];

export function unitCodeFor(unit?: string | null): string {
  const normalized = (unit ?? "")
    .trim()
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/\s+/g, "");
  const found = UNIT_TOKENS.find(([token]) => token === normalized);
  return found ? found[1] : unit?.trim() || "";
}

export function unitFamilyOf(unit?: string | null): UnitFamily {
  switch (unitCodeFor(unit)) {
    case "lts":
    case "cc":
      return "volume";
    case "kg":
    case "gr":
      return "mass";
    case "unidad":
      return "count";
    case "metro":
      return "length";
    case "bolsa":
    case "caja":
      return "package";
    default:
      return "unknown";
  }
}

/**
 * Factor de cada unidad normalizada respecto de la unidad base de su familia.
 * Refleja las mismas reglas que `UnitConverter` del backend.
 */
const UNIT_FACTOR_TO_BASE: Record<string, number> = {
  kg: 1,
  gr: 0.001,
  lts: 1,
  cc: 0.001,
  unidad: 1,
  metro: 1,
  bolsa: 1,
  caja: 1,
};

/**
 * Convierte una cantidad entre unidades de la misma familia.
 * Si alguna unidad es desconocida o las familias son incompatibles,
 * devuelve el valor sin modificar para no romper la UI.
 */
export function convertAmount(
  value: number,
  fromUnit?: string | null,
  toUnit?: string | null,
): number {
  if (!Number.isFinite(value)) return value;

  const fromCode = unitCodeFor(fromUnit);
  const toCode = unitCodeFor(toUnit);
  if (fromCode === toCode) return value;

  const fromFactor = UNIT_FACTOR_TO_BASE[fromCode];
  const toFactor = UNIT_FACTOR_TO_BASE[toCode];
  if (fromFactor == null || toFactor == null) return value;
  if (unitFamilyOf(fromCode) !== unitFamilyOf(toCode)) return value;

  return (value * fromFactor) / toFactor;
}

export function materialLabel(material: MaterialRecipeDTO): string {
  const name = material.articleName ?? `Material #${material.id}`;
  const commercial = material.commercialName
    ? ` (${material.commercialName})`
    : "";
  const unit = material.unitOfMeasure ? ` · ${material.unitOfMeasure}` : "";
  return `${name}${commercial}${unit}`;
}
