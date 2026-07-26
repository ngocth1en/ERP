using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewBaseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "SystemMenus",
                columns: new[] { "Code", "GroupCode", "Name", "OrderNo", "Route", "SystemCode" },
                values: new object[,]
                {
                    { "MAINT001", "MAINT_ACCOUNT", "Thông tin người dùng", 1, "/maint/user/profile", "MAINT" },
                    { "MAINT002", "MAINT_ACCOUNT", "Cập nhật hệ thống", 2, "/maint/system/update", "MAINT" },
                    { "MAINT003", "MAINT_ACCOUNT", "Hướng dẫn sử dụng", 3, "/maint/help", "MAINT" },
                    { "MAINT101", "MAINT_SYSTEM", "Dữ liệu người dùng", 1, "/maint/system/users", "MAINT" },
                    { "MAINT102", "MAINT_SYSTEM", "Dữ liệu nhóm", 2, "/maint/system/groups", "MAINT" },
                    { "MAINT103", "MAINT_SYSTEM", "Dữ liệu xưởng", 3, "/maint/system/factories", "MAINT" },
                    { "MAINT104", "MAINT_SYSTEM", "Dữ liệu hệ thống", 4, "/maint/system/config", "MAINT" },
                    { "MAINT105", "MAINT_SYSTEM", "Dữ liệu cửa sổ hiển thị", 5, "/maint/system/windows", "MAINT" },
                    { "MAINT106", "MAINT_SYSTEM", "Dữ liệu mục lục", 6, "/maint/system/catalogs", "MAINT" },
                    { "MAINT201", "MAINT_LANGUAGE", "Ngôn ngữ", 1, "/maint/language", "MAINT" },
                    { "MAINT202", "MAINT_LANGUAGE", "Văn bản hiển thị", 2, "/maint/language/resources", "MAINT" },
                    { "MAINT301", "MAINT_PERMISSION", "Phân quyền thao tác", 1, "/maint/permission/action", "MAINT" },
                    { "MAINT302", "MAINT_PERMISSION", "Phân quyền hệ thống", 2, "/maint/permission/system", "MAINT" },
                    { "MAINT303", "MAINT_PERMISSION", "Phân quyền thương hiệu", 3, "/maint/permission/brand", "MAINT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT001");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT002");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT003");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT101");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT102");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT103");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT104");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT105");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT106");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT201");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT202");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT301");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT302");

            migrationBuilder.DeleteData(
                table: "SystemMenus",
                keyColumn: "Code",
                keyValue: "MAINT303");

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
    }
}
