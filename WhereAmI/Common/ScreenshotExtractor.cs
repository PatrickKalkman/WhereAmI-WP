using Microsoft.Xna.Framework.Media;

using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WhereAmI.Common
{
    public class ScreenshotExtractor
    {
        private const string TileImageStorageFolder = @"Shared\ShellContent";

        public void Save(UIElement layoutRoot, int actualWidth, int actualHeight, int numberOfShares)
        {
            var bmpCurrentScreenImage = new WriteableBitmap(actualWidth, actualHeight);

            var transform = new ScaleTransform();
            transform.CenterX = actualWidth / 2;
            transform.CenterY = actualHeight / 2;
            transform.ScaleX = 639 / actualWidth;
            transform.ScaleY = 639 / actualWidth;

            bmpCurrentScreenImage.Render(layoutRoot, transform);

            bmpCurrentScreenImage.Invalidate();

            string shareInformation;
            if (numberOfShares > 0)
            {
                shareInformation = string.Format("Shares ({0})", numberOfShares);
            }
            else
            {
                shareInformation = string.Format("No Shares yet!", numberOfShares);
            }

            WriteableBitmap normalTileBitmap = bmpCurrentScreenImage.Resize(336, 336, WriteableBitmapExtensions.Interpolation.Bilinear);
            normalTileBitmap.FillRectangle(5, 10, 330, 80, Color.FromArgb(155, 231, 231, 231));
            normalTileBitmap.DrawText(new Point(20, 20), shareInformation, 47, Colors.Black);

            WriteableBitmap largeTileBitmap = bmpCurrentScreenImage.Crop(0, (336 * 639 / actualWidth) - (336 / 2), 639, 336);
            largeTileBitmap.FillRectangle(10, 17, 235, 54, Color.FromArgb(175, 231, 231, 231));
            largeTileBitmap.DrawText(new Point(20, 20), shareInformation, 27, Colors.Black);

            SaveToIsolatedStorage(normalTileBitmap, "WhereAmI_Normal.jpg", 100);
            SaveToIsolatedStorage(largeTileBitmap, "WhereAmI_Large.jpg", 100);
        }

        public void SaveToIsolatedStorage(WriteableBitmap bitmap, string name, int quality)
        {
            IsolatedStorageFile appIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            if (!appIsolatedStorage.DirectoryExists(TileImageStorageFolder))
            {
                appIsolatedStorage.CreateDirectory(TileImageStorageFolder);
            }

            using (IsolatedStorageFileStream fileStream = appIsolatedStorage.CreateFile(Path.Combine(TileImageStorageFolder, name)))
            {
                bitmap.SaveJpeg(fileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, quality);
            }
        }

        public void SaveToMediaLibrary(WriteableBitmap bitmap, string name, int quality)
        {
            using (var stream = new MemoryStream())
            {
                // Save the picture to the Windows Phone media library.
                bitmap.SaveJpeg(stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, quality);
                stream.Seek(0, SeekOrigin.Begin);
                new MediaLibrary().SavePicture(name, stream);
            }
        }
    }
}
