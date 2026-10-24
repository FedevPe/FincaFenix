import { api } from "../../../lib/api.client";
import type {
  FarmDTO,
  DetailSectorFarmDTO,
  MachineRecipeDTO,
  OperationResultDTO,
  PagedResult,
  ShowWorkOrderDTO,
  TaskDTO,
  WorkOrderCostDTO,
  WorkOrderDTO,
} from "../../../types/api";

export async function getWorkOrders(): Promise<ShowWorkOrderDTO[]> {
  const res = await api.get<ShowWorkOrderDTO[]>("/api/workorder/getallworkorderinfo");
  return res.data;
}

export interface GetWorkOrdersPaginatedParams {
  pageNumber: number;
  pageSize: number;
  status?: string;
  farmId?: number;
  taskId?: number;
  dateFrom?: string;
  dateTo?: string;
}

export async function getWorkOrdersPaginated(
  params: GetWorkOrdersPaginatedParams
): Promise<PagedResult<ShowWorkOrderDTO>> {
  const searchParams = new URLSearchParams();
  searchParams.set("pageNumber", String(params.pageNumber));
  searchParams.set("pageSize", String(params.pageSize));
  if (params.status) searchParams.set("status", params.status);
  if (params.farmId) searchParams.set("farmId", String(params.farmId));
  if (params.taskId) searchParams.set("taskId", String(params.taskId));
  if (params.dateFrom) searchParams.set("dateFrom", params.dateFrom);
  if (params.dateTo) searchParams.set("dateTo", params.dateTo);

  const res = await api.get<PagedResult<ShowWorkOrderDTO>>(
    `/api/workorder/${params.pageNumber}/${params.pageSize}/getworkorderlistpaginated`,
    { params: searchParams }
  );
  return res.data;
}

export async function getWorkOrderById(id: number): Promise<ShowWorkOrderDTO> {
  const res = await api.get<ShowWorkOrderDTO>(`/api/workorder/getCompleteInfoWorkOrder/${id}`);
  return res.data;
}

export async function getWorkOrderCosts(id: number): Promise<WorkOrderCostDTO> {
  const res = await api.get<WorkOrderCostDTO>(`/api/inventory/costs/workorder/${id}`);
  return res.data;
}

export async function getFarms(): Promise<FarmDTO[]> {
  const res = await api.get<FarmDTO[]>("/api/farm/getListFarm");
  return res.data;
}

export async function getTasks(includeDeleted = false): Promise<TaskDTO[]> {
  const res = await api.get<TaskDTO[]>("/api/task/GetTaskList", {
    params: { includeDeleted },
  });
  return res.data;
}

export async function createWorkOrder(dto: WorkOrderDTO): Promise<OperationResultDTO> {
  const res = await api.post<OperationResultDTO>("/api/workorder/createworkorder", dto);
  return res.data;
}

export async function getMachines(): Promise<MachineRecipeDTO[]> {
  const res = await api.get<MachineRecipeDTO[]>("/api/machine/getmachinelist");
  return res.data;
}

export async function getSectorsByFarm(farmId: number): Promise<DetailSectorFarmDTO[]> {
  const res = await api.get<DetailSectorFarmDTO[]>(`/api/detailsector/farm/${farmId}/sectors`);
  return res.data;
}

export async function updateWorkOrderState(
  workOrderId: number,
  newStatus: string
): Promise<boolean> {
  const res = await api.post<boolean>("/api/workorder/updatestateworkorder", null, {
    params: { workOrderId, newStatus },
  });
  return res.data;
}

export async function downloadWorkOrderPdf(id: number, orderNum: string): Promise<void> {
  const res = await api.get<Blob>(`/api/workorder/${id}/pdf`, {
    responseType: "blob",
  });

  const filename = extractFilename(res.headers["content-disposition"]) ??
    `Orden_de_trabajo_${orderNum}.pdf`;

  const url = URL.createObjectURL(res.data);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

function extractFilename(contentDisposition?: string): string | null {
  if (!contentDisposition) return null;
  const utf8Match = /filename\*=UTF-8''([^;]+)/i.exec(contentDisposition);
  if (utf8Match) return decodeURIComponent(utf8Match[1].replace(/"/g, ""));
  const match = /filename="?([^";]+)"?/i.exec(contentDisposition);
  return match ? match[1] : null;
}
