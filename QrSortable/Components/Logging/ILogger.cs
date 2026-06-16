namespace QrSortable.Components.Logging
{
    public interface ILogger
    {
        void Log(string message);

        void LogException(Exception ex);
    }
}
