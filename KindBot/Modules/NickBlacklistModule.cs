using System;
using System.Collections.Generic;
using System.Xml;
using KindBot.Objects;
using KindBot.Configuration;
using KindBot.Modules.Common;

namespace KindBot.Modules
{
    public class NickBlacklistFilter
    {
        public int Type = -1;
        //public string[] parameters;
        public string Parameter = "";
        /*
        public NickBlacklistFilter(int type, string parameter)
        {
            this.type = type;
            this.parameter = parameter;
        }
        */
    }
    public class NickBlacklistMember
    {
        public string Nick = "";
        //public NickBlacklistFilter[] filters;
        //public NickBlacklistFilter filter;
        public int FilterType = -1;
        public string FilterStr = "";
    }

    public class NickBlacklistModule : Module, IConfigurable
    {
        private int interval = -1;
        private string message = "";
        private readonly List<NickBlacklistMember> list = new List<NickBlacklistMember>();
        private double _time = 0;

        public NickBlacklistModule() : base()
        {
            ModuleName = "NickBlacklistModule";
        }

        public void CreateDefaultConfiguration(XmlWriter xmlWriter)
        {
            using(XmlWriter w = xmlWriter)
            {
                w.WriteStartElement(GetConfigurationFilename());

                string[,] nickBlacklist = {
                {"IsTurnedOn", "false", "Is this module turned on? 'true' or 'false'"}, { "CheckInterval", "2", "How long interval between each checkout. In seconds."} ,
                {"ReasonMessage", "Your nickname is blacklisted! Please change it!" , "The message will be used as a reason to the kick."}
                };
                ConfigurationLoader.WriteElementList(w, nickBlacklist);

                w.WriteStartElement("NickBan");

                w.WriteComment("What part of the name is banned from use?");
                w.WriteStartElement("nickname");
                w.WriteString("part of the name");
                w.WriteEndElement();

                w.WriteComment("Who isn't checked for this rule?");
                w.WriteStartElement("filter");
                w.WriteAttributeString("type", "dbid");
                w.WriteString("666");
                w.WriteEndElement();

                w.WriteEndElement();

                w.WriteEndElement();
            }
        }

        public string GetConfigurationFilename() => ModuleName;

        public bool LoadConfiguration(XmlReader xmlReader)
        {
            bool nickban = false;
            var nbNew = new NickBlacklistMember();
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
                        case "CheckInterval":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "CheckInterval", out int tmpint)) return false;
                            interval = tmpint;
                            break;
                        case "ReasonMessage":
                            message = xmlReader.ReadInnerXml();
                            break;
                        case "NickBan":
                            nickban = true;
                            break;
                        case "nickname":
                            if(nickban) nbNew.Nick = xmlReader.ReadInnerXml();
                            break;
                        case "filter":
                            if(nickban)
                            {
                                int filter = -1;
                                switch(xmlReader.GetAttribute("type"))
                                {
                                    case "dbid":
                                        filter = 1;
                                        break;
                                }
                                string str = xmlReader.ReadInnerXml();

                                if(filter == 1)
                                {
                                    if(int.TryParse(str, out int temp))
                                    {
                                        nbNew.FilterType = filter;
                                        nbNew.FilterStr = str;
                                    }
                                }
                            }
                            break;
                    }
                }
                else if(xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    if(xmlReader.Name == "NickBan")
                    {
                        nickban = false;
                        list.Add(nbNew);
                        nbNew = new NickBlacklistMember();
                    }
                }
            }
            return true;
        }

        public override void Run()
        {
            if(!Enabled) return;
            if(_time + interval < new TimeSpan(DateTime.Now.Ticks).TotalSeconds)
            {
                _time = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
                foreach(User user in User.AllUsers)
                {
                    foreach(NickBlacklistMember nbmem in list)
                    {
                        //Console.WriteLine("nbCheck nick: " + nbmem.nick);
                        if(nbmem.FilterType == -1)
                        {
                            if(user.Nickname.IndexOf(nbmem.Nick) != -1)
                            {
                                user.KickFromServer(message, 1);
                            }
                        }
                        else if(nbmem.FilterType == 1)
                        {
                            if(user.Nickname.IndexOf(nbmem.Nick) != -1 && user.DatabaseId != int.Parse(nbmem.FilterStr))
                            {
                                user.KickFromServer(message, 1);
                            }
                        }
                    }
                }
            }
        }
    }
}