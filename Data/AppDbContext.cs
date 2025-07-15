using Microsoft.EntityFrameworkCore;
using ProjectDB_API.Controllers;
using ProjectDB_API.Models;

namespace ProjectDB_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<LoginResult> LoginResult { get; set; }
        public DbSet<RoleResult> RoleResult { get; set; }
        public DbSet<MenuItemResult> MenuItemResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoleResult>().HasNoKey();
            modelBuilder.Entity<MenuItemResult>().HasNoKey();
        }
    }
}
