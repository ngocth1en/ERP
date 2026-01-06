// sidebar.mapper.ts
import type { MenuProps } from "antd";
import { Link } from "react-router-dom";
import type { SidebarItem } from "../features/assets/asset.sidebar";

export const mapSidebarToMenuItems = (
  items: SidebarItem[]
): MenuProps["items"] =>
  items.map((item) => ({
    key: item.path ?? item.key,
    label: item.path ? <Link to={item.path}>{item.label}</Link> : item.label,
    children: item.children
      ? mapSidebarToMenuItems(item.children)
      : undefined,
  }));
