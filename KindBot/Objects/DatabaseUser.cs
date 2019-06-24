using System;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Objects
{
    /// <summary>
    /// A basic Teamspeak 3 user class.
    /// </summary>
    [MoonSharpUserData]
    public class DatabaseUser : BaseTeamspeak3Object
    {
        protected override string GetParameterCommand => $"clientdbinfo cldbid={DatabaseId}";
        protected override string SetParameterCommand => $"clientdbedit cldbid={DatabaseId}";
        private const string clientPrefix = "client_";

        public int DatabaseId { get; private set; }

        public string UniqueId => GetParameter<string>(clientPrefix + "unique_identifier");
        public string Nickname => GetParameter<string>(clientPrefix + "nickname");
        public DateTime Created => TeamspeakTools.UnixTimeStampToDateTime(GetParameter<int>(clientPrefix + "created"));
        public DateTime LastConnected => TeamspeakTools.UnixTimeStampToDateTime(GetParameter<int>(clientPrefix + "lastconnected"));
        public int TotalConnections => GetParameter<int>(clientPrefix + "totalconnections");
        public string Description
        {
            get => GetParameter<string>(clientPrefix + "description");
            set => SetParameter(clientPrefix + "description", value);
        }
        public long MonthBytesDownloaded => GetParameter<long>(clientPrefix + "month_bytes_downloaded");
        public long MonthBytesUploaded => GetParameter<long>(clientPrefix + "month_bytes_uploaded");
        public long TotalBytesDownloaded => GetParameter<long>(clientPrefix + "total_bytes_downloaded");
        public long TotalBytesUploaded => GetParameter<long>(clientPrefix + "total_bytes_uploaded");
        public string LastIp => GetParameter<string>(clientPrefix + "lastip");

    }
}