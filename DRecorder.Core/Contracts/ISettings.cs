namespace DRecorder.Core.Contracts
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
