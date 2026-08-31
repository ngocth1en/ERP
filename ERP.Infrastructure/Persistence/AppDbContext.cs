using ERP.Domain.Entities.ASM;
using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset_Big_Types> Asset_Big_Types { get; set; }

    public virtual DbSet<Asset_Middle_Types> Asset_Middle_Types { get; set; }

    public virtual DbSet<Asset_Models> Asset_Models { get; set; }

    public virtual DbSet<Asset_Small_Types> Asset_Small_Types { get; set; }

    public virtual DbSet<Assets_Information> Assets_Information { get; set; }

    public virtual DbSet<Departments> Departments { get; set; }

    public virtual DbSet<Languages> Languages { get; set; }

    public virtual DbSet<StorageLocations> StorageLocations { get; set; }

    public virtual DbSet<SystemGuideFiles> SystemGuideFiles { get; set; }

    public virtual DbSet<SystemMenus> SystemMenus { get; set; }

    public virtual DbSet<SystemModules> SystemModules { get; set; }

    public virtual DbSet<Translations> Translations { get; set; }

    public virtual DbSet<UserMenuPermissions> UserMenuPermissions { get; set; }

    public virtual DbSet<UserSystems> UserSystems { get; set; }

    public virtual DbSet<Users> Users { get; set; }
    public virtual DbSet<Asset_Inventories> Asset_Inventories { get; set; }
    public virtual DbSet<Asset_Inventory_Details> Asset_Inventory_Details { get; set; }
    public virtual DbSet<Asset_Handovers> Asset_Handovers { get; set; }
    public virtual DbSet<Asset_Handover_Details> Asset_Handover_Details { get; set; }
    public virtual DbSet<Asset_Scraps> Asset_Scraps { get; set; }
    public virtual DbSet<Asset_Scrap_Details> Asset_Scrap_Details { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset_Big_Types>(entity =>
        {
            entity.HasKey(e => e.Big_Type_Id);

            entity.Property(e => e.Big_Type_Name_EN).HasMaxLength(200);
            entity.Property(e => e.Big_Type_Name_VN).HasMaxLength(200);
        });

        modelBuilder.Entity<Asset_Middle_Types>(entity =>
        {
            entity.HasKey(e => e.Middle_Type_Id);

            entity.HasIndex(e => e.Big_Type_Id, "IX_Asset_Middle_Types_Big_Type_Id");

            entity.Property(e => e.Inventory_Check_Type)
                .HasMaxLength(10)
                .HasDefaultValue("1");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Middle_Type_Name_EN).HasMaxLength(200);
            entity.Property(e => e.Middle_Type_Name_VN).HasMaxLength(200);

            entity.HasOne(d => d.Big_Type).WithMany(p => p.Asset_Middle_Types)
                .HasForeignKey(d => d.Big_Type_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_Middle_Types_Asset_Big_Types");
        });

        modelBuilder.Entity<Asset_Models>(entity =>
        {
            entity.HasKey(e => e.Model_Id);

            entity.HasIndex(e => e.Big_Type_Id, "IX_Asset_Models_Big_Type_Id");

            entity.HasIndex(e => e.Middle_Type_Id, "IX_Asset_Models_Middle_Type_Id");

            entity.HasIndex(e => e.Small_Type_Id, "IX_Asset_Models_Small_Type_Id");

            entity.Property(e => e.Model_Name).HasMaxLength(200);

            entity.HasOne(d => d.Big_Type).WithMany(p => p.Asset_Models)
                .HasForeignKey(d => d.Big_Type_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_Models_Asset_Big_Types");

            entity.HasOne(d => d.Middle_Type).WithMany(p => p.Asset_Models)
                .HasForeignKey(d => d.Middle_Type_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_Models_Asset_Middle_Types");

            entity.HasOne(d => d.Small_Type).WithMany(p => p.Asset_Models)
                .HasForeignKey(d => d.Small_Type_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_Models_Asset_Small_Types");
        });

        modelBuilder.Entity<Asset_Small_Types>(entity =>
        {
            entity.HasKey(e => e.Small_Type_Id);

            entity.HasIndex(e => e.Middle_Type_Id, "IX_Asset_Small_Types_Middle_Type_Id");

            entity.Property(e => e.Small_Type_Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Small_Type_Name_EN).HasMaxLength(200);
            entity.Property(e => e.Small_Type_Name_VN).HasMaxLength(200);

            entity.HasOne(d => d.Middle_Type).WithMany(p => p.Asset_Small_Types)
                .HasForeignKey(d => d.Middle_Type_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Asset_Small_Types_Asset_Middle_Types");
        });

        modelBuilder.Entity<Assets_Information>(entity =>
        {
            entity.HasKey(e => e.AssetNo);

            entity.HasIndex(e => e.DepartmentId, "IX_Assets_Information_DepartmentId");

            entity.HasIndex(e => e.Model_Id, "IX_Assets_Information_Model_Id");

            entity.HasIndex(e => e.Status, "IX_Assets_Information_Status");

            entity.HasIndex(e => e.StorageLocationNo, "IX_Assets_Information_StorageLocationNo");

            entity.HasIndex(e => e.UseDepartmentId, "IX_Assets_Information_UseDepartmentId");

            entity.Property(e => e.AssetNo).HasMaxLength(50);
            entity.Property(e => e.QRCode).HasMaxLength(200);
            entity.Property(e => e.StorageLocationNo)
                .HasMaxLength(50)
                .HasDefaultValue("");

            entity.HasOne(d => d.Department).WithMany(p => p.Assets_InformationDepartment)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Model).WithMany(p => p.Assets_Information)
                .HasForeignKey(d => d.Model_Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StorageLocationNoNavigation).WithMany(p => p.Assets_Information)
                .HasForeignKey(d => d.StorageLocationNo)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UseDepartment).WithMany(p => p.Assets_InformationUseDepartment)
                .HasForeignKey(d => d.UseDepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Languages>(entity =>
        {
            entity.HasKey(e => e.Code);
        });

        modelBuilder.Entity<StorageLocations>(entity =>
        {
            entity.HasKey(e => e.StorageLocationNo);

            entity.HasIndex(e => e.DepartmentId, "IX_StorageLocations_DepartmentId");

            entity.Property(e => e.StorageLocationNo).HasMaxLength(50);
            entity.Property(e => e.NameEN).HasMaxLength(200);
            entity.Property(e => e.NameVN).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Department).WithMany(p => p.StorageLocations)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SystemGuideFiles>(entity =>
        {
            entity.HasIndex(e => e.SystemCode, "IX_SystemGuideFiles_SystemCode");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.SystemCode).HasMaxLength(50);

            entity.HasOne(d => d.SystemCodeNavigation).WithMany(p => p.SystemGuideFiles).HasForeignKey(d => d.SystemCode);
        });

        modelBuilder.Entity<SystemMenus>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.HasIndex(e => e.ParentCode, "IX_SystemMenus_ParentCode");

            entity.HasIndex(e => e.SystemCode, "IX_SystemMenus_SystemCode");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.ParentCode).HasMaxLength(50);
            entity.Property(e => e.Route).HasMaxLength(200);
            entity.Property(e => e.SystemCode).HasMaxLength(50);

            entity.HasOne(d => d.ParentCodeNavigation).WithMany(p => p.InverseParentCodeNavigation).HasForeignKey(d => d.ParentCode);

            entity.HasOne(d => d.SystemCodeNavigation).WithMany(p => p.SystemMenus).HasForeignKey(d => d.SystemCode);
        });

        modelBuilder.Entity<SystemModules>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.Property(e => e.Code).HasMaxLength(50);
        });

        modelBuilder.Entity<Translations>(entity =>
        {
            entity.HasIndex(e => e.LanguageCode, "IX_Translations_LanguageCode");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.LanguageCodeNavigation).WithMany(p => p.Translations)
                .HasForeignKey(d => d.LanguageCode)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserMenuPermissions>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MenuCode });

            entity.HasIndex(e => e.MenuCode, "IX_UserMenuPermissions_MenuCode");

            entity.Property(e => e.MenuCode).HasMaxLength(50);
            entity.Property(e => e.PermissionMask).HasMaxLength(10);

            entity.HasOne(d => d.MenuCodeNavigation).WithMany(p => p.UserMenuPermissions).HasForeignKey(d => d.MenuCode);

            entity.HasOne(d => d.User).WithMany(p => p.UserMenuPermissions).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserSystems>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SystemCode });

            entity.HasIndex(e => e.SystemCode, "IX_UserSystems_SystemCode");

            entity.Property(e => e.SystemCode).HasMaxLength(50);

            entity.HasOne(d => d.SystemCodeNavigation).WithMany(p => p.UserSystems).HasForeignKey(d => d.SystemCode);

            entity.HasOne(d => d.User).WithMany(p => p.UserSystems).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "IX_Users_DepartmentId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Department).WithMany(p => p.Users).HasForeignKey(d => d.DepartmentId);
        });
        // Thêm sau dòng cuối cùng trước OnModelCreatingPartial()
        modelBuilder.Entity<Asset_Inventories>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Model_Id, "IX_Asset_Inventories_Model_Id");

            entity.HasIndex(e => e.DepartmentId, "IX_Asset_Inventories_DepartmentId");

            entity.HasIndex(e => e.CreatedAt, "IX_Asset_Inventories_CreatedAt");

            entity.Property(e => e.Inventory_Type)
                .HasMaxLength(10)
                .HasDefaultValue("1");

            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Model)
                .WithMany(p => p.Asset_Inventories)
                .HasForeignKey(d => d.Model_Id)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Inventories_Asset_Models");
        });

        modelBuilder.Entity<Asset_Inventory_Details>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.InventoryId, "IX_Asset_Inventory_Details_InventoryId");

            entity.HasIndex(e => e.AssetNo, "IX_Asset_Inventory_Details_AssetNo");

            entity.Property(e => e.AssetNo).HasMaxLength(50);

            entity.Property(e => e.QRCode).HasMaxLength(200);

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("FOUND");

            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Inventory)
                .WithMany(p => p.Details)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Asset_Inventory_Details_Asset_Inventories");
        });
        modelBuilder.Entity<Asset_Handovers>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue(HandoverStatus.Draft);

            entity.HasIndex(e => e.FromDepartmentId, "IX_Asset_Handovers_FromDepartmentId");
            entity.HasIndex(e => e.ToDepartmentId, "IX_Asset_Handovers_ToDepartmentId");
            entity.HasIndex(e => e.Status, "IX_Asset_Handovers_Status");
            entity.HasIndex(e => e.CreatedAt, "IX_Asset_Handovers_CreatedAt");

            entity.HasOne(d => d.FromDepartment)
                .WithMany()
                .HasForeignKey(d => d.FromDepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Handovers_FromDepartment");

            entity.HasOne(d => d.ToDepartment)
                .WithMany()
                .HasForeignKey(d => d.ToDepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Handovers_ToDepartment");

            entity.HasOne(d => d.FromStorageLocation)
                .WithMany()
                .HasForeignKey(d => d.FromStorageLocationNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Handovers_FromStorageLocation")
                .IsRequired(false);

            entity.HasOne(d => d.ToStorageLocation)
                .WithMany()
                .HasForeignKey(d => d.ToStorageLocationNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Handovers_ToStorageLocation")
                .IsRequired(false);
        });

        modelBuilder.Entity<Asset_Handover_Details>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AssetNo).HasMaxLength(50);
            entity.Property(e => e.PreviousUseDepartmentId).HasMaxLength(50);
            entity.Property(e => e.PreviousStorageLocationNo).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasIndex(e => e.HandoverId, "IX_Asset_Handover_Details_HandoverId");
            entity.HasIndex(e => e.AssetNo, "IX_Asset_Handover_Details_AssetNo");

            entity.HasOne(d => d.Handover)
                .WithMany(p => p.Details)
                .HasForeignKey(d => d.HandoverId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Asset_Handover_Details_Handovers");

            entity.HasOne(d => d.Asset)
                .WithMany()
                .HasForeignKey(d => d.AssetNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Handover_Details_Assets");
        });
        modelBuilder.Entity<Asset_Scraps>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue(ScrapStatus.Draft);

            entity.HasIndex(e => e.DepartmentId, "IX_Asset_Scraps_DepartmentId");
            entity.HasIndex(e => e.Status, "IX_Asset_Scraps_Status");
            entity.HasIndex(e => e.CreatedAt, "IX_Asset_Scraps_CreatedAt");

            entity.HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Scraps_Department");

            entity.HasOne(d => d.StorageLocation)
                .WithMany()
                .HasForeignKey(d => d.StorageLocationNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Scraps_StorageLocation")
                .IsRequired(false);
        });

        modelBuilder.Entity<Asset_Scrap_Details>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AssetNo).HasMaxLength(50);
            entity.Property(e => e.PreviousUseDepartmentId).HasMaxLength(50);
            entity.Property(e => e.PreviousStorageLocationNo).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasIndex(e => e.ScrapId, "IX_Asset_Scrap_Details_ScrapId");
            entity.HasIndex(e => e.AssetNo, "IX_Asset_Scrap_Details_AssetNo");

            entity.HasOne(d => d.Scrap)
                .WithMany(p => p.Details)
                .HasForeignKey(d => d.ScrapId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Asset_Scrap_Details_Scraps");

            entity.HasOne(d => d.Asset)
                .WithMany()
                .HasForeignKey(d => d.AssetNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Asset_Scrap_Details_Assets");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
