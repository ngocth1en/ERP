import {
  Card,
  Descriptions,
  Avatar,
  Divider,
  Tag,
  Space,
  Typography,
  Button,
  Modal,
  Form,
  Input,
  message
} from "antd";

import {
  UserOutlined,
  ApartmentOutlined,
  SafetyCertificateOutlined,
  CheckCircleOutlined,
  LockOutlined
} from "@ant-design/icons";

import { useState } from "react";
import { useAuthStore } from "../../../auth/auth.store";
import { changePassword } from "../../../api/user.api";

const { Title, Text } = Typography;

const UserProfile = () => {
  const username = useAuthStore((s) => s.username);
  const systemCode = useAuthStore((s) => s.systemCode);
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  // =========================
  // HANDLE CHANGE PASSWORD
  // =========================
  const handleChangePassword = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);

      await changePassword(username!, {
        oldPassword: values.oldPassword,
        newPassword: values.newPassword
      });

      message.success("Đổi mật khẩu thành công");

      setOpen(false);
      form.resetFields();
    } catch (err: any) {
      if (err?.response?.data?.message) {
        message.error(err.response.data.message);
      } else if (err?.message) {
        message.error(err.message);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {/* ========================= */}
      {/* USER INFO CARD */}
      {/* ========================= */}
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
        <Space>
          <Button
            type="primary"
            icon={<LockOutlined />}
            onClick={() => setOpen(true)}
          >
            Đổi mật khẩu
          </Button>
        </Space>

        <Divider />
      </Card>

      {/* ========================= */}
      {/* CHANGE PASSWORD MODAL */}
      {/* ========================= */}
      <Modal
        title="Đổi mật khẩu"
        open={open}
        onCancel={() => setOpen(false)}
        onOk={handleChangePassword}
        confirmLoading={loading}
        okText="Lưu"
        cancelText="Hủy"
        destroyOnClose
      >
        <Form
          form={form}
          layout="vertical"
        >
          <Form.Item
            label="Mật khẩu hiện tại"
            name="oldPassword"
            rules={[
              { required: true, message: "Nhập mật khẩu hiện tại" }
            ]}
          >
            <Input.Password />
          </Form.Item>

          <Form.Item
            label="Mật khẩu mới"
            name="newPassword"
            rules={[
              { required: true, message: "Nhập mật khẩu mới" },
              { min: 6, message: "Mật khẩu tối thiểu 6 ký tự" }
            ]}
          >
            <Input.Password />
          </Form.Item>

          <Form.Item
            label="Xác nhận mật khẩu mới"
            name="confirmPassword"
            dependencies={["newPassword"]}
            rules={[
              { required: true, message: "Xác nhận mật khẩu mới" },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || value === getFieldValue("newPassword")) {
                    return Promise.resolve();
                  }
                  return Promise.reject(
                    new Error("Mật khẩu xác nhận không khớp")
                  );
                }
              })
            ]}
          >
            <Input.Password />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default UserProfile;
