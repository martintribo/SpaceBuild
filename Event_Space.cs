//This file needs refactoring.

function GameConnection::teleportToModuleSlot(%client)
{
	%slot = $DefaultMiniGame.mcf.findSlotByBLID(%client.bl_id);
	if(%slot == -1)
		return;
	
	%client.player.setVelocity("0 0 0");
	%client.player.setTransform(vectorAdd(%slot.getPosition(), "-15 0 1") SPC "0 0 1" SPC $pi/2);
}


function fxDTSBrick::giveModuleSlot(%obj, %client)
{
	if(%obj.getGroup().bl_id != $SpaceBuild::SpawnBLID)
	{
		commandToClient(%client, 'bottomPrint', "You can't place the special events.", 2);
		return;
	}
	
	//a hack to get around some event object limits
	$DefaultMiniGame.evalQueue.addStatement("fxDTSBrickgiveModuleSlot(" @ %client @ ");");
}

function fxDTSBrickgiveModuleSlot(%client)
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

registerOutputEvent("fxDTSBrick", "giveModuleSlot", "", true);


function fxDTSBrick::nextSaveSlot(%obj, %client)
{
	if(!%obj.isSBTemplate)
	{
		commandToClient(%client, 'bottomPrint', "You can't place the special events.", 2);
		return;
	}
	
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
	if(!%obj.isSBTemplate)
	{
		commandToClient(%client, 'bottomPrint', "You can't place the special events.", 2);
		return;
	}
	
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
	if(%obj.slot.getOwnerBLID() != %client.getBLID())
	{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
		return;
	}
	
	if(!%obj.isSBTemplate)
	{
		commandToClient(%client, 'bottomPrint', "You can't place the special events.", 2);
		return;
	}
	
	%client.lastSaveEventBrick = %obj;
	commandToClient(%client, 'MessageBoxYesNo', "Load Bricks?", "Do you want to CLEAR YOUR SLOT and LOAD bricks?", 'loadSlotBricks');
}

function ServerCmdLoadSlotBricks(%client)
{
	%obj = %client.lastSaveEventBrick;

	if (!isObject(%obj) || !isObject(%client) || !isObject(%obj.slot))
		return;
	
	//this check happens twice for security
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
	if(%obj.slot.getOwnerBLID() != %client.getBLID())
	{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
		return;
	}
	
	if(!%obj.isSBTemplate)
	{
		commandToClient(%client, 'bottomPrint', "You can't place the special events.", 2);
		return;
	}
	
	%client.lastSaveEventBrick = %obj;
	commandToClient(%client, 'MessageBoxYesNo', "Save Bricks?", "Do you want to SAVE your bricks, potentially overwriting a save?", 'saveSlotBricks');
}

function ServerCmdSaveSlotBricks(%client)
{
	%obj = %client.lastSaveEventBrick;
	
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
	if(%obj.slot.getOwnerBLID() != %client.getBLID())
	{
		commandToClient(%client, 'bottomPrint', "This is not your slot!", 2);
		return;
	}
	
	if(!%obj.isSBTemplate)
	{
		commandToClient(%client, 'bottomPrint', "You can't place the special events.", 2);
		return;
	}
	
	%obj = %client.lastSaveEventBrick;
	
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

