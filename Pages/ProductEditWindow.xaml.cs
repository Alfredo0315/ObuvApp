using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using ObuvApp.Models;

namespace ObuvApp;

public partial class ProductEditWindow : Window
{
    private readonly int? _productId;
    private string? _newPhotoPath;

    public ProductEditWindow(int? productId)
    {
        InitializeComponent();
        _productId = productId;
        Title = (productId == null ? "Добавление товара" : "Редактирование товара") + " — ООО «Обувь»";
        try
        {
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось загрузить данные товара.\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Loaded += (_, _) => Close();
        }
    }

    private void LoadData()
    {
        using var db = new AppDbContext();
        CategoryBox.ItemsSource = db.Categories.ToList();
        ManufacturerBox.ItemsSource = db.Manufacturers.ToList();
        SupplierBox.ItemsSource = db.Suppliers.ToList();
        UnitBox.ItemsSource = db.Units.ToList();

        var product = _productId == null ? null : db.Products.Find(_productId.Value);
        if (product == null)
        {
            IdBox.Visibility = Visibility.Collapsed;
            DiscountBox.Text = StockBox.Text = "0";
            PhotoImage.Source = ImageStore.Load(null);
            return;
        }

        IdBox.Text = product.ProductID.ToString();
        ArticleBox.Text = product.Article;
        NameBox.Text = product.Name;
        CategoryBox.SelectedValue = product.CategoryID;
        ManufacturerBox.SelectedValue = product.ManufacturerID;
        SupplierBox.SelectedValue = product.SupplierID;
        UnitBox.SelectedValue = product.UnitID;
        PriceBox.Text = product.Price.ToString("0.00");
        DiscountBox.Text = product.Discount.ToString();
        StockBox.Text = product.StockQuantity.ToString();
        DescriptionBox.Text = product.Description;
        PhotoImage.Source = ImageStore.Load(product.Photo);
        DeleteButton.Visibility = Visibility.Visible;
    }

    private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp" };
        if (dialog.ShowDialog(this) != true) return;

        var image = ImageStore.Open(dialog.FileName);
        if (image == null)
        {
            Warn("Не удалось прочитать файл как изображение. Выберите файл JPG, PNG или BMP.");
            return;
        }
        _newPhotoPath = dialog.FileName;
        PhotoImage.Source = image;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (ArticleBox.Text.Trim() == "" || NameBox.Text.Trim() == "")
        {
            Warn("Заполните артикул и наименование товара.");
            return;
        }
        if (CategoryBox.SelectedValue == null || ManufacturerBox.SelectedValue == null
            || SupplierBox.SelectedValue == null || UnitBox.SelectedValue == null)
        {
            Warn("Выберите категорию, производителя, поставщика и единицу измерения.");
            return;
        }
        if (!decimal.TryParse(PriceBox.Text.Replace('.', ','), NumberStyles.Number,
                CultureInfo.GetCultureInfo("ru-RU"), out var price)
            || price < 0 || decimal.Round(price, 2) != price)
        {
            Warn("Цена должна быть неотрицательным числом не более чем с двумя знаками после запятой.");
            return;
        }
        if (!byte.TryParse(DiscountBox.Text, out var discount) || discount > 100)
        {
            Warn("Скидка должна быть целым числом от 0 до 100.");
            return;
        }
        if (!int.TryParse(StockBox.Text, out var stock) || stock < 0)
        {
            Warn("Количество на складе должно быть целым неотрицательным числом.");
            return;
        }

        string? newPhoto = null;
        try
        {
            using var db = new AppDbContext();
            string article = ArticleBox.Text.Trim();
            if (db.Products.Any(p => p.Article == article && p.ProductID != _productId))
            {
                Warn("Товар с таким артикулом уже существует. Укажите другой артикул.");
                return;
            }

            var product = _productId == null ? new Product() : db.Products.Find(_productId.Value)!;
            if (_productId == null) db.Products.Add(product);

            product.Article = article;
            product.Name = NameBox.Text.Trim();
            product.CategoryID = (int)CategoryBox.SelectedValue;
            product.ManufacturerID = (int)ManufacturerBox.SelectedValue;
            product.SupplierID = (int)SupplierBox.SelectedValue;
            product.UnitID = (int)UnitBox.SelectedValue;
            product.Price = price;
            product.Discount = discount;
            product.StockQuantity = stock;
            product.Description = DescriptionBox.Text.Trim();

            string? oldPhoto = null;
            if (_newPhotoPath != null)
            {
                oldPhoto = product.Photo;
                newPhoto = product.Photo = ImageStore.Save(ImageStore.Open(_newPhotoPath)!);
            }

            db.SaveChanges();
            ImageStore.Delete(oldPhoto);
        }
        catch (Exception ex)
        {
            ImageStore.Delete(newPhoto);
            MessageBox.Show("Не удалось сохранить товар.\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DialogResult = true;
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var db = new AppDbContext();
            if (db.OrderItems.Any(i => i.ProductID == _productId))
            {
                MessageBox.Show("Этот товар присутствует в заказах, поэтому его нельзя удалить.",
                    "Удаление товара", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Удалить товар безвозвратно?", "Удаление товара",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var product = db.Products.Find(_productId!.Value)!;
            db.Products.Remove(product);
            db.SaveChanges();
            ImageStore.Delete(product.Photo);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось удалить товар.\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DialogResult = true;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static void Warn(string message)
    {
        MessageBox.Show(message, "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
