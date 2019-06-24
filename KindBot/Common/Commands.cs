using System.Collections.Generic;
using System.Linq;
using KindBot.Communication;
using KindBot.Objects;
using KindBot.Tools;
using MoonSharp.Interpreter;

namespace KindBot.Common
{
    [MoonSharpUserData]
    public static class Commands
    {
        public static int GetClientsCountWithGroup(int groupid)
        {
            int count = 0;
            foreach(User user in User.AllUsers)
            {
                if(user.ConnectionType == User.ConnectionTypeEnum.Query) continue;
                string info = TelnetConnector.Instance.Execute($"clientinfo clid={user.Id}");
                var servergroups = new List<int>();
                string tmp = TeamspeakTools.GetParameter<string>(info, "client_servergroups");
                foreach(string s in tmp.Split(','))
                {
                    if(!int.TryParse(s, out int gr))
                        continue;
                    if(gr != 0) servergroups.Add(gr);
                }

                if(servergroups.IndexOf(groupid) != -1)
                {
                    count++;
                }
            }
            return count;
        }

        public static void SendGlobalMessage(string message)
        {
            TelnetConnector.Instance.Execute($"sendtextmessage targetmode=3 msg={message.ConvertToTeamspeakString()}");
            ConsoleEx.WriteLine($"[Server Message]: {message}");
        }

        public static void Quit() => TelnetConnector.Instance.Execute("quit");

        /// <summary>
        /// Returns an array of database ids of users with a specific group.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static int[] GetClientListOfServerGroup(int groupId)
        {
            return TelnetConnector.Instance.Execute($"servergroupclientlist sgid={groupId}")
                .Replace("cldbid=", "").Split('|').Select(x => int.Parse(x)).ToArray();
        }
    }
}