using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewForAssetMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemMenus",
                columns: new[] { "Code", "GroupCode", "Name", "OrderNo", "Route", "SystemCode" },
                values: new object[,]
                {
                    { "ASSET001", "ASSET_MASTER", "Địa điểm", 1, "/asset/locations", "ASSET" },
                    { "ASSET002", "ASSET_MASTER", "Vị trí", 2, "/asset/positions", "ASSET" },
                    { "ASSET003", "ASSET_MASTER", "Đơn vị tài sản", 3, "/asset/units", "ASSET" },
                    { "ASSET004", "ASSET_MASTER", "Phân loại", 4, "/asset/categories", "ASSET" },
                    { "ASSET005", "ASSET_MASTER", "Mô hình tài sản", 5, "/asset/models", "ASSET" },
                    { "ASSET006", "ASSET_MASTER", "Thông tin tài sản", 6, "/asset/assets", "ASSET" },
                    { "ASSET101", "ASSET_TRANSACTION", "Kiểm kê", 1, "/asset/inventory", "ASSET" },
                    { "ASSET102", "ASSET_TRANSACTION", "Bàn giao", 2, "/asset/handovers", "ASSET" },
                    { "ASSET103", "ASSET_TRANSACTION", "Mượn / Trả", 3, "/asset/borrow-return", "ASSET" },
                    { "ASSET104", "ASSET_TRANSACTION", "Báo phế", 4, "/asset/scraps", "ASSET" },
                    { "ASSET105", "ASSET_TRANSACTION", "Thanh lý", 5, "/asset/liquidations", "ASSET" },
                    { "ASSET106", "ASSET_TRANSACTION", "Đơn sửa chữa", 6, "/asset/repair-orders", "ASSET" },
                    { "ASSET107", "ASSET_TRANSACTION", "Bảo trì máy", 7, "/asset/maintenance", "ASSET" },
                    { "ASSET108", "ASSET_TRANSACTION", "Tài liệu hướng dẫn", 8, "/asset/manuals", "ASSET" },
                    { "ASSET109", "ASSET_TRANSACTION", "Định kỳ bảo dưỡng", 9, "/asset/periodic-maintenance", "ASSET" },
                    { "ASSET201", "ASSET_REPORT", "Tài sản - Đơn vị", 1, "/asset/reports/asset-unit", "ASSET" },
                    { "ASSET202", "ASSET_REPORT", "Tra cứu tài sản", 2, "/asset/reports/search", "ASSET" },
                    { "ASSET203", "ASSET_REPORT", "QRCode thẻ giấy", 3, "/asset/reports/qrcode", "ASSET" },
                    { "ASSET204", "ASSET_REPORT", "Tra bàn giao", 4, "/asset/reports/handovers", "ASSET" },
                    { "ASSET205", "ASSET_REPORT", "Cơ điện quản lý", 5, "/asset/reports/mechanical", "ASSET" },
                    { "ASSET206", "ASSET_REPORT", "Tập đoàn số lượng quản", 6, "/asset/reports/group-quantity", "ASSET" },
                    { "ASSET207", "ASSET_REPORT", "Thiết bị quản lý", 7, "/asset/reports/devices", "ASSET" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET001");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET002");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET003");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET004");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET005");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET006");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET101");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET102");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET103");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET104");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET105");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET106");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET107");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET108");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET109");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET201");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET202");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET203");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET204");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET205");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET206");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "ASSET207");
        }
    }
}
