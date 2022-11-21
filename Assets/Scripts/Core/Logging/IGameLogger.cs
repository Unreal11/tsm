namespace Core.Logging
{
    public interface IGameLogger
    {
        void Log(string message, LogTypeMessage logType);
    }
}