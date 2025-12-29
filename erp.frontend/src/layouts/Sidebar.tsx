import { Layout, Menu } from "antd";
import { Link, useLocation } from "react-router-dom";
import { menuBySystem } from "./sitebar.menu";
import { useAuthStore } from "../auth/auth.store";

const { Sider } = Layout;

const Sidebar = () => {
  const { systemCode } = useAuthStore();
  const location = useLocation();

  if (!systemCode) return null;

  const menus = menuBySystem[systemCode];

  return (
    <Sider width={220}>
      <div style={{ color: "#fff", padding: 16, fontWeight: 600 }}>
        {systemCode} SYSTEM
      </div>

      <Menu
        theme="dark"
        mode="inline"
        selectedKeys={[location.pathname.split("/").pop()!]}
        items={menus.map((m) => ({
          key: m.key,
          icon: m.icon,
          disabled: m.disabled,
          label: m.path ? <Link to={m.path}>{m.label}</Link> : m.label,
        }))}
      />
    </Sider>
  );
};

export default Sidebar;
