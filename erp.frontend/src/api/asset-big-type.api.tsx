import axios from "axios";

/* =========================
   DTOs
========================= */

export interface SearchAssetBigTypeDto {
  nameEN?: string;
  nameVN?: string;
  page: number;
  pageSize: number;
}

export interface AssetBigTypeListDto {
  stt: number;
  bigTypeId: number;
  bigTypeNameEN: string;
  bigTypeNameVN: string;
  updatedAt?: string;
  updatedByName?: string;
}

export interface CreateAssetBigTypeDto {
  nameEN: string;
  nameVN: string;
}

export interface UpdateAssetBigTypeDto {
  nameEN: string;
  nameVN: string;
}

export interface PagedResult<T> {
  total: number;
  items: T[];
}

/* =========================
   API CALLS (MATCH BACKEND)
========================= */

/** SEARCH */
export async function getAssetBigTypes(
  params: SearchAssetBigTypeDto
): Promise<PagedResult<AssetBigTypeListDto>> {
  const res = await axios.get<PagedResult<AssetBigTypeListDto>>(
    "/api/asset-big-types/search",
    { params }
  );
  return res.data;
}

/** CREATE */
export async function createAssetBigType(
  dto: CreateAssetBigTypeDto,
  userName: string
): Promise<void> {
  if (!userName) throw new Error("Missing username");

  await axios.post(
    "/api/asset-big-types",
    dto,
    {
      params: {
        userName,
      },
    }
  );
}

/** UPDATE */
export async function updateAssetBigType(
  id: number,
  dto: UpdateAssetBigTypeDto,userName:string
): Promise<void> {
  await axios.put(`/api/asset-big-types/${id}`, dto,
    {
      params: {
        userName,
      },
    });
}

/** DELETE (SOFT) */
export async function deleteAssetBigType(
  id: number,userName:string
): Promise<void> {
  await axios.delete(`/api/asset-big-types/${id}`,{
      params: {
        userName,
      },
    });
}
export const getBigTypes = () =>
  axios.get("/api/asset-big-types/big-types");