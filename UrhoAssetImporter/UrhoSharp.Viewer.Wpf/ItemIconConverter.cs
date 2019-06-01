using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UrhoSharp.Viewer.Wpf
{
    public class ItemIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return GetRegisteredIcon(value.ToString(), 16, 16);
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public static ImageSource GetRegisteredIcon(string filePath, int width, int height)
        {
            var shinfo = new SHfileInfo();
            const uint SHGFI_ICON = 0x100;
            const uint SHGFI_SMALLICON = 0x1;

            SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
            var icon = Icon.FromHandle(shinfo.hIcon);
            var hBitmap = icon.ToBitmap().GetHbitmap();
            var bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(width, height));

            DeleteObject(hBitmap);
            return bmpSource;
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHfileInfo psfi, uint cbSizeFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHfileInfo
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}
