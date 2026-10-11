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

export function materialLabel(material: MaterialRecipeDTO): string {
  const name = material.articleName ?? `Material #${material.id}`;
  const commercial = material.commercialName
    ? ` (${material.commercialName})`
    : "";
  const unit = material.unitOfMeasure ? ` · ${material.unitOfMeasure}` : "";
  return `${name}${commercial}${unit}`;
}
