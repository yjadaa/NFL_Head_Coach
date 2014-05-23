using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Ink;

namespace NFL_Head_Coach
{
    class Player : PlayerBehavior 
    {
        public enum PlayerRole
        {
            // Attack Position
            Quarter_Back = 0,
            Wide_Receiver = 1,
            Runing_Back = 2,
            TackleGurad = 3,

            //Defense Position
            MiddleLineBacker =4,
            CornorBack = 5,
            Safety = 6,
            OutsideLineBacker = 7
        }

        public PlayerRole Role;
        public MotionType CurrentMotionType;

        // Common feature
        public Image PlayerImage;
        public Point Position;
        public Path Path;

        public Player()
        {
            // Assign a texture for each player
            // Access the image file with bitmap
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"C:\Users\Student\Desktop\NFL Head Coach\NFL Head Coach\NFL Head Coach\Images\player.png");
            bitmap.EndInit();
            // Assign the bitmap to the image
            PlayerImage = new Image();
            PlayerImage.Height = 32;
            PlayerImage.Width = 32;
            PlayerImage.Source = bitmap;
            
            Path = new Path();

            // Set the original position for the player
            Position = new Point(0, 0);
            CurrentMotionType = MotionType.Null;
        }
    }
}
