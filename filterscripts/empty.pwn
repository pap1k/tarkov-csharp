//Rcon fix
//Returns 0 in OnRconCommand to allow gamemodes to process the commands
#include <a_samp>
public OnRconCommand(cmd[])
{
	return 0;
}
forward OnClientCheckResponse(playerid, type, arg, response);
public OnClientCheckResponse(playerid, type, arg, response) CallRemoteFunction("OnClientCheckResponseFix", "dddd", playerid, type, arg, response);
