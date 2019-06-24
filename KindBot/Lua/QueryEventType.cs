namespace KindBot.Lua
{
    public enum QueryEventType
    {
        UnknownNotify,
        PrivateMessage,
        ClientJoinedServer,
        ClientLeftServer,
        ClientMoved,
        ServerEdited,
        ChannelEdited,
        ChannelPasswordChanged,
        ChannelDescriptionChanged,
        ChannelCreated,
        ChannelDeleted
    }
}