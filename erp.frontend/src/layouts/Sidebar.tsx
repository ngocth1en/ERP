import { Layout, Menu } from "antd";
import { useEffect, useState } from "react";
import { useAuthStore } from "../auth/auth.store";
import { getSidebarBySystem } from "../api/menu.api";
import { mapSidebarToMenuItems } from "./sidebar.mapper";

const { Sider } = Layout;

const Sidebar = () => {
  const { currentMenuCode, setCurrentMenu } = useAuthStore();
  const [sidebar, setSidebar] = useState<any[]>([]);
  const username = useAuthStore((s) => s.username);
  const systemCode=useAuthStore((s)=>s.systemCode);

  useEffect(() => {
    if (!systemCode || !username) return;

    getSidebarBySystem(systemCode, username).then(setSidebar);
  }, [systemCode, username]);

  const onMenuClick = ({ key }: { key: string }) => {
    const findMenuByPath = (items: any[], path: string): any => {
      for (const i of items) {
        if (i.path === path) return i;

        if (i.children?.length) {
          const found = findMenuByPath(i.children, path);
          if (found) return found;
        }
      }
      return null;
    };
    const menu = findMenuByPath(sidebar, key);
    if (!menu) return;
    // ✅ CHỈ set menu + permission
    setCurrentMenu(menu.path, menu.permission);
  };

  return (
    <Sider width={240}>
      <Menu
        theme="dark"
        mode="inline"
        selectedKeys={currentMenuCode ? [currentMenuCode] : []}
        items={mapSidebarToMenuItems(sidebar)}
        onClick={onMenuClick}
      />
    </Sider>
  );
};

export default Sidebar;
