import { Button, Card, Form, Input, Select, Typography } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "./auth.store";
import { getSystemsByUser } from "../api/system.api";
import { loginApi } from "../api/auth.api";
import { useEffect, useMemo, useState } from "react";
import type { SystemCode } from "../constants/system";
import type { SeasonKey } from "../utils/season";

import { SeasonParticles } from "../components/season/SeasonParticles";
import "./login.css";

const { Title, Text } = Typography;

/* ===== THEME ===== */
const seasonThemes: Record<SeasonKey, { bg: string; icon: string; title: string }> =
{
  spring: { bg: "linear-gradient(135deg,#d4fc79,#96e6a1)", icon: "🌸", title: "Spring ERP" },
  summer: { bg: "linear-gradient(135deg,#89f7fe,#66a6ff)", icon: "☀️", title: "Summer ERP" },
  autumn: { bg: "linear-gradient(135deg,#fbc2eb,#a18cd1)", icon: "🍁", title: "Autumn ERP" },
  winter: { bg: "linear-gradient(135deg,#141e30,#243b55)", icon: "❄️", title: "Winter ERP" }
};

const seasonDecor: Record<SeasonKey, { left: string; right: string }> = {
  winter: { left: "🎄", right: "🎅" },
  spring: { left: "🌸", right: "🦋" },
  summer: { left: "☀️", right: "🏖️" },
  autumn: { left: "🍁", right: "🌾" }
};

const getSeason = (): SeasonKey => {
  const m = new Date().getMonth() + 1;
  if (m <= 3) return "spring";
  if (m <= 6) return "summer";
  if (m <= 9) return "autumn";
  return "winter";
};

const systemIcons: Partial<Record<SystemCode, string>> = {
  ASSET: "🖥️",
  MAINT: "🛠️",
  QC: "🧪"
};

const Login = () => {
  const login = useAuthStore((s) => s.login);
  const switchSystem = useAuthStore((s) => s.switchSystem);
  const navigate = useNavigate();

  const [systems, setSystems] = useState<SystemCode[]>([]);
  const [username, setUsername] = useState("");
  const [selectedSystem, setSelectedSystem] = useState<SystemCode | null>(null);
  const [loadingSystem, setLoadingSystem] = useState(false);

  const seasonKey = useMemo(getSeason, []);
  const theme = seasonThemes[seasonKey];

  useEffect(() => {
    if (!username) {
      setSystems([]);
      setSelectedSystem(null);
      return;
    }

    const t = setTimeout(async () => {
      setLoadingSystem(true);
      try {
        const data = await getSystemsByUser(username);
        setSystems(data.map((s) => s.code as SystemCode));
      } finally {
        setLoadingSystem(false);
      }
    }, 300);

    return () => clearTimeout(t);
  }, [username]);

  const onFinish = async (values: {
    username: string;
    password: string;
    systemCode: SystemCode;
  }) => {
    const res = await loginApi(values);
    login(res.accessToken);
    switchSystem(values.systemCode);
    navigate(`/${values.systemCode.toLowerCase()}`);
  };

  return (
    <>
      {/* 🌦️ BACKGROUND PARTICLES */}
      <SeasonParticles season={seasonKey} />

      <div
        className="login-page"
        style={{
          minHeight: "100vh",
          background: theme.bg,
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          position: "relative",
          overflow: "hidden"
        }}
      >
        <Card
          hoverable
          className={`login-card season-${seasonKey}`}
          style={{
            width: 420,
            borderRadius: 22,
            backdropFilter: "blur(14px)",
            position: "relative",
            zIndex: 10
          }}
          bodyStyle={{ padding: 30 }}
        >
          {/* 🎄 ICON SIGNATURE */}
          <div className="season-decor decor-left">
            {seasonDecor[seasonKey].left}
          </div>
          <div className="season-decor decor-right">
            {seasonDecor[seasonKey].right}
          </div>

          {/* HEADER */}
          <div style={{ textAlign: "center", marginBottom: 24 }}>
            <div className="header-icon">
              {selectedSystem
                ? systemIcons[selectedSystem] || theme.icon
                : theme.icon}
            </div>
            <Title level={3}>{theme.title}</Title>
            <Text type="secondary"></Text>
          </div>

          {/* FORM */}
          <Form layout="vertical" onFinish={onFinish}>
            <Form.Item label="🧩 System" name="systemCode" rules={[{ required: true }]}>
              <Select
                size="large"
                disabled={!username}
                loading={loadingSystem}
                onChange={(v) => setSelectedSystem(v)}
                options={systems.map((s) => ({
                  label: `${systemIcons[s] || "🧩"} ${s}`,
                  value: s
                }))}
              />
            </Form.Item>

            <Form.Item label="👤 Username" name="username" rules={[{ required: true }]}>
              <Input size="large" onChange={(e) => setUsername(e.target.value.trim())} />
            </Form.Item>

            <Form.Item label="🔒 Password" name="password" rules={[{ required: true }]}>
              <Input.Password size="large" />
            </Form.Item>

            <Button type="primary" htmlType="submit" size="large" block>
              🚀 Login
            </Button>
          </Form>
        </Card>
      </div>
    </>
  );
};

export default Login;
