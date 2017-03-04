namespace MatthiWare.UpdateLib.Logging
{
    public interface ILogWriter
    {
        LoggingLevel LoggingLevel { get; }

        void Log(string text);
    }
}
