using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Windows10WallPaper.model;

namespace Windows10WallPaper.service
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool SystemParametersInfo(uint uAction, uint uParam, string lpvParam, uint fuWinIni);
    }

    public class SetWallPaper
    {
        private const uint SPI_SETDESKWALLPAPER = 0x0014;
        private const uint SPIF_UPDATEINIFILE = 0x0001;
        private const uint SPIF_SENDWININICHANGE = 0x0002;

        public void process(List<ScreenModel> screenModelList, String filePath)
        {
            // 上下左右のMAXを取得し四角形を作成する
            int minLeft = 0;
            int minTop = 0;
            int maxRight = 0;
            int maxBottom = 0;

            int calcBottom = 0;
            int calcRight = 0;
            foreach (ScreenModel screenModel in screenModelList)
            {
                if (minLeft > screenModel.Screen.Bounds.Location.X)
                {
                    minLeft = screenModel.Screen.Bounds.Location.X;
                }

                if (minTop > screenModel.Screen.Bounds.Location.Y)
                {
                    minTop = screenModel.Screen.Bounds.Location.Y;
                }

                calcRight = screenModel.Screen.Bounds.Location.X + screenModel.Screen.Bounds.Width;
                calcBottom = screenModel.Screen.Bounds.Location.Y + screenModel.Screen.Bounds.Height;


                if (maxRight < calcRight)
                {
                    maxRight = calcRight;
                }

                if (maxBottom < calcBottom)
                {
                    maxBottom = calcBottom;
                }
            }

            // 壁紙用のbitmap作成
            using (Bitmap wallPaperBmp = new Bitmap(Math.Abs(minLeft) + Math.Abs(maxRight), Math.Abs(minTop) + Math.Abs(maxBottom)))
            {
                foreach (ScreenModel screenModel in screenModelList)
                {
                    using (Graphics graphics = Graphics.FromImage(wallPaperBmp))
                    {
                        Point dwowPoint = new Point(Math.Abs(minLeft) + screenModel.Screen.Bounds.Location.X, Math.Abs(minTop) + screenModel.Screen.Bounds.Location.Y);

                        graphics.DrawImage(screenModel.bitmap, new Rectangle(dwowPoint, screenModel.Screen.Bounds.Size));
                    }
                }
                wallPaperBmp.Save(filePath, ImageFormat.Png);


                using (RegistryKey regkeyDesktop = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    if (regkeyDesktop == null)
                    {
                        return;
                    }

                    regkeyDesktop.SetValue("TileWallpaper", "1");
                    regkeyDesktop.SetValue("Wallpaperstyle", "0");
                    regkeyDesktop.SetValue("WallpaperOriginX", "0");
                    regkeyDesktop.SetValue("WallpaperOriginY", "0");
                    regkeyDesktop.SetValue("Wallpaper", @"G:\wallpaper.png");
                }
                uint flags = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? SPIF_SENDWININICHANGE : SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE;
                NativeMethods.SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, flags);
            }
        }
    }
}
