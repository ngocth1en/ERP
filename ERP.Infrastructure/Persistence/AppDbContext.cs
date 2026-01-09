using ERP.Domain.Entities;
using ERP.Domain.Entities.ASM;
using ERP.Domain.Enum.ASM;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        /* =========================
         * DB SET
         * ========================= */
        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();

        public DbSet<SystemModule> SystemModules => Set<SystemModule>();
        public DbSet<UserSystem> UserSystems => Set<UserSystem>();

        public DbSet<SystemMenu> SystemMenus => Set<SystemMenu>();
        public DbSet<UserMenuPermission> UserMenuPermissions => Set<UserMenuPermission>();

        public DbSet<Language> Languages => Set<Language>();
        public DbSet<Translation> Translations => Set<Translation>();

        public DbSet<Assets_Information> Assets_Information => Set<Assets_Information>();
        public DbSet<Asset_Model> Asset_Models => Set<Asset_Model>();
        public DbSet<Asset_Big_Type> Asset_Big_Types => Set<Asset_Big_Type>();
        public DbSet<Asset_Middle_Type> Asset_Middle_Types => Set<Asset_Middle_Type>();
        public DbSet<Asset_Small_Type> Asset_Small_Types => Set<Asset_Small_Type>();
        public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
        public DbSet<SystemGuideFile> SystemGuideFiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* =========================
             * SYSTEM MODULE
             * ========================= */
            modelBuilder.Entity<SystemModule>(e =>
            {
                e.HasKey(x => x.Code);

                e.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();
            });

            /* =========================
             * USER SYSTEM (RBAC)
             * ========================= */
            modelBuilder.Entity<UserSystem>(entity =>
            {
                entity.HasKey(x => new { x.UserId, x.SystemCode });

                entity.Property(x => x.SystemCode)
                      .HasMaxLength(50)      // 🔥 PHẢI GIỐNG SystemModules.Code
                      .IsRequired();

                entity.HasOne(x => x.User)
                      .WithMany(x => x.UserSystems)
                      .HasForeignKey(x => x.UserId);

                entity.HasOne(x => x.SystemModule)
                      .WithMany(x => x.UserSystems)
                      .HasForeignKey(x => x.SystemCode)
                      .HasPrincipalKey(x => x.Code) // rõ ràng
                      .OnDelete(DeleteBehavior.Cascade);
            });


            /* =========================
             * SYSTEM MENU
             * ========================= */
            modelBuilder.Entity<SystemMenu>(e =>
            {
                e.HasKey(x => x.Code);

                e.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.SystemCode)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.Route)
                    .HasMaxLength(200);

                e.HasOne(x => x.SystemModule)
                    .WithMany()
                    .HasForeignKey(x => x.SystemCode)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.SystemCode);
            });
            modelBuilder.Entity<SystemMenu>().HasData(
                /* ===== MASTER ===== */
                new SystemMenu { Code = "ASSET001", SystemCode = "ASSET", GroupCode = "ASSET_MASTER", Name = "Địa điểm", Route = "/asset/locations", OrderNo = 1 },
                new SystemMenu { Code = "ASSET002", SystemCode = "ASSET", GroupCode = "ASSET_MASTER", Name = "Vị trí", Route = "/asset/positions", OrderNo = 2 },
                new SystemMenu { Code = "ASSET003", SystemCode = "ASSET", GroupCode = "ASSET_MASTER", Name = "Đơn vị tài sản", Route = "/asset/units", OrderNo = 3 },
                new SystemMenu { Code = "ASSET004", SystemCode = "ASSET", GroupCode = "ASSET_MASTER", Name = "Phân loại", Route = "/asset/categories", OrderNo = 4 },
                new SystemMenu { Code = "ASSET005", SystemCode = "ASSET", GroupCode = "ASSET_MASTER", Name = "Mô hình tài sản", Route = "/asset/models", OrderNo = 5 },
                new SystemMenu { Code = "ASSET006", SystemCode = "ASSET", GroupCode = "ASSET_MASTER", Name = "Thông tin tài sản", Route = "/asset/assets", OrderNo = 6 },

                /* ===== TRANSACTION ===== */
                new SystemMenu { Code = "ASSET101", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Kiểm kê", Route = "/asset/inventory", OrderNo = 1 },
                new SystemMenu { Code = "ASSET102", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Bàn giao", Route = "/asset/handovers", OrderNo = 2 },
                new SystemMenu { Code = "ASSET103", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Mượn / Trả", Route = "/asset/borrow-return", OrderNo = 3 },
                new SystemMenu { Code = "ASSET104", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Báo phế", Route = "/asset/scraps", OrderNo = 4 },
                new SystemMenu { Code = "ASSET105", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Thanh lý", Route = "/asset/liquidations", OrderNo = 5 },
                new SystemMenu { Code = "ASSET106", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Đơn sửa chữa", Route = "/asset/repair-orders", OrderNo = 6 },
                new SystemMenu { Code = "ASSET107", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Bảo trì máy", Route = "/asset/maintenance", OrderNo = 7 },
                new SystemMenu { Code = "ASSET108", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Tài liệu hướng dẫn", Route = "/asset/manuals", OrderNo = 8 },
                new SystemMenu { Code = "ASSET109", SystemCode = "ASSET", GroupCode = "ASSET_TRANSACTION", Name = "Định kỳ bảo dưỡng", Route = "/asset/periodic-maintenance", OrderNo = 9 },

                /* ===== REPORT ===== */
                new SystemMenu { Code = "ASSET201", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "Tài sản - Đơn vị", Route = "/asset/reports/asset-unit", OrderNo = 1 },
                new SystemMenu { Code = "ASSET202", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "Tra cứu tài sản", Route = "/asset/reports/search", OrderNo = 2 },
                new SystemMenu { Code = "ASSET203", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "QRCode thẻ giấy", Route = "/asset/reports/qrcode", OrderNo = 3 },
                new SystemMenu { Code = "ASSET204", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "Tra bàn giao", Route = "/asset/reports/handovers", OrderNo = 4 },
                new SystemMenu { Code = "ASSET205", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "Cơ điện quản lý", Route = "/asset/reports/mechanical", OrderNo = 5 },
                new SystemMenu { Code = "ASSET206", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "Tập đoàn số lượng quản", Route = "/asset/reports/group-quantity", OrderNo = 6 },
                new SystemMenu { Code = "ASSET207", SystemCode = "ASSET", GroupCode = "ASSET_REPORT", Name = "Thiết bị quản lý", Route = "/asset/reports/devices", OrderNo = 7 },
    /* ===== ACCOUNT ===== */
    new SystemMenu
    {
        Code = "MAINT001",
        SystemCode = "MAINT",
        GroupCode = "MAINT_ACCOUNT",
        Name = "Thông tin người dùng",
        Route = "/maint/user/profile",
        OrderNo = 1
    },
    new SystemMenu
    {
        Code = "MAINT002",
        SystemCode = "MAINT",
        GroupCode = "MAINT_ACCOUNT",
        Name = "Cập nhật hệ thống",
        Route = "/maint/system/update",
        OrderNo = 2
    },
    new SystemMenu
    {
        Code = "MAINT003",
        SystemCode = "MAINT",
        GroupCode = "MAINT_ACCOUNT",
        Name = "Hướng dẫn sử dụng",
        Route = "/maint/help",
        OrderNo = 3
    },

    /* ===== SYSTEM ===== */
    new SystemMenu
    {
        Code = "MAINT101",
        SystemCode = "MAINT",
        GroupCode = "MAINT_SYSTEM",
        Name = "Dữ liệu người dùng",
        Route = "/maint/system/users",
        OrderNo = 1
    },
    new SystemMenu
    {
        Code = "MAINT102",
        SystemCode = "MAINT",
        GroupCode = "MAINT_SYSTEM",
        Name = "Dữ liệu nhóm",
        Route = "/maint/system/groups",
        OrderNo = 2
    },
    new SystemMenu
    {
        Code = "MAINT103",
        SystemCode = "MAINT",
        GroupCode = "MAINT_SYSTEM",
        Name = "Dữ liệu xưởng",
        Route = "/maint/system/factories",
        OrderNo = 3
    },
    new SystemMenu
    {
        Code = "MAINT104",
        SystemCode = "MAINT",
        GroupCode = "MAINT_SYSTEM",
        Name = "Dữ liệu hệ thống",
        Route = "/maint/system/config",
        OrderNo = 4
    },
    new SystemMenu
    {
        Code = "MAINT105",
        SystemCode = "MAINT",
        GroupCode = "MAINT_SYSTEM",
        Name = "Dữ liệu cửa sổ hiển thị",
        Route = "/maint/system/windows",
        OrderNo = 5
    },
    new SystemMenu
    {
        Code = "MAINT106",
        SystemCode = "MAINT",
        GroupCode = "MAINT_SYSTEM",
        Name = "Dữ liệu mục lục",
        Route = "/maint/system/catalogs",
        OrderNo = 6
    },

    /* ===== LANGUAGE ===== */
    new SystemMenu
    {
        Code = "MAINT201",
        SystemCode = "MAINT",
        GroupCode = "MAINT_LANGUAGE",
        Name = "Ngôn ngữ",
        Route = "/maint/language",
        OrderNo = 1
    },
    new SystemMenu
    {
        Code = "MAINT202",
        SystemCode = "MAINT",
        GroupCode = "MAINT_LANGUAGE",
        Name = "Văn bản hiển thị",
        Route = "/maint/language/resources",
        OrderNo = 2
    },

    /* ===== PERMISSION ===== */
    new SystemMenu
    {
        Code = "MAINT301",
        SystemCode = "MAINT",
        GroupCode = "MAINT_PERMISSION",
        Name = "Phân quyền thao tác",
        Route = "/maint/permission/action",
        OrderNo = 1
    },
    new SystemMenu
    {
        Code = "MAINT302",
        SystemCode = "MAINT",
        GroupCode = "MAINT_PERMISSION",
        Name = "Phân quyền hệ thống",
        Route = "/maint/permission/system",
        OrderNo = 2
    },
    new SystemMenu
    {
        Code = "MAINT303",
        SystemCode = "MAINT",
        GroupCode = "MAINT_PERMISSION",
        Name = "Phân quyền thương hiệu",
        Route = "/maint/permission/brand",
        OrderNo = 3
    }
);

            /* =========================
             * USER MENU PERMISSION
             * ========================= */
            modelBuilder.Entity<UserMenuPermission>(e =>
            {
                e.HasKey(x => new { x.UserId, x.MenuCode });

                e.Property(x => x.MenuCode)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.PermissionMask)
                    .HasMaxLength(10)
                    .IsRequired();

                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.SystemMenu)
                    .WithMany(x => x.UserMenuPermissions)
                    .HasForeignKey(x => x.MenuCode)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            /* =========================
             * LANGUAGE & TRANSLATION
             * ========================= */
            modelBuilder.Entity<Language>(e =>
            {
                e.HasKey(x => x.Code);
                e.Property(x => x.Code).HasMaxLength(10).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Translation>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.LanguageCode)
                    .HasMaxLength(10)
                    .IsRequired();

                e.Property(x => x.Key)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.Value)
                    .IsRequired();

                e.HasOne(x => x.Language)
                    .WithMany(x => x.Translations)
                    .HasForeignKey(x => x.LanguageCode)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.LanguageCode, x.Key }).IsUnique();
            });

            /* =========================
             * ASSET INFORMATION
             * ========================= */
            modelBuilder.Entity<Assets_Information>(e =>
            {
                e.HasKey(x => x.AssetNo);

                e.Property(x => x.AssetNo)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.QRCode)
                    .HasMaxLength(200);

                e.Property(x => x.Status)
                    .HasConversion<int>();

                e.HasOne(x => x.Department)
                    .WithMany(d => d.ManagedAssets)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.UseDepartment)
                    .WithMany(d => d.UsedAssets)
                    .HasForeignKey(x => x.UseDepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.AssetModel)
                    .WithMany(m => m.Assets)
                    .HasForeignKey(x => x.Model_Id)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.StorageLocation)
                    .WithMany(l => l.Assets)
                    .HasForeignKey(x => x.StorageLocationNo)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.Model_Id);
                e.HasIndex(x => x.DepartmentId);
                e.HasIndex(x => x.UseDepartmentId);
                e.HasIndex(x => x.StorageLocationNo);
                e.HasIndex(x => x.Status);

                e.HasQueryFilter(x => x.Status != AssetStatus.Waiting);
            });

            /* =========================
             * ASSET MASTER DATA
             * ========================= */
            modelBuilder.Entity<Asset_Model>(e =>
            {
                e.HasKey(x => x.Model_Id);

                e.Property(x => x.Model_Name)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.ImageData)
                    .HasColumnType("varbinary(max)");
            });

            modelBuilder.Entity<Asset_Big_Type>(e =>
            {
                e.HasKey(x => x.Big_Type_Id);
                e.Property(x => x.Big_Type_Name_EN).HasMaxLength(200).IsRequired();
                e.Property(x => x.Big_Type_Name_VN).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<Asset_Middle_Type>(e =>
            {
                e.HasKey(x => x.Middle_Type_Id);
                e.Property(x => x.Middle_Type_Name_EN).HasMaxLength(200).IsRequired();
                e.Property(x => x.Middle_Type_Name_VN).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<Asset_Small_Type>(e =>
            {
                e.HasKey(x => x.Small_Type_Id);
                e.Property(x => x.Small_Type_Name_EN).HasMaxLength(200).IsRequired();
                e.Property(x => x.Small_Type_Name_VN).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<StorageLocation>(e =>
            {
                e.HasKey(x => x.StorageLocationNo);

                e.Property(x => x.StorageLocationNo)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(x => x.NameEN)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.NameVN)
                    .HasMaxLength(200)
                    .IsRequired();

                e.HasOne(x => x.Department)
                    .WithMany(d => d.StorageLocations)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.DepartmentId);
            });
            modelBuilder.Entity<SystemGuideFile>()
        .HasOne(x => x.SystemModule)
        .WithMany(x => x.GuideFiles)
        .HasForeignKey(x => x.SystemCode);
        }
    }
}
