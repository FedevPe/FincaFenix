import { api } from '../../../lib/api.client';
import type { LoginDTO, CurrentUserDTO } from '../../../types/api';

export async function login(dto: LoginDTO): Promise<CurrentUserDTO> {
  const res = await api.post<CurrentUserDTO>('/api/auth/login', dto);
  return res.data;
}

export async function me(): Promise<CurrentUserDTO> {
  const res = await api.get<CurrentUserDTO>('/api/auth/me');
  return res.data;
}

export async function logout(): Promise<void> {
  await api.post('/api/auth/logout');
}
