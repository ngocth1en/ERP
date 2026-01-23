import axios from "axios";

/* ================= DTO ================= */

export interface AssetMiddleTypeSearchDto {
    keyword?: string;
    machineType?: string;
    inventoryCheckType?: string;
    bigTypeId: number;
    page: number;
    pageSize: number;
}


export interface CreateAssetMiddleTypeDto {
    nameEN: string;
    nameVN: string;
    machine_Type: string;
    inventory_Check_Type: string;
    bigTypeId: number;
}

export interface UpdateAssetMiddleTypeDto
    extends CreateAssetMiddleTypeDto { }

/* ================= API ================= */

export async function getAssetMiddleTypes(
    params: AssetMiddleTypeSearchDto
) {
    const res = await axios.get("/api/asset-middle-types", { params });
    return res.data;
}

export async function createAssetMiddleType(
    dto: CreateAssetMiddleTypeDto,
    username: string
) {
    const res = await axios.post("/api/asset-middle-types", dto, {
        params: { username },
    });
    return res.data;
}

export async function updateAssetMiddleType(
    id: number,
    dto: UpdateAssetMiddleTypeDto,
    username: string
) {
    await axios.put(`/api/asset-middle-types/${id}`, dto, {
        params: { username },
    });
}

export async function toggleAssetMiddleType(
    id: number,
    username: string
) {
    await axios.patch(
        `/api/asset-middle-types/${id}/toggle-active`,
        null,
        { params: { username } }
    );
}
export const getMiddleTypes = (bigTypeId: number) =>
  axios.get("/api/asset-middle-types/middle-types", { params: { bigTypeId } });