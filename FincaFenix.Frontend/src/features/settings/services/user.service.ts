import { api } from "../../../lib/api.client";
import type {
  CreateUserDTO,
  PagedResult,
  UpdateUserDTO,
  UserDTO,
} from "../../../types/api";

export interface UserFilterParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  includeInactive?: boolean;
}

export async function getUsersPaged(
  params: UserFilterParams,
): Promise<PagedResult<UserDTO>> {
  const res = await api.get<PagedResult<UserDTO>>("/api/user/paged", { params });
  return res.data;
}

export async function getRoles(): Promise<string[]> {
  const res = await api.get<string[]>("/api/user/roles");
  return res.data;
}

export async function createUser(dto: CreateUserDTO): Promise<UserDTO> {
  const res = await api.post<UserDTO>("/api/user", dto);
  return res.data;
}

export async function updateUser(dto: UpdateUserDTO): Promise<UserDTO> {
  const res = await api.put<UserDTO>("/api/user", dto);
  return res.data;
}

export async function deactivateUser(id: string): Promise<void> {
  await api.put(`/api/user/DeactivateUser/${id}`);
}

export async function reactivateUser(id: string): Promise<UserDTO> {
  const res = await api.put<UserDTO>(`/api/user/ReactivateUser/${id}`);
  return res.data;
}
