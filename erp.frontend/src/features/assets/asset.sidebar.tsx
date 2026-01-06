import { ASSET_PERMISSION } from "./asset.sidebar.permission";

export interface SidebarItem {
  key: string;
  label: string;
  path?: string;
  permission?: string;
  children?: SidebarItem[];
}


