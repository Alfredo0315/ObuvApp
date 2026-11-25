using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ObuvApp.Models;

namespace ObuvApp;

public partial class MainWindow : Window
{
    private List<Product> _products = new();

    public MainWindow()
    {
        InitializeComponent();
        UserNameText.Text = Session.DisplayName;
        ToolPanel.Visibility = OrdersButton.Visibility =
            Session.CanFilter ? Visibility.Visible : Visibility.Collapsed;
        AddButton.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        LoadProducts();
    }

    private void LoadProducts()
    {
        try
        {
            using var db = new AppDbContext();
            _products = db.Products
                .Include(p => p.Category).Include(p => p.Manufacturer)
                .Include(p => p.Supplier).Include(p => p.Unit)
                .OrderBy(p => p.ProductID).ToList();

            var suppliers = new List<string> { "Все поставщики" };
            suppliers.AddRange(db.Suppliers.Select(s => s.Name));
            SupplierFilter.ItemsSource = suppliers;
            SupplierFilter.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось загрузить список товаров.\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        IEnumerable<Product> products = _products;

        if (Session.CanFilter)
        {
            if (SupplierFilter.SelectedIndex > 0)
                products = products.Where(p => p.Supplier.Name == (string)SupplierFilter.SelectedItem);
            
            var words = SearchBox.Text.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            products = products.Where(p =>
            {
                string text = $"{p.Article} {p.Name} {p.Description} {p.Category.Name} " +
                              $"{p.Manufacturer.Name} {p.Supplier.Name} {p.Unit.Name}".ToLower();
                return words.All(word => text.Contains(word));
            });

            if (SortBox.SelectedIndex == 1) products = products.OrderBy(p => p.StockQuantity);
            if (SortBox.SelectedIndex == 2) products = products.OrderByDescending(p => p.StockQuantity);
        }

        ProductsList.ItemsSource = products.ToList();
    }

    private void Filters_Changed(object sender, EventArgs e)
    {
        if (IsLoaded) ApplyFilters();
    }

    private void ProductItem_Click(object sender, MouseButtonEventArgs e)
    {
        if (Session.IsAdmin && ((FrameworkElement)sender).DataContext is Product product)
            OpenEditor(product.ProductID);
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        OpenEditor(null);
    }
    
    private void OpenEditor(int? productId)
    {
        if (new ProductEditWindow(productId) { Owner = this }.ShowDialog() == true)
            LoadProducts();
    }

    private void Orders_Click(object sender, RoutedEventArgs e)
    {
        new OrdersWindow { Owner = this }.ShowDialog();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        Session.CurrentUser = null;
        new LoginWindow().Show();
        Close();
    }
}
