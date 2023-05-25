namespace DRecorder;

using CommunityToolkit.Mvvm.DependencyInjection;

using DRecorder.Core.ViewModels;
using DRecorder.Extensions;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System;
using System.IO;

using Windows.Storage;
using Windows.System;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public RecordViewModel ViewModel { get; }

    public MainWindow()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<RecordViewModel>();

        //var content = (Content as FrameworkElement)!;
        //this.Theme = content.ActualTheme;
        //content.ActualThemeChanged += (s, e) => this.Theme = content.ActualTheme;

        Title = "ApplicationTitle".GetLocalized();

        AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
        var width = 210;
        var height = 180;
        var workArea = DisplayArea.Primary.WorkArea;
        AppWindow.MoveAndResize(new((workArea.Width - width) / 2, (workArea.Height - height) / 2, width, height), DisplayArea.Primary);
        AppWindow.Closing += MainWindow_Closing;
        //AppWindow.SetIcon()
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
        var bResult = ViewModel.DeleteLastRecordFile();

        if (bResult is true)
        {
            
        }
    }

    private void MainWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        //args.Cancel = true;
    }
}
