import { api } from "../../../lib/api.client";
import type {
  FruitDTO,
  FruitVarietyDTO,
  PagedResult,
  SaveFruitDTO,
  SaveFruitVarietyDTO,
} from "../../../types/api";

export interface FruitFilterParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
}

export interface FruitVarietyFilterParams extends FruitFilterParams {
  fruitId?: number;
}

export async function getFruits(): Promise<FruitDTO[]> {
  const res = await api.get<FruitDTO[]>("/api/fruit/getFruits");
  return res.data;
}

export async function getFruitsPaged(
  params: FruitFilterParams,
): Promise<PagedResult<FruitDTO>> {
  const res = await api.get<PagedResult<FruitDTO>>("/api/fruit/paged", { params });
  return res.data;
}

export async function createFruit(dto: SaveFruitDTO): Promise<FruitDTO> {
  const res = await api.post<FruitDTO>("/api/fruit", dto);
  return res.data;
}

export async function updateFruit(dto: SaveFruitDTO): Promise<FruitDTO> {
  const res = await api.put<FruitDTO>("/api/fruit", dto);
  return res.data;
}

export async function deleteFruit(id: number): Promise<void> {
  await api.delete(`/api/fruit/${id}`);
}

export async function getFruitVarieties(
  fruitId?: number,
): Promise<FruitVarietyDTO[]> {
  const res = await api.get<FruitVarietyDTO[]>("/api/fruitvariety/getVarieties", {
    params: fruitId ? { fruitId } : undefined,
  });
  return res.data;
}

export async function getFruitVarietiesPaged(
  params: FruitVarietyFilterParams,
): Promise<PagedResult<FruitVarietyDTO>> {
  const res = await api.get<PagedResult<FruitVarietyDTO>>(
    "/api/fruitvariety/paged",
    { params },
  );
  return res.data;
}

export async function createFruitVariety(
  dto: SaveFruitVarietyDTO,
): Promise<FruitVarietyDTO> {
  const res = await api.post<FruitVarietyDTO>("/api/fruitvariety", dto);
  return res.data;
}

export async function updateFruitVariety(
  dto: SaveFruitVarietyDTO,
): Promise<FruitVarietyDTO> {
  const res = await api.put<FruitVarietyDTO>("/api/fruitvariety", dto);
  return res.data;
}

export async function deleteFruitVariety(id: number): Promise<void> {
  await api.delete(`/api/fruitvariety/${id}`);
}
