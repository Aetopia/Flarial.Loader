using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Flarial.Launcher.SDK;

sealed partial class Window : System.Windows.Window
{
    const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    const int MB_ICONERROR = 0x00000010;

    [LibraryImport("Dwmapi")]
    private static partial nint DwmSetWindowAttribute(nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [LibraryImport("Wininet")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);

    [LibraryImport("Shell32", EntryPoint = "ShellMessageBoxW")]
    private static partial int ShellMessageBox(nint hAppInst, nint hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpcText, [MarshalAs(UnmanagedType.LPWStr)] string lpcTitle = "Error", int fuStyle = MB_ICONERROR);

    internal Window()
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(".ico"))
            Icon = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        Title = "Flarial Loader";
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        UseLayoutRounding = true;
        SizeToContent = SizeToContent.WidthAndHeight;
        ResizeMode = ResizeMode.NoResize;
        ThemeMode = ThemeMode.Dark;
        Content = new Content(this) { Width = 480, Height = 270, IsEnabled = default };

        var hWnd = new WindowInteropHelper(this).EnsureHandle();

        Dispatcher.UnhandledException += (_, e) =>
        {
            e.Handled = true;
            var exception = e.Exception;
            while (exception.InnerException != default) exception = e.Exception.InnerException;
            ShellMessageBox(default, hWnd, exception.Message);
            Close();
        };

        int pvAttribute = 1;
        DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref pvAttribute, sizeof(int));

        ContentRendered += async (_, _) => { if (InternetGetConnectedState(out _, default)) ((Content)Content).IsEnabled = await (await Catalog.GetAsync()).CompatibleAsync(); };
    }
}
