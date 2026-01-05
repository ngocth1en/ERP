export const ASSET_PERMISSION = {
  /* ===== TÀI SẢN CỐ ĐỊNH ===== */
  LOCATION: "1000000001",
  POSITION: "1000000002",
  UNIT: "1000000003",
  CATEGORY: "1000000004",
  MODEL: "1000000005",
  ASSET_INFO: "1000000006",

  /* ===== CHỨC NĂNG GIAO DỊCH ===== */
  INVENTORY: "1000000010",
  HANDOVER: "1000000011",
  BORROW_RETURN: "1000000012",
  SCRAP: "1000000013",          // Báo phế
  LIQUIDATION: "1000000014",    // Thanh lý
  REPAIR_ORDER: "1000000015",   // Đơn sửa chữa
  MAINTENANCE: "1000000016",    // Bảo trì máy
  MANUAL: "1000000017",         // Tài liệu hướng dẫn
  PERIODIC_MAINTENANCE: "1000000018", // Định kỳ bảo dưỡng

  /* ===== BÁO BIỂU ===== */
  REPORT: "1000000020",                 // Tra cứu tài sản
  REPORT_ASSET_UNIT: "1000000021",      // Tài sản - Đơn vị
  QRCODE: "1000000022",                  // QRCode thẻ giấy
  REPORT_HANDOVER: "1000000023",         // Tra bàn giao
  MECHANICAL: "1000000024",              // Cơ điện quản lý
  GROUP_QUANTITY: "1000000025",          // Tập đoàn số lượng quản
  DEVICE: "1000000026",                  // Thiết bị quản lý
} as const;
