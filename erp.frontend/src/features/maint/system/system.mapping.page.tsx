import {
  Table,
  Row,
  Col,
  Button,
  Typography,
  message,
} from "antd";
import { useEffect, useState } from "react";
import {
  getSystemsByUser,
  getSystemsByUserNotAllowed,
} from "../../../api/system.api";
import { getUsers } from "../../../api/user.api";
import {
  grantSystemToUser,
  revokeSystemFromUser,
} from "../../../api/system.api";

const { Title } = Typography;

/* ======================
   TYPES
====================== */
interface UserDto {
  userName: string;
  fullName: string;
}

interface SystemOptionDto {
  code: string;
  name: string;
}

/* ======================
   COMPONENT
====================== */
const UserSystemMappingPage = () => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);

  const [allowedSystems, setAllowedSystems] = useState<SystemOptionDto[]>([]);
  const [notAllowedSystems, setNotAllowedSystems] = useState<SystemOptionDto[]>([]);

  const [selectedAllowed, setSelectedAllowed] =
    useState<SystemOptionDto | null>(null);
  const [selectedNotAllowed, setSelectedNotAllowed] =
    useState<SystemOptionDto | null>(null);

  /* ======================
     LOAD USERS
  ====================== */
  useEffect(() => {
    getUsers().then(setUsers);
  }, []);

  /* ======================
     LOAD SYSTEMS
  ====================== */
  const loadSystems = async (username: string) => {
    const [allowed, notAllowed] = await Promise.all([
      getSystemsByUser(username),
      getSystemsByUserNotAllowed(username),
    ]);

    setAllowedSystems(allowed);
    setNotAllowedSystems(notAllowed);
    setSelectedAllowed(null);
    setSelectedNotAllowed(null);
  };

  /* ======================
     ACTIONS
  ====================== */
  const handleGrant = async () => {
    if (!selectedUser || !selectedNotAllowed) return;

    await grantSystemToUser({
      userName: selectedUser.userName,
      systemCode: selectedNotAllowed.code,
    });

    message.success("Đã cấp hệ thống");
    loadSystems(selectedUser.userName);
  };

  const handleRevoke = async () => {
    if (!selectedUser || !selectedAllowed) return;

    await revokeSystemFromUser({
      userName: selectedUser.userName,
      systemCode: selectedAllowed.code,
    });

    message.success("Đã thu hồi hệ thống");
    loadSystems(selectedUser.userName);
  };

  /* ======================
     RENDER
  ====================== */
  return (
    <>
      <Title level={4}>Phân quyền hệ thống theo người dùng</Title>

      <Row gutter={16}>
        {/* USERS */}
        <Col span={6}>
          <Table
            rowKey="userName"
            size="small"
            dataSource={users}
            title={() => "Người dùng"}
            onRow={(r) => ({
              onClick: () => {
                setSelectedUser(r);
                loadSystems(r.userName);
              },
            })}
            rowClassName={(r) =>
              r.userName === selectedUser?.userName ? "row-selected" : ""
            }
            columns={[
              { title: "User", dataIndex: "userName" },
              { title: "Tên", dataIndex: "fullName" },
            ]}
          />
        </Col>

        {/* ALLOWED */}
        <Col span={7}>
          <Table
            rowKey="code"
            size="small"
            title={() => "Hệ thống đã cấp"}
            dataSource={allowedSystems}
            onRow={(r) => ({
              onClick: () => setSelectedAllowed(r),
            })}
            rowClassName={(r) =>
              r.code === selectedAllowed?.code ? "row-selected" : ""
            }
            columns={[{ title: "Hệ thống", dataIndex: "name" }]}
          />
        </Col>

        {/* BUTTON */}
        <Col
          span={4}
          style={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            gap: 12,
          }}
        >
          <Button disabled={!selectedAllowed} onClick={handleRevoke}>
            {"<<"}
          </Button>
          <Button disabled={!selectedNotAllowed} onClick={handleGrant}>
            {">>"}
          </Button>
        </Col>

        {/* NOT ALLOWED */}
        <Col span={7}>
          <Table
            rowKey="code"
            size="small"
            title={() => "Hệ thống chưa cấp"}
            dataSource={notAllowedSystems}
            onRow={(r) => ({
              onClick: () => setSelectedNotAllowed(r),
            })}
            rowClassName={(r) =>
              r.code === selectedNotAllowed?.code ? "row-selected" : ""
            }
            columns={[{ title: "Hệ thống", dataIndex: "name" }]}
          />
        </Col>
      </Row>
    </>
  );
};

export default UserSystemMappingPage;
