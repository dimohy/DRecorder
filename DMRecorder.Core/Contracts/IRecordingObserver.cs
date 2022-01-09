using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder.Core.Contracts
{
    public interface IRecordingObserver
    {
        void SendState(RecordState beforeState, RecordState state);

        void SendValue(float value);
    }
}
