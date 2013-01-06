//This script is for enforcing rules and user-friendliness stuff.
//It is not required for normal function. Admins ignore all of these checks.
//There are some fun bonus commands at the bottom, too.

$sbDisableWrenches = 1; //disable wrenches in space
$sbDisableHammers = 1;  //disable hammering in space (should probably always be on)
$sbRemindBuilding = 1;  //remind players who try to build in space that you can't build in space
$sbDisableWands = 1;    //disable wands in space (should probably be on)

package SpaceBuildRulesPackage {
	function WrenchImage::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal)
	{
		if(getWord(%col.getTransform(), 2) >= $spaceHeight && !%obj.client.isAdmin && $sbDisableWrenches && %col.getClassName() $= "fxDTSBrick")
		{
			commandToClient(%obj.client, 'centerPrint', "\c3No wrenching in space!", 2);
			return;
		}
		else if (!%obj.client.isAdmin && %col.isSBTemplate)
		{
			return;
		}
		
		parent::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal);
	}
	
	function HammerImage::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal)
	{
		if(getWord(%col.getTransform(), 2) >= $spaceHeight && !%obj.client.isAdmin && $sbDisableHammers && %col.getClassName() $= "fxDTSBrick")
		{
			commandToClient(%obj.client, 'centerPrint', "\c3No hammering in space!", 2);
			return;
		}
		else if (%col.isSBTemplate)
		{
			return;
		}
		
		parent::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal);
	}
	
	function WandImage::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal)
	{
		if(getWord(%col.getTransform(), 2) >= $spaceHeight && !%obj.client.isAdmin && $sbDisableWands && %col.getClassName() $= "fxDTSBrick")
		{
			commandToClient(%obj.client, 'centerPrint', "\c3No wanding in space!", 2);
			return;
		}
		else if (%col.isSBTemplate)
		{
			return;
		}
		
		parent::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal);
	}
	
	function serverCmdPlantBrick(%client)
	{
		if (!isObject(%client.player) || !isObject(%client.player.tempbrick))
			return;
		if(getWord(%client.player.tempbrick.getPosition(), 2) >= $spaceHeight && !%client.isAdmin && $sbRemindBuilding)
		{
			commandToClient(%client, 'centerPrint', "\c3You can't build in space!\n\c0Build a module on the ground!", 5);
			return;
		}
		parent::serverCmdPlantBrick(%client);
	}
	
	function getTrustLevel(%obj, %other, %a1, %a2)
	{
		//if either of these objects are SB Template bricks, limit the max trust level to 1
		//(you can't delete your own template bricks, nor can anyone else)
		if((%other.getClassName() !$= "fxDTSBrick" || %obj.getClassName() !$= "fxDTSBrick") && (%obj.isSBTemplate || %other.isSBTemplate))
		{
			%ret = Parent::getTrustLevel(%obj, %other, %a1, %a2);
			if(%ret >= 2)
				%ret = 1;
		}
		else
			%ret = Parent::getTrustLevel(%obj, %other, %a1, %a2);
		
		return %ret;
	}
	
	function fxDTSBrick::onPlant(%this)
	{
		parent::onPlant(%this);
		
		%client = %this.client;
		%slotFound = -1;
		for(%i = 0; %i < %this.getNumDownBricks(); %i++)
		{
			%brick = %this.getDownBrick(%i);
			if(isObject(%brick.slot))
			{
				if(%slotFound == -1)
				{
					%slotFound = %brick.slot;
				}else if (%slotFound != %brick.slot){
					//a slot was already found; that means this brick is on 2 or more slots
					//this is not allowed, so break the brick and send an error message
					%this.killBrick();
					commandToClient(%client, 'centerPrint', "Bricks can only be in one building area!", 3);
					return;
				}
			}
		}
		
		//repeat the check for upBricks
		for(%i = 0; %i < %this.getNumUpBricks(); %i++)
		{
			%brick = %this.getUpBrick(%i);
			if(isObject(%brick.slot))
			{
				if(%slotFound == -1)
				{
					%slotFound = %brick.slot;
				}else if (%slotFound != %brick.slot) {
					//a slot was already found; that means this brick is on 2 or more slots
					//this is not allowed, so break the brick and send an error message
					%this.killBrick();
					commandToClient(%client, 'centerPrint', "Bricks can only be in one building area!", 3);
					return;
				}
			}
		}
		
		if(%slotFound == -1)
		{
			//no slot was found; they're probably building on the ground, or somewhere they shouldn't
			//unless they're admin, kill their brick and send an error message
			if(!%client.isAdmin)
			{
				%this.killBrick();
				commandToClient(%client, 'centerPrint', "You can only build in the building areas!", 3);
				return;
			}else{
				//if it's not connected to anything, well...there's no point continuing our checks
				return;
			}
		}
		
		//okay, they're building in a slot; do they have permission to build on it?
		//this check should be taken care of by the normal getTrustLevel in the original function.
		
		//is this brick within the bounds of the MCSlotSO it's connected to?
		if(!%this.withinBounds(%slotFound.getCenter(), %slotFound.size))
		{
			%this.killBrick();
			commandToClient(%client, 'centerPrint', "You're building too far out of your building area!", 3);
			return;
		}
		
		//okay, they passed all checks! now, we can add this new brick to the VBL of the slot... (this also sets the brick's slot property to %slotFound)
		%slotFound.addBrick(%this);
	}
	
	function ServerCmdClearBricks(%client)
	{
		%mcf = $DefaultMiniGame.mcf;
		%slot = %mcf.findSlotByBLID(%client.bl_id);
		if (isObject(%slot))
		{
			%slot.clearBuiltBricks();
			commandToClient(%client, 'centerPrint', "\c6Cleared slot!", 3);
		}
	}
	
};
activatePackage(SpaceBuildRulesPackage);

