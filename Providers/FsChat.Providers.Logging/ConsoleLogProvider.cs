using FsChat.Interfaces;
using FsChat.Interfaces.Logging;
using System;

namespace FsChat.Providers.Logging
{
    public class ConsoleLogProvider : ILogProvider
    {
        public void Trace(string methodName, string message)
        {
            Console.WriteLine(message);
        }

        public void Trace(Exception ex, string methodName, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"{message}. Exception Message: {ex.Message}");
            }
            else
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
