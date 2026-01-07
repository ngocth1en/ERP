import { Card, Descriptions, Avatar, Divider } from "antd";
import { UserOutlined } from "@ant-design/icons";
import { useAuthStore } from "../../../auth/auth.store";

const UserProfile = () => {
  const { username, systemCode } = useAuthStore();

  return (
    <Card title="Thông tin người dùng" bordered={false}>
      <div style={{ display: "flex", gap: 24 }}>
        <Avatar size={96} icon={<UserOutlined />} />

        <Descriptions column={1}>
          <Descriptions.Item label="Tên đăng nhập">
            {username}
          </Descriptions.Item>

          <Descriptions.Item label="Hệ thống hiện tại">
            {systemCode}
          </Descriptions.Item>

          <Descriptions.Item label="Vai trò">
            Người dùng hệ thống
          </Descriptions.Item>

          <Descriptions.Item label="Trạng thái">
            Đang hoạt động
          </Descriptions.Item>
        </Descriptions>
      </div>

      <Divider />

      <p style={{ color: "#888" }}>
        Đây là trang thông tin người dùng (chỉ xem).
      </p>
    </Card>
  );
};

export default UserProfile;
