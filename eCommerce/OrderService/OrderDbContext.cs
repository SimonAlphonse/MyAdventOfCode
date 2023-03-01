using OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "OrderService");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1,  UserId = 1, ProductId = 1, Quantity = 2, DateTime = DateTime.Now },
                new Order { Id = 2,  UserId = 2, ProductId = 2, Quantity = 3, DateTime = DateTime.Now },
                new Order { Id = 3,  UserId = 3, ProductId = 3, Quantity = 4, DateTime = DateTime.Now },
                new Order { Id = 4,  UserId = 1, ProductId = 4, Quantity = 5, DateTime = DateTime.Now },
                new Order { Id = 5,  UserId = 2, ProductId = 5, Quantity = 6, DateTime = DateTime.Now },
                new Order { Id = 6,  UserId = 3, ProductId = 6, Quantity = 7, DateTime = DateTime.Now },
                new Order { Id = 7,  UserId = 1, ProductId = 7, Quantity = 8, DateTime = DateTime.Now },
                new Order { Id = 8,  UserId = 2, ProductId = 8, Quantity = 9, DateTime = DateTime.Now },
                new Order { Id = 9,  UserId = 3, ProductId = 9, Quantity = 10, DateTime = DateTime.Now },
                new Order { Id = 10, UserId = 1, ProductId = 10, Quantity = 11, DateTime = DateTime.Now }
            );
        }
    }
}