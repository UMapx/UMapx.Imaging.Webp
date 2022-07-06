using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace UMapx.Imaging.Webp.Example
{
    internal static class Program
    {
        static void Main()
        {
            Console.WriteLine("UMapx.Imaging.Webp example");
            var files = Directory.GetFiles(@"..\..\..\images", "*.*", SearchOption.AllDirectories);
            var path = @"..\..\..\results";
            Directory.CreateDirectory(path);

            Console.WriteLine($"Processing {files.Length} images");

            foreach (var file in files)
            {
                Console.WriteLine($"Reading {file}");

                if (file.EndsWith(".jpeg") ||
                    file.EndsWith(".jpg"))
                 {
                    using var bitmap = new Bitmap(file);
                    var webP = bitmap.ToWebp();
                    var filename = Path.Combine(path, $"{Path.GetFileNameWithoutExtension(file)}.webp");
                    File.WriteAllBytes(filename, webP);
                    Console.WriteLine($"Image save as {filename}");
                }
                else if (file.EndsWith(".webp"))
                {
                    var webp = File.ReadAllBytes(file);
                    using var bitmap = BitmapWebp.FromWebp(webp);
                    using var jpeg = bitmap.ToJpeg();
                    var filename = Path.Combine(path, $"{Path.GetFileNameWithoutExtension(file)}.jpg");
                    jpeg.Save(filename, ImageFormat.Jpeg);
                    Console.WriteLine($"Image save as {filename}");
                }
                else
                {
                    Console.WriteLine($"Could not recognize image format {file}");
                }    
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
