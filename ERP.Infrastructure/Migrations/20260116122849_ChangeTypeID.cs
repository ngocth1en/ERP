using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    public partial class ChangeTypeID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* =====================================================
             * 1. DROP ALL FK TRỎ VỀ Asset_Big_Types
             * ===================================================== */

            migrationBuilder.Sql(@"
    DECLARE @sql NVARCHAR(MAX) = '';

    SELECT @sql += '
        ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' + OBJECT_NAME(parent_object_id) + ']
        DROP CONSTRAINT [' + name + '];'
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('Asset_Big_Types');

    EXEC sp_executesql @sql;
    ");

            /* =====================================================
             * 2. DROP PK
             * ===================================================== */

            migrationBuilder.Sql(@"
    IF EXISTS (
        SELECT 1 FROM sys.key_constraints
        WHERE name = 'PK_Asset_Big_Types'
    )
    BEGIN
        ALTER TABLE Asset_Big_Types DROP CONSTRAINT PK_Asset_Big_Types
    END
    ");

            /* =====================================================
             * 3. DROP COLUMN CŨ
             * ===================================================== */

            migrationBuilder.Sql(@"
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE name = 'Big_Type_Id'
          AND object_id = OBJECT_ID('Asset_Big_Types')
    )
    BEGIN
        ALTER TABLE Asset_Big_Types DROP COLUMN Big_Type_Id
    END
    ");

            /* =====================================================
             * 4. ADD IDENTITY COLUMN
             * ===================================================== */

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Asset_Big_Types",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            /* =====================================================
             * 5. ADD PK
             * ===================================================== */

            migrationBuilder.AddPrimaryKey(
                name: "PK_Asset_Big_Types",
                table: "Asset_Big_Types",
                column: "Id");

            /* =====================================================
             * 6. ADD FK LẠI (MODEL → BIG TYPE)
             * ===================================================== */

            migrationBuilder.Sql(@"
    IF NOT EXISTS (
        SELECT 1 FROM sys.columns
        WHERE name = 'BigTypeId'
          AND object_id = OBJECT_ID('Asset_Models')
    )
    BEGIN
        ALTER TABLE Asset_Models ADD BigTypeId INT NULL
    END
    ");

            migrationBuilder.Sql(@"
    ALTER TABLE Asset_Models
    ADD CONSTRAINT FK_Asset_Models_Asset_Big_Types
    FOREIGN KEY (BigTypeId)
    REFERENCES Asset_Big_Types(Id)
    ");
        }



        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    IF EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = 'FK_Asset_Models_Asset_Big_Types'
    )
    BEGIN
        ALTER TABLE Asset_Models
        DROP CONSTRAINT FK_Asset_Models_Asset_Big_Types
    END
    ");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Asset_Big_Types",
                table: "Asset_Big_Types");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Asset_Big_Types");
        }


    }
}
