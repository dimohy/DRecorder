using Microsoft.VisualBasic;

using NAudio.Wave;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder.Core
{
    public class AudioRecorder : IAudioRecorder
    {
        private string _driverName = "Focusrite USB ASIO";
        private int _sampleRate = 96000;
        private AsioOut? _asioOut;
        private float[]? _buffer;
        private WaveFileWriter? _writer;

        private string _path;

        private RecordState _state;

        public static string[] Drivers => AsioOut.GetDriverNames();

        public string RecordFilename { get; set; }
        public bool IsRecording { get; private set; }

        public AudioRecorder(string path)
        {
            _path = path;

            RecordFilename = $"untitled({DateTime.Now.ToShortDateString()}).wav";
        }

        public void Action(RecordState state)
        {
            _state = state;

            if (state == RecordState.Record)
            {
                if (IsRecording == true)
                    return;

                _asioOut = new(_driverName);
                _asioOut.InputChannelOffset = 0;
                _asioOut.InitRecordAndPlayback(null, 1, _sampleRate);
                _buffer = new float[_asioOut.FramesPerBuffer];

                if (Directory.Exists(_path) == false)
                    Directory.CreateDirectory(_path);
                var filename = Path.Combine(_path, RecordFilename);

                _writer = new WaveFileWriter(filename, new WaveFormat(_sampleRate, 1));
                _asioOut.AudioAvailable += (s, e) =>
                {
                    // 일시 중지이면 녹음하지 않음
                    if (_state == RecordState.RecordPause)
                        return;

                    var count = e.GetAsInterleavedSamples(_buffer);
                    _writer.WriteSamples(_buffer, 0, count);
                };

                _asioOut.Play();

                IsRecording = true;
            }
            else if (state == RecordState.Stop)
            {
                IsRecording = false;

                _asioOut?.Dispose();
                _asioOut = null;
                _writer?.Dispose();
                _writer = null;
            }
        }
    }

    public interface IAudioRecorder
    {
        void Action(RecordState state);
    }
}
