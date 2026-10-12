import { api } from "../../../lib/api.client";
import type { PagedResult, SaveTaskDTO, TaskDTO } from "../../../types/api";

export interface TaskFilterParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  includeDeleted?: boolean;
}

export async function getTasksPaged(
  params: TaskFilterParams,
): Promise<PagedResult<TaskDTO>> {
  const res = await api.get<PagedResult<TaskDTO>>("/api/task/paged", { params });
  return res.data;
}

export async function createTask(dto: SaveTaskDTO): Promise<TaskDTO> {
  const res = await api.post<TaskDTO>("/api/task", dto);
  return res.data;
}

export async function updateTask(dto: SaveTaskDTO): Promise<TaskDTO> {
  const res = await api.put<TaskDTO>("/api/task", dto);
  return res.data;
}

export async function deleteTask(id: number): Promise<void> {
  await api.delete(`/api/task/${id}`);
}

export async function reactivateTask(id: number): Promise<TaskDTO> {
  const res = await api.put<TaskDTO>(`/api/task/ReactivateTask/${id}`);
  return res.data;
}