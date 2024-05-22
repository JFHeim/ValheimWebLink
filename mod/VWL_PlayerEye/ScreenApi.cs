using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace VWL_PlayerEye;

[PublicAPI]
public static class ScreenApi
{
    public static Bitmap CaptureScreen()
    {
        IntPtr hDesktopWnd = GetDesktopWindow();
        IntPtr hDesktopDC = GetWindowDC(hDesktopWnd);
        IntPtr hCaptureDC = CreateCompatibleDC(hDesktopDC);
        IntPtr hBitmap = CreateCompatibleBitmap(hDesktopDC, Screen.width, Screen.height);
        IntPtr hOldBitmap = SelectObject(hCaptureDC, hBitmap);
        // bool success = BitBlt(hCaptureDC, 0, 0, Screen.width, Screen.height, hDesktopDC, 0, 0,
        //     CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
        Bitmap bitmap = Image.FromHbitmap(hBitmap);
        SelectObject(hCaptureDC, hOldBitmap);
        DeleteObject(hBitmap);
        DeleteDC(hCaptureDC);
        ReleaseDC(hDesktopWnd, hDesktopDC);
        return bitmap;
    }

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdc, uint nFlags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource,
        int xSrc, int ySrc, CopyPixelOperation rop);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    public static List<string> GetOpenWindowNames()
    {
        List<string> windowNames = [];

        EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
        {
            int length = GetWindowTextLength(hWnd);
            if (length > 0 && IsWindowVisible(hWnd))
            {
                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);
                windowNames.Add(builder.ToString());
            }

            return true;
        }, IntPtr.Zero);

        return windowNames;
    }

    public static byte[] GetScreenshot()
    {
        // IntPtr hWnd = FindWindow(null, "valheim");
        // if (hWnd == IntPtr.Zero)
        // {
        //     throw new InvalidOperationException("Unable to find window.");
        // }
        //
        // GetWindowRect(hWnd, out var rect);
        //
        // int width = rect.Right - rect.Left;
        // int height = rect.Bottom - rect.Top;
        //
        // IntPtr hScreenDC = GetDC(IntPtr.Zero);
        // IntPtr hDC = CreateCompatibleDC(hScreenDC);
        // IntPtr hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
        // IntPtr hOldBitmap = SelectObject(hDC, hBitmap);
        //
        // bool success = PrintWindow(hWnd, hDC, 0);
        // if (!success)
        // {
        //     throw new InvalidOperationException("Unable to capture window.");
        // }

        // Bitmap bitmap = Image.FromHbitmap(hBitmap);
        // 
        // SelectObject(hDC, hOldBitmap);
        // DeleteObject(hBitmap);
        // DeleteDC(hDC);
        // ReleaseDC(IntPtr.Zero, hScreenDC);

        //TODO: capture only valheim related staff
        Bitmap bitmap = CaptureScreen();
        bitmap.Save("screenshot.png", ImageFormat.Png);
        return BitmapToByteArray(bitmap);
    }

    private static byte[] BitmapToByteArray(Bitmap bitmap)
    {
        using MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);
        byte[] byteArray = memoryStream.ToArray();
        return byteArray;
    }
}