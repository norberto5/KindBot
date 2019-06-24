using System;
using System.Collections.Generic;
using System.Linq;
using KindBot.Communication;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Objects
{
    /// <summary>
    /// A basic Teamspeak 3 channel class.
    /// </summary>
    [MoonSharpUserData]
    public class Channel : BaseTeamspeak3Object
    {
        public enum ChannelCodec
        {
            SpeexNarrowband = 0, // 0: speex narrowband (mono, 16bit, 8kHz)
            SpeexWideband, // 1: speex wideband (mono, 16bit, 16kHz)
            SpeexUltraWideband, // 2: speex ultra-wideband (mono, 16bit, 32kHz)
            CeltMono // 3: celt mono (mono, 16bit, 48kHz)
        };

        protected override string GetParameterCommand => $"channelinfo cid={Id}";
        protected override string SetParameterCommand => $"channeledit cid={Id}";
        private const string channelPrefix = "channel_";

        /// <summary>
        /// Gets a class from existing channel
        /// </summary>
        /// <param name="id"></param>
        public Channel(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string Name
        {
            get => GetParameter<string>(channelPrefix + "name");
            set => SetParameter(channelPrefix + "name", value);
        }
        public string Topic
        {
            get => GetParameter<string>(channelPrefix + "topic");
            set => SetParameter(channelPrefix + "topic", value);
        }
        public string Description
        {
            get => GetParameter<string>(channelPrefix + "description");
            set => SetParameter(channelPrefix + "description", value);
        }
        public string Password
        {
            set => SetParameter(channelPrefix + "password", value);
        }
        public bool HasPassword => GetParameter<bool>(channelPrefix + "flag_password");
        public int Codec
        {
            get => GetParameter<int>(channelPrefix + "codec");
            set
            {
                if(value < 0 || value > 3) throw new ArgumentOutOfRangeException("Channel Codec cannot be less than zero or higher than 3");
                SetParameter(channelPrefix + "codec", value);
            }
        }
        public int CodecQuality
        {
            get => GetParameter<int>(channelPrefix + "codec_quality");
            set
            {
                if(value < 0 || value > 10) throw new ArgumentOutOfRangeException("Channel Codec Quality cannot be less than zero or higher than 10");
                SetParameter(channelPrefix + "codec_quality", value);
            }
        }
        public int MaxClients
        {
            get => GetParameter<int>(channelPrefix + "maxclients");
            set => SetParameter(channelPrefix + "maxclients", value);
        }
        public int MaxFamilyClients
        {
            get => GetParameter<int>(channelPrefix + "maxfamilyclients");
            set => SetParameter(channelPrefix + "maxfamilyclients", value);
        }
        public int Order
        {
            get => GetParameter<int>(channelPrefix + "order");
            set => SetParameter(channelPrefix + "order", value);
        }
        public bool IsPermanent
        {
            get => GetParameter<bool>(channelPrefix + "flag_permanent");
            set => SetParameter(channelPrefix + "flag_permanent", value);
        }
        public bool IsSemiPermanent
        {
            get => GetParameter<bool>(channelPrefix + "flag_semi_permanent");
            set => SetParameter(channelPrefix + "flag_semi_permanent", value);
        }
        public bool IsTemporary
        {
            get => GetParameter<bool>(channelPrefix + "flag_temporary");
            set => SetParameter(channelPrefix + "flag_temporary", value);
        }
        public bool IsDefault
        {
            get => GetParameter<bool>(channelPrefix + "flag_default");
            set => SetParameter(channelPrefix + "flag_default", value);
        }
        public bool HasMaxClientsUnlimited
        {
            get => GetParameter<bool>(channelPrefix + "flag_maxclients_unlimited");
            set => SetParameter(channelPrefix + "flag_maxclients_unlimited", value);
        }
        public bool HasMaxFamilyClientsUnlimited
        {
            get => GetParameter<bool>(channelPrefix + "flag_maxfamilyclients_unlimited");
            set => SetParameter(channelPrefix + "flag_maxfamilyclients_unlimited", value);
        }
        public bool HasMaxFamilyClientsInherited
        {
            get => GetParameter<bool>(channelPrefix + "flag_maxfamilyclients_inherited");
            set => SetParameter(channelPrefix + "flag_maxfamilyclients_inherited", value);
        }
        public int NeededTalkPower
        {
            get => GetParameter<int>(channelPrefix + "needed_talk_power");
            set => SetParameter(channelPrefix + "needed_talk_power", value);
        }
        public string PhoneticName
        {
            get => GetParameter<string>(channelPrefix + "name_phonetic");
            set => SetParameter(channelPrefix + "name_phonetic", value);
        }
        public string Filepath => GetParameter<string>(channelPrefix + "filepath");
        public bool IsSilenced => GetParameter<bool>(channelPrefix + "forced_silence");
        public int IconId
        {
            get => GetParameter<int>(channelPrefix + "icon_id");
            set => SetParameter(channelPrefix + "icon_id", value);
        }
        public bool IsUnencrypted
        {
            get => GetParameter<bool>(channelPrefix + "codec_is_unencrypted");
            set => SetParameter(channelPrefix + "codec_is_unencrypted", value);
        }
        public int ParentId
        {
            get => GetParameter<int>("pid");
            set => SetParameter("cpid", value);
        }

        public void Delete(bool force = true) => TelnetConnector.Instance.Execute($"channeldelete cid={Id} force={(force ? "1" : "0")}");

        public void Move(int parentId, int order = 0) => TelnetConnector.Instance.Execute($"channelmove cid={Id} cpid={parentId} order={order}");

        public static Channel FromId(int id) => new Channel(id);

        public static List<Channel> GetAllChannels()
        {
            string[] channels = TelnetConnector.Instance.Execute("channellist").Split('|');
            var list = channels.Select(x => new Channel(TeamspeakTools.GetParameter<int>(x, "cid"))).ToList();
            return list;
        }

        public List<Channel> GetChannelChildren()
        {
            string[] channels = TelnetConnector.Instance.Execute("channellist").Split('|');

            var list = new List<Channel>();
            foreach(string c in channels)
            {
                int cid = TeamspeakTools.GetParameter<int>(c, "cid");
                int parent = TeamspeakTools.GetParameter<int>(c, "pid");
                if(parent == Id)
                    list.Add(new Channel(cid));
            }
            return list;
        }

        public List<User> GetUsersWithGroup(int groupId)
        {
            string[] users = TelnetConnector.Instance.Execute($"channelgroupclientlist cgid={groupId} cid={Id}").Split('|');
            var list = new List<User>();
            foreach(string u in users)
            {
                var user = User.FromDatabaseID(TeamspeakTools.GetParameter<int>(u, "cldbid"));
                if(user != null)
                    list.Add(user);
            }
            return list;
        }

        public override string ToString() => $"Channel '{Name}' (ID: {Id}) (Parent: {ParentId})";
    }
}