namespace DMRecorder;

using CommunityToolkit.Mvvm.DependencyInjection;

using DMRecorder.Core.ViewModels;
using DMRecorder.Extensions;

using Microsoft.UI.Xaml;

using System;
using System.IO;

using Windows.Storage;
using Windows.System;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : CustomWindow
{
    public RecordViewModel ViewModel { get; }

    public MainWindow()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<RecordViewModel>();

        var content = (Content as FrameworkElement)!;
        this.Theme = content.ActualTheme;
        content.ActualThemeChanged += (s, e) => this.Theme = content.ActualTheme;

        Title = "ApplicationTitle".GetLocalized();
    }

    private async void openFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var path = ViewModel.RecordPath;

        var folder = await StorageFolder.GetFolderFromPathAsync(path);
        await Launcher.LaunchFolderAsync(folder);
    }

    private void settingCloseButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Settings.Save();
        ViewModel.RefreshDriver();

        recordPanel.Visibility = Visibility.Visible;
        settingPanel.Visibility = Visibility.Collapsed;
    }

    private void settingButton_Click(object sender, RoutedEventArgs e)
    {
        recordPanel.Visibility = Visibility.Collapsed;
        settingPanel.Visibility = Visibility.Visible;
    }

    private void deleteFileButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.DeleteLastRecordFile();
    }

    private void MainWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        //args.Cancel = true;
    }
}
