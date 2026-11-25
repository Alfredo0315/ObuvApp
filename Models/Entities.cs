using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace ObuvApp.Models;

public class Role
{
    public int RoleID { get; set; }
    public string Name { get; set; } = "";
}

public class User
{
    public int UserID { get; set; }
    public string LastName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? Patronymic { get; set; }
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public int RoleID { get; set; }

    public Role Role { get; set; } = null!;

    public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
}

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; } = "";
}

public class Supplier
{
    public int SupplierID { get; set; }
    public string Name { get; set; } = "";
}

public class Manufacturer
{
    public int ManufacturerID { get; set; }
    public string Name { get; set; } = "";
}

public class Unit
{
    public int UnitID { get; set; }
    public string Name { get; set; } = "";
}

public class Product
{
    public int ProductID { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public int UnitID { get; set; }
    public decimal Price { get; set; }
    public int SupplierID { get; set; }
    public int ManufacturerID { get; set; }
    public int CategoryID { get; set; }
    public byte Discount { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }

    public Unit Unit { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public Manufacturer Manufacturer { get; set; } = null!;
    public Category Category { get; set; } = null!;

    [NotMapped] public decimal FinalPrice => Math.Round(Price * (100 - Discount) / 100m, 2);
    [NotMapped] public bool HasDiscount => Discount > 0;
    [NotMapped] public bool IsBigDiscount => Discount > 15;
    [NotMapped] public bool IsOutOfStock => StockQuantity == 0;
    [NotMapped] public string Header => $"{Category.Name} | {Name}";
    [NotMapped] public ImageSource Image => ImageStore.Load(Photo);
}

public class PickupPoint
{
    public int PickupPointID { get; set; }
    public string Address { get; set; } = "";
}

public class OrderStatus
{
    public int StatusID { get; set; }
    public string Name { get; set; } = "";
}

public class Order
{
    public int OrderID { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public int PickupPointID { get; set; }
    public int UserID { get; set; }
    public int ReceiveCode { get; set; }
    public int StatusID { get; set; }

    public PickupPoint PickupPoint { get; set; } = null!;
    public User User { get; set; } = null!;
    public OrderStatus Status { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = new();
}

public class OrderItem
{
    public int OrderID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
