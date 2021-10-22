namespace DMRecorder;

using DMRecorder.Extensions;

using Microsoft.UI.Xaml;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : CustomWindow
{
    public MainWindow()
    {
        this.InitializeComponent();

        var content = (Content as FrameworkElement)!;
        this.Theme = content.ActualTheme;
        content.ActualThemeChanged += (s, e) => this.Theme = content.ActualTheme;

        Title = "ApplicationTitle".GetLocalized();
    }
}
