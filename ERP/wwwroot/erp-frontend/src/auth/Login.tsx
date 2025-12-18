import { Button, Card, Form, Input } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "./auth.store";

const Login = () => {
  const login = useAuthStore((s) => s.login);
  const navigate = useNavigate();

  const onFinish = () => {
    login();               // mock login
    navigate("/", { replace: true });
  };

  return (
    <div
      style={{
        height: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        background: "#f5f5f5",
      }}
    >
      <Card title="ERP Login" style={{ width: 360 }}>
        <Form layout="vertical" onFinish={onFinish}>
          <Form.Item
            label="Username"
            name="username"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            label="Password"
            name="password"
            rules={[{ required: true }]}
          >
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
