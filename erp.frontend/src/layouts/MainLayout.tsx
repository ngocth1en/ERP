import { Layout } from "antd";
import { Outlet, useLocation } from "react-router-dom";
import { useEffect } from "react";
import Sidebar from "./Sidebar";
import HeaderBar from "./HeaderBar";
import Toolbar from "./ToolBars";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";

const { Header, Sider, Content } = Layout;

const MainLayout = () => {
  const location = useLocation();
  const { systemCode, switchSystem } = useAuthStore();

  useEffect(() => {
    const urlSystem = location.pathname.split("/")[1]?.toUpperCase();

    if (
      urlSystem &&
      ["ASSET", "QC", "MAINT"].includes(urlSystem) &&
      urlSystem !== systemCode
    ) {
      switchSystem(urlSystem as SystemCode);
    }
  }, [location.pathname, systemCode, switchSystem]);

  return (
    <Layout style={{ minHeight: "100vh" }}>
      {/* ===== SIDEBAR ===== */}
      <Sider width={220}>
        <Sidebar />
      </Sider>

      {/* ===== MAIN ===== */}
      <Layout>
        {/* ===== HEADER (FULL WIDTH PHẦN MAIN) ===== */}
        <Header
          style={{
            padding: "0 16px",
            background: "transparent",
            height: "auto",
            lineHeight: "normal"
          }}
        >
          <HeaderBar />
        </Header>

        {/* ===== TOOLBAR ===== */}
        <div
          style={{
            padding: "10px 16px",
            borderBottom: "1px solid #eee",
            background: "#fff"
          }}
        >
          <Toolbar />
        </div>

        {/* ===== CONTENT ===== */}
        <Content style={{ padding: 16 }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
