using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Logging
{
    /// <summary>
    /// Logger for console unity 
    /// </summary>
    public class ConsoleLogger : IGameLogger
    {
        private List<LogTypeMessage> _logTypes;

        public ConsoleLogger(List<LogTypeMessage> logTypes)
        {
            _logTypes = logTypes;
        }

        public void Log(string message, LogTypeMessage logType)
        {
            _logTypes.ForEach(l => 
            {
                if (logType == l)
                {
                    Debug.Log($"[{DateTime.Now}][{logType}]: {message}");

                    return;
                }
            });
        }
    }
}