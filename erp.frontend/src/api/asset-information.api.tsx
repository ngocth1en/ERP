import axios from "axios";

/* ================= DTO ================= */

export interface AssetStatisticResultDto {
  modelId: number;
  departmentId?: string;
  assetName: string; // Big + Middle + Small
  modelName: string;
  totalAssets: number;
}

export interface AssetStatisticSearchDto {
  modelId?: number;
  departmentId?: string;
  bigTypeId?: number;
  middleTypeId?: number;
  page: number;
  pageSize: number;
}

export interface AssetByModelResultDto {
  assetNo: string;
  assetName: string; // Big - Middle - Small
  modelName: string;
  departmentId: string;
  status: number;
}

export interface AssetByModelSearchDto {
  modelId?: number;
  departmentId?: string;
  status?: number;
  page: number;
  pageSize: number;
}

export interface CreateAssetDto {
  departmentId: string;
  useDepartmentId: string;
  model_Id: number;
  status: number;
  storageLocationNo: string;
}

export interface UpdateAssetDto {
  departmentId: string;
  useDepartmentId: string;
  model_Id: number;
  status: number;
  storageLocationNo: string;
}

export interface PagedResult<T> {
  total: number;
  items: T[];
}

/* ================= API ================= */

/** Thống kê tài sản theo model */
export async function searchStatistic(
  dto: AssetStatisticSearchDto
): Promise<PagedResult<AssetStatisticResultDto>> {
  const fixedDto: AssetStatisticSearchDto = {
    ...dto,
    modelId: dto.modelId !== undefined ? Number(dto.modelId) : undefined,
    bigTypeId: dto.bigTypeId !== undefined ? Number(dto.bigTypeId) : undefined,
    middleTypeId:
      dto.middleTypeId !== undefined ? Number(dto.middleTypeId) : undefined,
  };

  const res = await axios.post(
    "/api/assets-information/statistics/search",
    fixedDto
  );
  return res.data;
}

/** Danh sách tài sản theo model */
export async function searchAssetsByModel(
  dto: AssetByModelSearchDto
): Promise<PagedResult<AssetByModelResultDto>> {
  const fixedDto: AssetByModelSearchDto = {
    ...dto,
    modelId: dto.modelId !== undefined ? Number(dto.modelId) : undefined,
    status: dto.status !== undefined ? Number(dto.status) : undefined,
  };

  const res = await axios.post(
    "/api/assets-information/by-model/search",
    fixedDto
  );
  return res.data;
}

/** Tạo tài sản */
export async function createAsset(dto: CreateAssetDto, username: string) {
  const res = await axios.post("/api/assets-information", dto, {
    params: { username },
  });
  return res.data;
}

/** Cập nhật tài sản */
export async function updateAsset(
  assetNo: string,
  dto: UpdateAssetDto,
  username: string
) {
  await axios.put(`/api/assets-information/${assetNo}`, dto, {
    params: { username },
  });
}
