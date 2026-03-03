import axios from "axios";

export interface InventoryItemDto {
  code: string;
  note?: string | null;
  isDamaged?: boolean;
}

export interface CreateInventoryOrderDto {
  modelId: number;
  departmentId?: string | null;
  note?: string | null;
}

export interface InventoryOrderListDto {
  id: string;
  modelId: number;
  departmentId?: string;
  inventoryType: string;
  expectedCount: number;
  counted: number;
  createdAt: string;
  createdByUserName?: string;
}

export interface InventoryOrderDto {
  id: string;
  modelId: number;
  departmentId?: string;
  inventoryType: string;
  expectedCount: number;
  counted: number;
  missingCount: number;
  unexpectedCount: number;
  createdAt: string;
  note?: string;
  details: Array<{ id: string; assetNo: string; qrCode?: string; status: string; note?: string }>;
}

export const createInventoryOrder = async (dto: CreateInventoryOrderDto, username?: string) => {
  const q = username ? `?username=${encodeURIComponent(username)}` : "";
  const r = await axios.post(`/api/assets-information/inventory/orders${q}`, dto);
  return r.data;
};

export const updateInventoryOrder = async (id: string, dto: any, username?: string) => {
  const q = username ? `?username=${encodeURIComponent(username)}` : "";
  const r = await axios.put(`/api/assets-information/inventory/orders/${id}${q}`, dto);
  return r.data;
};

export const searchInventoryOrders = async (params: { modelId?: number; departmentId?: string; pageIndex?: number; pageSize?: number }) => {
  const r = await axios.get("/api/assets-information/inventory/orders", { params });
  return r.data;
};

export const getInventoryOrder = async (id: string) => {
  const r = await axios.get(`/api/assets-information/inventory/orders/${id}`);
  return r.data as InventoryOrderDto;
};

export const inventoryCheck = async (body: any, username?: string) => {
  const q = username ? `?username=${encodeURIComponent(username)}` : "";
  const r = await axios.post(`/api/assets-information/inventory/check${q}`, body);
  return r.data;
};

export const getInventorySummary = async (modelId: number, departmentId?: string, months = 6) => {
  const r = await axios.get("/api/assets-information/inventory/summary", { params: { modelId, departmentId, months } });
  return r.data;
};