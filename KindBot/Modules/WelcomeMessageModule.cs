using System;
using KindBot.Objects;
using KindBot.Modules.Common;

namespace KindBot.Modules
{
    public class WelcomeMessageModule : Module
    {
        private double time = 0;

        public WelcomeMessageModule() : base()
        {
            ModuleName = "WelcomeMessageModule";
        }

        public override void Run()
        {
            if(!Enabled) return;
            if(time + 5 /* TODO: Interval config */ < new TimeSpan(DateTime.Now.Ticks).TotalSeconds)
            {
                time = new TimeSpan(DateTime.Now.Ticks).TotalSeconds;
                foreach(User user in User.AllUsers)
                {
                    if(user.ConnectionType == User.ConnectionTypeEnum.Query) continue;
                    //string info = Commands.GetClientInfo(user);
                    //int connectiontime = 0;
                    //if (!int.TryParse(TeamspeakTools.GetParameter(info, "connection_connected_time"), out connectiontime))
                    //	continue;

                    //if (user.ConnectedTime.Minute < 5 * 1000)
                    //{
                    /*
                    int admin_count = Commands.GetClientsCountWithGroup(2) + Commands.GetClientsCountWithGroup(41) + Commands.GetClientsCountWithGroup(62);

                    int connection_count = 0;
                    if (!int.TryParse(Tools.GetParameter(info, "client_totalconnections"), out connection_count))
                        continue;
                    long date = 0;
                    if (!long.TryParse(Tools.GetParameter(info, "client_created"), out date))
                        continue;
                    */


                    //LuaInterface.Events.RaiseTheUserJoinedTheServer(user);

                    //Commands.SendPrivateMessage(user, $"[b]Witaj [color=green]{user.client_nickname}[/color] na serwerze Teamspeak 3 [color=red]norberto5.pl[/color] !");
                    //Commands.SendPrivateMessage(user, $"Gościsz u nas od [b][color=blue]{first_connect.ToString("d.MM.yyyy r")}[/color][/b]. W sumie byłeś już na naszym serwerze [b][color=blue]{connection_count}[/color][/b] razy.");
                    //Commands.SendPrivateMessage(user, $"Aktualnie na serwerze znajduje się [b][color=green]{Commands.GetClientsCount()}[/color]/[color=red]{Commands.GetMaxClients()}[/color][/b] użytkowników.");
                    //Commands.SendPrivateMessage(user, $"Administratorzy online: [b][color={((admin_count > 0) ? "green" : "red")}]{admin_count}[/color][/b]");
                    //Commands.SendPrivateMessage(user, "Nie zapomnij dodać naszego serwera do zakładek ;) Kliknij [b][url=ts3server://norberto5.pl?addbookmark=norberto5.pl][TUTAJ][/url][/b], aby dodać nas do zakładek.");
                    //}
                }
            }
        }
    }
}