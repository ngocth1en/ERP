// menu.api.ts
import axios from "axios";
import type { SidebarItem } from "../features/assets/asset.sidebar";

export async function getSidebarBySystem(
  systemCode: string
): Promise<SidebarItem[]> {
  const res = await axios.get<SidebarItem[]>(
    `/api/menus/${systemCode.toLowerCase()}/sidebar`
  );
  return res.data;
}
