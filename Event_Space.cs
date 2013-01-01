function GameConnection::giveModuleSlot(%client)
{
	%mcf = $DefaultMiniGame.mcf;
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
	
	%client.teleportToModuleSlot();
}

function GameConnection::teleportToModuleSlot(%client)
{
	%slot = $DefaultMiniGame.mcf.findSlotByBLID(%client.bl_id);
	%client.player.setVelocity("0 0 0");
	%client.player.setTransform(vectorAdd(%slot.getPosition(), "2 16 1") SPC "0 0 1" SPC $pi/2);
}

registerOutputEvent("GameConnection","giveModuleSlot","",0);

function fxDTSBrick::nextSaveSlot(%obj, %client)
{
	if (isObject(%obj.slot) && %obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%obj.slot.nextSaveSlot();
		commandToClient(%client, 'centerPrint', "\c3Slot " @ %obj.slot.getCurrentSaveSlot() + 1 @ " selected.", 2);
	}
}

registerOutputEvent("fxDTSBrick", "nextSaveSlot", "", 1);

function fxDTSBrick::prevSaveSlot(%obj, %client)
{
	if (isObject(%obj.slot) && %obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%obj.slot.prevSaveSlot();
		commandToClient(%client, 'centerPrint', "\c3Slot " @ %obj.slot.getCurrentSaveSlot() + 1 @ " selected.", 2);
	}
}

registerOutputEvent("fxDTSBrick", "prevSaveSlot", "", 1);

function fxDTSBrick::loadSaveSlot(%obj, %client)
{	
	if (isObject(%obj.slot) && %obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%curTime = getSimTime();
		if (%client.lastLoadSlotTime $= "")
			%client.lastLoadSlotTime = %curTime;
		else if ((%curTime - %client.lastLoadSlotTime) < 2000)
		{
			commandToClient(%client, 'centerPrint', "\c3Please wait before loading again.", 2);
			return;
		}
	
		%obj.slot.loadBuiltBricksInSaveSlot();
		commandToClient(%client, 'centerPrint', "\c3Bricks loaded from slot " @ %obj.slot.getCurrentSaveSlot() + 1 @ ".", 2);
	}
}

registerOutputEvent("fxDTSBrick", "loadSaveSlot", "", 1);

function fxDTSBrick::saveSaveSlot(%obj, %client)
{	
	if (isObject(%obj.slot) && %obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%curTime = getSimTime();
		if (%client.lastSaveSlotTime $= "")
			%client.lastSaveSlotTime = %curTime;
		else if ((%curTime - %client.lastSaveSlotTime) < 2000)
		{
			commandToClient(%client, 'centerPrint', "\c3Please wait before saving again.", 2);
			return;
		}
	
		%obj.slot.saveBuiltBricksInSaveSlot();
		commandToClient(%client, 'centerPrint', "\c3Bricks saved in slot " @ %obj.slot.getCurrentSaveSlot() + 1 @ ".", 2);
	}
}

registerOutputEvent("fxDTSBrick", "saveSaveSlot", "", 1);

function fxDTSBrick::clearSaveSlot(%obj, %client)
{
	if (isObject(%obj.slot) && %obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%obj.slot.clearBuiltBricks();
		commandToClient(%client, 'centerPrint', "\c3Bricks cleared.", 2);
	}
}

registerOutputEvent("fxDTSBrick", "clearSaveSlot", "", 1);
