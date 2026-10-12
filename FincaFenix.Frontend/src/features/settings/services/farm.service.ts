import { api } from "../../../lib/api.client";
import type { FarmDTO, PagedResult, SaveFarmDTO } from "../../../types/api";

export interface FarmFilterParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  includeDeleted?: boolean;
}

export async function getFarmsPaged(
  params: FarmFilterParams,
): Promise<PagedResult<FarmDTO>> {
  const res = await api.get<PagedResult<FarmDTO>>("/api/farm/paged", { params });
  return res.data;
}

export async function createFarm(dto: SaveFarmDTO): Promise<FarmDTO> {
  const res = await api.post<FarmDTO>("/api/farm", dto);
  return res.data;
}

export async function updateFarm(dto: SaveFarmDTO): Promise<FarmDTO> {
  const res = await api.put<FarmDTO>("/api/farm", dto);
  return res.data;
}

export async function deleteFarm(id: number): Promise<void> {
  await api.delete(`/api/farm/${id}`);
}

export async function reactivateFarm(id: number): Promise<FarmDTO> {
  const res = await api.put<FarmDTO>(`/api/farm/ReactivateFarm/${id}`);
  return res.data;
}
