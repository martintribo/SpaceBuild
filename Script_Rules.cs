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
		if(getWord(%col.getPosition(), 2) >= $spaceHeight && !%obj.client.isAdmin && $sbDisableWrenches && %col.getClassName() $= "fxDTSBrick")
		{
			commandToClient(%obj.client, 'centerPrint', "\c3No wrenching in space!", 2);
			return;
		}
		
		parent::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal);
	}
	
	function HammerImage::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal)
	{
		if(getWord(%col.getPosition(), 2) >= $spaceHeight && !%obj.client.isAdmin && $sbDisableHammers && %col.getClassName() $= "fxDTSBrick")
		{
			commandToClient(%obj.client, 'centerPrint', "\c3No hammering in space!", 2);
			return;
		}
		
		parent::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal);
	}
	
	function WandImage::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal)
	{
		if(getWord(%col.getPosition(), 2) >= $spaceHeight && !%obj.client.isAdmin && $sbDisableWands && %col.getClassName() $= "fxDTSBrick")
		{
			commandToClient(%obj.client, 'centerPrint', "\c3No wanding in space!", 2);
			return;
		}
		
		parent::onHitObject(%this, %obj, %a, %col, %fade, %pos, %normal);
	}
	
	//function serverCmdPlantBrick(%client)
	//{
	//	if(getWord(%client.player.tempbrick.getPosition(), 2) >= $spaceHeight && !%client.isAdmin && $sbRemindBuilding)
	//	{
	//		commandToClient(%client, 'centerPrint', "\c3You can't build in space!\n\c0Build a module on the ground!", 5);
	//		return;
	//	}
	//	parent::serverCmdPlantBrick(%client);
	//}
	
	//function getTrustLevel(%obj, %other, %a1, %a2)
	//{
	//	if(%obj.dataBlock != 0 && %obj.getDataBlock().getName() $= "SpaceShuttleVehicle")
	//		return 1;
		
	//	if(%other.dataBlock != 0 && %other.getDataBlock().getName() $= "SpaceShuttleVehicle")
	//		return 1;
		
	//	Parent::getTrustLevel(%obj, %other, %a1, %a2);
	//}
	
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
				}else{
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
				}else{
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
		if(!%this.withinBounds(%slotFound.getPosition(), %slotFound.size))
		{
			%this.killBrick();
			commandToClient(%client, 'centerPrint', "You're building too far out of your building area!", 3);
			return;
		}
		
		//okay, they passed all checks! now, we can add this new brick to the VBL of the slot... (this also sets the brick's slot property to %slotFound)
		%slotFound.addBrick(%this);
	}
	
	function fxDTSBrick::onRemove(%this)
	{
		//if this brick is in a slot, remove it from the slot's VBL
		if(isObject(%this.slot))
			%this.slot.removeBrick(%this);
		
		parent::onRemove(%this);
	}
	
};
activatePackage(SpaceBuildRulesPackage);

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
	%time = getTimeString(mFloor(getSimTime() / 1000));
	messageClient(%client, "", "\c2This server has been running for \c0" @ %time @ "\c2.");
}

//easy save
function ServerCmdSaveSpaceBuild(%client)
{
	if(!%client.isAdmin)
		return;
	
	if(!isObject($DebugStation))
		return;
	
	$DebugMCF.export("config/server/SpaceBuild/debugmcf.sbm");
	$DebugStation.export("config/server/SpaceBuild/debugstation.sbs");
}

function ServerCmdLoadSpaceBuild(%client)
{
	if(!%client.isAdmin)
		return;
	
	if(!isObject($DebugStation))
		setupSpace();
	
	$DebugMCF.import("config/server/SpaceBuild/debugmcf.sbm");
	$DebugStation.import("config/server/SpaceBuild/debugstation.sbs"); //also creates bricks (in moduleSO.import)
}
