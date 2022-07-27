
using NAudio.Wave;

using System.Diagnostics;

namespace DRecorder.Core;

public class AudioRecorder
{
    private readonly string _driverName;
    private readonly int _sampleRate;
    private AsioOut? _asioOut;
    private float[]? _buffer;
    private WaveFileWriter? _writer;

    private AudioFileReader? _reader;
    private WaveOutEvent? _waveOutEvent;

    private RecordState _state = RecordState.Stop;

    public static string[] Drivers => AsioOut.GetDriverNames();


    public string? RecordPath { get; set; }

    public string? RecordFilename { get; set; }
    public bool IsRecording => _state is RecordState.Record or RecordState.RecordPause;
    public bool IsPlaying => _state is RecordState.Play or RecordState.PlayPause;

    public AudioRecorder(string path, string driver, int samplerate)
    {
        RecordPath = path;
        _driverName = driver;
        _sampleRate = samplerate;
    }

    public void Record(Action<AudioRecorderState> onStateCallback)
    {
        if (IsRecording is true)
        {
            return;
        }

        _asioOut = new(_driverName);
        _asioOut.InputChannelOffset = 0;
        _asioOut.InitRecordAndPlayback(null, 1, _sampleRate);
        _buffer = new float[_asioOut.FramesPerBuffer];

        if (RecordPath is null)
        {
            return;
        }

        if (Directory.Exists(RecordPath) is false)
        {
            Directory.CreateDirectory(RecordPath);
        }

        if (RecordFilename is null)
        {
            return;
        }

        var filename = Path.Combine(RecordPath, RecordFilename);
        _writer = new WaveFileWriter(filename, new WaveFormat(_sampleRate, 1));
        var totalCount = 0;
        _asioOut.AudioAvailable += (s, e) =>
        {
            // 일시 중지이면 녹음하지 않음
            if (_state is RecordState.RecordPause)
            {
                return;
            }

            var count = e.GetAsInterleavedSamples(_buffer);

            // TODO: 임시로 예외 발생하지 않도록 이후 수정할 것
            try
            {
                _writer?.WriteSamples(_buffer, 0, count);
            }
            catch { }

            totalCount += count;
            var recordTimeSpan = TimeSpan.FromSeconds((double)totalCount / _sampleRate);
            onStateCallback?.Invoke(new(_state, TimeSpan.FromSeconds(0), recordTimeSpan, _buffer[0]));
        };

        _asioOut.Play();

        _state = RecordState.Record;
    }

    public void Stop()
    {
        if (IsRecording is false && IsPlaying is false)
        {
            return;
        }

        if (IsRecording is true)
        {
            _writer?.Dispose();
            _writer = null;
            _asioOut?.Dispose();
            _asioOut = null;
        }
        else if (IsPlaying is true)
        {
            _waveOutEvent?.Dispose();
            _waveOutEvent = null;
            _reader?.Dispose();
            _reader = null;
        }

        _state = RecordState.Stop;
    }

    public void Pause()
    {
        if (IsRecording is false && IsPlaying is false)
        {
            return;
        }

        switch (_state)
        {
            case RecordState.Record:
                _state = RecordState.RecordPause;
                break;
            case RecordState.RecordPause:
                _state = RecordState.Record;
                break;
            case RecordState.Play:
                _state = RecordState.PlayPause;
                _waveOutEvent!.Pause();
                break;
            case RecordState.PlayPause:
                _state = RecordState.Play;
                _waveOutEvent!.Play();
                break;

            default:
                break;
        }
    }

    public void Play(Action<AudioRecorderState> onStateCallback)
    {
        if (IsPlaying is true)
        {
            return;
        }

        _reader = new AudioFileReader(Path.Combine(RecordPath!, RecordFilename!));

        var totalTimeSpan = TimeSpan.FromSeconds(_reader.Length / _sampleRate / 4);

        _waveOutEvent = new WaveOutEvent();
        _waveOutEvent.PlaybackStopped += (s, e) =>
        {
            Stop();

            var audioRecorderState = new AudioRecorderState(_state, totalTimeSpan, TimeSpan.FromSeconds(0));
            onStateCallback?.Invoke(audioRecorderState);
        };
        _waveOutEvent.Init(_reader);
        _waveOutEvent.Play();

        // 플레이가 종료되었는지를 감지한다.
        _ = Task.Run(() =>
        {
            var waveOut = _waveOutEvent;
            while (waveOut is not null && waveOut.PlaybackState is not PlaybackState.Stopped)
            {
                var posiiton = waveOut.GetPosition();
                var recordTimeSpan = TimeSpan.FromSeconds((double)posiiton / _sampleRate / 4);
                onStateCallback?.Invoke(new(_state, totalTimeSpan, recordTimeSpan));

                Thread.Sleep(50);
            }
        });

        _state = RecordState.Play;
    }
}

public record struct AudioRecorderState(RecordState State, TimeSpan TotalTimeSpan, TimeSpan RunningTimeSpan, float RecordValue = 0f);
