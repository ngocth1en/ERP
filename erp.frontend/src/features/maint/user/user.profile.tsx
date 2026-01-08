// import { Card, Descriptions, Avatar, Divider } from "antd";
// import { UserOutlined } from "@ant-design/icons";
// import { useAuthStore } from "../../../auth/auth.store";

// const UserProfile = () => {
//   const { username, systemCode } = useAuthStore();

//   return (
//     <Card title="Thông tin người dùng" bordered={false}>
//       <div style={{ display: "flex", gap: 24 }}>
//         <Avatar size={96} icon={<UserOutlined />} />

//         <Descriptions column={1}>
//           <Descriptions.Item label="Tên đăng nhập">
//             {username}
//           </Descriptions.Item>

//           <Descriptions.Item label="Hệ thống hiện tại">
//             {systemCode}
//           </Descriptions.Item>

//           <Descriptions.Item label="Vai trò">
//             Người dùng hệ thống
//           </Descriptions.Item>

//           <Descriptions.Item label="Trạng thái">
//             Đang hoạt động
//           </Descriptions.Item>
//         </Descriptions>
//       </div>

//       <Divider />

//       <p style={{ color: "#888" }}>
//         Đây là trang thông tin người dùng (chỉ xem).
//       </p>
//     </Card>
//   );
// };

// export default UserProfile;

import {
  Card,
  Descriptions,
  Avatar,
  Divider,
  Tag,
  Space,
  Typography
} from "antd";
import {
  UserOutlined,
  ApartmentOutlined,
  SafetyCertificateOutlined,
  CheckCircleOutlined
} from "@ant-design/icons";
import { useAuthStore } from "../../../auth/auth.store";

const { Title, Text } = Typography;

const UserProfile = () => {
  const { username, systemCode } = useAuthStore();

  return (
    <Card
      bordered={false}
      style={{
        maxWidth: 900,
        margin: "0 auto"
      }}
    >
      {/* HEADER */}
      <Space size={24} align="center">
        <Avatar
          size={96}
          icon={<UserOutlined />}
          style={{
            backgroundColor: "#1677ff"
          }}
        />

        <div>
          <Title level={4} style={{ marginBottom: 4 }}>
            {username}
          </Title>

          <Text type="secondary">
            Thông tin tài khoản người dùng
          </Text>

          <div style={{ marginTop: 8 }}>
            <Tag
              icon={<CheckCircleOutlined />}
              color="green"
            >
              Đang hoạt động
            </Tag>
          </div>
        </div>
      </Space>

      <Divider />

      {/* BODY */}
      <Descriptions
        bordered
        size="middle"
        column={1}
      >
        <Descriptions.Item
          label={
            <Space>
              <UserOutlined />
              Tên đăng nhập
            </Space>
          }
        >
          {username}
        </Descriptions.Item>

        <Descriptions.Item
          label={
            <Space>
              <ApartmentOutlined />
              Hệ thống hiện tại
            </Space>
          }
        >
          <Tag color="blue">
            {systemCode}
          </Tag>
        </Descriptions.Item>

        <Descriptions.Item
          label={
            <Space>
              <SafetyCertificateOutlined />
              Vai trò
            </Space>
          }
        >
          <Tag color="geekblue">
            Người dùng hệ thống
          </Tag>
        </Descriptions.Item>

        <Descriptions.Item label="Quyền truy cập">
          Chỉ xem và sử dụng chức năng được phân quyền
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      {/* FOOTER */}
      <Text type="secondary">
        ℹ️ Trang này chỉ hiển thị thông tin cơ bản.
        Nếu cần thay đổi mật khẩu hoặc quyền truy cập,
        vui lòng liên hệ quản trị hệ thống.
      </Text>
    </Card>
  );
};

export default UserProfile;
