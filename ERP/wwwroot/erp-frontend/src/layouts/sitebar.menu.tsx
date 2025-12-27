import {
  AppstoreOutlined,
  DatabaseOutlined,
  TeamOutlined,
  FileSearchOutlined,
} from "@ant-design/icons";
import type { SidebarMenuItem } from "./sidebar.types";

export const menuBySystem: Record<string, SidebarMenuItem[]> = {
  ASM: [
    {
      key: "assets",
      icon: <DatabaseOutlined />,
      label: "Quản lý tài sản",
      path: "/asm/assets",
    },
    {
      key: "warehouse",
      icon: <AppstoreOutlined />,
      label: "Kho",
      disabled: true,
    },
  ],

  QCM: [
    {
      key: "qc",
      icon: <FileSearchOutlined />,
      label: "Kiểm nghiệm",
      path: "/qcm/check",
    },
  ],

  HRM: [
    {
      key: "employee",
      icon: <TeamOutlined />,
      label: "Nhân sự",
      path: "/hrm/employees",
    },
  ],
};
