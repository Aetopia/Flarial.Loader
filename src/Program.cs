using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

static class Program
{
    [STAThread]
    static void Main()
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(Environment.ProcessPath);
        using Mutex mutex = new(true, "622D66FB-75AD-47CF-963B-A2C499E9DAF0", out var createdNew); if (!createdNew) return;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        new Application().Run(new Window());
    }
}