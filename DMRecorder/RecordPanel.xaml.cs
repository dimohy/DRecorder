namespace DMRecorder;

using CommunityToolkit.Mvvm.DependencyInjection;

using DMRecorder.Core;
using DMRecorder.Core.Contracts;
using DMRecorder.Core.ViewModels;

using Microsoft.UI.Xaml.Controls;

public sealed partial class RecordPanel : UserControl, IRecordingObserver
{
    public RecordViewModel ViewModel { get; }


    public RecordPanel()
    {
        ViewModel = Ioc.Default.GetRequiredService<RecordViewModel>();
        ViewModel.RecordingObserver = this;

        this.InitializeComponent();
    }

    public void SendValue(float value)
    {
        recordingVisualizer.AddValue(value);
    }

    public void SendState(RecordState beforeState, RecordState state)
    {
        if (beforeState is RecordState.Stop && state is RecordState.Record)
            recordingVisualizer.ClearValues();
    }
}
