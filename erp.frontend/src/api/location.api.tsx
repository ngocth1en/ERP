import axios from "axios";

/* ======================
   DTOs
====================== */

export interface SearchStorageLocationDto {
    name?: string;
    departmentId?: string;
    status?: number;
    inUse?: number;

    page: number;
    pageSize: number;
}


export interface CreateStorageLocationDto {
    status: number;        // 1 | 2 | 3
    floor: number;         // tầng người dùng nhập
    name: string;          // tên người dùng nhập
    departmentId: string;
    inUse: number;         // 0 | 1
}

export interface UpdateStorageLocationDto {
    name: string;
    status: number;
    inUse: number;
}

export interface StorageLocationListDto {
    stt: number;
    storageLocationNo: string;
    nameVN: string;
    departmentName: string;
    status: number;
    inUse: number;
}
/** DTO dùng cho combobox (GetAll) */
export interface StorageLocationDto {
    storageLocationNo: string;
    nameEN: string;
    nameVN: string;
    status: number;
    statusText: string;
    inUse: number;
}
export interface PagedResult<T> {
    total: number;
    items: T[];
}

/* ======================
   WAREHOUSE 3D FLOOR PLAN
====================== */

export interface WarehouseCellDto {
    storageLocationNo: string;
    nameVN: string;
    departmentName: string;
    status: number; // 1 = Đang dùng, 2 = Trống, 3 = Báo phế
    inUse: number;
    assetCount: number;
    gridX: number;
    gridZ: number;
}

export interface WarehouseFloorPlanDto {
    floor: number;
    cells: WarehouseCellDto[];
}

export interface WarehouseCellAssetDto {
    assetNo: string;
    modelName: string;
    assetFullName: string;
    status: number;
    model3DUrl?: string | null;
    imageDataBase64?: string | null;
}

/* ======================
   API BASE
====================== */

const BASE_URL = "/api/storage-locations";

/* ======================
   APIs
====================== */
export const getAllStorageLocations = () => {
    return axios.get<StorageLocationDto[]>(`${BASE_URL}`);
};
/**
 * Tìm kiếm vị trí lưu trữ
 * filter theo:
 * - tên vị trí
 * - bộ phận
 * - trạng thái
 * - inUse
 */
export const searchStorageLocations = (
    dto: SearchStorageLocationDto
) => {
    return axios.get<PagedResult<StorageLocationListDto>>(
        `${BASE_URL}/search`,
        {
            params: dto,
        }
    );
};


/**
 * Tạo mới vị trí lưu trữ
 * - Backend tự sinh mã theo rule
 * - FE không truyền StorageLocationNo
 */
export const createStorageLocation = (
    dto: CreateStorageLocationDto
) => {
    return axios.post(
        `${BASE_URL}`,
        dto
    );
};

/**
 * Cập nhật vị trí lưu trữ
 * Chỉ cho sửa:
 * - Name
 * - Status
 * - InUse
 */
export const updateStorageLocation = (
    storageLocationNo: string,
    dto: UpdateStorageLocationDto
) => {
    return axios.put(
        `${BASE_URL}/${storageLocationNo}`,
        dto
    );
};

/**
 * Lấy sơ đồ kho 3D theo tầng (CAD/3D Warehouse Floor Plan Viewer).
 */
export const getWarehouseFloorPlan = (floor: number) => {
    return axios.get<WarehouseFloorPlanDto>(
        `${BASE_URL}/floor-plan/${floor}`
    );
};

/**
 * Lấy danh sách tài sản tại 1 vị trí lưu trữ (dùng khi raycasting click chọn ô trong sơ đồ 3D).
 */
export const getAssetsByStorageLocation = (storageLocationNo: string) => {
    return axios.get<WarehouseCellAssetDto[]>(
        `${BASE_URL}/${storageLocationNo}/assets`
    );
};