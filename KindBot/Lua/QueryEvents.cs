using System;
using System.Collections.Generic;
using System.Linq;
using KindBot.Communication;
using KindBot.Features;
using KindBot.Objects;
using KindBot.Tools;

namespace KindBot.Lua
{
    public class QueryEvents
    {
        public List<QueryEvent> EventsQueue = new List<QueryEvent>();
        public int Count => EventsQueue.Count;

        private readonly LuaController luaController;
        private readonly Dictionary<string, QueryEventType> events = new Dictionary<string, QueryEventType>()
        {
            { "notifycliententerview", QueryEventType.ClientJoinedServer },
            { "notifyclientleftview", QueryEventType.ClientLeftServer },
            { "notifytextmessage", QueryEventType.PrivateMessage },
            { "notifyclientmoved", QueryEventType.ClientMoved },
            { "notifyserveredited", QueryEventType.ServerEdited },
            { "notifychanneledited", QueryEventType.ChannelEdited },
            { "notifychannelpasswordchanged", QueryEventType.ChannelPasswordChanged },
            { "notifychanneldescriptionchanged", QueryEventType.ChannelDescriptionChanged },
            { "notifychannelcreated", QueryEventType.ChannelCreated },
            { "notifychanneldeleted", QueryEventType.ChannelDeleted },
            { "notify", QueryEventType.UnknownNotify }
        };

        public QueryEvents(LuaController luaController)
        {
            this.luaController = luaController;

            TelnetConnector.Instance.EventHandled += (sender, msg) =>
            {
                KeyValuePair<string, QueryEventType> handledEvent = events.First(e => msg.IndexOf(e.Key) == 0);
                AddQueryEvent(handledEvent.Value, msg);

            };
        }

        public void AddQueryEvent(QueryEventType qevent, string eventmsg)
        {
            var queryEvent = new QueryEvent(qevent, eventmsg);
            if(EventsQueue.Exists(x => x.QueryEventType == qevent && x.EventMessage == eventmsg)) return;
            EventsQueue.Add(queryEvent);

        }

        public void HandleQueryEvents()
        {
            try
            {
                var queryEvents = new QueryEvent[EventsQueue.Count];
                EventsQueue.CopyTo(queryEvents);
                //ConsoleEx.WriteDebug($"Handling Query Events... Count: {QueryEvents.Count}");
                foreach(QueryEvent queryEvent in queryEvents)
                {
                    EventsQueue.Remove(queryEvent);
                    //ConsoleEx.WriteDebug($"{queryEvent.QueryEventType} - {queryEvent.EventMessage}");

                    if(queryEvent.QueryEventType == QueryEventType.UnknownNotify)
                    {
                        ConsoleEx.Debug($"Unknown notify detected: {queryEvent.EventMessage}");
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.PrivateMessage)
                    {
                        string msg = TeamspeakTools.GetParameter<string>(queryEvent.EventMessage, "msg");
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "invokerid", out int userid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse own messages

                        if(msg.IndexOf("!") == 0)
                        {
                            luaController.Events.OnUserSentComand(User.FromId(userid), msg);
                            LiveCommands.Add(userid, msg);
                        }
                        else
                        {
                            luaController.Events.OnUserSentPrivateMessage(User.FromId(userid), msg);
                        }

                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ClientJoinedServer)
                    {
                        if(TeamspeakTools.GetParameter<int>(queryEvent.EventMessage, "cfid") != 0) continue;
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "clid", out int userid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse myself
                        User.InvalidateAllUsers();
                        luaController.Events.OnUserJoinedTheServer(User.FromId(userid));
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ClientLeftServer)
                    {
                        if(TeamspeakTools.GetParameter<int>(queryEvent.EventMessage, "ctid") != 0) continue;
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "clid", out int userid)) continue;
                        var user = User.FromId(userid);
                        if(user == null) continue;
                        User.InvalidateAllUsers();
                        if(userid == User.Bot.Id) continue; // don't parse myself
                        luaController.Events.OnUserLeftTheServer(user);
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ClientMoved)
                    {
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "clid", out int userid)) continue;
                        var user = User.FromId(userid);
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "ctid", out int channelid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse myself
                        luaController.Events.OnUserSwitchedChannel(user, channelid);
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ServerEdited)
                    {
                        string reason = TeamspeakTools.GetParameter<string>(queryEvent.EventMessage, "reasonid");
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "invokerid", out int userid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse own edits
                        ConsoleEx.Debug($"Server was edited by {User.FromId(userid).Nickname}");
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ChannelEdited)
                    {
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "cid", out int channelId)) continue;
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "invokerid", out int userid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse own edits
                        ConsoleEx.Debug($"Channel {new Channel(channelId).Name} was edited by {User.FromId(userid).Nickname}");
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ChannelDescriptionChanged)
                    {
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "cid", out int channelId)) continue;
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ChannelPasswordChanged)
                    {
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "cid", out int channelId)) continue;
                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ChannelCreated)
                    {
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "cid", out int channelId)) continue;
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "invokerid", out int userid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse own edits
                        ConsoleEx.Debug($"Channel {new Channel(channelId).Name} was created by {User.FromId(userid).Nickname}");

                    }
                    else if(queryEvent.QueryEventType == QueryEventType.ChannelDeleted)
                    {
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "cid", out int channelId)) continue;
                        if(!TeamspeakTools.TryGetParameter(queryEvent.EventMessage, "invokerid", out int userid)) continue;
                        if(userid == User.Bot.Id) continue; // don't parse own edits
                        ConsoleEx.Debug($"Channel with id {channelId} was deleted by {User.FromId(userid).Nickname}");
                    }
                }
            }
            catch(Exception e)
            {
                ConsoleEx.Error($"QueryEvents Exception: {e.Message}\n{e.StackTrace}");
            }

        }
    }
}