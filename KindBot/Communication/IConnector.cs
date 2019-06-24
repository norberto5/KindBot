using System;

namespace KindBot.Communication
{
    public interface IConnector
    {
        event EventHandler<string> EventHandled;

        bool Connected { get; }

        bool Connect(string hostname, int port);
        bool Login(LoginDetails loginDetails);
        void Stop();

        string Execute(string command, bool attachError = false);
    }
}