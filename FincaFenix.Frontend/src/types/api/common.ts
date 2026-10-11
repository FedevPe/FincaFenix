export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ValidationDTO {
  propertyName: string;
  errorMessage: string;
}

export interface OperationResultDTO {
  success: boolean;
  errors: ValidationDTO[];
}
