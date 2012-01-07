function GameConnection::giveModuleSlot(%client)
{
	%mcf = $DebugMCF;
	if(!isObject(%mcf))
		return;
	
	%slot = %mcf.findSlotByBLID(%client.bl_id);
	if(%slot == -1) //no slot exists; give them one
		%slot = %mcf.createSlotForClient(%client);
	if(%slot == -1) //if the next free slot was -1 it means we're at max
	{
		messageClient(%client, "", "It seems we've run out of module slots! Sorry about that.");
		return;
	}
	
	%client.player.setVelocity("0 0 0");
	%client.player.setTransform(vectorAdd(%slot.getPosition(), "-12 0 -6") SPC "0 0 1" SPC $pi/2);
}

registerOutputEvent("GameConnection","giveModuleSlot","",0);
