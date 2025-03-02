using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using Flarial.Launcher.SDK;

sealed class Content : Grid
{
    readonly CheckBox CheckBox = new()
    {
        VerticalAlignment = VerticalAlignment.Bottom,
        HorizontalAlignment = HorizontalAlignment.Center,
    };

    internal bool Value { set { CheckBox.IsChecked = value; } }

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

        SetRow(button, default); SetColumn(button, default);
        Children.Add(button);

        SetRow(CheckBox, 0); SetColumn(CheckBox, default);
        Children.Add(CheckBox);

        SetRow(progressBar, default); SetColumn(progressBar, default);
        Children.Add(progressBar);

        button.Click += async (_, _) =>
        {
            button.IsEnabled = false;
            CheckBox.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Visible;

            await Client.DownloadAsync((bool)CheckBox.IsChecked, (_) => Dispatcher.Invoke(() =>
            {
                if (progressBar.Value != _)
                {
                    if (progressBar.IsIndeterminate) progressBar.IsIndeterminate = false;
                    progressBar.Value = _;
                }
            }));

            progressBar.Value = default;
            progressBar.IsIndeterminate = true;

            bool value = await Client.LaunchAsync((bool)CheckBox.IsChecked);

            progressBar.Visibility = Visibility.Hidden;
            CheckBox.Visibility = Visibility.Visible;
            button.IsEnabled = true;

            if (value) @this.Close();
        };

        Application.Current.Exit += (_, _) => File.WriteAllText("Flarial.Loader.json", JsonSerializer.Serialize((bool)CheckBox.IsChecked));
    }
}