export interface WorkOrderCostItemDTO {
  id: number;
  materialId: number;
  materialName: string;
  unitOfMeasure: string;
  plannedAmount: number;
  unitCost: number;
  totalCost: number;
  consumedAmount: number;
  realCost: number;
  currencyCode: string;
  currencySymbol: string;
  frozenDate: string;
}

export interface WorkOrderCostDTO {
  workOrderId: number;
  orderNum: string;
  totalCost: number;
  totalRealCost: number;
  items: WorkOrderCostItemDTO[];
}

export interface StockDTO {
  materialId: number;
  articleName: string;
  commercialName: string;
  unitOfMeasure: string;
  farmId: number;
  farmName: string;
  stockFisico: number;
  stockReservado: number;
  stockDisponible: number;
  stockMinimo: number;
  isLow: boolean;
}

export interface CurrencyDTO {
  id: number;
  code: string;
  name: string;
  symbol: string;
}

export interface RegisterMovementDTO {
  materialId: number;
  farmId: number;
  movementType: string;
  amount: number;
  unitCost?: number | null;
  currencyId: number;
  stockMinimum?: number | null;
  observations?: string | null;
  userId?: number | null;
}

export interface MovementResultDTO {
  movementId: number;
  previousStock: number;
  resultingStock: number;
}

export interface CurrentMaterialCostDTO {
  materialId: number;
  articleName: string;
  commercialName: string;
  unitOfMeasure: string;
  categoryId: number;
  categoryName?: string | null;
  referenceCost?: number | null;
  currencyId: number;
  currencyCode: string;
  currencySymbol: string;
}

export interface ReservationItemDTO {
  reservationId: number;
  workOrderId: number;
  orderNum: string;
  workOrderStatus: string;
  farmId: number;
  farmName: string;
  reservedAmount: number;
  consumedAmount: number;
  pendingAmount: number;
  state: string;
  createdDate: string;
  releasedDate?: string | null;
}

export interface MaterialReservationsDTO {
  materialId: number;
  articleName: string;
  commercialName: string;
  unitOfMeasure: string;
  items: ReservationItemDTO[];
}

export interface CostHistoryItemDTO {
  movementId: number;
  movementType: string;
  origin: string;
  amount: number;
  unitCost?: number | null;
  totalCost?: number | null;
  farmId: number;
  farmName: string;
  date: string;
}

export interface CostHistoryDTO {
  materialId: number;
  articleName: string;
  commercialName: string;
  unitOfMeasure: string;
  referenceCost?: number | null;
  currencyCode: string;
  currencySymbol: string;
  items: CostHistoryItemDTO[];
}

export interface InventoryMovementDTO {
  id: number;
  materialId: number;
  articleName: string;
  commercialName: string;
  unitOfMeasure: string;
  farmId: number;
  farmName: string;
  movementType: string;
  origin: string;
  amount: number;
  unitCost?: number | null;
  totalCost?: number | null;
  currencyCode: string;
  currencySymbol: string;
  resultingStock: number;
  date: string;
  observations?: string | null;
  workOrderId?: number | null;
  orderNum?: string | null;
}

