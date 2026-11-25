using System.Windows;
using Microsoft.EntityFrameworkCore;
using ObuvApp.Models;

namespace ObuvApp;

public partial class OrdersWindow : Window
{
    private record OrderRow(int Number, string OrderDate, string DeliveryDate, string Client,
        string Address, string Items, int Code, string Status);

    public OrdersWindow()
    {
        InitializeComponent();
        LoadOrders();
    }

    private void LoadOrders()
    {
        try
        {
            using var db = new AppDbContext();
            OrdersGrid.ItemsSource = db.Orders
                .Include(o => o.User)
                .Include(o => o.PickupPoint)
                .Include(o => o.Status)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .OrderBy(o => o.OrderID)
                .ToList()
                .Select(o => new OrderRow(
                    o.OrderID,
                    o.OrderDate.ToString("dd.MM.yyyy"),
                    o.DeliveryDate.ToString("dd.MM.yyyy"),
                    o.User.FullName,
                    o.PickupPoint.Address,
                    string.Join("; ", o.OrderItems.Select(i => $"{i.Product.Name} ({i.Product.Article}) × {i.Quantity}")),
                    o.ReceiveCode,
                    o.Status.Name))
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось загрузить список заказов.\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