//Used for calculating if a brick is within a slot's bounds (thus a player can place it).
function fxDTSBrick::withinBounds(%this, %center, %checkSize)
{
	%brickBox = %this.getWorldBox();
	%checkBoxMin = vectorSub(%center, vectorScale(%checkSize, 0.5));
	%checkBoxMax = vectorAdd(%center, vectorScale(%checkSize, 0.5));
	
	for(%i = 0; %i < 3; %i++)
	{
		if(getWord(%brickBox, %i) < getWord(%checkBoxMin, %i))
			return false;
		if(getWord(%brickBox, %i + 3) > getWord(%checkBoxMax, %i))
			return false;
	}
	
	return true;
}


//bonus commands!
function serverCmdUptime(%client)
{
	%time = getTimeString(mFloor(getSimTime() / 1000 / 60));
	messageClient(%client, "", "\c2This server has been running for \c0" @ %time @ "\c2 minutes.");
}

//easy save in case of emergency
function ServerCmdSaveSpaceBuild(%client, %name)
{
	if(!%client.isAdmin)
		return;

	if(%name $= "")
		%name = "sbsave";
	
	$DefaultMinigame.mcf.export("config/server/SpaceBuild/" @ %name @ ".sbmcf");
	$DefaultMinigame.station.export("config/server/SpaceBuild/" @ %name @ ".sbs");
}

function ServerCmdLoadSpaceBuild(%client, %name)
{
	if(!%client.isAdmin)
		return;
	
	if(%name $= "")
		%name = "sbsave";
	
	$DefaultMinigame.mcf.import("config/server/SpaceBuild/" @ %name @ ".sbmcf");
	$DefaultMinigame.station.import("config/server/SpaceBuild/" @ %name @ ".sbs");
}

function ServerCmdPurgeEmptySlots(%client)
{
	if(!%client.isAdmin)
		return;
	
	$DefaultMinigame.mcf.purgeEmptySlots();
	commandToClient(%client, 'centerPrint', "Deleted all empty slots.", 3);
}
