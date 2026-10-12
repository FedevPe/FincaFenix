import type { LucideIcon } from "lucide-react";
import {
  Apple,
  ClipboardList,
  Leaf,
  Package,
  Ruler,
  Tags,
  UserCog,
  Users,
  Warehouse,
} from "lucide-react";

export const SETTINGS_READ_POLICIES = [
  "FARM_READ",
  "MATERIAL_READ",
  "TASK_READ",
  "EMPLOYEE_READ",
  "MATERIAL_CATEGORY_READ",
  "UNIT_OF_MEASURE_READ",
  "FRUIT_READ",
  "FRUIT_VARIETY_READ",
  "USER_READ",
] as const;

export type SettingsTab =
  | "fincas"
  | "materiales"
  | "tareas"
  | "empleados"
  | "categorias"
  | "unidades"
  | "frutas"
  | "variedades"
  | "usuarios";

export interface SettingsTabDef {
  id: SettingsTab;
  label: string;
  icon: LucideIcon;
  policy: string;
}

export const SETTINGS_TABS: SettingsTabDef[] = [
  { id: "fincas", label: "Fincas", icon: Warehouse, policy: "FARM_READ" },
  { id: "materiales", label: "Materiales", icon: Package, policy: "MATERIAL_READ" },
  { id: "tareas", label: "Tareas", icon: ClipboardList, policy: "TASK_READ" },
  { id: "empleados", label: "Empleados", icon: Users, policy: "EMPLOYEE_READ" },
  {
    id: "categorias",
    label: "Categorías",
    icon: Tags,
    policy: "MATERIAL_CATEGORY_READ",
  },
  {
    id: "unidades",
    label: "Unidades",
    icon: Ruler,
    policy: "UNIT_OF_MEASURE_READ",
  },
  { id: "frutas", label: "Frutas", icon: Apple, policy: "FRUIT_READ" },
  {
    id: "variedades",
    label: "Variedades",
    icon: Leaf,
    policy: "FRUIT_VARIETY_READ",
  },
  { id: "usuarios", label: "Usuarios", icon: UserCog, policy: "USER_READ" },
];
