import { Layout, Menu } from "antd";
import {
  AppstoreOutlined,
  DatabaseOutlined,
} from "@ant-design/icons";
import { Link } from "react-router-dom";

const { Sider } = Layout;

const Sidebar = () => {
  return (
    <Sider width={220}>
      <div style={{ color: "#fff", padding: 16, fontWeight: 600 }}>
        ERP SYSTEM
      </div>

      <Menu
        theme="dark"
        mode="inline"
        items={[
          {
            key: "assets",
            icon: <DatabaseOutlined />,
            label: <Link to="/assets">Quản lý tài sản</Link>,
          },
          {
            key: "warehouse",
            icon: <AppstoreOutlined />,
            label: "Kho",
            disabled: true,
          },
        ]}
      />
    </Sider>
  );
};

export default Sidebar;
