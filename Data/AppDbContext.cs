using Microsoft.EntityFrameworkCore;
using ProjectDB_API.Controllers;
using ProjectDB_API.Models; // Asegúrate de que esta línea apunte al namespace correcto

namespace ProjectDB_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<LoginResult> LoginResult { get; set; }

    }
}
