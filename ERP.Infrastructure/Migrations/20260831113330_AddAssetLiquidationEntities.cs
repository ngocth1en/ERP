using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetLiquidationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset_Liquidations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    LiquidationMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RecoveredAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Liquidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Liquidations_Department",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Liquidation_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiquidationId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScrapId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreviousUseDepartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Liquidation_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Liquidation_Details_Assets",
                        column: x => x.AssetNo,
                        principalTable: "Assets_Information",
                        principalColumn: "AssetNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Liquidation_Details_Liquidations",
                        column: x => x.LiquidationId,
                        principalTable: "Asset_Liquidations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Liquidation_Details_AssetNo",
                table: "Asset_Liquidation_Details",
                column: "AssetNo");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Liquidation_Details_LiquidationId",
                table: "Asset_Liquidation_Details",
                column: "LiquidationId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Liquidations_CreatedAt",
                table: "Asset_Liquidations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Liquidations_DepartmentId",
                table: "Asset_Liquidations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Liquidations_Status",
                table: "Asset_Liquidations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asset_Liquidation_Details");

            migrationBuilder.DropTable(
                name: "Asset_Liquidations");
        }
    }
}
