using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_SystemMenu_And_Permission_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* =====================================================
             * 1. DROP FK + PK PHỤ THUỘC SystemModules.Code
             * ===================================================== */

            migrationBuilder.DropForeignKey(
                name: "FK_UserSystems_SystemModules_SystemCode",
                table: "UserSystems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSystems",
                table: "UserSystems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemModules",
                table: "SystemModules");

            /* =====================================================
             * 2. ALTER COLUMN (FIX LENGTH)
             * ===================================================== */

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SystemModules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SystemCode",
                table: "UserSystems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            /* =====================================================
             * 3. ADD PK BACK
             * ===================================================== */

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemModules",
                table: "SystemModules",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSystems",
                table: "UserSystems",
                columns: new[] { "UserId", "SystemCode" });

            /* =====================================================
             * 4. ADD FK BACK
             * ===================================================== */

            migrationBuilder.AddForeignKey(
                name: "FK_UserSystems_SystemModules_SystemCode",
                table: "UserSystems",
                column: "SystemCode",
                principalTable: "SystemModules",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            /* =====================================================
             * 5. CREATE SystemMenus
             * ===================================================== */

            migrationBuilder.CreateTable(
                name: "SystemMenus",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SystemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Route = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMenus", x => x.Code);
                    table.ForeignKey(
                        name: "FK_SystemMenus_SystemModules_SystemCode",
                        column: x => x.SystemCode,
                        principalTable: "SystemModules",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemMenus_SystemCode",
                table: "SystemMenus",
                column: "SystemCode");

            /* =====================================================
             * 6. CREATE UserMenuPermissions
             * ===================================================== */

            migrationBuilder.CreateTable(
                name: "UserMenuPermissions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    MenuCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PermissionMask = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMenuPermissions", x => new { x.UserId, x.MenuCode });
                    table.ForeignKey(
                        name: "FK_UserMenuPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMenuPermissions_SystemMenus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "SystemMenus",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMenuPermissions_MenuCode",
                table: "UserMenuPermissions",
                column: "MenuCode");

            /* =====================================================
             * 7. SEED SystemMenus (GIỮ NGUYÊN)
             * ===================================================== */

            migrationBuilder.InsertData(
                table: "SystemMenus",
                columns: new[] { "Code", "SystemCode", "GroupCode", "Name", "Route", "OrderNo" },
                values: new object[,]
                {
            { "ASSET001", "ASSET", "ASSET_MASTER", "Địa điểm", "/asset/locations", 1 },
            { "ASSET002", "ASSET", "ASSET_MASTER", "Vị trí", "/asset/positions", 2 },
            { "ASSET003", "ASSET", "ASSET_MASTER", "Đơn vị tài sản", "/asset/units", 3 },
            { "ASSET004", "ASSET", "ASSET_MASTER", "Phân loại", "/asset/categories", 4 },
            { "ASSET005", "ASSET", "ASSET_MASTER", "Mô hình tài sản", "/asset/models", 5 },
            { "ASSET006", "ASSET", "ASSET_MASTER", "Thông tin tài sản", "/asset/assets", 6 },

            { "ASSET101", "ASSET", "ASSET_TRANSACTION", "Kiểm kê", "/asset/inventory", 1 },
            { "ASSET102", "ASSET", "ASSET_TRANSACTION", "Bàn giao", "/asset/handovers", 2 },
            { "ASSET103", "ASSET", "ASSET_TRANSACTION", "Mượn / Trả", "/asset/borrow-return", 3 },
            { "ASSET104", "ASSET", "ASSET_TRANSACTION", "Báo phế", "/asset/scraps", 4 },
            { "ASSET105", "ASSET", "ASSET_TRANSACTION", "Thanh lý", "/asset/liquidations", 5 },
            { "ASSET106", "ASSET", "ASSET_TRANSACTION", "Đơn sửa chữa", "/asset/repair-orders", 6 },
            { "ASSET107", "ASSET", "ASSET_TRANSACTION", "Bảo trì máy", "/asset/maintenance", 7 },
            { "ASSET108", "ASSET", "ASSET_TRANSACTION", "Tài liệu hướng dẫn", "/asset/manuals", 8 },
            { "ASSET109", "ASSET", "ASSET_TRANSACTION", "Định kỳ bảo dưỡng", "/asset/periodic-maintenance", 9 },

            { "ASSET201", "ASSET", "ASSET_REPORT", "Tài sản - Đơn vị", "/asset/reports/asset-unit", 1 },
            { "ASSET202", "ASSET", "ASSET_REPORT", "Tra cứu tài sản", "/asset/reports/search", 2 },
            { "ASSET203", "ASSET", "ASSET_REPORT", "QRCode thẻ giấy", "/asset/reports/qrcode", 3 },
            { "ASSET204", "ASSET", "ASSET_REPORT", "Tra bàn giao", "/asset/reports/handovers", 4 },
            { "ASSET205", "ASSET", "ASSET_REPORT", "Cơ điện quản lý", "/asset/reports/mechanical", 5 },
            { "ASSET206", "ASSET", "ASSET_REPORT", "Tập đoàn số lượng quản", "/asset/reports/group-quantity", 6 },
            { "ASSET207", "ASSET", "ASSET_REPORT", "Thiết bị quản lý", "/asset/reports/devices", 7 }
                });
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserMenuPermissions");
            migrationBuilder.DropTable(name: "SystemMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSystems_SystemModules_SystemCode",
                table: "UserSystems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSystems",
                table: "UserSystems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemModules",
                table: "SystemModules");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SystemModules",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SystemCode",
                table: "UserSystems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemModules",
                table: "SystemModules",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSystems",
                table: "UserSystems",
                columns: new[] { "UserId", "SystemCode" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserSystems_SystemModules_SystemCode",
                table: "UserSystems",
                column: "SystemCode",
                principalTable: "SystemModules",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

    }
}
