import type { MenuProps } from "antd";
import { assetSidebar } from "../features/assets/asset.sidebar";
import { mapSidebarToMenuItems } from "./sidebar.mapper";

export const menuBySystem: Record<string, MenuProps["items"]> = {
  ASSET: mapSidebarToMenuItems(assetSidebar),

  QCM: [
    {
      key: "qc",
      label: "Kiểm nghiệm",
    },
  ],

  HRM: [
    {
      key: "employee",
      label: "Nhân sự",
    },
  ],
};
