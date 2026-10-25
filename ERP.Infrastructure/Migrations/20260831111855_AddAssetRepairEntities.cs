using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetRepairEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset_Repairs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScrapId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RepairVendor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Repairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Repairs_Assets",
                        column: x => x.AssetNo,
                        principalTable: "Assets_Information",
                        principalColumn: "AssetNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Repairs_Department",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Repairs_Scraps",
                        column: x => x.ScrapId,
                        principalTable: "Asset_Scraps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Repairs_AssetNo",
                table: "Asset_Repairs",
                column: "AssetNo");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Repairs_CreatedAt",
                table: "Asset_Repairs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Repairs_DepartmentId",
                table: "Asset_Repairs",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Repairs_ScrapId",
                table: "Asset_Repairs",
                column: "ScrapId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Repairs_Status",
                table: "Asset_Repairs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asset_Repairs");
        }
    }
}
