using System.Collections.Generic;
using System.Linq;
using System.Xml;
using KindBot.Configuration;
using KindBot.Modules.Common;
using KindBot.Objects;
using KindBot.Tools;

namespace KindBot.Modules
{
    public class GroupAuth
    {
        public int GroupId;
        public List<int> FilterList = new List<int>();

        public GroupAuth(int groupid, string filter)
        {
            GroupId = groupid;
            foreach(string s in filter.Split(','))
            {
                if(!int.TryParse(s, out int gr))
                {
                    ConsoleEx.Warning("[Configuration]: There was a problem with parsing 'GroupId' authorized groups in '<GroupAuthorization>' configuration");
                    ConsoleEx.Warning("You should use a ',' character to split few database ids. For example: '1,103,233,555'");
                    continue;
                }
                if(gr != 0) FilterList.Add(gr);
            }
        }
    }

    public class GroupAuthorizationCheckModule : Module, IConfigurable
    {
        private readonly List<GroupAuth> groupAuthList = new List<GroupAuth>();

        public GroupAuthorizationCheckModule() : base()
        {
            ModuleName = "GroupAuthorizationCheckModule";
        }

        public void CreateDefaultConfiguration(XmlWriter xmlWriter)
        {
            using(XmlWriter w = xmlWriter)
            {
                w.WriteStartElement(GetConfigurationFilename());

                w.WriteComment("Is this module turned on? 'true' or 'false'");
                w.WriteStartElement("IsTurnedOn");
                w.WriteString("false");
                w.WriteEndElement();

                w.WriteComment("'id' is the group id which requires an authorization. Integers split by commas is a white-list of users. White-list is using users database ids.");
                w.WriteStartElement("GroupId");
                w.WriteAttributeString("id", "2");
                w.WriteString("1,102,405");
                w.WriteEndElement();

                w.WriteComment("You can set as many groups as you want.");
                w.WriteStartElement("GroupId");
                w.WriteAttributeString("id", "41");
                w.WriteString("13,105");
                w.WriteEndElement();

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
                        case "GroupId":
                            if(!ConfigurationParser.Parse(xmlReader.GetAttribute("id"), GetConfigurationFilename(), "GroupId", out int tmpint)) return false;
                            groupAuthList.Add(new GroupAuth(tmpint, xmlReader.ReadInnerXml()));
                            break;
                    }
                }
                else if(xmlReader.NodeType == XmlNodeType.EndElement)
                {
                    //if (xmlReader.Name == "Messages") messages = false;
                }
            }
            return true;
        }

        public override void Run()
        {
            if(!Enabled) return;
            foreach(User user in User.AllUsers)
            {
                IEnumerable<int> servergroups = user.GetServerGroups();

                foreach(GroupAuth auth in groupAuthList)
                {
                    if(servergroups.Contains(auth.GroupId) && !auth.FilterList.Contains(user.DatabaseId))
                    {
                        user.RemoveServerGroup(auth.GroupId);
                        ConsoleEx.Warning($"[GroupAuth]: Removed {auth.GroupId} group from the unauthorized user! Nickname: {user.Nickname} (dbid: {user.DatabaseId} ip: {user.Ip} )");
                    }
                }
            }
        }
    }
}