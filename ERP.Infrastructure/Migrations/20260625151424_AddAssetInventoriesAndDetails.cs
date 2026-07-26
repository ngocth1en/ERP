using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetInventoriesAndDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset_Inventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Model_Id = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Inventory_Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "1"),
                    ExpectedCount = table.Column<int>(type: "int", nullable: false),
                    Counted = table.Column<int>(type: "int", nullable: false),
                    MissingCount = table.Column<int>(type: "int", nullable: false),
                    UnexpectedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Inventories_Asset_Models_Model_Id",
                        column: x => x.Model_Id,
                        principalTable: "Asset_Models",
                        principalColumn: "Model_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Inventory_Details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "FOUND"),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Inventory_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Inventory_Details_Asset_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Asset_Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Inventories_Model_Id",
                table: "Asset_Inventories",
                column: "Model_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Inventories_DepartmentId",
                table: "Asset_Inventories",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Inventories_CreatedAt",
                table: "Asset_Inventories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Inventory_Details_InventoryId",
                table: "Asset_Inventory_Details",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Inventory_Details_AssetNo",
                table: "Asset_Inventory_Details",
                column: "AssetNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asset_Inventory_Details");

            migrationBuilder.DropTable(
                name: "Asset_Inventories");
        }
    }
}