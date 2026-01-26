import axios from "axios";

/* ================= DTO ================= */

export interface AssetModelListDto {
  modelId: number;

  bigTypeNameVN: string;
  middleTypeNameVN: string;
  smallTypeNameVN: string;

  assetFullName: string;
  modelName: string;

  imageData?: string; // base64 từ backend

  machineType: string;
  inventoryCheckType: string;

  createdAt: string;
  createdBy: string;

  updatedAt?: string;
  updatedBy?: string;
}

export interface AssetModelSearchDto {
  bigTypeId?: number;
  middleTypeId?: number;
  smallTypeId?: number;
  modelName?: string;

  page: number;
  pageSize: number;
}

export interface CreateAssetModelDto {
  bigTypeId: number;
  middleTypeId: number;
  smallTypeId: number;
  modelName: string;
  imageData?: string | null;
}

export interface UpdateAssetModelDto {
  bigTypeId: number;
  middleTypeId: number;
  smallTypeId: number;
  modelName: string;
  imageData?: string | null;
  updateImage: boolean;
}

export interface PagedResult<T> {
  total: number;
  items: T[];
}

/* ================= API ================= */

export async function searchAssetModels(dto: AssetModelSearchDto) {
  const res = await axios.get("/api/asset-models/search", {
    params: dto,
  });
  return res.data; // { total, items }
}


export async function createAssetModel(
  dto: CreateAssetModelDto,
  username: string
) {
  const res = await axios.post("/api/asset-models", dto, {
    params: { username },
  });
  return res.data;
}

export async function updateAssetModel(
  id: number,
  dto: UpdateAssetModelDto,
  username: string
) {
  await axios.put(`/api/asset-models/${id}`, dto, {
    params: { username },
  });
}
