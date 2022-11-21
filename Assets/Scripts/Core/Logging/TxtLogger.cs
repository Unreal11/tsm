using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Logging
{
    /// <summary>
    /// Logger for txt files unity 
    /// </summary>
    public class TxtLogger : IGameLogger
    {
        private List<LogTypeMessage> _logTypes;
        
        private string _workDirectory;
        private FileWriter _fileWriter;

        public TxtLogger(List<LogTypeMessage> logTypes)
        {
            _logTypes = logTypes;
        }
        
        private void Awake()
        {
            _workDirectory = $"{Environment.CurrentDirectory}/Logs";
            if (!Directory.Exists(_workDirectory))
            {
                Directory.CreateDirectory(_workDirectory);
            }

            _fileWriter = new FileWriter(_workDirectory);
        }
        
        public void Log(string message, LogTypeMessage logType)
        {
            _logTypes.ForEach(l =>
            {
                if (logType == l)
                {
                    _fileWriter.Write(message);
                }
            });
        }
    }
}
