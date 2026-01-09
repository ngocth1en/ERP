import { Layout } from "antd";
import { useLocation } from "react-router-dom";
import { useEffect } from "react";
import Sidebar from "./Sidebar";
import HeaderBar from "./HeaderBar";
import Toolbar from "./ToolBars";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";
import MainContent from "./MainContent";

const { Header, Sider, Content } = Layout;
const HEADER_HEIGHT = 64;

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
    <Layout style={{ height: "100vh", overflow: "hidden" }}>
      {/* ===== HEADER FULL WIDTH ===== */}
      <Header
        style={{
          height: HEADER_HEIGHT,
          padding: "0 16px",
          background: "#fff",
          borderBottom: "1px solid #eee",
          display: "flex",
          alignItems: "center",
        }}
      >
        <HeaderBar />
      </Header>

      {/* ===== BODY ===== */}
      <Layout style={{ height: `calc(100vh - ${HEADER_HEIGHT}px)` }}>
        {/* ===== SIDEBAR ===== */}
        <Sider
          width={240}
          style={{
            height: "100%",
            overflow: "hidden", // ❗ sidebar không scroll
          }}
        >
          <Sidebar />
        </Sider>

        {/* ===== CONTENT (SCROLL CONTAINER) ===== */}
        <Content
          style={{
            height: "100%",
            overflowY: "auto",
            background: "#f5f6f8",
            padding: 0,
          }}
        >
          {/* ===== TOOLBAR (STICKY / FROZEN) ===== */}
          <div
            style={{
              position: "sticky",
              top: 0,
              zIndex: 10,
              background: "#fff",
              padding: "10px 16px",
              marginBottom: 16,
              borderRadius: 6,
              border: "1px solid #eee",
            }}
          >
            <Toolbar />
          </div>

          {/* ===== PAGE CONTENT ===== */}
          <div
            style={{
              background: "#fff",
              borderRadius: 6,
              padding: 16,
              minHeight: 600,
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
