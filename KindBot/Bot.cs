using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using KindBot.Communication;
using KindBot.Configuration;
using KindBot.Features;
using KindBot.Lua;
using KindBot.Modules;
using KindBot.Modules.Common;
using KindBot.Objects;
using KindBot.Tools;

namespace KindBot
{
    public class Bot
    {
        private void MakeSomeTest()
        {
            //var list = Channel.GetChannel(75).GetChannelChildren();
            //foreach (Channel c in list)
            //{
            //	string console = c.ToString();
            //	User owner = c.GetUsersWithGroup(23).FirstOrDefault();
            //	if (owner != null)
            //		console += $" - Owner is online: {owner.Nickname} Current channel: {new Channel(owner.ChannelId).Name}";
            //	ConsoleEx.WriteLine(console);
            //}

            //var user = User.FindUserById(1);
            //ConsoleEx.WriteDebug(user.IdleTime.ToString());

            //user.IsTalker = true;

            //for (int i = 0; i < 100; i++)
            //{
            //	user.Poke($"{i}: I LUV U MUCH <3");
            //}

            //User.Move(new User[] { User.FindUserById(8), User.FindUserById(29) }, new Channel(178));
            //user.SetClientChannelGroup(23, new Channel(178));
            //ConsoleEx.WriteDebug(user.Nickname);

            //ChannelFactory factory = new ChannelFactory("Najlepszy bot na świecie").Description("Mówię prawdę, nie kłamię").
            //Topic("On mówi prawdę").Codec(Channel.ChannelCodec.SpeexUltraWideband).CodecQuality(10).
            //MaxClients(2).MaxFamilyClients(3).Parent(92);

            //Channel channel = factory.Create();
            //if (channel != null) ConsoleEx.WriteDebug($"I've created a channel '{channel.Name}' with ID: {channel.Id} :)");

            //ServerInfo.HostMessage = "Test message";
            //ServerInfo.HostMessageMode = ServerInfo.HostMessageModeEnum.Modal;
            //ConsoleEx.WriteLine("Parameter: " + ServerInfo.TotalBytesUploaded);
        }

        private readonly List<Module> modules = new List<Module>();
        private readonly ModulesController modulesController = new ModulesController();
        private readonly BotConfiguration botConfiguration = new BotConfiguration();
        private BackgroundWorker backgroundWorker;
        private LuaController luaController;
        private QueryEvents queryEvents;

        private bool isClosing;

        public void Start()
        {
            // bot debug/log/logic modules
            modules.Add(new PrintDateModule());
            // bot real action modules
            //Modules.Add(new WelcomeMessageModule()); // TODO: Fix or remove and replace by Lua
            modules.Add(new GlobalMessagesModule());
            modules.Add(new NickBlacklistModule());
            modules.Add(new RegisterUsersModule());
            modules.Add(new GroupAuthorizationCheckModule());

            LoadConfiguration();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);
            backgroundWorker.WorkerSupportsCancellation = true;

            while(!isClosing)
            {
                if(!Initialize())
                {
                    ConsoleEx.Error("Couldn't initalize the bot. Retry in 5 seconds...");
                    Thread.Sleep(5000);
                    continue;
                }

                while(TelnetConnector.Instance.Connected && !isClosing)
                {
#if DEBUG
                    if(Console.KeyAvailable)
                    {
                        string input = Console.ReadLine();
                        if(string.IsNullOrEmpty(input))
                        {
                            continue;
                        }
                        else if(input == "list")
                        {
                            foreach(User user in User.AllUsers)
                            {
                                ConsoleEx.WriteLine($"{user.Id}: {user.Nickname} - channel: {user.ChannelId} type: {user.ConnectionType} (dbid: {user.DatabaseId} )");
                            }
                        }
                        else if(input.IndexOf("lua ") == 0)
                        {
                            string script = input.Substring(4);
                            luaController.RunConsoleScript(script);
                        }
                        else if(input.IndexOf("telnet ") == 0)
                        {
                            backgroundWorker.CancelAsync();
                            while(backgroundWorker.IsBusy) { continue; }
                            ConsoleEx.WriteLine($"Executing: {input.Substring(7)}");
                            ConsoleEx.WriteLine(TelnetConnector.Instance.Execute(input.Substring(7), true));
                        }
                    }
#endif
                    if(!backgroundWorker.IsBusy)
                        backgroundWorker.RunWorkerAsync();
                    Thread.Sleep(100);
                }
                if(!isClosing)
                {
                    ConsoleEx.Error("Connection lost. Reconnecting...");
                }
            }
        }

