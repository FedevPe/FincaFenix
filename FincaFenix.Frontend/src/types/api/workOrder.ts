export interface TaskDTO {
  id: number;
  description: string;
  rendimientoMode?: string | null;
}

export interface FarmDTO {
  id: number;
  name: string;
}

export interface DetailSectorFarmDTO {
  id: number;
  farmId: number;
  farm?: FarmDTO | null;
  sectorName: string;
  varietyId?: number | null;
  varietyName?: string | null;
  fruitName?: string | null;
  numberPlants?: number | null;
  area?: number | null;
  selected?: boolean;
}

export interface EmployeeDTO {
  id: number;
  name: string;
  lastName?: string | null;
}

export interface MaterialCategoryDTO {
  id: number;
  description: string;
}

export interface ActivityWorkOrderDTO {
  id: number;
  machinePasses: number;
  workedHours: number;
  producedAmount?: number | null;
  areaWorked?: number | null;
  rendimiento?: number | null;
  rendimientoUnit?: string | null;
  rendimientoMode?: string | null;
  description?: string | null;
  activityDate: string;
  sector?: DetailSectorFarmDTO | null;
  employee?: EmployeeDTO | null;
}

export interface MaterialRecipeDTO {
  id: number;
  articleName?: string | null;
  commercialName?: string | null;
  categoryId?: number | null;
  category?: MaterialCategoryDTO | null;
  brand?: string | null;
  diseasePlague?: string | null;
  codeSAP?: string | null;
  descriptionSAP?: string | null;
  unitOfMeasure?: string | null;
}

export interface DetailRecipeDTO {
  categoryId: number;
  materialId: number;
  material?: MaterialRecipeDTO | null;
  amountRequired: number;
  amountRequiredUnit?: string | null;
  estimatedAmount: number;
  estimatedAmountUnit?: string | null;
  brand?: string | null;
  pestDisease?: string | null;
  totalAmountConsumed: number;
  theoreticalAmount?: number | null;
  rendimiento?: number | null;
}

export interface MachineRecipeDTO {
  id: number;
  name?: string | null;
  capacity?: number | null;
  capacityUnit?: string | null;
  trv?: number | null;
}

export interface RecipeWorkOrderDTO {
  id: number;
  numRecipe?: string | null;
  machineId: number;
  machine?: MachineRecipeDTO | null;
  volumeMachine: number;
  volumeMachineUnit?: string | null;
  trv: number;
  status?: string | null;
  isDeleted?: boolean | null;
  totalAplications: number;
  theoreticalVolume: number;
  realVolume: number;
  theoreticalMachinePasses?: number | null;
  realMachinePasses: number;
  rendimiento?: number | null;
  details: DetailRecipeDTO[];
}

export interface ShowWorkOrderDTO {
  id: number;
  orderNum: string;
  status: string;
  description?: string | null;
  totalAreaWorked: number;
  createdDate?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  task?: TaskDTO | null;
  farm?: FarmDTO | null;
  detailsWorkOrder?: ActivityWorkOrderDTO[] | null;
  sectorList?: DetailSectorFarmDTO[] | null;
  recipe?: RecipeWorkOrderDTO | null;
  totalManHours: number;
  totalProducedAmount?: number | null;
  rendimiento?: number | null;
  rendimientoUnit?: string | null;
  rendimientoMode?: string | null;
  isDeleted?: boolean | null;
}

export interface InfoWorkOrderDTO {
  id: number;
  orderNum: string;
  taskOrder?: TaskDTO | null;
  farmOrder?: FarmDTO | null;
  createdDate?: string | null;
  startDate?: string | null;
  status: string;
  description?: string | null;
  relationatedSector?: DetailSectorFarmDTO[];
}

export interface InfoDetailWorkOrderDTO {
  sectorWorkedId: number;
  machinePasses: number;
  workedHours: number;
  producedAmount?: number | null;
  description: string;
}

export interface AddDetailWorkOrderDTO {
  orderId: number;
  employeeId: number;
  activityDate?: string | null;
  info: InfoDetailWorkOrderDTO;
}

export interface CreateDetailRecipeDTO {
  categoryId: number;
  materialId: number;
  amountRequired: number;
  amountRequiredUnit?: string | null;
  estimatedAmount: number;
  estimatedAmountUnit?: string | null;
  brand?: string | null;
  pestDisease?: string | null;
}

export interface CreateRecipeDTO {
  machineId: number;
  volumeMachine: number;
  volumeMachineUnit: string;
  trv: number;
  status?: string | null;
  details: CreateDetailRecipeDTO[];
}

export interface WorkOrderDTO {
  taskId: number;
  farmId: number;
  description?: string | null;
  sectorList: DetailSectorFarmDTO[];
  createdDate: string;
  startDate?: string | null;
  endDate: string;
  status: string;
  totalArea?: number | null;
  isDeleted: boolean;
  recipe?: CreateRecipeDTO | null;
}
