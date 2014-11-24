using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WhereAmI.Common
{
    public static class WritableBitmapExEx
    {
        public static void DrawText(this WriteableBitmap wBmp, Point at, string text, double fontSize, Color textColor)
        {
            TextBlock lbl = new TextBlock();
            lbl.Text = text;
            lbl.FontSize = fontSize;
            lbl.Foreground = new SolidColorBrush(textColor);

            WriteableBitmap tBmp = new WriteableBitmap(lbl, null);
            wBmp.Blit(at, tBmp, new Rect(0, 0, tBmp.PixelWidth, tBmp.PixelHeight), Colors.White, System.Windows.Media.Imaging.WriteableBitmapExtensions.BlendMode.Alpha);
        }
    }
}
