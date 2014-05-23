using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NFL_Head_Coach
{
    class Arrow
    {
        public Arrow()
        {
            // Assign a texture for each player
            // Access the image file with bitmap
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"C:\Users\Student\Desktop\NFL Head Coach\NFL Head Coach\NFL Head Coach\Images\arrow.png");
            bitmap.EndInit();
            // Assign the bitmap to the image
            ArrowImage = new Image();
            ArrowImage.Height = 32;
            ArrowImage.Width = 64;
            ArrowImage.Source = bitmap;
        }
        public Image ArrowImage;
    }
}
