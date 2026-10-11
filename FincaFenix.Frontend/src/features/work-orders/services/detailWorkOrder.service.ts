import { api } from "../../../lib/api.client";
import type {
  ActivityWorkOrderDTO,
  AddDetailWorkOrderDTO,
} from "../../../types/api";

export async function getActivities(orderId: number): Promise<ActivityWorkOrderDTO[]> {
  const res = await api.get<ActivityWorkOrderDTO[]>(`/api/detailworkorder/order/${orderId}/getDetail`);
  return res.data;
}

export async function addActivity(dto: AddDetailWorkOrderDTO): Promise<boolean> {
  const res = await api.post<boolean>("/api/detailworkorder/addDetailWO", dto);
  return res.data;
}