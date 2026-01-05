import { Layout, Menu } from "antd";
import { Link, useLocation } from "react-router-dom";
import { useAuthStore } from "../auth/auth.store";
import type { SidebarMenuItem } from "./sidebar.types";
import type { MenuProps } from "antd";
import { assetSidebar } from "../features/assets/asset.sidebar";

const { Sider } = Layout;

const mapMenuItems = (
  items: SidebarMenuItem[]
): MenuProps["items"] =>
  items.map((m) => ({
    key: m.path ?? m.key,
    icon: m.icon,
    label: m.path ? <Link to={m.path}>{m.label}</Link> : m.label,
    disabled: m.disabled,
    children: m.children ? mapMenuItems(m.children) : undefined,
  }));

const getOpenKeys = (
  items: SidebarMenuItem[],
  path: string
): string[] => {
  for (const item of items) {
    if (item.children?.some((c) => c.path === path)) {
      return [item.key];
    }
  }
  return [];
};

const Sidebar = () => {
  const { systemCode } = useAuthStore();
  const location = useLocation();

  if (!systemCode) return null;

  const rawSidebar =
    systemCode === "ASSET" ? assetSidebar : [];

  const selectedKey = location.pathname;
  const openKeys = getOpenKeys(rawSidebar, selectedKey);

  return (
    <Sider width={240}>
      <Menu
        theme="dark"
        mode="inline"
        selectedKeys={[selectedKey]}
        defaultOpenKeys={openKeys}
        items={mapMenuItems(rawSidebar)}
      />
    </Sider>
  );
};

export default Sidebar;
