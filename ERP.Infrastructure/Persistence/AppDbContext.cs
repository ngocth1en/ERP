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
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<SystemModule> SystemModules => Set<SystemModule>();
        public DbSet<UserSystem> UserSystems => Set<UserSystem>();
        public DbSet<Language> Languages => Set<Language>();
        public DbSet<Translation> Translations => Set<Translation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SystemModule
            modelBuilder.Entity<SystemModule>()
                .HasKey(x => x.Code);

            // Language
            modelBuilder.Entity<Language>()
                .HasKey(x => x.Code);

            // UserSystem (composite key)
            modelBuilder.Entity<UserSystem>()
                .HasKey(x => new { x.UserId, x.SystemCode });

            modelBuilder.Entity<UserSystem>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSystems)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserSystem>()
                .HasOne(x => x.SystemModule)
                .WithMany(x => x.UserSystems)
                .HasForeignKey(x => x.SystemCode);

            // Translation
            modelBuilder.Entity<Translation>()
                .HasOne(x => x.Language)
                .WithMany()
                .HasForeignKey(x => x.LanguageCode);
        }
    }




}
