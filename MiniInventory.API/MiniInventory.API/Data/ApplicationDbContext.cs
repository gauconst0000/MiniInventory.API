using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Models;

namespace MiniInventory.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<InventoryTransactionDetail> InventoryTransactionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tự động tạo tài khoản Admin khi khởi tạo Database
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "123456", // Trong thực tế nên mã hóa, nhưng ở mức cơ bản ta để nguyên theo yêu cầu
                    Role = "Admin"
                }
            );
        }
    }
}