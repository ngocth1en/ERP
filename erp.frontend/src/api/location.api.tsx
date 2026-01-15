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
export interface PagedResult<T> {
  total: number;
  items: T[];
}

/* ======================
   API BASE
====================== */

const BASE_URL = "/api/storage-locations";

/* ======================
   APIs
====================== */

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
