using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder.Core
{
    public enum RecordState
    {
        /// <summary>
        /// 녹음 중지 상태
        /// </summary>
        Stop,
        /// <summary>
        /// 녹음 중
        /// </summary>
        Record,
        /// <summary>
        /// 녹음 일시 정지
        /// </summary>
        RecordPause,
        /// <summary>
        /// 재생 중
        /// </summary>
        Play,
        /// <summary>
        /// 재생 일시 정지
        /// </summary>
        PlayPause
    }
}
