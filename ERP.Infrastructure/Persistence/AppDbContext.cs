using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<SystemModule> SystemModules => Set<SystemModule>();
        public DbSet<UserSystemPermission> UserSystemPermissions => Set<UserSystemPermission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemModule>()
                .HasKey(x => x.Code);

            modelBuilder.Entity<UserSystemPermission>()
                .HasKey(x => new { x.UserId, x.SystemCode });

            modelBuilder.Entity<UserSystemPermission>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSystemPermissions)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserSystemPermission>()
                .HasOne(x => x.SystemModule)
                .WithMany(x => x.UserSystemPermissions)
                .HasForeignKey(x => x.SystemCode);
        }
    }



}
