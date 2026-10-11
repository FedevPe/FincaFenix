import { api } from "../../../lib/api.client";
import type { EmployeeDTO } from "../../../types/api";

export async function getEmployees(farmId: number): Promise<EmployeeDTO[]> {
  const res = await api.get<EmployeeDTO[]>(`/api/employee/${farmId}/employees`);
  return res.data;
}