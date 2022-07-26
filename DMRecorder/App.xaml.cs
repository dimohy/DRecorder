using CommunityToolkit.Mvvm.DependencyInjection;

using DMRecorder.Core.Contracts;
using DMRecorder.Core.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;

using System;
using System.ComponentModel;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DMRecorder;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private static readonly string AppName = "DMRecorder";

    private Window m_window = default!;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        Ioc.Default.ConfigureServices(ConfigureServices());
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow {
            Icon = "Assets/dmrecorder.ico"
        };
        m_window.Activate();
    }

    private IServiceProvider ConfigureServices()
    {
        var s = new ServiceCollection();

        s.AddSingleton<Core.Contracts.IResourceManager>(new ResourceManager());

        s.AddSingleton<IDispatcherQueue>(new DispatcherQueue(() => m_window.DispatcherQueue));


        // 설정 파일 위치
        var appSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
        if (Directory.Exists(appSettingsPath) is false)
            Directory.CreateDirectory(appSettingsPath);
        var appSettingsFilename = Path.Combine(appSettingsPath, "settings.config");
        s.AddSingleton<ISettings>(AppSettings.Load(appSettingsFilename));

        //s.AddTransient<RecordViewModel>();
        // 구조가 단순하므로 ViewModel을 싱글톤으로 유지한다.
        s.AddSingleton<RecordViewModel>();


        return s.BuildServiceProvider();
    }
}


public class ResourceManager : Core.Contracts.IResourceManager
{
    private readonly ResourceLoader _resLoader = new();

    public string GetLocalized(string resourceKey)
    {
        return _resLoader.GetString(resourceKey);
    }
}

public class DispatcherQueue : IDispatcherQueue
{
    private readonly Func<Microsoft.UI.Dispatching.DispatcherQueue> _dispoacherQueueFunc;

    public DispatcherQueue(Func<Microsoft.UI.Dispatching.DispatcherQueue> dispoacherQueueFunc)
    {
        _dispoacherQueueFunc = dispoacherQueueFunc;
    }

    public void TryEnqueue(Action action)
    {
        try
        {
            _dispoacherQueueFunc().TryEnqueue(new Microsoft.UI.Dispatching.DispatcherQueueHandler(action));
        }
        catch { }
    }
}
