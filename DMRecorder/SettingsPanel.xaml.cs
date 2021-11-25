namespace DMRecorder;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using DMRecorder.Core.ViewModels;

public sealed partial class SettingsPanel : UserControl
{
    public RecordViewModel ViewModel { get; }

    public SettingsPanel()
    {
        ViewModel = Ioc.Default.GetRequiredService<RecordViewModel>();

        this.InitializeComponent();
    }
}
