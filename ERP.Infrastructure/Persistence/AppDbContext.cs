using ERP.Domain.Entities;
using ERP.Domain.Entities.ASM;
using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<SystemModule> SystemModules => Set<SystemModule>();
        public DbSet<UserSystem> UserSystems => Set<UserSystem>();
        public DbSet<Language> Languages => Set<Language>();
        public DbSet<Translation> Translations => Set<Translation>();

        public DbSet<Assets_Information> Assets_Information => Set<Assets_Information>();
        public DbSet<Asset_Model> Asset_Models => Set<Asset_Model>();
        public DbSet<Asset_Big_Type> Asset_Big_Types => Set<Asset_Big_Type>();
        public DbSet<Asset_Middle_Type> Asset_Middle_Types => Set<Asset_Middle_Type>();
        public DbSet<Asset_Small_Type> Asset_Small_Types => Set<Asset_Small_Type>();
        public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* =========================
             * SYSTEM MODULE
             * ========================= */
            var adminId = Guid.Parse("00000000-0000-0000-0000-000000000000");

            var passwordHasher = new PasswordHasher();

            var admin = new User
            {
                Id = adminId,
                UserName = "999999",
                DepartmentId = "ADMIN",
                MustChangePassword = false,
                PasswordHash = passwordHasher.Hash("999999") // pass = username
            };

            modelBuilder.Entity<User>().HasData(admin);

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = "ADMIN",
                    Name = "Quản trị hệ thống"
                }
            );
            modelBuilder.Entity<User>().HasData(admin);
            modelBuilder.Entity<SystemModule>()
                .HasKey(x => x.Code);

            modelBuilder.Entity<SystemModule>().HasData(
                new SystemModule { Code = "ASSET", Name = "Quản lý tài sản" },
                new SystemModule { Code = "QC", Name = "Quality Control" },
                new SystemModule { Code = "MAINT", Name = "Maintenance" }
            );

            /* =========================
             * LANGUAGE & TRANSLATION
             * ========================= */
            modelBuilder.Entity<Language>()
                .HasKey(x => x.Code);

            modelBuilder.Entity<Language>().HasData(
                new Language { Code = "vi", Name = "Tiếng Việt" },
                new Language { Code = "en", Name = "English" },
                new Language { Code = "zh", Name = "中文" }
            );
            modelBuilder.Entity<Translation>().HasData(
                new Translation
                {
                    Id = Guid.NewGuid(),
                    LanguageCode = "vi",
                    Key = "MENU_ASSET",
                    Value = "Quản lý tài sản"
                },
                new Translation
                {
                    Id = Guid.NewGuid(),
                    LanguageCode = "en",
                    Key = "MENU_ASSET",
                    Value = "Asset Management"
                }
            );

            modelBuilder.Entity<Translation>()
                .HasOne(x => x.Language)
                .WithMany()
                .HasForeignKey(x => x.LanguageCode)
                .OnDelete(DeleteBehavior.Restrict);

            /* =========================
             * USER SYSTEM (PERMISSION)
             * ========================= */
            modelBuilder.Entity<UserSystem>()
                .HasKey(x => new { x.UserId, x.SystemCode });

            modelBuilder.Entity<UserSystem>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSystems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSystem>()
                .HasOne(x => x.SystemModule)
                .WithMany(x => x.UserSystems)
                .HasForeignKey(x => x.SystemCode)
                .OnDelete(DeleteBehavior.Cascade);

            /* =========================
             * ASSET INFORMATION
             * ========================= */
            modelBuilder.Entity<Assets_Information>(entity =>
            {
                entity.HasKey(x => x.AssetNo);

                entity.Property(x => x.AssetNo)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(x => x.QRCode)
                      .HasMaxLength(200);

                entity.Property(x => x.Model_Id)
                      .IsRequired();

                entity.Property(x => x.Status)
                      .HasConversion<int>();

                // Department quản lý
                entity.HasOne(x => x.Department)
                      .WithMany(d => d.ManagedAssets)
                      .HasForeignKey(x => x.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Department sử dụng
                entity.HasOne(x => x.UseDepartment)
                      .WithMany(d => d.UsedAssets)
                      .HasForeignKey(x => x.UseDepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Asset -> Model
                entity.HasOne(x => x.AssetModel)
                      .WithMany(m => m.Assets)
                      .HasForeignKey(x => x.Model_Id)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.StorageLocation)
                      .WithMany(l => l.Assets)
                      .HasForeignKey(x => x.StorageLocationNo)
                      .OnDelete(DeleteBehavior.Restrict);
                // Index
                entity.HasIndex(x => x.Model_Id);
                entity.HasIndex(x => x.DepartmentId);
                entity.HasIndex(x => x.UseDepartmentId);
                entity.HasIndex(x => x.Status);
                entity.HasIndex(x => x.StorageLocationNo);

                // Soft delete (CHỈ Deleted)
                entity.HasQueryFilter(x => x.Status != AssetStatus.Waiting);
            });

            /* =========================
             * ASSET MODEL
             * ========================= */
            modelBuilder.Entity<Asset_Model>(entity =>
            {
                entity.HasKey(x => x.Model_Id);

                entity.Property(x => x.Model_Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.ImageData)
                      .HasColumnType("varbinary(max)");

                entity.HasIndex(x => x.Big_Type_Id);
                entity.HasIndex(x => x.Middle_Type_Id);
                entity.HasIndex(x => x.Small_Type_Id);

                entity.HasOne(x => x.Asset_Big_Type)
                      .WithMany()
                      .HasForeignKey(x => x.Big_Type_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Asset_Middle_Type)
                      .WithMany()
                      .HasForeignKey(x => x.Middle_Type_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Asset_Small_Type)
                      .WithMany()
                      .HasForeignKey(x => x.Small_Type_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Asset_Big_Type>(entity =>
            {
                entity.HasKey(x => x.Big_Type_Id);

                entity.Property(x => x.Big_Type_Name_EN)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(x => x.Big_Type_Name_VN)
                      .IsRequired()
                      .HasMaxLength(200);
            });
            modelBuilder.Entity<Asset_Middle_Type>(entity =>
            {
                entity.HasKey(x => x.Middle_Type_Id);

                entity.Property(x => x.Middle_Type_Name_EN)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(x => x.Middle_Type_Name_VN)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasOne(x => x.Asset_Big_Type)
                      .WithMany()
                      .HasForeignKey(x => x.Big_Type_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Asset_Small_Type>(entity =>
            {
                entity.HasKey(x => x.Small_Type_Id);

                entity.Property(x => x.Small_Type_Name_EN)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(x => x.Small_Type_Name_VN)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasOne(x => x.Asset_Middle_Type)
                      .WithMany()
                      .HasForeignKey(x => x.Middle_Type_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<StorageLocation>(entity =>
            {
                entity.HasKey(x => x.StorageLocationNo);

                entity.Property(x => x.StorageLocationNo)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(x => x.NameEN)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(x => x.NameVN)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.HasOne(x => x.Department)
                      .WithMany(d => d.StorageLocations)
                      .HasForeignKey(x => x.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.DepartmentId);
            });

        }
    }
}
