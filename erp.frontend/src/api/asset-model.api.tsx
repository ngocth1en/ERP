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

  model3DUrl?: string | null; // URL file .glb cho CAD/3D Visualization

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
  model3DUrl?: string | null;
}

export interface UpdateAssetModelDto {
  bigTypeId: number;
  middleTypeId: number;
  smallTypeId: number;
  modelName: string;
  imageData?: string | null;
  updateImage: boolean;
  model3DUrl?: string | null;
  updateModel3D: boolean;
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

/* ===== UPLOAD MODEL 3D (.glb/.gltf) ===== */
export async function uploadAssetModel3D(file: File) {
  const formData = new FormData();
  formData.append("file", file);

  const res = await axios.post<{ url: string }>(
    "/api/asset-models/upload-3d",
    formData,
    { headers: { "Content-Type": "multipart/form-data" } }
  );
  return res.data.url;
}

export interface AssetModelOptionDto {
  id: number;
  name: string;
}

export const getAssetModelOptions = () => {
  return axios.get<AssetModelOptionDto[]>(
    "/api/asset-models/options"
  );
};