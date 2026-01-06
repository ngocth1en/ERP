import { Button, Card, Form, Input, Select, Typography } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "./auth.store";
import { getSystemsByUser } from "../api/system.api";
import { loginApi } from "../api/auth.api";
import { useEffect, useMemo, useState } from "react";
import type { SystemCode } from "../constants/system";
import type { SeasonKey } from "../utils/season";
import { SeasonOrbCanvas } from "../components/season/SeasonOrbCanvas";
import "../components/season/seasonOrb.css";


import { SeasonParticles } from "../components/season/SeasonParticles";
import "./login.css";
import { SeasonModelCanvas } from "../components/season/SeasonModelCanvas";


const { Title, Text } = Typography;

/* ===== THEME ===== */
const seasonThemes: Record<
  SeasonKey,
  { bg: string; icon: string; title: string }
> = {
  spring: {
    bg: "linear-gradient(135deg,#d4fc79,#96e6a1)",
    icon: "🌸",
    title: "Spring ERP"
  },
  summer: {
    bg: "linear-gradient(135deg,#89f7fe,#66a6ff)",
    icon: "☀️",
    title: "Summer ERP"
  },
  autumn: {
    bg: "linear-gradient(135deg,#fbc2eb,#a18cd1)",
    icon: "🍁",
    title: "Autumn ERP"
  },
  winter: {
    bg: "linear-gradient(135deg,#141e30,#243b55)",
    icon: "❄️",
    title: "Winter ERP"
  }
};

const seasonDecor: Record<SeasonKey, { left: string; right: string }> = {
  winter: { left: "🎄", right: "🎅" },
  spring: { left: "🌸", right: "🦋" },
  summer: { left: "☀️", right: "🏖️" },
  autumn: { left: "🍁", right: "🌾" }
};

const getSeason = (): SeasonKey => {
  // const m = new Date().getMonth() + 1;
  // if (m <= 3) return "spring";
  // if (m <= 6) return "summer";
  // if (m <= 9) return "autumn";
  return "spring";
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
  const [selectedSystem, setSelectedSystem] =
    useState<SystemCode | null>(null);
  const [loadingSystem, setLoadingSystem] = useState(false);

  const seasonKey = useMemo(getSeason, []);
  const theme = seasonThemes[seasonKey];

  // useEffect(() => {
  //   if (!username) {
  //     setSystems([]);
  //     setSelectedSystem(null);
  //     return;
  //   }

  //   const t = setTimeout(async () => {
  //     setLoadingSystem(true);
  //     try {
  //       const data = await getSystemsByUser(username);
  //       setSystems(data.map((s) => s.code as SystemCode));
  //     } finally {
  //       setLoadingSystem(false);
  //     }
  //   }, 300);

  //   return () => clearTimeout(t);
  // }, [username]);
  useEffect(() => {
    if (!username) {
      setSystems([]);
      setSelectedSystem(null);
      return;
    }

    let cancelled = false;

    const t = setTimeout(async () => {
      setLoadingSystem(true);
      try {
        const data = await getSystemsByUser(username);
        if (!cancelled) {
          setSystems(data.map((s) => s.code as SystemCode));
        }
      } finally {
        if (!cancelled) {
          setLoadingSystem(false);
        }
      }
    }, 300);

    return () => {
      cancelled = true;
      clearTimeout(t);
    };
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
      {/* 🌍 BACKGROUND PHONG CẢNH */}
      <div className={`season-bg season-bg-${seasonKey}`} />

      {/* 🌦️ PARTICLES – NẰM SAU CARD */}
      <div className="season-particles">
        <SeasonParticles season={seasonKey} />
      </div>


      {/* 🌄 NGOẠI CẢNH */}
      <div className={`season-scenery scenery-${seasonKey}`}>
        {seasonKey === "winter" && (
          <>
            <div className="moon">🌕</div>
            <div className="snowman">⛄</div>
          </>
        )}
        {seasonKey === "spring" && <div className="big-flower">🌸</div>}
        {seasonKey === "summer" && <div className="sun">☀️</div>}
        {seasonKey === "autumn" && <div className="tree">🍁</div>}
      </div>
      {/* 🔮 3D ORB */}
      <div className="orb-wrapper">
        <div className="orb-container">
          <SeasonOrbCanvas season={seasonKey} />
        </div>
      </div>
      {/* 🎭 MODEL TƯƠNG TÁC */}
      <div className={`season-model-wrapper season-${seasonKey}`}>
        <div className="season-model-container">
          <SeasonModelCanvas season={seasonKey} />
        </div>
      </div>

      {/* 🪟 LOGIN */}
      <div className="login-page">
        <Card
          hoverable
          className={`login-card season-${seasonKey}`}
          bodyStyle={{ padding: 32 }}
          style={{ width: 420, borderRadius: 26 }}
        >
          {/* ICON SIGNATURE */}
          <div className="season-decor decor-left">
            {seasonDecor[seasonKey].left}
          </div>
          <div className="season-decor decor-right">
            {seasonDecor[seasonKey].right}
          </div>

          <div style={{ textAlign: "center", marginBottom: 24 }}>
            <div className="header-icon">
              {selectedSystem
                ? systemIcons[selectedSystem] || theme.icon
                : theme.icon}
            </div>
            <Title level={3}>{theme.title}</Title>
            <Text type="secondary">Welcome back 👋</Text>
          </div>

          <Form layout="vertical" onFinish={onFinish}>
            <Form.Item
              label="🧩 System"
              name="systemCode"
              rules={[{ required: true }]}
            >
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

            <Form.Item
              label="👤 Username"
              name="username"
              rules={[{ required: true }]}
            >
              <Input
                size="large"
                onChange={(e) => setUsername(e.target.value.trim())}
              />
            </Form.Item>

            <Form.Item
              label="🔒 Password"
              name="password"
              rules={[{ required: true }]}
            >
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
