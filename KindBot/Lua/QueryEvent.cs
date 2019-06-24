namespace KindBot.Lua
{
    public class QueryEvent
    {
        public QueryEventType QueryEventType;
        public string EventMessage;

        public QueryEvent(QueryEventType qevent, string eventmsg)
        {
            QueryEventType = qevent;
            EventMessage = eventmsg;
        }
    }
}