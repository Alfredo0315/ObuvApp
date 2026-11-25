using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ObuvApp;

internal static class ImageStore
{
    private static string Folder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

    public static ImageSource Load(string? fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            var image = Open(Path.Combine(Folder, fileName));
            if (image != null) return image;
        }
        return new BitmapImage(new Uri("pack://application:,,,/picture.png"));
    }

    public static BitmapImage? Open(string path)
    {
        try
        {
            if (!File.Exists(path)) return null;
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string Save(BitmapImage source)
    {
        double scale = Math.Min(1.0, Math.Min(300.0 / source.PixelWidth, 200.0 / source.PixelHeight));
        BitmapSource result = new TransformedBitmap(source, new ScaleTransform(scale, scale));

        Directory.CreateDirectory(Folder);
        string fileName = Guid.NewGuid().ToString("N") + ".png";
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(result));
        using var stream = File.Create(Path.Combine(Folder, fileName));
        encoder.Save(stream);
        return fileName;
    }

    public static void Delete(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;
        try { File.Delete(Path.Combine(Folder, fileName)); }
        catch (IOException) { }
    }
}
