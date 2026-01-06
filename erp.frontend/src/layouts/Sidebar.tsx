import { Layout, Menu } from "antd";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { useAuthStore } from "../auth/auth.store";
import { mapSidebarToMenuItems } from "./sidebar.mapper";
import type { SidebarItem } from "../features/assets/asset.sidebar";
import { getSidebarBySystem } from "../api/menu.api";

const { Sider } = Layout;

const getOpenKeys = (
  items: SidebarItem[],
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

  const [sidebar, setSidebar] = useState<SidebarItem[]>([]);

  useEffect(() => {
    if (!systemCode) return;

    getSidebarBySystem(systemCode).then(setSidebar);
  }, [systemCode]);

  if (!systemCode) return null;

  const selectedKey = location.pathname;
  const openKeys = getOpenKeys(sidebar, selectedKey);

  return (
    <Sider width={240}>
      <Menu
        theme="dark"
        mode="inline"
        selectedKeys={[selectedKey]}
        defaultOpenKeys={openKeys}
        items={mapSidebarToMenuItems(sidebar)}
      />
    </Sider>
  );
};

export default Sidebar;
