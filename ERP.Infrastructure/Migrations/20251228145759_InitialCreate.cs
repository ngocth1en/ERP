using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset_Big_Types",
                columns: table => new
                {
                    Big_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Big_Type_Name_EN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Big_Type_Name_VN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Big_Types", x => x.Big_Type_Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "SystemModules",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemModules", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Middle_Types",
                columns: table => new
                {
                    Middle_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Middle_Type_Name_EN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Middle_Type_Name_VN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Machine_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Big_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Asset_Big_TypeBig_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Middle_Types", x => x.Middle_Type_Id);
                    table.ForeignKey(
                        name: "FK_Asset_Middle_Types_Asset_Big_Types_Asset_Big_TypeBig_Type_Id",
                        column: x => x.Asset_Big_TypeBig_Type_Id,
                        principalTable: "Asset_Big_Types",
                        principalColumn: "Big_Type_Id");
                    table.ForeignKey(
                        name: "FK_Asset_Middle_Types_Asset_Big_Types_Big_Type_Id",
                        column: x => x.Big_Type_Id,
                        principalTable: "Asset_Big_Types",
                        principalColumn: "Big_Type_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalTable: "Languages",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Small_Types",
                columns: table => new
                {
                    Small_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Small_Type_Name_EN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Small_Type_Name_VN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Middle_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Asset_Middle_TypeMiddle_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Small_Types", x => x.Small_Type_Id);
                    table.ForeignKey(
                        name: "FK_Asset_Small_Types_Asset_Middle_Types_Asset_Middle_TypeMiddle_Type_Id",
                        column: x => x.Asset_Middle_TypeMiddle_Type_Id,
                        principalTable: "Asset_Middle_Types",
                        principalColumn: "Middle_Type_Id");
                    table.ForeignKey(
                        name: "FK_Asset_Small_Types_Asset_Middle_Types_Middle_Type_Id",
                        column: x => x.Middle_Type_Id,
                        principalTable: "Asset_Middle_Types",
                        principalColumn: "Middle_Type_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSystems",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: false),
                    PermissionMask = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSystems", x => new { x.UserId, x.SystemCode });
                    table.ForeignKey(
                        name: "FK_UserSystems_SystemModules_SystemCode",
                        column: x => x.SystemCode,
                        principalTable: "SystemModules",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSystems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asset_Models",
                columns: table => new
                {
                    Model_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Big_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Middle_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Small_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Asset_Small_TypeSmall_Type_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset_Models", x => x.Model_Id);
                    table.ForeignKey(
                        name: "FK_Asset_Models_Asset_Big_Types_Big_Type_Id",
                        column: x => x.Big_Type_Id,
                        principalTable: "Asset_Big_Types",
                        principalColumn: "Big_Type_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Models_Asset_Middle_Types_Middle_Type_Id",
                        column: x => x.Middle_Type_Id,
                        principalTable: "Asset_Middle_Types",
                        principalColumn: "Middle_Type_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Models_Asset_Small_Types_Asset_Small_TypeSmall_Type_Id",
                        column: x => x.Asset_Small_TypeSmall_Type_Id,
                        principalTable: "Asset_Small_Types",
                        principalColumn: "Small_Type_Id");
                    table.ForeignKey(
                        name: "FK_Asset_Models_Asset_Small_Types_Small_Type_Id",
                        column: x => x.Small_Type_Id,
                        principalTable: "Asset_Small_Types",
                        principalColumn: "Small_Type_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assets_Information",
                columns: table => new
                {
                    AssetNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UseDepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Model_Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets_Information", x => x.AssetNo);
                    table.ForeignKey(
                        name: "FK_Assets_Information_Asset_Models_Model_Id",
                        column: x => x.Model_Id,
                        principalTable: "Asset_Models",
                        principalColumn: "Model_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Information_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Information_Departments_UseDepartmentId",
                        column: x => x.UseDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "en", "English" },
                    { "vi", "Tiếng Việt" },
                    { "zh", "中文" }
                });

            migrationBuilder.InsertData(
                table: "SystemModules",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "ASSET", "Quản lý tài sản" },
                    { "MAINT", "Maintenance" },
                    { "QC", "Quality Control" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Middle_Types_Asset_Big_TypeBig_Type_Id",
                table: "Asset_Middle_Types",
                column: "Asset_Big_TypeBig_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Middle_Types_Big_Type_Id",
                table: "Asset_Middle_Types",
                column: "Big_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Models_Asset_Small_TypeSmall_Type_Id",
                table: "Asset_Models",
                column: "Asset_Small_TypeSmall_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Models_Big_Type_Id",
                table: "Asset_Models",
                column: "Big_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Models_Middle_Type_Id",
                table: "Asset_Models",
                column: "Middle_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Models_Small_Type_Id",
                table: "Asset_Models",
                column: "Small_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Small_Types_Asset_Middle_TypeMiddle_Type_Id",
                table: "Asset_Small_Types",
                column: "Asset_Middle_TypeMiddle_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Small_Types_Middle_Type_Id",
                table: "Asset_Small_Types",
                column: "Middle_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Information_DepartmentId",
                table: "Assets_Information",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Information_Model_Id",
                table: "Assets_Information",
                column: "Model_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Information_Status",
                table: "Assets_Information",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Information_UseDepartmentId",
                table: "Assets_Information",
                column: "UseDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageCode",
                table: "Translations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSystems_SystemCode",
                table: "UserSystems",
                column: "SystemCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets_Information");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "UserSystems");

            migrationBuilder.DropTable(
                name: "Asset_Models");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "SystemModules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Asset_Small_Types");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Asset_Middle_Types");

            migrationBuilder.DropTable(
                name: "Asset_Big_Types");
        }
    }
}
