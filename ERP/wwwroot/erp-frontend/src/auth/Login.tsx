import { Button, Card, Form, Input, Select } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "./auth.store";
import { getSystemsByUser } from "../api/system.api";
import { loginApi } from "../api/auth.api";
import { useEffect, useState } from "react";
import type { SystemCode } from "../constants/system";

const Login = () => {
  const login = useAuthStore((s) => s.login);
  const switchSystem = useAuthStore((s) => s.switchSystem);
  const navigate = useNavigate();

  const [systems, setSystems] = useState<SystemCode[]>([]);
  const [username, setUsername] = useState("");
  const [loadingSystem, setLoadingSystem] = useState(false);

  // 🔹 load systems theo username
  useEffect(() => {
    if (!username) {
      setSystems([]);
      return;
    }

    const timer = setTimeout(async () => {
      setLoadingSystem(true);
      try {
        const data = await getSystemsByUser(username);
        setSystems(data.map(s => s.Code as SystemCode));
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
    // ✅ gọi backend login
    const res = await loginApi({
      username: values.username,
      password: values.password,
      system: values.system
    });

    // ✅ lưu token + decode quyền
    login(res.token);

    // ✅ system do user chọn
    switchSystem(values.system);

    navigate(`/${values.system.toLowerCase()}`);
  };

  return (
    <div style={{ height: "100vh", display: "flex", justifyContent: "center", alignItems: "center" }}>
      <Card title="ERP Login" headStyle={{ textAlign: "center" }} style={{ width: 360 }}>
        <Form layout="vertical" onFinish={onFinish}>
          
          <Form.Item label="System" name="system" rules={[{ required: true }]}>
            <Select
              disabled={!username}
              loading={loadingSystem}
              options={systems.map((s) => ({
                label: s,
                value: s,
              }))}
            />
          </Form.Item>
          <Form.Item label="Username" name="username" rules={[{ required: true }]}>
            <Input onChange={(e) => setUsername(e.target.value.trim())} />
          </Form.Item>
          <Form.Item label="Password" name="password" rules={[{ required: true }]}>
            <Input.Password />
          </Form.Item>

          <Button type="primary" htmlType="submit" block>
            Login
          </Button>
        </Form>
      </Card>
    </div>
  );
};

export default Login;
