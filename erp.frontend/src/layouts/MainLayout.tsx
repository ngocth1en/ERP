import { Layout, Drawer, Grid } from "antd";
import { useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import Sidebar from "./Sidebar";
import HeaderBar from "./HeaderBar";
import Toolbar from "./ToolBars";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";
import MainContent from "./MainContent";

const { Header, Content } = Layout;
const { useBreakpoint } = Grid;
const HEADER_HEIGHT = 64;

const MainLayout = () => {
    const location = useLocation();
    const { systemCode, switchSystem } = useAuthStore();
    const screens = useBreakpoint();
    const isMobile = !screens.md;
    const [drawerOpen, setDrawerOpen] = useState(false);

    // ===== GIỮ NGUYÊN LOGIC CŨ =====
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

    // Tự đóng drawer khi chuyển route (mobile)
    useEffect(() => {
        setDrawerOpen(false);
    }, [location.pathname]);

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
                <HeaderBar onMenuToggle={() => setDrawerOpen(true)} />
            </Header>

            {/* ===== BODY ===== */}
            <Layout style={{ height: `calc(100vh - ${HEADER_HEIGHT}px)` }}>
                {/* ===== SIDEBAR — desktop: Sider tự quản lý bên trong Sidebar ===== */}
                {!isMobile && <Sidebar />}

                {/* ===== SIDEBAR — mobile: nằm trong Drawer ===== */}
                {isMobile && (
                    <Drawer
                        placement="left"
                        open={drawerOpen}
                        onClose={() => setDrawerOpen(false)}
                        width={240}
                        styles={{
                            body: { padding: 0, background: "#001529" },
                            header: { display: "none" },
                        }}
                    >
                        <Sidebar onClose={() => setDrawerOpen(false)} />
                    </Drawer>
                )}

                {/* ===== CONTENT (SCROLL CONTAINER) ===== */}
                <Content
                    style={{
                        height: "100%",
                        minWidth: 0,         // ✅ fix: ngăn flex child tràn ngang
                        overflow: "auto",    // ✅ fix: cuộn cả 2 chiều thay vì chỉ dọc
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
                        <Toolbar isMobile={isMobile} />
                    </div>

                    {/* ===== PAGE CONTENT ===== */}
                    <div
                        style={{
                            background: "#fff",
                            borderRadius: 6,
                            padding: isMobile ? 8 : 16,
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