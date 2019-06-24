event.userJoinedTheServer.add(
function(o, a)
	local admin_count = commands:getClientsCountWithGroup(2) + commands:getClientsCountWithGroup(41) + commands:getClientsCountWithGroup(62)

	local color = "red"
	if(admin_count > 0) then color = "green" end
	
	a.user:sendPrivateMessage("[b]Welcome [color=green]" .. a.user.nickname .. "[/color] on the [color=red]example.com[/color] TS3 server !");
	a.user:sendPrivateMessage("You'are here since [b][color=blue]" .. a.user.created .."[/color][/b]. You were here [b][color=blue]" .. a.user.totalConnections .. "[/color][/b] times.");
	a.user:sendPrivateMessage("There are currently [b][color=green]" .. serverInfo.ClientsOnline .. "[/color]/[color=red]" .. serverInfo.MaxClients .. "[/color][/b] users online.");
	a.user:sendPrivateMessage("Admins online: [b][color=" .. color .. "]" .. admin_count .. "[/color][/b]");
	a.user:sendPrivateMessage("Don't forget to add this server to your bookmarks ;) Click [b][url=ts3server://example.com?addbookmark=example.com][HERE][/url][/b] to add a bookmark.");
end
);

event.userLeftTheServer.add(
function(o, a)
	--console:writeLine( "User " .. a.user.nickname .. " left the server!")
end
);