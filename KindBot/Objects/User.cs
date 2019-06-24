using System;
using System.Collections.Generic;
using System.Linq;
using KindBot.Communication;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Objects
{
    /// <summary>
    /// A basic Teamspeak 3 user class.
    /// </summary>
    [MoonSharpUserData]
    public class User : BaseTeamspeak3Object
    {
        protected override string GetParameterCommand => $"clientinfo clid={Id}";
        protected override string SetParameterCommand => $"clientedit clid={Id}";
        private const string clientPrefix = "client_";

        public static List<User> AllUsers
        {
            get
            {
                if(allUsers == null)
                {
                    var users = new List<User>();
                    string output = TelnetConnector.Instance.Execute("clientlist");
                    string[] arr = output.Trim().Split('|');
                    foreach(string s in arr)
                    {
                        users.Add(new User(s));
                    }
                    allUsers = users;
                }
                return allUsers;
            }
        }
        private static List<User> allUsers;

        public static User Bot { get; private set; }

        private string lastSeenNickname;

        public int Id { get; }
        public int DatabaseId { get; }
        public ConnectionTypeEnum ConnectionType { get; }
        public string Nickname
        {
            get
            {
                string getNickname = GetParameter<string>(clientPrefix + "nickname");
                if(!string.IsNullOrEmpty(getNickname))
                {
                    lastSeenNickname = getNickname;
                }
                return lastSeenNickname;
            }
            set => SetParameter(clientPrefix + "nickname", value);
        }

        public string UniqueId => GetStaticParameter<string>(clientPrefix + "unique_identifier");
        public string Version => GetStaticParameter<string>(clientPrefix + "version");
        public string Platform => GetStaticParameter<string>(clientPrefix + "platform");
        public DateTime Created => TeamspeakTools.UnixTimeStampToDateTime(GetStaticParameter<int>(clientPrefix + "created"));
        public DateTime LastConnected => TeamspeakTools.UnixTimeStampToDateTime(GetStaticParameter<int>(clientPrefix + "lastconnected"));
        public int TotalConnections => GetStaticParameter<int>(clientPrefix + "totalconnections");

        public int ChannelId => GetParameter<int>("cid");
        public DateTime ConnectedTime => TeamspeakTools.UnixTimeStampToDateTime(GetParameter<int>("connection_connected_time"));
        public bool InputMuted => GetParameter<bool>(clientPrefix + "input_muted");
        public bool OutputMuted => GetParameter<bool>(clientPrefix + "output_muted");
        public bool OutputOnlyMuted => GetParameter<bool>(clientPrefix + "outputonly_muted");
        public bool IsRecording => GetParameter<bool>(clientPrefix + "is_recording");
        public bool InputHardware => GetParameter<bool>(clientPrefix + "input_hardware");
        public bool OutputHardware => GetParameter<bool>(clientPrefix + "output_hardware");
        public string DefaultChannel => GetParameter<string>(clientPrefix + "default_channel");
        public string LoginName => GetParameter<string>(clientPrefix + "login_name");
        public int ChannelGroupId => GetParameter<int>(clientPrefix + "channel_group_id");
        public bool IsAway => GetParameter<bool>(clientPrefix + "away");
        public string AwayMessage => GetParameter<string>(clientPrefix + "away_message");
        public string Avatar => GetParameter<string>(clientPrefix + "flag_avatar");
        public int TalkPower => GetParameter<int>(clientPrefix + "talk_power");
        public bool TalkRequested => GetParameter<bool>(clientPrefix + "talk_request");
        public string TalkRequstMessage => GetParameter<string>(clientPrefix + "talk_request_msg");
        public bool IsTalker
        {
            get => GetParameter<bool>(clientPrefix + "is_talker");
            set => SetParameter(clientPrefix + "is_talker", value);
        }
        public long MonthBytesDownloaded => GetParameter<long>(clientPrefix + "month_bytes_downloaded");
        public long MonthBytesUploaded => GetParameter<long>(clientPrefix + "month_bytes_uploaded");
        public long TotalBytesDownloaded => GetParameter<long>(clientPrefix + "total_bytes_downloaded");
        public long TotalBytesUploaded => GetParameter<long>(clientPrefix + "total_bytes_uploaded");
        public bool IsPrioritySpeaker => GetParameter<bool>(clientPrefix + "is_priority_speaker");
        public int UnreadMessages => GetParameter<int>(clientPrefix + "unread_messages");
        public string PhoneticName => GetParameter<string>(clientPrefix + "nickname_phonetic");
        public string Description
        {
            get => GetParameter<string>(clientPrefix + "description");
            set => SetParameter(clientPrefix + "description", value);
        }
        public int NeededServerQueryViewPower => GetParameter<int>(clientPrefix + "needed_serverquery_view_power");

        public string Ip => GetParameter<string>("connection_client_ip");
        public bool IsChannelCommander => GetParameter<bool>(clientPrefix + "is_channel_commander");
        public long IconId
        {
            get => GetParameter<long>(clientPrefix + "icon_id");
            set => SetParameter(clientPrefix + "icon_id", value);
        }
        public string Country => GetParameter<string>(clientPrefix + "country");
        public TimeSpan IdleTime => TimeSpan.FromMilliseconds(GetParameter<int>(clientPrefix + "idle_time"));

        private User(string info)
        {
            Dictionary<string, string> parameters = TeamspeakTools.GetParameters(info);

            parameters.TryGetValue("clid", out string tmp);
            if(!int.TryParse(tmp, out int tmpint)) return;
            Id = tmpint;

            parameters.TryGetValue("client_database_id", out tmp);
            if(!int.TryParse(tmp, out tmpint)) return;
            DatabaseId = tmpint;

            parameters.TryGetValue("client_type", out tmp);
            if(!int.TryParse(tmp, out tmpint)) return;
            ConnectionType = (tmpint == 1) ? ConnectionTypeEnum.Query : ConnectionTypeEnum.Normal;
        }

        public static User FromId(int id) => AllUsers.FirstOrDefault(u => u.Id == id);

        public static User FromDatabaseID(int dbid) => AllUsers.FirstOrDefault(u => u.DatabaseId == dbid);

        public static void SetClientChannelGroup(User user, int groupId, Channel channel)
        {
            if(user == null || channel == null) return;
            TelnetConnector.Instance.Execute($"setclientchannelgroup cgid={groupId} cid={channel.Id} cldbid={user.DatabaseId}", true);
        }
        public static void SetClientChannelGroup(User user, int groupId) => SetClientChannelGroup(user, groupId, new Channel(user.ChannelId));
        public void SetClientChannelGroup(int groupId, Channel channel) => SetClientChannelGroup(this, groupId, channel);
        public void SetClientChannelGroup(int groupId) => SetClientChannelGroup(this, groupId, new Channel(ChannelId));

        public static void Move(User[] users, Channel channel)
        {
            if(users == null || channel == null || users.Length == 0) return;
            string str = "";
            foreach(User u in users)
                str += $"clid={u.Id}|";
            str = str.Remove(str.Length - 1);
            TelnetConnector.Instance.Execute($"clientmove {str} cid={channel.Id}");
        }
        public static void Move(User user, Channel channel)
        {
            if(user == null || channel == null) return;
            TelnetConnector.Instance.Execute($"clientmove clid={user.Id} cid={channel.Id}");
        }
        public void Move(Channel channel) => Move(this, channel);

        public static void Poke(User user, string message)
        {
            if(user == null) return;
            TelnetConnector.Instance.Execute($"clientpoke clid={user.Id} msg={message.ConvertToTeamspeakString()}");
        }
        public void Poke(string message) => Poke(this, message);

        public static void SendPrivateMessage(User user, string message)
        {
            if(user == null) return;
            TelnetConnector.Instance.Execute($"sendtextmessage targetmode=1 target={user.Id} msg={message.ConvertToTeamspeakString()}");
        }
        public void SendPrivateMessage(string message) => SendPrivateMessage(this, message);

        public static void KickFromServer(User user, string reason, int systemid = -1)
        {
            if(user == null) return;
            TelnetConnector.Instance.Execute($"clientkick reasonid=5 reasonmsg={reason.ConvertToTeamspeakString()} clid={user.Id}");
            string consolemsg = "";

            switch(systemid)
            {
                case 1:
                    consolemsg = "[Nick Blacklist]: ";
                    break;
                case 418:
                    return;
                default:
                    consolemsg = "[SYSTEM]: ";
                    break;
            }
            consolemsg += $"User '{user.Nickname}' (id: {user.Id} dbid: {user.DatabaseId} ) was kicked from the server!";
            ConsoleEx.WriteLine(consolemsg);
        }
        public void KickFromServer(string reason, int systemid = -1) => KickFromServer(this, reason, systemid);

        public static void KickFromChannel(User user, string reason)
        {
            if(user == null) return;

            var userChannel = Channel.FromId(user.ChannelId);

            TelnetConnector.Instance.Execute($"clientkick reasonid=4 reasonmsg={reason.ConvertToTeamspeakString()} clid={user.Id}");

            ConsoleEx.WriteLine($"[SYSTEM]: User '{user.Nickname}' (id: {user.Id} dbid: {user.DatabaseId} ) was kicked from channel '{userChannel.Name}'!");
        }
        public void KickFromChannel(string reason) => KickFromChannel(this, reason);

        public static void Ban(User user, int timeInSeconds = 0, string reason = "")
        {
            if(user == null) return;
            string cmd = $"banclient clid={user.Id}";
            if(timeInSeconds > 0) cmd = $"{cmd} time={timeInSeconds}";
            if(reason.Length > 0) cmd = $"{cmd} banreason={reason.ConvertToTeamspeakString()}";
            string output = TelnetConnector.Instance.Execute(cmd, true);

            TeamspeakTools.TryGetParameter(output, "id", out int errorId);

            string msg = $"User '{user.Nickname}' (id: {user.Id} dbid: {user.DatabaseId} ) was banned from the server{(timeInSeconds > 0 ? $" for {timeInSeconds} seconds" : "")}{(reason.Length > 0 ? $" (reason: '{reason}')" : "")}!";

            if(errorId == 0) ConsoleEx.WriteLine(msg);
            else ConsoleEx.Warning($"An error occured when trying to ban {user.Nickname} (Error ID: {errorId})");
        }
        public void Ban(int timeInSeconds, string reason = "") => Ban(this, timeInSeconds, reason);

        public static void AddServerGroup(User user, int groupId)
        {
            if(user == null) return;
            TelnetConnector.Instance.Execute($"servergroupaddclient sgid={groupId} cldbid={user.DatabaseId}");
        }
        public void AddServerGroup(int groupId) => AddServerGroup(this, groupId);

        public static void RemoveServerGroup(User user, int groupId)
        {
            if(user == null) return;
            TelnetConnector.Instance.Execute($"servergroupdelclient sgid={groupId} cldbid={user.DatabaseId}");
        }
        public void RemoveServerGroup(int groupId) => RemoveServerGroup(this, groupId);

        public static List<int> GetUserServerGroups(User user) => user != null ? user.GetServerGroups() : new List<int>();
        public List<int> GetServerGroups() => TeamspeakTools.GetListOfGroups(GetParameter<string>("client_servergroups")).ToList();

        public static bool IsUserAnAdmin(User user) => user != null ? user.IsAnAdmin() : false;
        public bool IsAnAdmin() => TeamspeakTools.GetListOfGroups(GetParameter<string>("client_servergroups")).Contains(2);

        public static void InvalidateAllUsers() => allUsers = null;
        public static void MarkUserAsBot(User botUser) => Bot = botUser;

        public override string ToString() => $"User '{Nickname}' (ID: {Id})";

        private Dictionary<string, string> staticParameters;

        private T GetStaticParameter<T>(string parameter)
        {
            if(staticParameters == null)
            {
                string info = TelnetConnector.Instance.Execute($"clientinfo clid={Id}");
                staticParameters = TeamspeakTools.GetParameters(info);
            }
            staticParameters.TryGetValue(parameter, out string value);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public enum ConnectionTypeEnum
        {
            Normal = 0,
            Query = 1
        }
    }
}