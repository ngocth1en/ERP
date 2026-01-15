import {
  Table,
  Typography,
  Row,
  Col,
  Input,
  Select,
  Switch,
  message,
} from "antd";
import { useEffect, useState } from "react";
import { useUiCommandStore } from "../../../stores/ui-command.store";
import { getUserByUserName } from "../../../api/user.api";
import { getSystems } from "../../../api/system.api";
import {
  getAllPermissionUserOnSystem,
  bulkUpdateUserMenuPermission,
} from "../../../api/menu.api";

const { Title } = Typography;

/* ======================
   TYPES
====================== */
interface UserDto {
  userName: string;
}

interface PermissionRow {
  menuCode: string;
  menuName: string;
  permissionMask: string;
}

/* ======================
   CONSTANT
====================== */
const PERMISSION_COLUMNS = [
  "Query",
  "Insert",
  "Modify",
  "Delete",
  "Confirm",
  "Unlock",
  "Print",
  "Excel",
  "Special1",
  "Special2",
];

const PAGE_SIZE = 5;

const SystemPermissionPage = () => {
  /* ======================
     TOOLBAR
  ====================== */
  const command = useUiCommandStore((s) => s.command);
  const clearCommand = useUiCommandStore((s) => s.clear);

  /* ======================
     STATE
  ====================== */
  const [keyword, setKeyword] = useState("");
  const [users, setUsers] = useState<UserDto[]>([]);
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);

  const [systems, setSystems] = useState<any[]>([]);
  const [systemCode, setSystemCode] = useState<string>();

  const [permissions, setPermissions] = useState<PermissionRow[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  /* ======================
     LOAD SYSTEM
  ====================== */
  useEffect(() => {
    getSystems().then(setSystems);
  }, []);

  /* ======================
     TOOLBAR HANDLER
  ====================== */
  useEffect(() => {
    if (!command) return;

    switch (command) {
      case "QUERY":
        handleQuery();
        break;
      case "MODIFY":
        handleSave();
        break;
    }

    clearCommand();
  }, [command]);

  /* ======================
     HANDLER
  ====================== */

  const handleQuery = async () => {
    // const data = await getUsers();

    // setUsers(
    //   keyword
    //     ? data.filter((u: UserDto) =>
    //         u.userName.toLowerCase().includes(keyword.toLowerCase())
    //       )
    //     : data
    // );
    const data = await getUserByUserName(keyword);
    setUsers(data);
  };

  const loadPermission = async (
    user: UserDto,
    sysCode: string,
    pageIndex = page
  ) => {
    setLoading(true);
    try {
      const res = await getAllPermissionUserOnSystem({
        userName: user.userName,
        systemCode: sysCode,
        page: pageIndex,
        pageSize: PAGE_SIZE,
      });

      setPermissions(
        res.items.map((x) => ({
          ...x,
          permissionMask: x.permissionMask.padEnd(10, "0"),
        }))
      );
      setTotal(res.total);
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    if (!selectedUser || !systemCode) {
      message.warning("Chọn user và hệ thống");
      return;
    }

    await bulkUpdateUserMenuPermission({
      userName: selectedUser.userName,
      systemCode,
      menus: permissions.map((p) => ({
        menuCode: p.menuCode,
        permissionMask: p.permissionMask,
      })),
    });

    message.success("Cập nhật phân quyền thành công");
  };

  /* ======================
     TOGGLE PERMISSION
  ====================== */
  const togglePermission = (
    row: PermissionRow,
    index: number,
    checked: boolean
  ) => {
    const chars = row.permissionMask.split("");
    chars[index] = checked ? "1" : "0";
    row.permissionMask = chars.join("");
    setPermissions([...permissions]);
  };

  /* ======================
     RENDER
  ====================== */
  return (
    <>
      <Title level={4}>Phân quyền người dùng</Title>

      {/* ===== FILTER ===== */}
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={6}>
          <Input
            placeholder="Username"
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
          />
        </Col>

        <Col span={6}>
          <Select
            placeholder="Hệ thống"
            style={{ width: "100%" }}
            value={systemCode}
            onChange={(v) => {
              setSystemCode(v);
              if (selectedUser) loadPermission(selectedUser, v, 1);
            }}
            options={systems.map((s) => ({
              value: s.code,
              label: s.name,
            }))}
          />
        </Col>
      </Row>

      {/* ===== TABLE ===== */}
      <Row gutter={16}>
        {/* USER */}
        <Col span={8}> {/* Có thể tăng span từ 6 lên 8 hoặc 10 để đủ chỗ hiển thị 2 cột */}
          <Table
            rowKey="userName"
            size="small"
            dataSource={users}
            rowClassName={(r) =>
              r.userName === selectedUser?.userName ? "row-selected" : ""
            }
            onRow={(r) => ({
              onClick: () => {
                setSelectedUser(r);
                if (systemCode) loadPermission(r, systemCode, 1);
              },
            })}
            columns={[
              {
                title: "Tài khoản",
                dataIndex: "userName",
                width: '40%' // Set chiều rộng cố định nếu cần
              },
              {
                title: "Họ và tên",
                dataIndex: "fullName", 
                width: '60%'
              }
            ]}
          />
        </Col>


        {/* PERMISSION */}
        <Col span={18}>
          <Table
            rowKey="menuCode"
            size="small"
            loading={loading}
            dataSource={permissions}
            pagination={{
              current: page,
              pageSize: PAGE_SIZE,
              total,
              onChange: (p) => {
                setPage(p);
                if (selectedUser && systemCode)
                  loadPermission(selectedUser, systemCode, p);
              },
            }}
            scroll={{ x: 1400 }}
            columns={[
              {
                title: "Mã menu",
                dataIndex: "menuCode",
                fixed: "left",
                width: 120,
              },
              {
                title: "Tên menu",
                dataIndex: "menuName",
                fixed: "left",
                width: 200,
              },
              ...PERMISSION_COLUMNS.map((p, index) => ({
                title: p,
                width: 90,
                align: "center" as const,
                render: (_: any, r: PermissionRow) => (
                  <Switch
                    checked={r.permissionMask[index] === "1"}
                    onChange={(checked) =>
                      togglePermission(r, index, checked)
                    }
                  />
                ),
              })),
            ]}
          />
        </Col>
      </Row>
    </>
  );
};

export default SystemPermissionPage;
