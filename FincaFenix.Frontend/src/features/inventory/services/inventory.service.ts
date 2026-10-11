import { api } from "../../../lib/api.client";
import type {
  CostHistoryDTO,
  CurrencyDTO,
  CurrentMaterialCostDTO,
  InventoryMovementDTO,
  MaterialReservationsDTO,
  MovementResultDTO,
  PagedResult,
  RegisterMovementDTO,
  StockDTO,
} from "../../../types/api";

export interface CostFilterParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  categoryId?: number;
}

export async function getLowStock(): Promise<StockDTO[]> {
  const res = await api.get<StockDTO[]>("/api/inventory/stock/low");
  return res.data;
}

export async function getConsolidatedStock(): Promise<StockDTO[]> {
  const res = await api.get<StockDTO[]>("/api/inventory/stock/consolidated");
  return res.data;
}

export async function getStockByFarm(farmId: number): Promise<StockDTO[]> {
  const res = await api.get<StockDTO[]>(`/api/inventory/stock/farm/${farmId}`);
  return res.data;
}

export async function getZeroStock(): Promise<StockDTO[]> {
  const res = await api.get<StockDTO[]>("/api/inventory/stock/zero");
  return res.data;
}

export async function getRecentMovements(
  take = 10,
): Promise<InventoryMovementDTO[]> {
  const res = await api.get<InventoryMovementDTO[]>(
    "/api/inventory/movements/recent",
    { params: { take } },
  );
  return res.data;
}

export async function getCurrencies(): Promise<CurrencyDTO[]> {
  const res = await api.get<CurrencyDTO[]>("/api/inventory/currencies");
  return res.data;
}

export async function getCurrentCosts(): Promise<CurrentMaterialCostDTO[]> {
  const res = await api.get<CurrentMaterialCostDTO[]>(
    "/api/inventory/costs/current",
  );
  return res.data;
}

export async function getCurrentCostsPaged(
  params: CostFilterParams,
): Promise<PagedResult<CurrentMaterialCostDTO>> {
  const res = await api.get<PagedResult<CurrentMaterialCostDTO>>(
    "/api/inventory/costs/current/paged",
    { params },
  );
  return res.data;
}

export async function getMaterialReservations(
  materialId: number,
): Promise<MaterialReservationsDTO> {
  const res = await api.get<MaterialReservationsDTO>(
    `/api/inventory/reservations/material/${materialId}`,
  );
  return res.data;
}

export async function getMaterialCostHistory(
  materialId: number,
): Promise<CostHistoryDTO> {
  const res = await api.get<CostHistoryDTO>(
    `/api/inventory/costs/material/${materialId}/history`,
  );
  return res.data;
}

export async function registerMovement(
  dto: RegisterMovementDTO,
): Promise<MovementResultDTO> {
  const res = await api.post<MovementResultDTO>("/api/inventory/movement", dto);
  return res.data;
}
