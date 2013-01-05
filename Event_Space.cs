function GameConnection::teleportToModuleSlot(%client)
{
	%slot = $DefaultMiniGame.mcf.findSlotByBLID(%client.bl_id);
	if(%slot == -1)
		return;
	
	%client.player.setVelocity("0 0 0");
	%client.player.setTransform(vectorAdd(%slot.getPosition(), "-15 0 1") SPC "0 0 1" SPC $pi/2);
}

function GameConnection::giveModuleSlot(%client)
{
	//a hack to get around some event object limits
	$DefaultMiniGame.evalQueue.addStatement("GameConnectiongiveModuleSlot(" @ %client @ ");");
}

function GameConnectiongiveModuleSlot(%client)
{
	if (!isObject(%client))
		return;
	
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

registerOutputEvent("GameConnection", "giveModuleSlot", "", false);


function fxDTSBrick::nextSaveSlot(%obj, %client)
{
	$DefaultMiniGame.evalQueue.addStatement("fxDTSBricknextSaveSlot(" @ %obj @ ", " @ %client @ ");");
}

function fxDTSBricknextSaveSlot(%obj, %client)
{
	if (!isObject(%obj) || !isObject(%client) || !isObject(%obj.slot))
		return;
	
	if (%obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%obj.slot.nextSaveSlot();
	}else{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
	}
}

registerOutputEvent("fxDTSBrick", "nextSaveSlot", "", true);


function fxDTSBrick::prevSaveSlot(%obj, %client)
{
	$DefaultMiniGame.evalQueue.addStatement("fxDTSBrickprevSaveSlot(" @ %obj @ ", " @ %client @ ");");
}

function fxDTSBrickprevSaveSlot(%obj, %client)
{
	if (!isObject(%obj) || !isObject(%client) || !isObject(%obj.slot))
		return;
	
	if (%obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%obj.slot.prevSaveSlot();
	}else{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
	}
}

registerOutputEvent("fxDTSBrick", "prevSaveSlot", "", true);


function fxDTSBrick::loadSaveSlot(%obj, %client)
{
	$DefaultMiniGame.evalQueue.addStatement("fxDTSBrickloadSaveSlot(" @ %obj @ ", " @ %client @ ");");
}

function fxDTSBrickloadSaveSlot(%obj, %client)
{
	if (!isObject(%obj) || !isObject(%client) || !isObject(%obj.slot))
		return;
	
	if (%obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%curTime = getSimTime();

		if ((%curTime - %client.lastLoadSlotTime) < 2000)
		{
			commandToClient(%client, 'centerPrint', "\c3Please wait before loading again.", 2);
			return;
		}
		
		%client.lastLoadSlotTime = %curTime;
		%obj.slot.loadBuiltBricksInSaveSlot();
		commandToClient(%client, 'centerPrint', "\c3Bricks loaded from slot " @ %obj.slot.getCurrentSaveSlot() + 1 @ ".", 2);
	}else{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
	}
}

registerOutputEvent("fxDTSBrick", "loadSaveSlot", "", true);


function fxDTSBrick::saveSaveSlot(%obj, %client)
{	
	$DefaultMiniGame.evalQueue.addStatement("fxDTSBricksaveSaveSlot(" @ %obj @ ", " @ %client @ ");");
}

function fxDTSBricksaveSaveSlot(%obj, %client)
{
	if (!isObject(%obj) || !isObject(%client) || !isObject(%obj.slot))
		return;

	if (%obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%curTime = getSimTime();

		if ((%curTime - %client.lastSaveSlotTime) < 2000)
		{
			commandToClient(%client, 'centerPrint', "\c3Please wait before saving again.", 2);
			return;
		}
		
		%client.lastSaveSlotTime = %curTime;
		%obj.slot.saveBuiltBricksInSaveSlot();
		commandToClient(%client, 'centerPrint', "\c3Bricks saved in slot " @ %obj.slot.getCurrentSaveSlot() + 1 @ ".", 2);
	}else{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
	}
}

registerOutputEvent("fxDTSBrick", "saveSaveSlot", "", true);


function fxDTSBrick::clearSaveSlot(%obj, %client)
{
	$DefaultMiniGame.evalQueue.addStatement("fxDTSBrickclearSaveSlot(" @ %obj @ ", " @ %client @ ");");
}

function fxDTSBrickclearSaveSlot(%obj, %client)
{
	if (!isObject(%obj) || !isObject(%client) || !isObject(%obj.slot))
		return;
	
	if (%obj.slot.getOwnerBLID() == %client.getBLID())
	{
		%obj.slot.clearBuiltBricks();
		commandToClient(%client, 'centerPrint', "\c3Bricks cleared.", 2);
	}else{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
	}
}

registerOutputEvent("fxDTSBrick", "clearSaveSlot", "", true);

