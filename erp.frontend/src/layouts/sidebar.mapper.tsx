// sidebar.mapper.ts
import type { MenuProps } from "antd";
import type { SidebarItem } from "../features/sidebar/sidebar.item";

export const mapSidebarToMenuItems = (
  items: SidebarItem[]
): MenuProps["items"] =>
  items.map((item) => ({
    key: item.path ?? item.key,
    label: item.path ? <span >{item.label}</span> : item.label,
    children: item.children
      ? mapSidebarToMenuItems(item.children)
      : undefined,
  }));
