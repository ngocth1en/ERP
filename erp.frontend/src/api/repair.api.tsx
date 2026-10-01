import axios from "axios";

const BASE = "/api/asset-repairs";

export enum RepairStatus {
    Draft = 1,
    Pending = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
}

export const REPAIR_STATUS_CONFIG: Record<RepairStatus, { label: string; color: string }> = {
    [RepairStatus.Draft]: { label: "Nháp", color: "default" },
    [RepairStatus.Pending]: { label: "Chờ duyệt", color: "processing" },
    [RepairStatus.InProgress]: { label: "Đang sửa chữa", color: "gold" },
    [RepairStatus.Completed]: { label: "Hoàn thành", color: "success" },
    [RepairStatus.Cancelled]: { label: "Đã hủy", color: "error" },
};

// ─── DTOs ─────────────────────────────────────────────────────

export interface RepairOrderDto {
    id: string;
    departmentId: string;
    departmentName: string;
    assetNo: string;
    assetName?: string;
    modelName?: string;
    scrapId?: string;
    repairVendor?: string;
    status: RepairStatus;
    reason?: string;
    cost?: number;
    note?: string;
    createdAt: string;
    createdByUserName?: string;
    completedAt?: string;
    completedByUserName?: string;
}

export interface RepairOrderListDto {
    id: string;
    departmentId: string;
    departmentName: string;
    assetNo: string;
    assetName?: string;
    status: RepairStatus;
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

export interface CreateRepairDto {
    assetNo: string;
    scrapId?: string;
    repairVendor?: string;
    reason?: string;
    cost?: number;
    note?: string;
}

export interface UpdateRepairDto {
    repairVendor?: string;
    reason?: string;
    cost?: number;
    note?: string;
}

export interface SearchRepairOrdersDto {
    departmentId?: string;
    assetNo?: string;
    status?: RepairStatus;
    pageIndex?: number;
    pageSize?: number;
}

export interface SearchScrappedAssetsDto {
    departmentId?: string;
    assetNo?: string;
    pageIndex?: number;
    pageSize?: number;
}
// ─── BẢO TRÌ MÁY (LỊCH SỬ + THỐNG KÊ CHI PHÍ) ──────────────────

export interface MaintenanceHistoryQueryDto {
    assetNo?: string;
    departmentId?: string;
    fromDate?: string;
    toDate?: string;
    status?: RepairStatus;
    pageIndex?: number;
    pageSize?: number;
}

export interface RepairHistoryItemDto {
    id: string;
    assetNo: string;
    assetName?: string;
    departmentId: string;
    departmentName: string;
    repairVendor?: string;
    reason?: string;
    cost?: number;
    status: RepairStatus;
    createdAt: string;
    createdByUserName?: string;
    completedAt?: string;
    completedByUserName?: string;
}

export interface AssetMaintenanceSummaryDto {
    assetNo: string;
    assetName?: string;
    modelName?: string;
    departmentId: string;
    departmentName: string;
    totalRepairCount: number;
    completedRepairCount: number;
    totalCost: number;
    lastRepairAt?: string;
}

export interface MaintenanceOverallStatisticDto {
    totalRepairOrders: number;
    completedRepairOrders: number;
    inProgressRepairOrders: number;
    totalCost: number;
    topAssetsByCost: AssetMaintenanceSummaryDto[];
}

// ─── API ──────────────────────────────────────────────────────

export const createRepair = async (dto: CreateRepairDto): Promise<RepairOrderDto> =>
    (await axios.post<RepairOrderDto>(BASE, dto)).data;

export const updateRepair = async (id: string, dto: UpdateRepairDto): Promise<RepairOrderDto> =>
    (await axios.put<RepairOrderDto>(`${BASE}/${id}`, dto)).data;

export const searchRepairs = async (dto: SearchRepairOrdersDto): Promise<PagedResult<RepairOrderListDto>> =>
    (await axios.get<PagedResult<RepairOrderListDto>>(BASE, { params: dto })).data;

export const getRepair = async (id: string): Promise<RepairOrderDto> =>
    (await axios.get<RepairOrderDto>(`${BASE}/${id}`)).data;

export const startRepair = async (id: string): Promise<RepairOrderDto> =>
    (await axios.post<RepairOrderDto>(`${BASE}/${id}/start`)).data;

export const completeRepair = async (id: string): Promise<RepairOrderDto> =>
    (await axios.post<RepairOrderDto>(`${BASE}/${id}/complete`)).data;

export const cancelRepair = async (id: string): Promise<RepairOrderDto> =>
    (await axios.post<RepairOrderDto>(`${BASE}/${id}/cancel`)).data;

export const searchScrappedAssets = async (dto: SearchScrappedAssetsDto): Promise<PagedResult<ScrappedAssetCandidateDto>> =>
    (await axios.get<PagedResult<ScrappedAssetCandidateDto>>(`${BASE}/scrapped-assets`, { params: dto })).data;

export const getMaintenanceHistory = async (
    dto: MaintenanceHistoryQueryDto
): Promise<PagedResult<RepairHistoryItemDto>> =>
    (await axios.get<PagedResult<RepairHistoryItemDto>>(`${BASE}/maintenance/history`, { params: dto })).data;

export const getMaintenanceSummaryByAsset = async (
    dto: MaintenanceHistoryQueryDto
): Promise<PagedResult<AssetMaintenanceSummaryDto>> =>
    (await axios.get<PagedResult<AssetMaintenanceSummaryDto>>(`${BASE}/maintenance/summary-by-asset`, { params: dto })).data;

export const getMaintenanceOverallStatistic = async (
    fromDate?: string,
    toDate?: string,
    topCount = 10
): Promise<MaintenanceOverallStatisticDto> =>
    (await axios.get<MaintenanceOverallStatisticDto>(`${BASE}/maintenance/overall-statistic`, {
        params: { fromDate, toDate, topCount },
    })).data;