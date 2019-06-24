using System;
using System.Collections.Generic;
using System.Xml;
using KindBot.Common;
using KindBot.Configuration;
using KindBot.Modules.Common;
using KindBot.Tools;

namespace KindBot.Modules
{
    public class GlobalMessagesModule : Module, IConfigurable
    {
        private static int interval = -1;
        private static readonly List<string> messages = new List<string>();

        private double _time = 0;
        private int _index = 0;

        public GlobalMessagesModule() : base()
        {
            ModuleName = "GlobalMessagesModule";
        }

        public void CreateDefaultConfiguration(XmlWriter xmlWriter)
        {
            using(XmlWriter w = xmlWriter)
            {
                w.WriteStartElement(GetConfigurationFilename());

                string[,] globalMessage = {
                {"IsTurnedOn", "false", "Is this module turned on? 'true' or 'false'"}, { "Interval", "180", "How big interval will be between each message. In seconds."} };
                ConfigurationLoader.WriteElementList(w, globalMessage);

                w.WriteStartElement("Messages");

                string[,] globalMessageMessages = {
                {"msg", "[COLOR=#C41111][b]Test message[/b][/COLOR]", "You can give as many messages as you want to use."}, { "msg", "[COLOR=#1931CB][b]Another message[/b][/COLOR]", "You can use all chat bbcodes like [b] or [color]."},
                {"msg", "[COLOR=#C41111][b]Hello again[/b][/COLOR]", "If you want to make a new line in one message use '\\n'"}, { "msg", "[COLOR=#1931CB][b]And so on[/b][/COLOR]", "The messages will be sent in the order from the top to the bottom."} };
                ConfigurationLoader.WriteElementList(w, globalMessageMessages);

                w.WriteEndElement(); // End of Messages group
                w.WriteEndElement(); // End of GlobalMessage group
            }
        }

        public string GetConfigurationFilename() => ModuleName;

        public bool LoadConfiguration(XmlReader xmlReader)
        {
            bool messages = false;
            while(xmlReader.Read())
            {
                if(xmlReader.NodeType == XmlNodeType.Whitespace || xmlReader.NodeType == XmlNodeType.XmlDeclaration)
                    continue;

                if(xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch(xmlReader.Name)
                    {
                        case "IsTurnedOn":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "IsTurnedOn", out bool tmp)) return false;
                            Enabled = tmp;
                            break;
                        case "Interval":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "Interval", out int tmpint)) return false;
                            interval = tmpint;
                            break;
                        case "Messages":
                            messages = true;
                            break;
                        case "msg":
                            if(messages) GlobalMessagesModule.messages.Add(xmlReader.ReadInnerXml());
                            break;
                    }
                }
                else if(xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    if(xmlReader.Name == "Messages") messages = false;
                }
            }
            return true;
        }

        public override void Run()
        {
            if(!Enabled) return;
            if(_time + interval < new TimeSpan(DateTime.Now.Ticks).TotalSeconds)
            {

                if(messages.Count == 0)
                {
                    ConsoleEx.Warning("[GlobalMsg]: Global messages module is enabled, but there is any message in config! Fix the config file!");
                    Enabled = false;
                    return;
                }
                else
                {
                    if(_index >= messages.Count) _index = 0;
                    Commands.SendGlobalMessage(messages[_index]);
                }
                _time = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
            }
        }
    }
}