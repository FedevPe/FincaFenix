import type { MaterialCategoryDTO } from "./workOrder";

export interface SaveFarmDTO {
  id?: number | null;
  name: string;
}

export interface SaveEmployeeDTO {
  id?: number | null;
  name: string;
  lastName: string;
  farmIds: number[];
}

export interface MaterialDTO {
  id: number;
  codeSap?: string | null;
  descriptionSap?: string | null;
  articleName: string;
  commercialName: string;
  categoryId: number;
  category?: MaterialCategoryDTO | null;
  brand?: string | null;
  description?: string | null;
  unitOfMeasureId?: number | null;
  unitOfMeasure?: string | null;
  referenceCost?: number | null;
  currencyId: number;
  currencyCode?: string | null;
  isDeleted: boolean;
}

export interface SaveMaterialDTO {
  id?: number | null;
  codeSap?: string | null;
  descriptionSap?: string | null;
  articleName: string;
  commercialName: string;
  categoryId: number;
  brand?: string | null;
  description?: string | null;
  unitOfMeasureId: number;
  referenceCost?: number | null;
  currencyId: number;
}

export interface UnitOfMeasureDTO {
  id: number;
  description: string;
  code?: string | null;
  isDeleted: boolean;
}

export interface SaveUnitOfMeasureDTO {
  id?: number | null;
  description: string;
}

export interface SaveMaterialCategoryDTO {
  id?: number | null;
  description: string;
}

export interface FruitDTO {
  id: number;
  description: string;
  varietyIds: number[];
  varietyNames: string[];
}

export interface SaveFruitDTO {
  id?: number | null;
  description: string;
  varietyIds: number[];
}

export interface FruitVarietyDTO {
  id: number;
  description: string;
  fruitId?: number | null;
  fruitName?: string | null;
  isOrganic: boolean;
}

export interface SaveFruitVarietyDTO {
  id?: number | null;
  description: string;
  isOrganic: boolean;
}

export interface UserDTO {
  id: string;
  userName: string;
  email?: string | null;
  name?: string | null;
  lastName?: string | null;
  isActive: boolean;
  roles: string[];
}

export interface CreateUserDTO {
  userName: string;
  email?: string | null;
  name?: string | null;
  lastName?: string | null;
  password: string;
  roleName: string;
}

export interface UpdateUserDTO {
  id: string;
  email?: string | null;
  name?: string | null;
  lastName?: string | null;
  roleName: string;
}
