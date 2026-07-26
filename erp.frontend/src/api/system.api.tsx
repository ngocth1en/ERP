import axios from "axios";

/* ======================
   DTOs
====================== */

export interface SystemOptionDto {
  code: string;
  name: string;
}

export interface SystemModuleDto {
  code: string;
  name: string;
  isActive: boolean;
  updatedAt?: string;
}

export interface UpdateSystemModuleDto {
  name: string;
  isActive: boolean;
}

export interface SystemGuideFileDto {
  id: number;
  fileName: string;
  uploadedAt: string;
}
export interface GrantUserSystemDto {
  userName: string;
  systemCode: string;
}
/* ======================
   APIs
====================== */

// 1️⃣ Lấy tất cả hệ thống
export const getSystems = async (): Promise<SystemOptionDto[]> => {
  const res = await axios.get<SystemOptionDto[]>("/api/systems");
  return res.data;
};

// 2️⃣ Lấy hệ thống theo user
export const getSystemsByUser = async (
  username: string
): Promise<SystemOptionDto[]> => {
  if (!username) return [];
  const res = await axios.get<SystemOptionDto[]>(
    `/api/systems/by-user/${username}`
  );
  return res.data;
};
export const getSystemsByUserNotAllowed = async (
  username: string
): Promise<SystemOptionDto[]> => {
  if (!username) return [];
  const res = await axios.get<SystemOptionDto[]>(
    `/api/systems/by-user-not-allowed/${username}`
  );
  return res.data;
};

// 3️⃣ Lấy danh sách module hệ thống (dùng cho quản lý)
export const getSystemModules = async (): Promise<SystemModuleDto[]> => {
  const res = await axios.get<SystemModuleDto[]>(
    "/api/systems/modules"
  );
  return res.data;
};

// 4️⃣ Cập nhật tên / trạng thái hệ thống
export const updateSystemModule = async (
  code: string,
  data: UpdateSystemModuleDto
) => {
  return axios.put(
    `/api/systems/modules/${code}`,
    data
  );
};

// 5️⃣ Lấy file hướng dẫn theo hệ thống
export const getSystemGuideFiles = async (
  systemCode: string
): Promise<SystemGuideFileDto[]> => {
  const res = await axios.get<SystemGuideFileDto[]>(
    `/api/systems/${systemCode}/files`
  );
  return res.data;
};

// 6️⃣ Upload file hướng dẫn
export const uploadSystemGuideFile = async (
  systemCode: string,
  file: File
) => {
  const formData = new FormData();
  formData.append("file", file);

  return axios.post(
    `/api/systems/${systemCode}/upload`,
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data"
      }
    }
  );
};
export const grantSystemToUser = async (
  dto: GrantUserSystemDto
): Promise<void> => {
  await axios.post("/api/systems/grant", dto);
};

export const revokeSystemFromUser = async (
  dto: GrantUserSystemDto
): Promise<void> => {
  await axios.post("/api/systems/revoke", dto);
};