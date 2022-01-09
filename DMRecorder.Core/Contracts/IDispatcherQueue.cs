namespace DMRecorder.Core.Contracts
{
    public interface IDispatcherQueue
    {
        void TryEnqueue(Action action);
    }
}
