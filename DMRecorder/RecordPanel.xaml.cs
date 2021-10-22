namespace DMRecorder;

using DMRecorder.Core.ViewModels;

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

public sealed partial class RecordPanel : UserControl
{
    public RecordViewModel ViewModel { get; }


    public RecordPanel()
    {
        ViewModel = Ioc.Default.GetRequiredService<RecordViewModel>();
        DataContext = this;

        this.InitializeComponent();
    }
}
