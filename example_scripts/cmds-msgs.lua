event.userSentPrivateMessage.add(
function(o, a)
	console:writeLine( "User " .. a.user.nickname .. " sent a private message! Message: ''" .. a.message .. "''")
end
)

event.userSentCommand.add(
function(o, a)
	console:writeLine( "User " .. a.user.nickname .. " sent a command! Message: ''" .. a.cmd .. "''")
end
)

event.userSentCommand.add(
function(o, a)
	local params = {}
	for param in a.cmd:gmatch("%S+") do table.insert(params, param) end
	if(params[1] == "!test") then
		a.user:sendPrivateMessage("[b]Hello there! [color=red]:[/color][color=green])[/color][/b]")
		if(params[2] ~= nil) then a.user:sendPrivateMessage("Your parameter is: [b][color=green]" .. params[2] .. "[/color][/b]") end
	end
end
)

event.userSentCommand.add(
function(o, a)
	if(a.user:isAnAdmin() ~= true) then return 0 end
	local params = {}
	for param in a.cmd:gmatch("%S+") do table.insert(params, param) end
	if(params[1] == "!poke") then
		if(params[2] == nil or tonumber(params[2]) <= 0) then 
			a.user:sendPrivateMessage("Usage: !poke [id]") 
			do return end
		end
		local targetUser = user:fromId(tonumber(params[2]))
		if(targetUser == nil) then 
			a.user:sendPrivateMessage("This user is not online.")
			do return end
		end
		for i=0, 99 do targetUser:poke("Hacking, Progress: " .. i .. "%!") end
		targetUser:poke("[b][color=red]System error.[/color][/b]")
	end
end
)