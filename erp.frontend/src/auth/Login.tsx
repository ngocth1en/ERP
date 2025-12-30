import { Button, Card, Form, Input, Select, Typography } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "./auth.store";
import { getSystemsByUser } from "../api/system.api";
import { loginApi } from "../api/auth.api";
import { useEffect, useMemo, useState } from "react";
import type { SystemCode } from "../constants/system";

import "./login.css";

const { Title, Text } = Typography;

type SeasonKey = "winter" | "spring" | "summer" | "autumn";

/* ================= THEME THEO MÙA ================= */
const seasonThemes = {
  spring: {
    bg: "linear-gradient(135deg, #d4fc79, #96e6a1)",
    icon: "🌸",
    title: "Spring ERP"
  },
  summer: {
    bg: "linear-gradient(135deg, #89f7fe, #66a6ff)",
    icon: "☀️",
    title: "Summer ERP"
  },
  autumn: {
    bg: "linear-gradient(135deg, #fbc2eb, #a18cd1)",
    icon: "🍁",
    title: "Autumn ERP"
  },
  winter: {
    bg: "linear-gradient(135deg, #141e30, #243b55)",
    icon: "❄️",
    title: "Winter ERP"
  }
};

/* ================= XÁC ĐỊNH MÙA ================= */
const getSeason = (): {
  key: SeasonKey;
  bg: string;
  icon: string;
  title: string;
} => {
  const m = new Date().getMonth() + 1;
  if (m <= 3) return { key: "spring", ...seasonThemes.spring };
  if (m <= 6) return { key: "summer", ...seasonThemes.summer };
  if (m <= 9) return { key: "autumn", ...seasonThemes.autumn };
  return { key: "winter", ...seasonThemes.winter };
};

/* ================= ICON THEO SYSTEM ================= */
const systemIcons: Partial<Record<SystemCode, string>> = {
  ASSET: "🖥️",
  MAINT: "🛠️",
  QC: "🧪"
};

/* ================= TUYẾT RƠI ================= */
const renderSnow = (count = 26) =>
  Array.from({ length: count }).map((_, i) => (
    <span
      key={i}
      className="snow"
      style={{
        left: `${Math.random() * 100}%`,
        fontSize: `${10 + Math.random() * 14}px`,
        animationDuration: `${6 + Math.random() * 8}s`,
        animationDelay: `${Math.random() * 6}s`
      }}
    >
      ❄️
    </span>
  ));

/* ================= COMPONENT ================= */
const Login = () => {
  const login = useAuthStore((s) => s.login);
  const switchSystem = useAuthStore((s) => s.switchSystem);
  const navigate = useNavigate();

  const [systems, setSystems] = useState<SystemCode[]>([]);
  const [username, setUsername] = useState("");
  const [selectedSystem, setSelectedSystem] = useState<SystemCode | null>(null);
  const [loadingSystem, setLoadingSystem] = useState(false);

  const theme = useMemo(() => getSeason(), []);

  /* ===== LOAD SYSTEM THEO USERNAME (GIỮ NGUYÊN LOGIC) ===== */
  useEffect(() => {
    if (!username) {
      setSystems([]);
      setSelectedSystem(null);
      return;
    }

    const timer = setTimeout(async () => {
      setLoadingSystem(true);
      try {
        const data = await getSystemsByUser(username);
        setSystems(data.map((s) => s.code as SystemCode));
      } finally {
        setLoadingSystem(false);
      }
    }, 300);

    return () => clearTimeout(timer);
  }, [username]);

  const onFinish = async (values: {
    username: string;
    password: string;
    system: SystemCode;
  }) => {
    const res = await loginApi(values);
    login(res.token);
    switchSystem(values.system);
    navigate(`/${values.system.toLowerCase()}`);
  };

  return (
    <div
      style={{
        height: "100vh",
        background: theme.bg,
        display: "flex",
        justifyContent: "center",
        alignItems: "center"
      }}
    >
      <Card
        hoverable
        className={`login-card season-${theme.key}`}
        style={{
          width: 420,
          borderRadius: 22,
          backdropFilter: "blur(14px)"
        }}
        bodyStyle={{ padding: 30, position: "relative", zIndex: 2 }}
      >
        {/* ===== DECOR THEO MÙA ===== */}
        {theme.key === "winter" && (
          <>
            <div className="season-decor decor-left">🎄</div>
            <div className="season-decor decor-right">🎅</div>
            {renderSnow()}
          </>
        )}

        {/* ===== HEADER ===== */}
        <div style={{ textAlign: "center", marginBottom: 26 }}>
          <div className="header-icon">
            {selectedSystem
              ? systemIcons[selectedSystem] || theme.icon
              : theme.icon}
          </div>

          <Title level={3} className="login-title">
            {theme.title}
          </Title>

          <Text type="secondary" className="login-subtitle">
            Smart Enterprise Resource Planning
          </Text>
        </div>

        {/* ===== FORM ===== */}
        <Form layout="vertical" onFinish={onFinish}>
          <Form.Item
            label={<span className="fancy-label">🧩 System</span>}
            name="system"
            rules={[{ required: true }]}
          >
            <Select
              size="large"
              disabled={!username}
              loading={loadingSystem}
              placeholder="Select system"
              onChange={(v) => setSelectedSystem(v)}
              options={systems.map((s) => ({
                label: `${systemIcons[s] || "🧩"} ${s}`,
                value: s
              }))}
            />
          </Form.Item>

          <Form.Item
            label={<span className="fancy-label">👤 Username</span>}
            name="username"
            rules={[{ required: true }]}
          >
            <Input
              size="large"
              placeholder="Enter username"
              onChange={(e) => setUsername(e.target.value.trim())}
            />
          </Form.Item>

          <Form.Item
            label={<span className="fancy-label">🔒 Password</span>}
            name="password"
            rules={[{ required: true }]}
          >
            <Input.Password size="large" placeholder="Enter password" />
          </Form.Item>

          <Button
            type="primary"
            htmlType="submit"
            size="large"
            block
            className="login-btn"
          >
            🚀 Login
          </Button>
        </Form>
      </Card>
    </div>
  );
};

export default Login;
