namespace DRecorder.Core.Contracts
{
    public interface IDispatcherQueue
    {
        void TryEnqueue(Action action);
    }
}
