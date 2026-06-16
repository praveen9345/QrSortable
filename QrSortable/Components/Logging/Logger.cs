namespace QrSortable.Components.Logging
{
    using System.Diagnostics;

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            try
            {
                string logLine =
                    "QrSortable: " + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}";

                Console.WriteLine(logLine);
                Debug.WriteLine(logLine);
            }
            catch
            {
                // Never crash because logging failed
            }
        }

        public void LogException(Exception ex)
        {
            Log("EXCEPTION");
            Log(ex.ToString());
        }
    }
}
