import { api } from "../../../lib/api.client";
import type { EmployeeDTO, PagedResult, SaveEmployeeDTO } from "../../../types/api";

export interface EmployeeFilterParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  includeDeleted?: boolean;
  farmId?: number;
}

export async function getEmployeesPaged(
  params: EmployeeFilterParams,
): Promise<PagedResult<EmployeeDTO>> {
  const res = await api.get<PagedResult<EmployeeDTO>>("/api/employee/paged", {
    params,
  });
  return res.data;
}

export async function createEmployee(dto: SaveEmployeeDTO): Promise<EmployeeDTO> {
  const res = await api.post<EmployeeDTO>("/api/employee", dto);
  return res.data;
}

export async function updateEmployee(dto: SaveEmployeeDTO): Promise<EmployeeDTO> {
  const res = await api.put<EmployeeDTO>("/api/employee", dto);
  return res.data;
}

export async function deleteEmployee(id: number): Promise<void> {
  await api.delete(`/api/employee/${id}`);
}

export async function reactivateEmployee(id: number): Promise<EmployeeDTO> {
  const res = await api.put<EmployeeDTO>(`/api/employee/ReactivateEmployee/${id}`);
  return res.data;
}
