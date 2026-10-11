import { api } from "../../../lib/api.client";
import type { MaterialCategoryDTO, MaterialRecipeDTO } from "../../../types/api";

export async function getMaterials(): Promise<MaterialRecipeDTO[]> {
  const res = await api.get<MaterialRecipeDTO[]>("/api/material/getmateriallist");
  return res.data;
}

export async function getMaterialCategories(): Promise<MaterialCategoryDTO[]> {
  const res = await api.get<MaterialCategoryDTO[]>("/api/materialcategory/getCategories");
  return res.data;
}