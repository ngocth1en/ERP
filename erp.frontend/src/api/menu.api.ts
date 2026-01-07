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

/* =========================
   API CALLS
========================= */

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
