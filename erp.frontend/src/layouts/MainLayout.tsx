import { Layout } from "antd";
import { Outlet, useLocation } from "react-router-dom";
import { useEffect } from "react";
import Sidebar from "./Sidebar";
import HeaderBar from "./HeaderBar";
import Toolbar from "./ToolBars";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";
import MainContent from "./MainContent";

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
      <Sider
        width={220}
        style={{
          position: "sticky",
          top: 0,
          height: "100vh",
          overflow: "auto",
        }}
      >
        <Sidebar />
      </Sider>

      {/* ===== MAIN ===== */}
      <Layout>
        {/* ===== HEADER ===== */}
        <Header
          style={{
            padding: "0 16px",
            background: "#fff",
            height: "auto",
            lineHeight: "normal",
            borderBottom: "1px solid #eee",
          }}
        >
          <HeaderBar />
        </Header>

        {/* ===== CONTENT (BAO TOOLBAR + PAGE) ===== */}
        <Content
          style={{
            padding: 16,
            background: "#f5f6f8",
            overflow: "auto",
          }}
        >
          {/* TOOLBAR nằm TRONG Content */}
          <div
            style={{
              padding: "10px 16px",
              marginBottom: 16,
              background: "#fff",
              borderRadius: 6,
              border: "1px solid #eee",
            }}
          >
            <Toolbar />
          </div>

          {/* PAGE */}
          <div
            style={{
              padding: 16,
              background: "#fff",
              borderRadius: 6,
              minHeight: 300,
            }}
          >
            <MainContent />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
