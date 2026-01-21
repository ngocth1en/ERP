import axios from "axios";

/* ================= DTO ================= */

export interface AssetSmallTypeSearchDto {
    keyword?: string;
    middleTypeId: number;
    page: number;
    pageSize: number;
}

export interface CreateAssetSmallTypeDto {
    nameEN: string;
    nameVN: string;
    middleTypeId: number;
}

export interface UpdateAssetSmallTypeDto
    extends CreateAssetSmallTypeDto { }

/* ================= API ================= */

export async function getAssetSmallTypes(
    params: AssetSmallTypeSearchDto
) {
    const res = await axios.get("/api/asset-small-types", { params });
    return res.data;
}

export async function createAssetSmallType(
    dto: CreateAssetSmallTypeDto,
    username: string
) {
    const res = await axios.post("/api/asset-small-types", dto, {
        params: { username },
    });
    return res.data;
}

export async function updateAssetSmallType(
    id: number,
    dto: UpdateAssetSmallTypeDto,
    username: string
) {
    await axios.put(`/api/asset-small-types/${id}`, dto, {
        params: { username },
    });
}

export async function toggleAssetSmallType(
    id: number,
    username: string
) {
    await axios.patch(
        `/api/asset-small-types/${id}/toggle`,
        null,
        { params: { username } }
    );
}
