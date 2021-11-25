using DMRecorder.Core;
using DMRecorder.Core.Contracts;
using DMRecorder.Extensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder;

public class AppSettings : Settings<AppSettings>, ISettings
{
    private static readonly int DefaultSampleRate = 48000;
    private static readonly string DefaultRecordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Voice Records".GetLocalized());
    private static readonly string DefaultRecordFileFormat = $"{"untitled".GetLocalized()}({{DATE}})";

    public string RecordDriver { get; set; } = default!;
    public int RecordSampleRate { get; set; } = DefaultSampleRate;
    public string RecordPath { get; set; } = DefaultRecordPath;
    public string RecordFileFormat { get; set; } = DefaultRecordFileFormat;

    protected override void SetDefault()
    {
        RecordDriver = default!;
        RecordSampleRate = DefaultSampleRate;

        RecordPath = DefaultRecordPath;
        RecordFileFormat = DefaultRecordFileFormat;
    }
}
