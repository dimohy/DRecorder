namespace DMRecorder.Core.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DMRecorder.Core.Contracts;


public partial class RecordViewModel : ObservableRecipient
{
    private AudioRecorder _audioRecorder;
    //private readonly IResourceManager _reosurceManager;
    private readonly FilenameFormat _filenamePattern;
    private readonly ISettings _settings;
    private readonly IDispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private bool _isShowErrorMessage;

    [ObservableProperty]
    private RecordState _recordState;

    [ObservableProperty]
    private string? _lastRecordFilename;

    [ObservableProperty]
    private TimeSpan _recordingTime;

    [ObservableProperty]
    private double _totalPlayTime;

    public double RecordingTimeValue => _recordingTime.TotalSeconds;

    public bool IsRecording => _audioRecorder.IsRecording;

    public bool CanPlay
    {
        get => LastRecordFilename is not null && File.Exists(Path.Combine(RecordPath, LastRecordFilename));
    }

    public RelayCommand<RecordState> RecordCommand { get; }

    public ISettings Settings => _settings;

    public IRecordingObserver? RecordingObserver { get; set; }

    public string RecordPath
    {
        get => _settings.RecordPath;
        set => _settings.RecordPath = value;
    }

    public string RecrodFileFormat
    {
        get => _settings.RecordFileFormat;
        set
        {
            _settings.RecordFileFormat = value;

            _filenamePattern.Format = value;
            OnPropertyChanged(nameof(RecordFilename));
        }
    }

    public string RecordFilename
    {
        get => _filenamePattern.Filename;
    }

    public string[] Drivers => AudioRecorder.Drivers;
    public int[] SampleRates { get; } = new[] { 48000, 96000 };

    public RecordViewModel(/*IResourceManager resourceManager, */ISettings settings, IDispatcherQueue dispatcherQueue)
    {
        //_reosurceManager = resourceManager;
        _settings = settings;
        _dispatcherQueue = dispatcherQueue;

        _filenamePattern = new(RecordPath, RecrodFileFormat, ".wav");
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _audioRecorder = new(RecordPath, settings.RecordDriver, settings.RecordSampleRate);

        RecordCommand = new(state =>
        {
            switch (state)
            {
                case RecordState.Play:
                    _audioRecorder.Play(recorderState =>
                    {
                        if (recorderState.State is RecordState.Stop)
                        {
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                RecordState = RecordState.Stop;

                                RecordCommand!.NotifyCanExecuteChanged();
                            });
                        }
                        else if (recorderState.State is RecordState.Play)
                        {
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                TotalPlayTime = recorderState.TotalTimeSpan.TotalSeconds;
                                RecordingTime = recorderState.RunningTimeSpan;
                                OnPropertyChanged(nameof(RecordingTimeValue));
                            });
                        }
                    });

                    //RestartTimer();

                    break;

                case RecordState.PlayPause:
                    _audioRecorder.Pause();

                    break;
                case RecordState.Record:
                    OnPropertyChanged(nameof(RecordFilename));

                    LastRecordFilename = RecordFilename;

                    _audioRecorder.RecordFilename = LastRecordFilename;

                    try
                    {
                        _audioRecorder.Record(recorderState =>
                        {
                            RecordingObserver?.SendValue(recorderState.RecordValue);

                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                RecordingTime = recorderState.RunningTimeSpan;
                                OnPropertyChanged(nameof(RecordingTimeValue));
                            });
                        });
                    }
                    catch
                    {
                        IsShowErrorMessage = true;
                        state = RecordState.Stop;
                    }

                    break;
                case RecordState.RecordPause:
                    _audioRecorder.Pause();

                    break;
                case RecordState.Stop:
                    _audioRecorder.Stop();

                    break;
            }

            RecordingObserver?.SendState(RecordState, state);
            RecordState = state;

            RecordCommand!.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(IsRecording));
        }, state => state switch
            {
                RecordState.Play => RecordState is RecordState.Stop && CanPlay is true,
                RecordState.Stop => RecordState is RecordState.Play or RecordState.Record or RecordState.RecordPause or RecordState.PlayPause,
                RecordState.Record => RecordState is RecordState.Stop,
                RecordState.RecordPause => RecordState is RecordState.Record or RecordState.RecordPause or RecordState.Play or RecordState.PlayPause,
                _ => false
            });
    }

    public void DeleteLastRecordFile()
    {
        if (LastRecordFilename is null)
        {
            return;
        }

        var filename = Path.Combine(RecordPath, LastRecordFilename);
        if (File.Exists(filename) is false)
        {
            return;
        }

        File.Delete(filename);

        OnPropertyChanged(nameof(RecordFilename));
    }

    public void RefreshDriver()
    {
        _audioRecorder = new(RecordPath, Settings.RecordDriver, Settings.RecordSampleRate);
    }
}
