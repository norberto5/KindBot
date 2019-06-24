using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using KindBot.Configuration;
using KindBot.Modules.Common;
using KindBot.Objects;
using KindBot.Tools;

namespace KindBot.Modules
{
    public class RegisterUsersModule : Module, IConfigurable
    {
        private int interval = -1;
        private int newGroupID = -1;
        private int registeredGroupID = -1;
        private TimeSpan connectionTimeRequirement = TimeSpan.Zero;
        private string messsage = "";

        private double _time = 0;

        public RegisterUsersModule() : base()
        {
            ModuleName = "RegisterUsersModule";
        }

        public void CreateDefaultConfiguration(XmlWriter xmlWriter)
        {
            using(XmlWriter w = xmlWriter)
            {
                w.WriteStartElement(GetConfigurationFilename());

                string[,] registerUsers = {
                {"IsTurnedOn", "false", "Is this module turned on? 'true' or 'false'"}, { "CheckInterval", "10", "How big interval will be between each checkout. In seconds."} ,
                {"NewUserGroupID", "43" , "ID of the default 'guest' group."}, {"RegisteredGroupID", "42" , "ID of the group bot will add user to."},
                {"ConnectionTimeRequirement", "1800" , "The condition to change user group to registered - time of one connection in seconds."},
                {"RegisterMessage", "You have been registered successfully. Be nice! :)\\n[b][color=red]And don't forget to bookmark our server.[/color][/b]" , "The message will be sent to the user."}
                };
                ConfigurationLoader.WriteElementList(w, registerUsers);

                w.WriteEndElement();
            }
        }

        public string GetConfigurationFilename() => ModuleName;

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
                        case "IsTurnedOn":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "IsTurnedOn", out bool tmp)) return false;
                            Enabled = tmp;
                            break;
                        case "CheckInterval":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "CheckInterval", out int tmpint)) return false;
                            interval = tmpint;
                            break;
                        case "NewUserGroupID":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "NewUserGroupID", out tmpint)) return false;
                            newGroupID = tmpint;
                            break;
                        case "RegisteredGroupID":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "RegisteredGroupID", out tmpint)) return false;
                            registeredGroupID = tmpint;
                            break;
                        case "ConnectionTimeRequirement":
                            if(!ConfigurationParser.Parse(xmlReader.ReadInnerXml(), GetConfigurationFilename(), "ConnectionTimeRequirement", out tmpint)) return false;
                            connectionTimeRequirement = TimeSpan.FromSeconds(tmpint);
                            break;
                        case "RegisterMessage":
                            messsage = xmlReader.ReadInnerXml();
                            break;
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
                    if(user.ConnectionType == User.ConnectionTypeEnum.Query) continue;

                    DateTime connectedTime = user.ConnectedTime;
                    IEnumerable<int> servergroups = user.GetServerGroups();

                    if(servergroups.Contains(newGroupID))
                    {
                        if(connectedTime.Add(connectionTimeRequirement) > DateTime.Now)
                        {
                            user.AddServerGroup(registeredGroupID);
                            user.SendPrivateMessage(messsage);
                            ConsoleEx.WriteLine($"Succesfully registered a new user! Nickname: {user.Nickname} (dbid: {user.DatabaseId} )");
                        }
                    }
                }
            }
        }
    }
}