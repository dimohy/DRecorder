namespace DMRecorder.Core.ViewModels;

using DMRecorder.Core.Contracts;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.VisualBasic;

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime;

public partial class RecordViewModel : ObservableRecipient
{

    private AudioRecorder _audioRecorder;
    private IResourceManager _reosurceManager;
    private FilenamePattern _filenamePattern;
    private ISettings _settings;
    private PeriodicTimer? _recordingTimer;
    private Stopwatch? _recordingStopwatch;
    private IDispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private RecordState _recordState;

    [ObservableProperty]
    private string? _lastRecordFilename;

    [ObservableProperty]
    private TimeSpan _recordingTime;

    public RelayCommand<RecordState> RecordCommand { get; }

    public ISettings Settings => _settings;

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

            _filenamePattern.Pattern = value;
            OnPropertyChanged(nameof(RecordFilename));
        }
    }

    public string RecordFilename
    {
        get => _filenamePattern.Filename;
    }

    public string[] Drivers => AudioRecorder.Drivers;
    public int[] SampleRates { get; } = new[] { 48000, 96000 };

    public RecordViewModel(IResourceManager resourceManager, ISettings settings, IDispatcherQueue dispatcherQueue)
    {
        _reosurceManager = resourceManager;
        _settings = settings;
        _dispatcherQueue = dispatcherQueue;

        _filenamePattern = new(RecordPath, RecrodFileFormat, ".wav");
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _audioRecorder = new(RecordPath, settings.RecordDriver, settings.RecordSampleRate);

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
                    OnPropertyChanged(nameof(RecordFilename));

                    LastRecordFilename = RecordFilename;

                    if (_audioRecorder.IsRecording == false)
                    {
                        _recordingTimer = new(TimeSpan.FromMilliseconds(50));
                        _ = Task.Run(async () =>
                        {
                            _recordingStopwatch = Stopwatch.StartNew();

                            while (await _recordingTimer.WaitForNextTickAsync())
                            {
                                _dispatcherQueue.TryEnqueue(() => RecordingTime = _recordingStopwatch.Elapsed);
                            }
                        });
                    }
                    else
                        _recordingStopwatch?.Start();

                    _audioRecorder.RecordFilename = LastRecordFilename;
                    _audioRecorder.Action(state);

                    break;
                case RecordState.RecordPause:
                    _audioRecorder.Action(state);

                    _recordingStopwatch?.Stop();

                    break;
                case RecordState.Stop:
                    _audioRecorder.Action(state);

                    _recordingTimer?.Dispose();

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

    public void DeleteLastRecordFile()
    {
        if (LastRecordFilename is null)
            return;

        var filename = Path.Combine(RecordPath, LastRecordFilename);
        if (File.Exists(filename) == false)
            return;

        File.Delete(filename);

        OnPropertyChanged(nameof(RecordFilename));
    }

    public void RefreshDriver()
    {
        _audioRecorder = new(RecordPath, Settings.RecordDriver, Settings.RecordSampleRate);
    }
}
