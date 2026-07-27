using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableForNewFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset_Handovers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FromDepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToDepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromStorageLocationNo = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ToStorageLocationNo = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_Asset_Handovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Handovers_FromDepartment",
                        column: x => x.FromDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Handovers_FromStorageLocation",
                        column: x => x.FromStorageLocationNo,
                        principalTable: "StorageLocations",
                        principalColumn: "StorageLocationNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Handovers_ToDepartment",
                        column: x => x.ToDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Handovers_ToStorageLocation",
                        column: x => x.ToStorageLocationNo,
                        principalTable: "StorageLocations",
                        principalColumn: "StorageLocationNo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Handover_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HandoverId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    AssetNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreviousUseDepartmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PreviousStorageLocationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Handover_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Handover_Details_Assets",
                        column: x => x.AssetNo,
                        principalTable: "Assets_Information",
                        principalColumn: "AssetNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Handover_Details_Handovers",
                        column: x => x.HandoverId,
                        principalTable: "Asset_Handovers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handover_Details_AssetNo",
                table: "Asset_Handover_Details",
                column: "AssetNo");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handover_Details_HandoverId",
                table: "Asset_Handover_Details",
                column: "HandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handovers_CreatedAt",
                table: "Asset_Handovers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handovers_FromDepartmentId",
                table: "Asset_Handovers",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handovers_FromStorageLocationNo",
                table: "Asset_Handovers",
                column: "FromStorageLocationNo");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handovers_Status",
                table: "Asset_Handovers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handovers_ToDepartmentId",
                table: "Asset_Handovers",
                column: "ToDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Handovers_ToStorageLocationNo",
                table: "Asset_Handovers",
                column: "ToStorageLocationNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asset_Handover_Details");

            migrationBuilder.DropTable(
                name: "Asset_Handovers");
        }
    }
}
