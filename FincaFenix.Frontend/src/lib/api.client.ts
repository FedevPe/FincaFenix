import axios from 'axios';
import type { AxiosError, InternalAxiosRequestConfig } from 'axios';

export const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";

export const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
});

api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  return config;
});

export interface ProblemDetails {
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  errors?: Array<{ propertyName: string; errorMessage: string }>;
  [key: string]: unknown;
}

export function getProblemDetails(error: unknown): ProblemDetails | null {
  const axiosError = error as AxiosError<ProblemDetails> | undefined;
  if (axiosError?.response?.data) {
    return axiosError.response.data;
  }
  return null;
}