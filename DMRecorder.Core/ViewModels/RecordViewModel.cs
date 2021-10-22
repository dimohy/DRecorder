namespace DMRecorder.Core.ViewModels;

using DMRecorder.Core.Contracts;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

public partial class RecordViewModel : ObservableRecipient
{
    private AudioRecorder _audioRecorder;
    private IResourceManager _reosurceManager;

    [ObservableProperty]
    private RecordState _recordState;

    public RelayCommand<RecordState> RecordCommand { get; }

    public string[] Drivers => AudioRecorder.Drivers;

    public RecordViewModel(IResourceManager resourceManager)
    {
        _reosurceManager = resourceManager;

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _audioRecorder = new(Path.Combine(appData, _reosurceManager.GetLocalized("Voice Records")));

        RecordCommand = new(state =>
        {
            // Record 또는 Play에 따라 일시 정지 분기
            if (state == RecordState.RecordPause)
            {
                state = RecordState switch
                {
                    RecordState.Record => RecordState.RecordPause,
                    RecordState.RecordPause => RecordState.Record,
                    RecordState.Play => RecordState.PlayPause,
                    RecordState.PlayPause => RecordState.Play,
                    _ => state
                };
            }

            switch (state)
            {
                case RecordState.Play:
                    break;
                case RecordState.PlayPause:
                    break;
                case RecordState.Record:
                    _audioRecorder.Action(state);
                    break;
                case RecordState.RecordPause:
                    _audioRecorder.Action(state);
                    break;
                case RecordState.Stop:
                    _audioRecorder.Action(state);
                    break;
            }

            RecordState = state;

            RecordCommand!.NotifyCanExecuteChanged();
        }, state => state switch
            {
                RecordState.Play => RecordState is RecordState.Stop,
                RecordState.Stop => RecordState is RecordState.Play or RecordState.Record or RecordState.RecordPause or RecordState.PlayPause,
                RecordState.Record => RecordState is RecordState.Stop,
                RecordState.RecordPause => RecordState is RecordState.Record or RecordState.RecordPause or RecordState.Play or RecordState.PlayPause,
                _ => false
            });
    }

    public void GetTest() { }
}
