using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<ProductMaster> ProductMaster { get; set; }
    public DbSet<CartDetails> CartDetails { get; set; }
    public DbSet<CategoryMaster> CategoryMaster { get; set; }
    public DbSet<UserMaster> UserMaster { get; set; }
}
