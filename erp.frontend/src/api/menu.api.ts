// src/api/menu.api.ts
import axios from "axios";

/* =========================
   TYPES
========================= */

export interface SidebarItem {
  key: string;
  label: string;
  path?: string | null;
  permission?: string | null;
  children?: SidebarItem[];
}

export interface CreateSystemMenuDto {
  code: string;
  systemCode: string;
  groupCode: string;
  name: string;
  route?: string;
  orderNo: number;
}

export interface CreateUserMenuPermissionDto {
  userName: string;
  menuCode: string;
  permissionMask: string;
}

export interface MenuPermissionItemDto {
  menuCode: string;
  permissionMask: string;
}

export interface BulkUserMenuPermissionDto {
  userId: string;
  systemCode: string;
  menus: MenuPermissionItemDto[];
}

export interface SystemMenuStatusDto {
  menuCode: string;
  menuName: string;
  isActive: boolean;
}

export interface UpdateSystemMenuStatusDto {
  Code: string;
  isUsing: boolean;
}
export interface MenuTreeDto {
  key: string;
  label: string;
  isUsing: boolean;
  children?: MenuTreeDto[];
}
/* =========================
   API CALLS
========================= */
export const getMenuTreeBySystem = async (
  systemCode: string
): Promise<MenuTreeDto[]> => {
  const res = await axios.get("/api/menus/menu-tree", {
    params: { systemCode },
  });

  return res.data;
};
/**
 * Lấy sidebar theo hệ thống + user
 */
export async function getSidebarBySystem(
  systemCode: string,
  userName: string
): Promise<SidebarItem[]> {
  const res = await axios.get<SidebarItem[]>(
    `/api/menus/${systemCode}/sidebar`,
    {
      params: { userName },
    }
  );
  return res.data;
}

/**
 * Lấy permission mask của user trên menu
 */
export async function getPermissionOnMenuByUser(
  systemCode: string,
  menuCode: string,
  userName: string
): Promise<string> {
  const res = await axios.get<string>(
    `/api/menus/${systemCode}/sidebar/${menuCode}/permission`,
    {
      params: { userName },
    }
  );
  return res.data;
}

/**
 * Gán quyền cho 1 menu
 */
export async function createUserMenuPermission(
  dto: CreateUserMenuPermissionDto
): Promise<void> {
  await axios.post("/api/menus/permission", dto);
}

/**
 * Gán quyền hàng loạt
 */
export async function bulkAssignMenuPermission(
  dto: BulkUserMenuPermissionDto
): Promise<void> {
  await axios.post("/api/menus/permission/bulk", dto);
}

/**
 * Tạo menu hệ thống
 */
export async function createSystemMenu(
  dto: CreateSystemMenuDto
): Promise<void> {
  await axios.post("/api/menus/system-menu", dto);
}
/**
 * Lấy trạng thái menu theo hệ thống
 */
export async function getMenuStatus(
  systemCode: string
): Promise<SystemMenuStatusDto[]> {
  const res = await axios.get<SystemMenuStatusDto[]>(
    `/api/menus/${systemCode}/status`
  );
  return res.data;
}

/**
 * Cập nhật trạng thái menu
 */
export async function updateMenuStatus(
  dto: UpdateSystemMenuStatusDto
): Promise<void> {
  await axios.put("/api/menus/status", dto);
}
