event.userSwitchedChannel.add(
function(o, a)
	if(a.channelId == 135) then
		a.user:sendPrivateMessage("Welcome to the special channel.")
		a.user:sendPrivateMessage("How can I serve you?")
	end
end
);