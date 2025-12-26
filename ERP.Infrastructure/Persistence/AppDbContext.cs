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
        public DbSet<UserSystem> UserSystems => Set<UserSystem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemModule>()
                .HasKey(x => x.Code);

            modelBuilder.Entity<UserSystem>()
                .HasKey(x => new { x.UserId, x.SystemCode });

            modelBuilder.Entity<UserSystem>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserSystem>()
                .HasOne(x => x.SystemModule)
                .WithMany()
                .HasForeignKey(x => x.SystemCode);
        }

    }



}
