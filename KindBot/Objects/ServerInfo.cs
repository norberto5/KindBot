using System;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Objects
{
    [MoonSharpUserData]
    public class ServerInfo : BaseTeamspeak3Object
    {
        public static ServerInfo Instance => instance ?? (instance = new ServerInfo());
        private static ServerInfo instance;

        protected override string GetParameterCommand => "serverinfo";
        protected override string SetParameterCommand => "serveredit";
        private const string virtualServerPrefix = "virtualserver_";

        public enum HostMessageModeEnum
        {
            None = 0,
            Log = 1, // 1: display message in chatlog
            Modal, // 2: display message in modal dialog
            Modalquit // 3: display message in modal dialog and close connection
        };
        public enum HostBannerModeEnum
        {
            NoAdjust = 0, // 0: do not adjust
            IgnoreAspect, // 1: adjust but ignore aspect ratio (like TeamSpeak 2)
            KeepAspect // 2: adjust and keep aspect ratio
        };
        public enum CodecEncryptionModeEnum
        {
            Individual = 0, // 0: configure per channel
            GloballyDisabled, // 1: globally disabled
            GloballyEnabled // 2: globally enabled
        };


        public string Name
        {
            get => GetParameter<string>(virtualServerPrefix + "name");
            set => SetParameter(virtualServerPrefix + "name", value);
        }
        public string WelcomeMessage
        {
            get => GetParameter<string>(virtualServerPrefix + "welcomemessage");
            set => SetParameter(virtualServerPrefix + "welcomemessage", value);
        }
        public int MaxClients
        {
            get => GetParameter<int>(virtualServerPrefix + "maxclients");
            set => SetParameter(virtualServerPrefix + "maxclients", value);
        }
        public string Password
        {
            set => SetParameter(virtualServerPrefix + "password", value);
        }
        public bool HasPassword => GetParameter<bool>(virtualServerPrefix + "flag_password");
        public int ClientsOnline => GetParameter<int>(virtualServerPrefix + "clientsonline");
        public int QueryClientsOnline => GetParameter<int>(virtualServerPrefix + "queryclientsonline");
        public int ChannelsOnline => GetParameter<int>(virtualServerPrefix + "channelsonline");
        public DateTime Created => TeamspeakTools.UnixTimeStampToDateTime(GetParameter<int>(virtualServerPrefix + "created"));
        public long Uptime => GetParameter<long>(virtualServerPrefix + "uptime");
        public string HostMessage
        {
            get => GetParameter<string>(virtualServerPrefix + "hostmessage");
            set => SetParameter(virtualServerPrefix + "hostmessage", value);
        }
        public int HostMessageMode
        {
            get => GetParameter<int>(virtualServerPrefix + "hostmessage_mode");
            set
            {
                if(value < 0 || value > 3) throw new ArgumentOutOfRangeException("HostMessageMode cannot be less than zero or higher than 3");
                SetParameter(virtualServerPrefix + "hostmessage_mode", value);
            }
        }
        public int DefaultServerGroup
        {
            get => GetParameter<int>(virtualServerPrefix + "default_server_group");
            set => SetParameter(virtualServerPrefix + "default_server_group", value);
        }
        public int DefaultChannelGroup
        {
            get => GetParameter<int>(virtualServerPrefix + "default_channel_group");
            set => SetParameter(virtualServerPrefix + "default_channel_group", value);
        }
        public int DefaultChannelAdminGroup
        {
            get => GetParameter<int>(virtualServerPrefix + "default_channel_admin_group");
            set => SetParameter(virtualServerPrefix + "default_channel_admin_group", value);
        }
        public string Platform => GetParameter<string>(virtualServerPrefix + "platform");
        public string Version => GetParameter<string>(virtualServerPrefix + "version");
        public long MaxDownloadTotalBandwidth
        {
            get => GetParameter<long>(virtualServerPrefix + "max_download_total_bandwidth");
            set => SetParameter(virtualServerPrefix + "max_download_total_bandwidth", value);
        }
        public long MaxUploadTotalBandwidth
        {
            get => GetParameter<long>(virtualServerPrefix + "max_upload_total_bandwidth");
            set => SetParameter(virtualServerPrefix + "max_upload_total_bandwidth", value);
        }
        public string HostbannerUrl
        {
            get => GetParameter<string>(virtualServerPrefix + "hostbanner_url");
            set => SetParameter(virtualServerPrefix + "hostbanner_url", value);
        }
        public string HostbannerGfxUrl
        {
            get => GetParameter<string>(virtualServerPrefix + "hostbanner_gfx_url");
            set => SetParameter(virtualServerPrefix + "hostbanner_gfx_url", value);
        }
        public int HostbannerGfxInterval
        {
            get => GetParameter<int>(virtualServerPrefix + "hostbanner_gfx_interval");
            set => SetParameter(virtualServerPrefix + "hostbanner_gfx_interval", value);
        }
        public int ComplainAutobanCount
        {
            get => GetParameter<int>(virtualServerPrefix + "complain_autoban_count");
            set => SetParameter(virtualServerPrefix + "complain_autoban_count", value);
        }
        public int ComplainAutobanTime
        {
            get => GetParameter<int>(virtualServerPrefix + "complain_autoban_time");
            set => SetParameter(virtualServerPrefix + "complain_autoban_time", value);
        }
        public int ComplainRemoveTime
        {
            get => GetParameter<int>(virtualServerPrefix + "complain_remove_time");
            set => SetParameter(virtualServerPrefix + "complain_remove_time", value);
        }
        public int MinClientsInChannelBeforeForcedSilence
        {
            get => GetParameter<int>(virtualServerPrefix + "min_clients_in_channel_before_forced_silence");
            set => SetParameter(virtualServerPrefix + "min_clients_in_channel_before_forced_silence", value);
        }
        public float PrioritySpeakerDimmModificator
        {
            get => GetParameter<float>(virtualServerPrefix + "priority_speaker_dimm_modificator");
            set => SetParameter(virtualServerPrefix + "priority_speaker_dimm_modificator", value);
        }
        public int AntifloodPointsTickReduce
        {
            get => GetParameter<int>(virtualServerPrefix + "antiflood_points_tick_reduce");
            set => SetParameter(virtualServerPrefix + "antiflood_points_tick_reduce", value);
        }
        public int AntifloodPointsNeededCommandBlock
        {
            get => GetParameter<int>(virtualServerPrefix + "antiflood_points_needed_command_block");
            set => SetParameter(virtualServerPrefix + "antiflood_points_needed_command_block", value);
        }
        public int AntifloodPointsNeededIpBlock
        {
            get => GetParameter<int>(virtualServerPrefix + "antiflood_points_needed_ip_block");
            set => SetParameter(virtualServerPrefix + "antiflood_points_needed_ip_block", value);
        }
        public int HostBannerMode
        {
            get => GetParameter<int>(virtualServerPrefix + "hostbanner_mode");
            set
            {
                if(value < 0 || value > 2) throw new ArgumentOutOfRangeException("HostBannerMode cannot be less than zero or higher than 2");
                SetParameter(virtualServerPrefix + "hostbanner_mode", value);
            }
        }
        public bool AskForPrivilegeKey => GetParameter<bool>(virtualServerPrefix + "ask_for_privilegekey");
        public int ClientConnections => GetParameter<int>(virtualServerPrefix + "client_connections");
        public int QueryClientConnections => GetParameter<int>(virtualServerPrefix + "query_client_connections");
        public string HostButtonTooltip
        {
            get => GetParameter<string>(virtualServerPrefix + "hostbutton_tooltip");
            set => SetParameter(virtualServerPrefix + "hostbutton_tooltip", value);
        }
        public string HostButtonGfxUrl
        {
            get => GetParameter<string>(virtualServerPrefix + "hostbutton_gfx_url");
            set => SetParameter(virtualServerPrefix + "hostbutton_gfx_url", value);
        }
        public string HostButtonUrl
        {
            get => GetParameter<string>(virtualServerPrefix + "hostbutton_url");
            set => SetParameter(virtualServerPrefix + "hostbutton_url", value);
        }
        public long DownloadQuota
        {
            get => GetParameter<long>(virtualServerPrefix + "download_quota");
            set => SetParameter(virtualServerPrefix + "download_quota", value);
        }
        public long UploadQuota
        {
            get => GetParameter<long>(virtualServerPrefix + "upload_quota");
            set => SetParameter(virtualServerPrefix + "upload_quota", value);
        }
        public long MonthBytesDownloaded => GetParameter<long>(virtualServerPrefix + "month_bytes_downloaded");
        public long MonthBytesUploaded => GetParameter<long>(virtualServerPrefix + "month_bytes_uploaded");
        public long TotalBytesDownloaded => GetParameter<int>(virtualServerPrefix + "total_bytes_downloaded");
        public long TotalBytesUploaded => GetParameter<long>(virtualServerPrefix + "total_bytes_uploaded");
        public string UniqueIdentifier => GetParameter<string>(virtualServerPrefix + "unique_identifier");
        public int Id => GetParameter<int>(virtualServerPrefix + "id");
        public int MachineId
        {
            get => GetParameter<int>(virtualServerPrefix + "machine_id");
            set => SetParameter(virtualServerPrefix + "machine_id", value);
        }
        public int Port
        {
            get => GetParameter<int>(virtualServerPrefix + "port");
            set => SetParameter(virtualServerPrefix + "port", value);
        }
        public bool Autostart
        {
            get => GetParameter<bool>(virtualServerPrefix + "autostart");
            set => SetParameter(virtualServerPrefix + "autostart", value);
        }

        public long ConnectionFiletransferBandwidthSent => GetParameter<long>("connection_filetransfer_bandwidth_sent");
        public long ConnectionFiletransferBandwidthReceived => GetParameter<long>("connection_filetransfer_bandwidth_received");
        public long ConnectionPacketsSentTotal => GetParameter<long>("connection_packets_sent_total");
        public long ConnectionPacketsReceivedTotal => GetParameter<long>("connection_packets_received_total");
        public long ConnectionBytesSentTotal => GetParameter<long>("connection_bytes_sent_total");
        public long ConnectionBytesReceivedTotal => GetParameter<long>("connection_bytes_received_total");
        public long ConnectionBandwidthSentLastSecondTotal => GetParameter<long>("connection_bandwidth_sent_last_second_total");
        public long ConnectionBandwidthReceivedLastSecondTotal => GetParameter<long>("connection_bandwidth_received_last_second_total");
        public long ConnectionBandwidthSentLastMinuteTotal => GetParameter<long>("connection_bandwidth_sent_last_minute_total");
        public long ConnectionBandwidthReceivedLastMinuteTotal => GetParameter<long>("connection_bandwidth_received_last_minute_total");

        public string Status
        {
            get => GetParameter<string>(virtualServerPrefix + "status");
            set => SetParameter(virtualServerPrefix + "status", value);
        }
        public bool LogClient
        {
            get => GetParameter<bool>(virtualServerPrefix + "log_client");
            set => SetParameter(virtualServerPrefix + "log_client", value);
        }
        public bool LogQuery
        {
            get => GetParameter<bool>(virtualServerPrefix + "log_query");
            set => SetParameter(virtualServerPrefix + "log_query", value);
        }
        public bool LogChannel
        {
            get => GetParameter<bool>(virtualServerPrefix + "log_channel");
            set => SetParameter(virtualServerPrefix + "log_channel", value);
        }
        public bool LogPermissions
        {
            get => GetParameter<bool>(virtualServerPrefix + "log_permissions");
            set => SetParameter(virtualServerPrefix + "log_permissions", value);
        }
        public bool LogServer
        {
            get => GetParameter<bool>(virtualServerPrefix + "log_server");
            set => SetParameter(virtualServerPrefix + "log_server", value);
        }
        public bool LogFiletransfer
        {
            get => GetParameter<bool>(virtualServerPrefix + "log_filetransfer");
            set => SetParameter(virtualServerPrefix + "log_filetransfer", value);
        }
        public long MinClientVersion
        {
            get => GetParameter<long>(virtualServerPrefix + "min_client_version");
            set => SetParameter(virtualServerPrefix + "min_client_version", value);
        }
        public int NeededIdentitySecurityLevel
        {
            get => GetParameter<int>(virtualServerPrefix + "needed_identity_security_level");
            set => SetParameter(virtualServerPrefix + "needed_identity_security_level", value);
        }
        public string PhoneticName
        {
            get => GetParameter<string>(virtualServerPrefix + "name_phonetic");
            set => SetParameter(virtualServerPrefix + "name_phonetic", value);
        }
        public int IconId
        {
            get => GetParameter<int>(virtualServerPrefix + "icon_id");
            set => SetParameter(virtualServerPrefix + "icon_id", value);
        }
        public int ReservedSlots
        {
            get => GetParameter<int>(virtualServerPrefix + "reserved_slots");
            set => SetParameter(virtualServerPrefix + "reserved_slots", value);
        }
        public float TotalPacketlossSpeech => GetParameter<float>(virtualServerPrefix + "total_packetloss_speech");
        public float TotalPacketlossKeepAlive => GetParameter<float>(virtualServerPrefix + "total_packetloss_keepalive");
        public float TotalPacketlossControl => GetParameter<float>(virtualServerPrefix + "total_packetloss_control");
        public float TotalPacketlossTotal => GetParameter<float>(virtualServerPrefix + "total_packetloss_total");
        public float TotalPing => GetParameter<float>(virtualServerPrefix + "total_ping");
        public string Ip => GetParameter<string>(virtualServerPrefix + "ip");
        public bool WeblistEnabled
        {
            get => GetParameter<bool>(virtualServerPrefix + "weblist_enabled");
            set => SetParameter(virtualServerPrefix + "weblist_enabled", value);
        }
        public int CodecEncryptionMode
        {
            get => GetParameter<int>(virtualServerPrefix + "codec_encryption_mode");
            set
            {
                if(value < 0 || value > 2) throw new ArgumentOutOfRangeException("CodecEncryptionMode cannot be less than zero or higher than 2");
                SetParameter(virtualServerPrefix + "codec_encryption_mode", value);
            }
        }
        public string Filebase => GetParameter<string>(virtualServerPrefix + "filebase");
    }
}