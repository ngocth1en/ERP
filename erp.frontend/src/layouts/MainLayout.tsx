import { Layout } from "antd";
import { Outlet, useLocation } from "react-router-dom";
import Sidebar from "./Sidebar";
import HeaderBar from "./HeaderBar";
import { useEffect } from "react";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";
import { Header } from "antd/es/layout/layout";
import Toolbar from "./ToolBars";

const MainLayout = () => {
  const location = useLocation();
  const { systemCode, switchSystem } = useAuthStore();

  useEffect(() => {
    const urlSystem = location.pathname.split("/")[1]?.toUpperCase();

    if (
      urlSystem &&
      ["ASM", "QCM", "HRM"].includes(urlSystem) &&
      urlSystem !== systemCode
    ) {
      switchSystem(urlSystem as SystemCode);
    }
  }, [location.pathname]);

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Sidebar />

      <Layout>
        <Header
          style={{
            background: "#fff",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <HeaderBar />
        </Header>
        <div style={{ padding: 12, borderBottom: "1px solid #eee" }}>
          <Toolbar />
        </div>
        <Layout.Content style={{ padding: 16 }}>
          <Outlet />
        </Layout.Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
