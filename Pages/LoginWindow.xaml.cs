using System.Windows;
using Microsoft.EntityFrameworkCore;
using ObuvApp.Models;

namespace ObuvApp;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        string login = LoginBox.Text.Trim();
        string password = PasswordInput.Password;

        if (login.Length == 0 || password.Length == 0)
        {
            MessageBox.Show("Введите логин и пароль.", "Ошибка авторизации",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var db = new AppDbContext();
            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == login);
            
            if (user == null || user.Password != password)
            {
                MessageBox.Show("Неверный логин или пароль. Проверьте данные и повторите ввод.",
                    "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OpenCatalog(user);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось подключиться к базе данных.\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Guest_Click(object sender, RoutedEventArgs e)
    {
        OpenCatalog(null);
    }

    private void OpenCatalog(User? user)
    {
        Session.CurrentUser = user;
        new MainWindow().Show();
        Close();
    }
}
