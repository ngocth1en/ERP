import { ASSET_PERMISSION } from "./asset.sidebar.permission";

export interface SidebarItem {
  key: string;
  label: string;
  path?: string;
  permission?: string;
  children?: SidebarItem[];
}

export const assetSidebar: SidebarItem[] = [
  /* ================= TÀI SẢN CỐ ĐỊNH ================= */
  {
    key: "asset-master",
    label: "Tài sản cố định",
    children: [
      {
        key: "location",
        label: "Địa điểm",
        path: "/asset/locations",
        permission: ASSET_PERMISSION.LOCATION,
      },
      {
        key: "position",
        label: "Vị trí",
        path: "/asset/positions",
        permission: ASSET_PERMISSION.POSITION,
      },
      {
        key: "unit",
        label: "Đơn vị tài sản",
        path: "/asset/units",
        permission: ASSET_PERMISSION.UNIT,
      },
      {
        key: "category",
        label: "Phân loại",
        path: "/asset/categories",
        permission: ASSET_PERMISSION.CATEGORY,
      },
      {
        key: "model",
        label: "Mô hình tài sản",
        path: "/asset/models",
        permission: ASSET_PERMISSION.MODEL,
      },
      {
        key: "asset-info",
        label: "Thông tin tài sản",
        path: "/asset/assets",
        permission: ASSET_PERMISSION.ASSET_INFO,
      },
    ],
  },

  /* ================= CHỨC NĂNG GIAO DỊCH ================= */
  {
    key: "asset-transaction",
    label: "Chức năng giao dịch",
    children: [
      {
        key: "inventory",
        label: "Kiểm kê",
        path: "/asset/inventory",
        permission: ASSET_PERMISSION.INVENTORY,
      },
      {
        key: "handover",
        label: "Bàn giao",
        path: "/asset/handovers",
        permission: ASSET_PERMISSION.HANDOVER,
      },
      {
        key: "borrow-return",
        label: "Mượn / Trả",
        path: "/asset/borrow-return",
        permission: ASSET_PERMISSION.BORROW_RETURN,
      },
      {
        key: "scrap",
        label: "Báo phế",
        path: "/asset/scraps",
        permission: ASSET_PERMISSION.SCRAP,
      },
      {
        key: "liquidation",
        label: "Thanh lý",
        path: "/asset/liquidations",
        permission: ASSET_PERMISSION.LIQUIDATION,
      },
      {
        key: "repair",
        label: "Đơn sửa chữa",
        path: "/asset/repair-orders",
        permission: ASSET_PERMISSION.REPAIR_ORDER,
      },
      {
        key: "maintenance",
        label: "Bảo trì máy",
        path: "/asset/maintenance",
        permission: ASSET_PERMISSION.MAINTENANCE,
      },
      {
        key: "manual",
        label: "Tài liệu hướng dẫn",
        path: "/asset/manuals",
        permission: ASSET_PERMISSION.MANUAL,
      },
      {
        key: "periodic-maintenance",
        label: "Định kỳ bảo dưỡng",
        path: "/asset/periodic-maintenance",
        permission: ASSET_PERMISSION.PERIODIC_MAINTENANCE,
      },
    ],
  },

  /* ================= BÁO BIỂU ================= */
  {
    key: "asset-report",
    label: "Báo biểu",
    children: [
      {
        key: "asset-unit-report",
        label: "Tài sản - Đơn vị",
        path: "/asset/reports/asset-unit",
        permission: ASSET_PERMISSION.REPORT_ASSET_UNIT,
      },
      {
        key: "asset-search",
        label: "Tra cứu tài sản",
        path: "/asset/reports/search",
        permission: ASSET_PERMISSION.REPORT,
      },
      {
        key: "qrcode",
        label: "QRCode thẻ giấy",
        path: "/asset/reports/qrcode",
        permission: ASSET_PERMISSION.QRCODE,
      },
      {
        key: "handover-report",
        label: "Tra bàn giao",
        path: "/asset/reports/handovers",
        permission: ASSET_PERMISSION.REPORT_HANDOVER,
      },
      {
        key: "mechanical",
        label: "Cơ điện quản lý",
        path: "/asset/reports/mechanical",
        permission: ASSET_PERMISSION.MECHANICAL,
      },
      {
        key: "group-quantity",
        label: "Tập đoàn số lượng quản",
        path: "/asset/reports/group-quantity",
        permission: ASSET_PERMISSION.GROUP_QUANTITY,
      },
      {
        key: "device-management",
        label: "Thiết bị quản lý",
        path: "/asset/reports/devices",
        permission: ASSET_PERMISSION.DEVICE,
      },
    ],
  },
];
