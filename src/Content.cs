using System;
using System.Windows;
using System.Windows.Controls;
using Flarial.Launcher.SDK;

sealed class Content : Grid
{
    internal Content(Window @this)
    {
        RowDefinitions.Add(new());

        Margin = new(10);

        Button button = new()
        {
            Content = "▶",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new(75, 25, 75, 25)
        };

        ProgressBar progressBar = new()
        {
            Visibility = Visibility.Hidden,
            IsIndeterminate = true,
            VerticalAlignment = VerticalAlignment.Bottom
        };

        CheckBox checkBox = new()
        {
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        SetRow(button, default); SetColumn(button, default);
        Children.Add(button);

        SetRow(checkBox, 0); SetColumn(checkBox, default);
        Children.Add(checkBox);

        SetRow(progressBar, default); SetColumn(progressBar, default);
        Children.Add(progressBar);

        button.Click += async (_, _) =>
        {
            button.IsEnabled = false;
            checkBox.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Visible;

            await Client.DownloadAsync((bool)checkBox.IsChecked, (_) => Dispatcher.Invoke(() =>
            {
                if (progressBar.Value != _)
                {
                    if (progressBar.IsIndeterminate) progressBar.IsIndeterminate = false;
                    progressBar.Value = _;
                }
            }));

            progressBar.Value = default;
            progressBar.IsIndeterminate = true;

            bool _ = true;
            try { await Client.LaunchAsync((bool)checkBox.IsChecked); }
            catch (OperationCanceledException) { _ = false; }

            progressBar.Visibility = Visibility.Hidden;
            checkBox.Visibility = Visibility.Visible;
            button.IsEnabled = true;

            if (_) @this.Close();
        };
    }
}