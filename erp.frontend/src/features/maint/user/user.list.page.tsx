import { Table, Typography } from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../../stores/ui-command.store";
import { getUsers } from "../../../api/user.api";

const { Title } = Typography;

const UserListPage = () => {
  const command = useUiCommandStore((s) => s.command);
  const clear = useUiCommandStore((s) => s.clear);

  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (command !== "QUERY") return;

    setLoading(true);
    getUsers()
      .then(setData)
      .finally(() => {
        setLoading(false);
        clear();
      });
      console.log("users:", data, Array.isArray(data));

  }, [command, clear]);

  return (
    <>
      <Title level={4}>Dữ liệu người dùng</Title>

      <Table
        rowKey="userName"
        loading={loading}
        dataSource={data}
        size="small"
        pagination={{ pageSize: 20 }}
        columns={[
          {
            title: "Tên đăng nhập",
            dataIndex: "userName",
          },
          {
            title: "Họ tên",
            dataIndex: "fullName",
          },
          {
            title: "Email",
            dataIndex: "email",
          },
          {
            title: "Lần đăng nhập cuối",
            dataIndex: "lastLogin",
            render: (v) => (v ? new Date(v).toLocaleString() : ""),
          },
        ]}
      />
    </>
  );
};

export default UserListPage;
