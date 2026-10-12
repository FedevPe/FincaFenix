import { api } from "../../../lib/api.client";
import type {
  MaterialCategoryDTO,
  MaterialDTO,
  MaterialRecipeDTO,
  PagedResult,
  SaveMaterialCategoryDTO,
  SaveMaterialDTO,
  SaveUnitOfMeasureDTO,
  UnitOfMeasureDTO,
} from "../../../types/api";

export interface MaterialFilterParams {
  pageNumber: number;
  pageSize: number;
  categoryId?: number;
  search?: string;
  includeDeleted?: boolean;
}

export async function getMaterials(): Promise<MaterialRecipeDTO[]> {
  const res = await api.get<MaterialRecipeDTO[]>("/api/material/getmateriallist");
  return res.data;
}

export async function getMaterialsPaged(
  params: MaterialFilterParams,
): Promise<PagedResult<MaterialDTO>> {
  const res = await api.get<PagedResult<MaterialDTO>>("/api/material/paged", {
    params,
  });
  return res.data;
}

export async function getMaterialById(id: number): Promise<MaterialDTO> {
  const res = await api.get<MaterialDTO>(`/api/material/${id}`);
  return res.data;
}

export async function createMaterial(dto: SaveMaterialDTO): Promise<MaterialDTO> {
  const res = await api.post<MaterialDTO>("/api/material", dto);
  return res.data;
}

export async function updateMaterial(dto: SaveMaterialDTO): Promise<MaterialDTO> {
  const res = await api.put<MaterialDTO>("/api/material", dto);
  return res.data;
}

export async function deleteMaterial(id: number): Promise<void> {
  await api.delete(`/api/material/${id}`);
}

export async function reactivateMaterial(id: number): Promise<MaterialDTO> {
  const res = await api.put<MaterialDTO>(`/api/material/ReactivateMaterial/${id}`);
  return res.data;
}

export async function getMaterialCategories(): Promise<MaterialCategoryDTO[]> {
  const res = await api.get<MaterialCategoryDTO[]>("/api/materialcategory/getCategories");
  return res.data;
}

export async function createMaterialCategory(
  dto: SaveMaterialCategoryDTO,
): Promise<MaterialCategoryDTO> {
  const res = await api.post<MaterialCategoryDTO>("/api/materialcategory", dto);
  return res.data;
}

export async function updateMaterialCategory(
  dto: SaveMaterialCategoryDTO,
): Promise<MaterialCategoryDTO> {
  const res = await api.put<MaterialCategoryDTO>("/api/materialcategory", dto);
  return res.data;
}

export async function deleteMaterialCategory(id: number): Promise<void> {
  await api.delete(`/api/materialcategory/${id}`);
}

export async function getUnitsOfMeasure(
  includeDeleted = false,
): Promise<UnitOfMeasureDTO[]> {
  const res = await api.get<UnitOfMeasureDTO[]>("/api/unitofmeasure", {
    params: { includeDeleted },
  });
  return res.data;
}

export async function createUnitOfMeasure(
  dto: SaveUnitOfMeasureDTO,
): Promise<UnitOfMeasureDTO> {
  const res = await api.post<UnitOfMeasureDTO>("/api/unitofmeasure", dto);
  return res.data;
}

export async function updateUnitOfMeasure(
  dto: SaveUnitOfMeasureDTO,
): Promise<UnitOfMeasureDTO> {
  const res = await api.put<UnitOfMeasureDTO>("/api/unitofmeasure", dto);
  return res.data;
}

export async function deleteUnitOfMeasure(id: number): Promise<void> {
  await api.delete(`/api/unitofmeasure/${id}`);
}

export async function reactivateUnitOfMeasure(id: number): Promise<UnitOfMeasureDTO> {
  const res = await api.put<UnitOfMeasureDTO>(
    `/api/unitofmeasure/ReactivateUnitOfMeasure/${id}`,
  );
  return res.data;
}
