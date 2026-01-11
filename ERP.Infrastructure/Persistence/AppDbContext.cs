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

                // 👇 CHA – CON
                e.HasOne(x => x.Parent)
                    .WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentCode)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.SystemCode);
                e.HasIndex(x => x.ParentCode);
            });

            



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
