using System.Xml;
using KindBot.Tools;

namespace KindBot.Configuration
{
    public class BotConfiguration : IConfigurable
    {
        public string IP { get; private set; } = "";
        public int Port { get; private set; } = -1;
        public int VirtualServerID { get; private set; } = -1;
        public int BotChannelID { get; private set; } = -1;
        public string QueryLogin { get; private set; } = "";
        public string QueryPassword { get; private set; } = "";
        public string BotNickname { get; private set; } = "";
        public string SecondBotNickname { get; private set; } = "";

        public void CreateDefaultConfiguration(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(GetConfigurationFilename());
            string[,] basicConfig = {
                {"IP", "127.0.0.1", "Your teamspeak server's address. You can use domain name or ip address (Default: 127.0.0.1)"}, { "Port", "10011", "Your teamspeak server's QUERY port. Not your server's port (Default: 10011)"} ,
                {"VirtualServerID", "1" , "Your virtual server ID. If you run only one server on the machine and it's your first server it will be '1'. (default: 1)"}, {"BotChannelID", "1" , "ID of the channel the bot will join to. This is not too important."},
                {"Query_Login", "qwerty" , "You need to change it to the query login you typed when getting query access."}, {"Query_Password", "qwerty" , "You need to change it to the query password you've got from making a query access."},
                {"Bot_Nickname", "[BOT] nickname" , "The displayed nickname your bot will use."},
                {"Bot_Nickname2", "[BOT] nickname2", "This nickname will be used only when first nickname is currently used by someone else." }
                };
            ConfigurationLoader.WriteElementList(xmlWriter, basicConfig);

            xmlWriter.WriteEndElement();
        }

        public string GetConfigurationFilename() => "BotConfiguration";

        public bool LoadConfiguration(XmlReader xmlReader)
        {
            while(xmlReader.Read())
            {
                if(xmlReader.NodeType == XmlNodeType.Whitespace || xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                    continue;

                if(xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch(xmlReader.Name)
                    {
                        case "IP":
                            IP = xmlReader.ReadInnerXml();
                            break;
                        case "Port":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "Port", out int temp)) return false;
                            if(temp < 1 || temp > 65535)
                            {
                                ConsoleEx.Error("[Configuration]: Invalid port. Must be in range of <1;65535>");
                                return false;
                            }
                            Port = temp;
                            break;
                        case "VirtualServerID":
                            temp = -1;
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "VirtualServerID", out temp)) return false;
                            VirtualServerID = temp;
                            break;
                        case "BotChannelID":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "BotChannelID", out temp)) return false;
                            BotChannelID = temp;
                            break;
                        case "Query_Login":
                            QueryLogin = xmlReader.ReadInnerXml();
                            break;
                        case "Query_Password":
                            QueryPassword = xmlReader.ReadInnerXml();
                            break;
                        case "Bot_Nickname":
                            BotNickname = xmlReader.ReadInnerXml();
                            break;
                        case "Bot_Nickname2":
                            SecondBotNickname = xmlReader.ReadInnerXml();
                            break;
                    }
                }
            }
            return true;
        }
    }
}