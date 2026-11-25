using Microsoft.EntityFrameworkCore;

namespace ObuvApp.Models;

public class AppDbContext : DbContext
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<PickupPoint> PickupPoints => Set<PickupPoint>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=Obuv;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Role>().ToTable("Roles").HasKey(e => e.RoleID);
        model.Entity<User>().ToTable("Users").HasKey(e => e.UserID);
        model.Entity<Category>().ToTable("Categories").HasKey(e => e.CategoryID);
        model.Entity<Supplier>().ToTable("Suppliers").HasKey(e => e.SupplierID);
        model.Entity<Manufacturer>().ToTable("Manufacturers").HasKey(e => e.ManufacturerID);
        model.Entity<Unit>().ToTable("Units").HasKey(e => e.UnitID);
        model.Entity<PickupPoint>().ToTable("PickupPoints").HasKey(e => e.PickupPointID);
        model.Entity<OrderStatus>().ToTable("OrderStatuses").HasKey(e => e.StatusID);
        model.Entity<Order>().ToTable("Orders").HasKey(e => e.OrderID);
        model.Entity<OrderItem>().ToTable("OrderItems")
            .HasKey(e => new { e.OrderID, e.ProductID });

        model.Entity<User>().Ignore(e => e.FullName);

        model.Entity<Product>(entity =>
        {
            entity.ToTable("Products").HasKey(e => e.ProductID);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
        });
    }
}
