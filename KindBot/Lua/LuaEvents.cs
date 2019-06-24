using System;
using KindBot.Objects;
using MoonSharp.Interpreter;

namespace KindBot.Lua
{
    [MoonSharpUserData]
    public class LuaEvents
    {
        public class UserArg : EventArgs
        {
            public User User;
        }
        public class PrivateMessageArgs : EventArgs
        {
            public User User;
            public string Message;
        }
        public class PrivateCommandArgs : EventArgs
        {
            public User User;
            public string Cmd;
        }
        public class ChannelSwitchedArgs : EventArgs
        {
            public User User;
            public int ChannelId;
        }

        public event EventHandler<UserArg> UserJoinedTheServer;
        public event EventHandler<UserArg> UserLeftTheServer;
        public event EventHandler<ChannelSwitchedArgs> UserSwitchedChannel;
        public event EventHandler<PrivateMessageArgs> UserSentPrivateMessage;
        public event EventHandler<PrivateCommandArgs> UserSentCommand;


        public void OnUserJoinedTheServer(User user) => UserJoinedTheServer?.Invoke(this, new UserArg { User = user });
        public void OnUserLeftTheServer(User user) => UserLeftTheServer?.Invoke(this, new UserArg { User = user });
        public void OnUserSentPrivateMessage(User user, string msg) => UserSentPrivateMessage?.Invoke(this, new PrivateMessageArgs { User = user, Message = msg });
        public void OnUserSentComand(User user, string cmd) => UserSentCommand?.Invoke(this, new PrivateCommandArgs { User = user, Cmd = cmd });
        public void OnUserSwitchedChannel(User user, int channelid) => UserSwitchedChannel?.Invoke(this, new ChannelSwitchedArgs { User = user, ChannelId = channelid });
    }
}