        public void Stop()
        {
            isClosing = true;
            backgroundWorker.CancelAsync();
            backgroundWorker.Dispose();
            backgroundWorker = null;
        }

        private void Reset() => User.InvalidateAllUsers();

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            for(int i = 0; i < modulesController.Count; i++)
            {
                if(worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                modulesController.RunNextModule();
                TelnetConnector.Instance.Execute("whoami"); // just to ensure we will get all notifys
                if(LiveCommands.Count > 0) LiveCommands.RunCommands();
                if(queryEvents.Count > 0) queryEvents.HandleQueryEvents();
            }
            Thread.Sleep(100);
            //ConsoleEx.WriteLine("Users currently online: " + Commands.GetClientsCount() + "/" + Commands.GetMaxClients());
        }

        private bool Initialize()
        {
            Reset();

            if(!MakeConnection()) return false;
            if(!InitializeBot()) return false;

            modulesController.LoadModules(modules);

            TelnetConnector.Instance.Execute("servernotifyregister event=server");
            TelnetConnector.Instance.Execute("servernotifyregister event=channel id=0");
            TelnetConnector.Instance.Execute("servernotifyregister event=textprivate");
            //TelnetConnector.Instance.Execute("servernotifyregister event=textserver");

            if(luaController == null)
            {
                luaController = new LuaController();
                queryEvents = new QueryEvents(luaController);
                luaController.InitializeLua();
            }

            ConsoleEx.WriteLine("Ready to go...");

            MakeSomeTest();
            return true;
        }

        private bool MakeConnection()
        {
            if(!TelnetConnector.Instance.Connect(botConfiguration.IP, botConfiguration.Port)) return false;
            ConsoleEx.WriteLine($"Successfully connected to {botConfiguration.IP} on port {botConfiguration.Port}");

            var loginDetails = new LoginDetails()
            {
                VirtualServerId = botConfiguration.VirtualServerID,
                Username = botConfiguration.QueryLogin,
                Password = botConfiguration.QueryPassword
            };

            if(!TelnetConnector.Instance.Login(loginDetails))
            {
                ConsoleEx.Error($"Couldn't log in as '{botConfiguration.QueryLogin}' to virtual server with id {botConfiguration.VirtualServerID}. (check credentials)");
                return false;
            }
            return true;
        }

        private bool InitializeBot()
        {
            TelnetConnector.Instance.Execute($"clientupdate client_nickname={botConfiguration.BotNickname.ConvertToTeamspeakString()}"); // changing bot nickname
            string output = TelnetConnector.Instance.Execute("whoami");
            if(TeamspeakTools.GetParameter<string>(output, "client_nickname").CompareTo(botConfiguration.BotNickname.ConvertToTeamspeakString()) != 0)
                TelnetConnector.Instance.Execute($"clientupdate client_nickname={botConfiguration.SecondBotNickname.ConvertToTeamspeakString()}");

            if(!TeamspeakTools.TryGetParameter(TelnetConnector.Instance.Execute("whoami"), "client_id", out int botId)) return false;
            ConsoleEx.WriteLine($"Bot has logged in successfully! (Bot ID: {botId} )");
            User.MarkUserAsBot(User.FromId(botId));
            User.Bot.Move(new Channel(botConfiguration.BotChannelID));

            ConsoleEx.WriteLine($"Users currently online: { ServerInfo.Instance.ClientsOnline }/{ ServerInfo.Instance.MaxClients }");

            return true;
        }

        private void LoadConfiguration()
        {
            ConfigurationLoader.AddConfigurationFile(botConfiguration);
            foreach(IConfigurable configurableModule in modules.OfType<IConfigurable>())
            {
                ConfigurationLoader.AddConfigurationFile(configurableModule);
            }

            if(!ConfigurationLoader.LoadConfiguration())
            {
                ConsoleEx.Warning("There was a problem with loading configuration files. Fix the problem.");
                Thread.Sleep(5000);
                return;
            }
        }
    }
}