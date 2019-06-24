event.userJoinedTheServer.add(
function(o, a)
	if serverInfo.clientsOnline >= serverInfo.maxClients-serverInfo.reservedSlots then
		local mostIdle = nil
		local mostIdleTime = 0
	
		local users = user.allUsers
		for i, user in ipairs(users) do
			local idle = user.idleTime
			--console:writeLine( "User " .. user.nickname .. " Idle time:" .. idle)
			if(idle > mostIdleTime and not user.isAnAdmin()) then
				mostIdle = user
				mostIdleTime = idle
			end
		end
			
		if(mostIdleTime > 600) then
			console:writeLine( "Kicking user " .. mostIdle.nickname .. " to make slot free (Idle time: " .. mostIdleTime	.. " )")
			mostIdle.kickFromServer("Making slot free, the longest inactive user.")
		else
			console:writeLine( "I'm unable to kick anyone from server to free a slot, because no one is idle for more than 10 minutes.")
		end
			
	end
end
);