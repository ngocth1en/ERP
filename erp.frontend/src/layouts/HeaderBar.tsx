import { Select, Dropdown, Avatar } from "antd";
import type { MenuProps } from "antd";
import {
  UserOutlined,
  LockOutlined,
  LogoutOutlined
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../auth/auth.store";
import type { SystemCode } from "../constants/system";
import { getSeasonKey } from "../utils/season";
import { SeasonParticles } from "../components/season/SeasonParticles";

const systemLabelMap: Record<SystemCode, string> = {
  ASSET: "Quản lý tài sản",
  QC: "Quản lý chất lượng",
  MAINT: "Quản lý hệ thống",
  DEVELOP:""
};

const HeaderBar = () => {
  const {
    systems,
    systemCode,
    switchSystem,
    username,
    logout
  } = useAuthStore();

  const navigate = useNavigate();
  const season = getSeasonKey();

  /* ===== MENU USER (AntD v5) ===== */
  const userMenuItems: MenuProps["items"] = [
    {
      key: "profile",
      label: "Thông tin tài khoản",
      icon: <UserOutlined />,
      onClick: () => navigate("/profile")
    },
    {
      key: "change-password",
      label: "Đổi mật khẩu",
      icon: <LockOutlined />,
      onClick: () => navigate("/change-password")
    },
    { type: "divider" },
    {
      key: "logout",
      label: "Đăng xuất",
      icon: <LogoutOutlined />,
      danger: true,
      onClick: () => {
        logout();
        navigate("/login");
      }
    }
  ];

  return (
    <div
      className={`header header-${season}`}
      style={{
        position: "relative",
        overflow: "hidden",
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        padding: "10px 18px",
        borderRadius: 18,
        fontFamily: '"Poppins", sans-serif',
        boxShadow: "0 10px 30px rgba(0,0,0,.2)"
      }}
    >
      {/* ===== HIỆU ỨNG MÙA ===== */}
      <SeasonParticles season={season} />

      {/* ===== LEFT ===== */}
      <div style={{ display: "flex", alignItems: "center", gap: 14 }}>
        <span style={{ fontSize: 22 }}>
          {season === "winter" && "❄️"}
          {season === "spring" && "🌸"}
          {season === "summer" && "☀️"}
          {season === "autumn" && "🍁"}
        </span>

        <span style={{ fontWeight: 600 }}>ERP SYSTEM</span>

        <Select<SystemCode>
          value={systemCode}
          variant="borderless"
          style={{
            width: 220,
            fontWeight: 600
          }}
          onChange={(value) => {
            switchSystem(value);
            navigate(`/${value.toLowerCase()}`);
          }}
          options={systems.map((s) => ({
            value: s,
            label: systemLabelMap[s]
          }))}
        />
      </div>

      {/* ===== RIGHT (USER) ===== */}
      <Dropdown menu={{ items: userMenuItems }} trigger={["click"]}>
        <div
          style={{
            display: "flex",
            alignItems: "center",
            gap: 10,
            cursor: "pointer",
            padding: "6px 14px",
            borderRadius: 16,
            background: "rgba(255,255,255,.15)",
            backdropFilter: "blur(8px)"
          }}
        >
          <Avatar
            icon={<UserOutlined />}
            style={{
              backgroundColor:
                season === "winter"
                  ? "#90caf9"
                  : season === "spring"
                  ? "#81c784"
                  : season === "summer"
                  ? "#ffb300"
                  : "#ff8a65"
            }}
          />
          <span style={{ fontWeight: 600 }}>{username}</span>
        </div>
      </Dropdown>
    </div>
  );
};

export default HeaderBar;
