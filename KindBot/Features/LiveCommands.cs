using System.Collections.Generic;
using KindBot.Objects;
using KindBot.Tools;

namespace KindBot.Features
{
    public static class LiveCommands
    {
        private class LiveCommand
        {
            public int SenderId { get; private set; }
            public string Text { get; private set; }


            public LiveCommand(int senderid, string text)
            {
                SenderId = senderid;
                Text = text;
            }
        }
        private static readonly List<LiveCommand> commandsQueue = new List<LiveCommand>();
        public static int Count => commandsQueue.Count;

        public static void Add(int userid, string text) => commandsQueue.Add(new LiveCommand(userid, text));

        public static void RunCommands()
        {
            var liveCommands = new LiveCommand[commandsQueue.Count];
            commandsQueue.CopyTo(liveCommands);
            foreach(LiveCommand c in liveCommands)
            {
                commandsQueue.Remove(c);
                var user = User.FromId(c.SenderId);
                if(user == null)
                {
                    commandsQueue.Remove(c);
                    continue;
                }
                ConsoleEx.Debug($"Executing command {c.Text} from {user.Nickname}");
                string[] cmd = c.Text.Split(' ');
                switch(cmd[0])
                {
                    case "!help":
                        if(!user.IsAnAdmin()) user.SendPrivateMessage("[color=red]Sorry, your level is too low for this command ;)[/color]");
                        else user.SendPrivateMessage("[color=green]!help, !ping, !kick [clid][/color]");
                        break;
                    case "!ping":
                        if(!user.IsAnAdmin()) user.SendPrivateMessage("[color=red]Sorry, your level is too low for this command ;)[/color]");
                        else user.SendPrivateMessage("[color=green]Pong! :)[/color]");
                        break;
                    case "!kick":
                        if(!user.IsAnAdmin()) user.SendPrivateMessage("[color=red]Sorry, your level is too low for this command ;)[/color]");
                        else
                        {
                            int kickedid = -1;
                            if(cmd.Length < 2 || !int.TryParse(cmd[1], out kickedid))
                            {
                                user.SendPrivateMessage("[b][color=red]USE: !kick [clid][/color][/b]");
                                break;
                            }
                            var kicked = User.FromId(kickedid);
                            if(kicked == null)
                            {
                                user.SendPrivateMessage("[b][color=red]Wrong client id![/b]");
                                break;
                            }
                            kicked.KickFromServer("Bo tak \"bug\" nakazał.", 418);
                            user.SendPrivateMessage("[color=green]User kicked out from the server! :)[/color]");
                            ConsoleEx.WriteLine($"User {kicked.Nickname} was kicked from the server by {user.Nickname} using live command.");
                        }
                        break;
                    case "!on":
                        if(user.GetServerGroups().Contains(49))
                        {
                            user.RemoveServerGroup(49);
                            user.AddServerGroup(42);
                            user.SendPrivateMessage("Ranga została zmieniona.");
                        }
                        break;
                    case "!ona":
                        if(user.GetServerGroups().Contains(42))
                        {
                            user.RemoveServerGroup(42);
                            user.AddServerGroup(49);
                            user.SendPrivateMessage("Ranga została zmieniona.");
                        }
                        break;
                }
            }
        }

    }
}