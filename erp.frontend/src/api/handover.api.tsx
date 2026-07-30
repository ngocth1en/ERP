import axios from "axios";

const BASE = "/api/asset-handovers";

export enum HandoverStatus {
    Draft = 1,
    Pending = 2,
    Confirmed = 3,
    Cancelled = 4,
}

export const HANDOVER_STATUS_CONFIG: Record<HandoverStatus, { label: string; color: string }> = {
    [HandoverStatus.Draft]: { label: "Nháp", color: "default" },
    [HandoverStatus.Pending]: { label: "Chờ duyệt", color: "processing" },
    [HandoverStatus.Confirmed]: { label: "Hoàn thành", color: "success" },
    [HandoverStatus.Cancelled]: { label: "Đã hủy", color: "error" },
};

/** Tính ngày hết hạn = createdAt + 3 ngày */
export const calcExpiredAt = (createdAt: string): Date =>
    new Date(new Date(createdAt).getTime() + 3 * 24 * 60 * 60 * 1000);

export const isExpired = (createdAt: string): boolean =>
    calcExpiredAt(createdAt) < new Date();

// ─── DTOs ─────────────────────────────────────────────────────

export interface HandoverDetailDto {
    id: number;
    assetNo: string;
    assetName?: string;
    modelName?: string;
    previousUseDepartmentId: string;
    previousStorageLocationNo: string;
    note?: string;
}

export interface HandoverOrderDto {
    id: string;
    fromDepartmentId: string;
    fromDepartmentName: string;
    toDepartmentId: string;
    toDepartmentName: string;
    fromStorageLocationNo?: string;
    toStorageLocationNo?: string;
    status: HandoverStatus;
    note?: string;
    createdAt: string;
    createdByUserName?: string;
    confirmedAt?: string;
    confirmedByUserName?: string;
    details: HandoverDetailDto[];
}

export interface HandoverOrderListDto {
    id: string;
    fromDepartmentId: string;
    fromDepartmentName: string;
    toDepartmentId: string;
    toDepartmentName: string;
    status: HandoverStatus;
    assetCount: number;
    createdAt: string;
    createdByUserName?: string;
}

export interface HandoverAssetCandidateDto {
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

export interface CreateHandoverDto {
    fromDepartmentId: string;
    toDepartmentId: string;
    fromStorageLocationNo?: string;
    toStorageLocationNo?: string;
    note?: string;
    assetNos: string[];
}

export interface UpdateHandoverDto {
    toDepartmentId?: string;
    fromStorageLocationNo?: string;
    toStorageLocationNo?: string;
    note?: string;
}

export interface AddHandoverAssetsDto {
    assetNos: string[];
}

export interface SearchHandoverOrdersDto {
    fromDepartmentId?: string;
    toDepartmentId?: string;
    status?: HandoverStatus;
    pageIndex?: number;
    pageSize?: number;
}

export interface SearchHandoverAssetsDto {
    departmentId: string;
    storageLocationNo?: string;
    assetNo?: string;
    pageIndex?: number;
    pageSize?: number;
}

// ─── API ──────────────────────────────────────────────────────

export const createHandover = async (dto: CreateHandoverDto): Promise<HandoverOrderDto> =>
    (await axios.post<HandoverOrderDto>(BASE, dto)).data;

export const updateHandover = async (id: string, dto: UpdateHandoverDto): Promise<HandoverOrderDto> =>
    (await axios.put<HandoverOrderDto>(`${BASE}/${id}`, dto)).data;

export const searchHandovers = async (dto: SearchHandoverOrdersDto): Promise<PagedResult<HandoverOrderListDto>> =>
    (await axios.get<PagedResult<HandoverOrderListDto>>(BASE, { params: dto })).data;

export const getHandover = async (id: string): Promise<HandoverOrderDto> =>
    (await axios.get<HandoverOrderDto>(`${BASE}/${id}`)).data;

export const confirmHandover = async (id: string): Promise<HandoverOrderDto> =>
    (await axios.post<HandoverOrderDto>(`${BASE}/${id}/confirm`)).data;

export const cancelHandover = async (id: string): Promise<HandoverOrderDto> =>
    (await axios.post<HandoverOrderDto>(`${BASE}/${id}/cancel`)).data;

export const addHandoverAssets = async (id: string, dto: AddHandoverAssetsDto): Promise<HandoverOrderDto> =>
    (await axios.post<HandoverOrderDto>(`${BASE}/${id}/details`, dto)).data;

export const removeHandoverAsset = async (id: string, detailId: number): Promise<void> => {
    await axios.delete(`${BASE}/${id}/details/${detailId}`);
};

export const searchCandidateAssets = async (dto: SearchHandoverAssetsDto): Promise<PagedResult<HandoverAssetCandidateDto>> =>
    (await axios.get<PagedResult<HandoverAssetCandidateDto>>(`${BASE}/candidate-assets`, { params: dto })).data;