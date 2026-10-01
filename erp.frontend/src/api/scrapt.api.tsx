import axios from "axios";

const BASE = "/api/asset-scraps";

export enum ScrapStatus {
    Draft = 1,
    Pending = 2,
    Confirmed = 3,
    Rejected = 4,
    Cancelled = 5,
}

export const SCRAP_STATUS_CONFIG: Record<ScrapStatus, { label: string; color: string }> = {
    [ScrapStatus.Draft]: { label: "Nháp", color: "default" },
    [ScrapStatus.Pending]: { label: "Chờ duyệt", color: "processing" },
    [ScrapStatus.Confirmed]: { label: "Đã báo phế", color: "success" },
    [ScrapStatus.Rejected]: { label: "Từ chối", color: "warning" },
    [ScrapStatus.Cancelled]: { label: "Đã hủy", color: "error" },
};

// ─── DTOs ─────────────────────────────────────────────────────

export interface ScrapDetailDto {
    id: number;
    assetNo: string;
    assetName?: string;
    modelName?: string;
    previousUseDepartmentId: string;
    previousStorageLocationNo: string;
    note?: string;
}

export interface ScrapOrderDto {
    id: string;
    departmentId: string;
    departmentName: string;
    storageLocationNo?: string;
    status: ScrapStatus;
    reason?: string;
    note?: string;
    createdAt: string;
    createdByUserName?: string;
    confirmedAt?: string;
    confirmedByUserName?: string;
    details: ScrapDetailDto[];
}

export interface ScrapOrderListDto {
    id: string;
    departmentId: string;
    departmentName: string;
    status: ScrapStatus;
    assetCount: number;
    createdAt: string;
    createdByUserName?: string;
}

export interface ScrapAssetCandidateDto {
    assetNo: string;
    assetName: string;
    modelName: string;
    storageLocationNo: string;
    useDepartmentId: string;
    status: number;
}

export interface PagedResult<T> {
    total: number;
    items: T[];
}

export interface CreateScrapDto {
    departmentId: string;
    storageLocationNo?: string;
    reason?: string;
    note?: string;
    assetNos: string[];
}

export interface UpdateScrapDto {
    reason?: string;
    note?: string;
}

export interface AddScrapAssetsDto {
    assetNos: string[];
}

export interface SearchScrapOrdersDto {
    departmentId?: string;
    status?: ScrapStatus;
    pageIndex?: number;
    pageSize?: number;
}

export interface SearchScrapAssetsDto {
    departmentId: string;
    storageLocationNo?: string;
    assetNo?: string;
    pageIndex?: number;
    pageSize?: number;
}

// ─── API ──────────────────────────────────────────────────────

export const createScrap = async (dto: CreateScrapDto): Promise<ScrapOrderDto> =>
    (await axios.post<ScrapOrderDto>(BASE, dto)).data;

export const updateScrap = async (id: string, dto: UpdateScrapDto): Promise<ScrapOrderDto> =>
    (await axios.put<ScrapOrderDto>(`${BASE}/${id}`, dto)).data;

export const searchScraps = async (dto: SearchScrapOrdersDto): Promise<PagedResult<ScrapOrderListDto>> =>
    (await axios.get<PagedResult<ScrapOrderListDto>>(BASE, { params: dto })).data;

export const getScrap = async (id: string): Promise<ScrapOrderDto> =>
    (await axios.get<ScrapOrderDto>(`${BASE}/${id}`)).data;

export const confirmScrap = async (id: string): Promise<ScrapOrderDto> =>
    (await axios.post<ScrapOrderDto>(`${BASE}/${id}/confirm`)).data;

export const cancelScrap = async (id: string): Promise<ScrapOrderDto> =>
    (await axios.post<ScrapOrderDto>(`${BASE}/${id}/cancel`)).data;

export const addScrapAssets = async (id: string, dto: AddScrapAssetsDto): Promise<ScrapOrderDto> =>
    (await axios.post<ScrapOrderDto>(`${BASE}/${id}/details`, dto)).data;

export const removeScrapAsset = async (id: string, detailId: number): Promise<void> => {
    await axios.delete(`${BASE}/${id}/details/${detailId}`);
};

export const searchCandidateAssets = async (dto: SearchScrapAssetsDto): Promise<PagedResult<ScrapAssetCandidateDto>> =>
    (await axios.get<PagedResult<ScrapAssetCandidateDto>>(`${BASE}/candidate-assets`, { params: dto })).data;