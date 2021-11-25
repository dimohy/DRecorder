using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder.Core.Contracts
{
    public interface ISettings
    {
        string RecordDriver { get; set; }
        int RecordSampleRate { get; set; }

        string RecordFileFormat { get; set; }
        string RecordPath { get; set; }

        void Save();
    }
}
