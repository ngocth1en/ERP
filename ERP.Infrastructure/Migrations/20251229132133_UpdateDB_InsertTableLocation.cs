using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB_InsertTableLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankNum",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageLocationNo",
                table: "Assets_Information",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StorageLocations",
                columns: table => new
                {
                    StorageLocationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameVN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    inUse = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLocations", x => x.StorageLocationNo);
                    table.ForeignKey(
                        name: "FK_StorageLocations_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { "ADMIN", "AD", "Quản trị hệ thống" });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Key", "LanguageCode", "Value" },
                values: new object[,]
                {
                    { new Guid("ba4c37bf-47b2-41fe-9dff-57df0b2b1631"), "MENU_ASSET", "vi", "Quản lý tài sản" },
                    { new Guid("db594786-9d67-48de-9578-3d9632f9386a"), "MENU_ASSET", "en", "Asset Management" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BankName", "BankNum", "DepartmentId", "Email", "FName", "LName", "MustChangePassword", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), null, null, null, "ADMIN", null, null, null, false, "$2a$11$EiNLhO24Rv3N08PBYJgdrusVy5cNZbD4kiPcU/16TYn64oSb3Oo1.", null, "999999" });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Information_StorageLocationNo",
                table: "Assets_Information",
                column: "StorageLocationNo");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_DepartmentId",
                table: "StorageLocations",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Information_StorageLocations_StorageLocationNo",
                table: "Assets_Information",
                column: "StorageLocationNo",
                principalTable: "StorageLocations",
                principalColumn: "StorageLocationNo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Information_StorageLocations_StorageLocationNo",
                table: "Assets_Information");

            migrationBuilder.DropTable(
                name: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Information_StorageLocationNo",
                table: "Assets_Information");

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("ba4c37bf-47b2-41fe-9dff-57df0b2b1631"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("db594786-9d67-48de-9578-3d9632f9386a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: "ADMIN");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankNum",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StorageLocationNo",
                table: "Assets_Information");
        }
    }
}
