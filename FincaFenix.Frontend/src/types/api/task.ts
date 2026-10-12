export interface TaskDTO {
  id: number;
  description: string;
  rendimientoMode?: string | null;
  isDeleted?: boolean;
}

export interface SaveTaskDTO {
  id?: number | null;
  description: string;
  rendimientoMode: string;
}
