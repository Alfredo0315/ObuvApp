using ObuvApp.Models;

namespace ObuvApp;

internal static class Session
{
    public static User? CurrentUser { get; set; }

    public static bool IsGuest => CurrentUser == null;

    public static string DisplayName => CurrentUser?.FullName ?? "Гость";

    public static bool IsAdmin => CurrentUser?.Role.Name == "Администратор";

    public static bool IsManager => CurrentUser?.Role.Name == "Менеджер";

    public static bool CanFilter => IsAdmin || IsManager;
}
