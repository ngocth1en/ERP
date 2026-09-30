using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetScrapEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset_Scraps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StorageLocationNo = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Asset_Scraps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Scraps_Department",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Scraps_StorageLocation",
                        column: x => x.StorageLocationNo,
                        principalTable: "StorageLocations",
                        principalColumn: "StorageLocationNo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Scrap_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScrapId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreviousUseDepartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreviousStorageLocationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Scrap_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Scrap_Details_Assets",
                        column: x => x.AssetNo,
                        principalTable: "Assets_Information",
                        principalColumn: "AssetNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Scrap_Details_Scraps",
                        column: x => x.ScrapId,
                        principalTable: "Asset_Scraps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Scrap_Details_AssetNo",
                table: "Asset_Scrap_Details",
                column: "AssetNo");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Scrap_Details_ScrapId",
                table: "Asset_Scrap_Details",
                column: "ScrapId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Scraps_CreatedAt",
                table: "Asset_Scraps",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Scraps_DepartmentId",
                table: "Asset_Scraps",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Scraps_Status",
                table: "Asset_Scraps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Scraps_StorageLocationNo",
                table: "Asset_Scraps",
                column: "StorageLocationNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asset_Scrap_Details");

            migrationBuilder.DropTable(
                name: "Asset_Scraps");
        }
    }
}
