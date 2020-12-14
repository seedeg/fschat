using System;

namespace FsChat.Interfaces.Logging
{
    public interface ILogProvider
    {
        void Trace(string methodName, string message);

        void Trace(Exception ex, string methodName, string message = null);
    }
}
