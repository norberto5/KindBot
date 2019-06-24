using System.Text;
using KindBot.Communication;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Objects
{
    [MoonSharpUserData]
    public class ChannelFactory
    {
        public enum ChannelType
        {
            Temporary,
            SemiPermanent,
            Permanent,
            Default
        }

        public ChannelFactory(string name)
        {
            this.name = name;
        }

        private readonly string name;
        private string topic;
        private string description;
        private string password;
        private Channel.ChannelCodec? codec;
        private int? codecQuality;
        private int? maxClients;
        private int? maxFamilyClients;
        private int? order;
        private int? parentId;
        private ChannelType type = ChannelType.Permanent;
        private bool? hasMaxClientsUnlimited;
        private bool? hasMaxFamilyClientsUnlimited;
#pragma warning disable CS0649
        private bool? hasMaxFamilyClientsInherited;
        private int? neededTalkPower;
        private readonly string phoneticName;
        private int? iconId;
        private bool? isUnencrypted;
#pragma warning restore

        public Channel Create()
        {
            if(name == null) return null;
            var str = new StringBuilder($"channelcreate channel_name={name.ConvertToTeamspeakString()}");

            if(topic != null) str.Append($" channel_topic={topic.ConvertToTeamspeakString()}");
            if(description != null) str.Append($" channel_description={description.ConvertToTeamspeakString()}");
            if(password != null) str.Append($" channel_password={password.ConvertToTeamspeakString()}");
            if(codec != null) str.Append($" channel_codec={(int)codec}");
            if(codecQuality != null) str.Append($" channel_codec_quality={codecQuality}");
            if(maxClients != null) str.Append($" channel_maxclients={maxClients}");
            if(maxFamilyClients != null) str.Append($" channel_maxfamilyclients={maxFamilyClients}");
            if(order != null) str.Append($" channel_order={order}");
            if(parentId != null) str.Append($" cpid={parentId}");
            switch(type)
            {
                case ChannelType.Default:
                    str.Append($" channel_flag_permanent=1 channel_flag_default=1");
                    break;
                case ChannelType.Permanent:
                    str.Append($" channel_flag_permanent=1");
                    break;
                case ChannelType.SemiPermanent:
                    str.Append($" channel_flag_semi_permanent=1");
                    break;
                case ChannelType.Temporary:
                    str.Append($" channel_flag_temporary=1");
                    break;
            }
            if(hasMaxClientsUnlimited != null) str.Append($" channel_flag_maxclients_unlimited={(hasMaxClientsUnlimited.Value ? "1" : "0")}");
            if(hasMaxFamilyClientsUnlimited != null) str.Append($" channel_flag_maxfamilyclients_unlimited={(hasMaxFamilyClientsUnlimited.Value ? "1" : "0")}");
            if(hasMaxFamilyClientsInherited != null) str.Append($" channel_flag_maxfamilyclients_inherited={(hasMaxFamilyClientsInherited.Value ? "1" : "0")}");
            if(neededTalkPower != null) str.Append($" channel_needed_talk_power={neededTalkPower}");
            if(phoneticName != null) str.Append($" channel_name_phonetic={phoneticName}");
            if(iconId != null) str.Append($" channel_icon_id={iconId}");
            if(isUnencrypted != null) str.Append($" channel_codec_is_unencrypted={(isUnencrypted.Value ? "1" : "0")}");

            string output = TelnetConnector.Instance.Execute(str.ToString());
            return TeamspeakTools.TryGetParameter(output, "cid", out int cid) ? new Channel(cid) : null;
        }

        public ChannelFactory Topic(string topic)
        {
            this.topic = topic;
            return this;
        }

        public ChannelFactory Description(string description)
        {
            this.description = description;
            return this;
        }

        public ChannelFactory Password(string password)
        {
            this.password = password;
            return this;
        }

        public ChannelFactory Codec(Channel.ChannelCodec codec)
        {
            this.codec = codec;
            return this;
        }

        public ChannelFactory CodecQuality(int codecQuality)
        {
            if(codecQuality >= 0 || codecQuality <= 10)
                this.codecQuality = codecQuality;
            return this;
        }

        public ChannelFactory MaxClients(int maxClients)
        {
            this.maxClients = maxClients;
            hasMaxClientsUnlimited = true;
            return this;
        }

        public ChannelFactory MaxFamilyClients(int maxFamilyClients)
        {
            this.maxFamilyClients = maxFamilyClients;
            hasMaxFamilyClientsUnlimited = true;
            return this;
        }

        public ChannelFactory Order(int order)
        {
            this.order = order;
            return this;
        }

        public ChannelFactory Parent(int parentId)
        {
            this.parentId = parentId;
            return this;
        }
        public ChannelFactory Parent(Channel parent)
        {
            parentId = parent.Id;
            return this;
        }

        public ChannelFactory Type(ChannelType type)
        {
            this.type = type;
            return this;
        }

    }
}