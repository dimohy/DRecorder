using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder.Core.Contracts
{
    public interface IDispatcherQueue
    {
        void TryEnqueue(Action action);
    }
}
