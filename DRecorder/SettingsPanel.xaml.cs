namespace DRecorder;

using Microsoft.UI.Xaml.Controls;

using DRecorder.Core.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

public sealed partial class SettingsPanel : UserControl
{
    public RecordViewModel ViewModel { get; }

    public SettingsPanel()
    {
        ViewModel = Ioc.Default.GetRequiredService<RecordViewModel>();

        this.InitializeComponent();
    }
}
