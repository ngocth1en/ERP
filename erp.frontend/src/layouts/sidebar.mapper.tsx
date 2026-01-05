import type { MenuProps } from "antd";
import type { SidebarItem } from "../features/assets/asset.sidebar";
import { Link } from "react-router-dom";

export const mapSidebarToMenuItems = (
  items: SidebarItem[]
): MenuProps["items"] => {
  return items.map((item) => ({
    key: item.key,
    label: item.path ? <Link to={item.path}>{item.label}</Link> : item.label,
    children: item.children ? mapSidebarToMenuItems(item.children) : undefined,
  }));
};
