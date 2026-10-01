import axios from "axios";

const BASE = "/api/asset-liquidations";

export enum LiquidationStatus {
    Draft = 1,
    Pending = 2,
    Confirmed = 3,
    Cancelled = 4,
}

export const LIQUIDATION_STATUS_CONFIG: Record<LiquidationStatus, { label: string; color: string }> = {
    [LiquidationStatus.Draft]: { label: "Nháp", color: "default" },
    [LiquidationStatus.Pending]: { label: "Chờ duyệt", color: "processing" },
    [LiquidationStatus.Confirmed]: { label: "Đã thanh lý", color: "success" },
    [LiquidationStatus.Cancelled]: { label: "Đã hủy", color: "error" },
};

// ─── DTOs ─────────────────────────────────────────────────────

export interface LiquidationDetailDto {
    id: number;
    assetNo: string;
    assetName?: string;
    modelName?: string;
    scrapId?: string;
    previousUseDepartmentId: string;
    note?: string;
}

export interface LiquidationOrderDto {
    id: string;
    departmentId: string;
    departmentName: string;
    status: LiquidationStatus;
    liquidationMethod?: string;
    recoveredAmount?: number;
    note?: string;
    createdAt: string;
    createdByUserName?: string;
    confirmedAt?: string;
    confirmedByUserName?: string;
    details: LiquidationDetailDto[];
}

export interface LiquidationOrderListDto {
    id: string;
    departmentId: string;
    departmentName: string;
    status: LiquidationStatus;
    assetCount: number;
    createdAt: string;
    createdByUserName?: string;
}

export interface ScrappedAssetCandidateDto {
    assetNo: string;
    assetName: string;
    modelName: string;
    scrapId: string;
    departmentId: string;
    scrapConfirmedAt: string;
}

export interface PagedResult<T> {
    total: number;
    items: T[];
}

export interface CreateLiquidationDto {
    departmentId: string;
    liquidationMethod?: string;
    recoveredAmount?: number;
    note?: string;
    assetNos: string[];
}

export interface UpdateLiquidationDto {
    liquidationMethod?: string;
    recoveredAmount?: number;
    note?: string;
}

export interface AddLiquidationAssetsDto {
    assetNos: string[];
}

export interface SearchLiquidationOrdersDto {
    departmentId?: string;
    status?: LiquidationStatus;
    pageIndex?: number;
    pageSize?: number;
}

export interface SearchScrappedAssetsDto {
    departmentId?: string;
    assetNo?: string;
    pageIndex?: number;
    pageSize?: number;
}

// ─── API ──────────────────────────────────────────────────────

export const createLiquidation = async (dto: CreateLiquidationDto): Promise<LiquidationOrderDto> =>
    (await axios.post<LiquidationOrderDto>(BASE, dto)).data;

export const updateLiquidation = async (id: string, dto: UpdateLiquidationDto): Promise<LiquidationOrderDto> =>
    (await axios.put<LiquidationOrderDto>(`${BASE}/${id}`, dto)).data;

export const searchLiquidations = async (dto: SearchLiquidationOrdersDto): Promise<PagedResult<LiquidationOrderListDto>> =>
    (await axios.get<PagedResult<LiquidationOrderListDto>>(BASE, { params: dto })).data;

export const getLiquidation = async (id: string): Promise<LiquidationOrderDto> =>
    (await axios.get<LiquidationOrderDto>(`${BASE}/${id}`)).data;

export const confirmLiquidation = async (id: string): Promise<LiquidationOrderDto> =>
    (await axios.post<LiquidationOrderDto>(`${BASE}/${id}/confirm`)).data;

export const cancelLiquidation = async (id: string): Promise<LiquidationOrderDto> =>
    (await axios.post<LiquidationOrderDto>(`${BASE}/${id}/cancel`)).data;

export const addLiquidationAssets = async (id: string, dto: AddLiquidationAssetsDto): Promise<LiquidationOrderDto> =>
    (await axios.post<LiquidationOrderDto>(`${BASE}/${id}/details`, dto)).data;

export const removeLiquidationAsset = async (id: string, detailId: number): Promise<void> => {
    await axios.delete(`${BASE}/${id}/details/${detailId}`);
};

export const searchScrappedAssets = async (dto: SearchScrappedAssetsDto): Promise<PagedResult<ScrappedAssetCandidateDto>> =>
    (await axios.get<PagedResult<ScrappedAssetCandidateDto>>(`${BASE}/scrapped-assets`, { params: dto })).data;