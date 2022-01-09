namespace DMRecorder;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using WinRT.Interop;

using Windows.UI;
using Microsoft.UI;

public class CustomWindow : Window
{
    private WindowLocationKind _windowLocation;
    private ElementTheme _appTheme;


    public int Width
    {
        get => AppWindow.Size.Width;
        set => AppWindow.Resize(new(value, Height));
    }

    public int Height
    {
        get => AppWindow.Size.Height;
        set => AppWindow.Resize(new(Width, value));
    }

    public AppWindowPresenterKind Presenter
    {
        get => AppWindow.Presenter.Kind;
        set => AppWindow.SetPresenter(value);
    }

    public bool IsMaximize
    {
        get
        {
            var presneter = AppWindow.Presenter;
            if (presneter is not OverlappedPresenter olPresenter)
                return false;

            return olPresenter.IsMaximizable;
        }

        set
        {
            var presneter = AppWindow.Presenter;
            if (presneter is not OverlappedPresenter olPresenter)
                return;

            if (value is true)
                DispatcherQueue.TryEnqueue(() => olPresenter.Maximize());
        }
    }

    /// <summary>
    /// TODO: AppWindow.TitleBar를 통한 TitleBar 색 변경은 Windows App SDK 버젼이 올라가면 다르게 동작할 수 있다.
    /// </summary>
    public ElementTheme Theme
    {
        get => _appTheme;
        set
        {
            if (value == default)
                value = (Content as FrameworkElement)!.ActualTheme is ElementTheme.Light ? ElementTheme.Light : ElementTheme.Dark;

            if (_appTheme == value)
                return;
            _appTheme = value;

            var titleBar = AppWindow.TitleBar;
            if (_appTheme is ElementTheme.Light)
            {
                titleBar.ForegroundColor = Colors.Black;
                titleBar.BackgroundColor = Colors.White;
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Colors.White;

                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonBackgroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Colors.White;

                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 245, 245, 245);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Colors.White;
            }
            else if (_appTheme is ElementTheme.Dark)
            {
                titleBar.ForegroundColor = Colors.White;
                titleBar.BackgroundColor = Color.FromArgb(255, 31, 31, 31);
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Color.FromArgb(255, 31, 31, 31);

                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonBackgroundColor = Color.FromArgb(255, 31, 31, 31);
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 31, 31, 31);

                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 51, 51, 51);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }

            // 아이콘 배경 색이 적용 안되는 버그 수정
            titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            titleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
        }
    }

    public WindowLocationKind WindowLocation
    {
        get => _windowLocation;
        set
        {
            if (value == _windowLocation)
                return;

            _windowLocation = value;
            switch (value)
            {
                case WindowLocationKind.Default:
                    break;
                case WindowLocationKind.PrimaryCenter:
                    var displayArea = DisplayArea.Primary;
                    var x = (displayArea.WorkArea.Width - Width) / 2;
                    var y = (displayArea.WorkArea.Height - Height) / 2;
                    AppWindow.MoveAndResize(new(x, y, Width, Height), displayArea);
                    break;
            }
        }
    }
    public AppWindow AppWindow => GetAppWindowForCurrentWidow();

    public CustomWindow()
    {
    }

    private AppWindow GetAppWindowForCurrentWidow()
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var winId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(winId);
    }
}

public enum WindowLocationKind
{
    Default,
    PrimaryCenter,
}
