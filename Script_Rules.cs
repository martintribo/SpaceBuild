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
	
	function serverCmdPlantBrick(%client)
	{
		if(getWord(%client.player.tempbrick.getPosition(), 2) >= $spaceHeight && !%client.isAdmin && $sbRemindBuilding)
		{
			commandToClient(%client, 'centerPrint', "\c3You can't build in space!\n\c0Build a module on the ground!", 5);
			return;
		}
		parent::serverCmdPlantBrick(%client);
	}
};
activatePackage(SpaceBuildRulesPackage);




//bonus commands!
function serverCmdUptime(%client)
{
	%time = getTimeString(mFloor(getSimTime() / 1000));
	messageClient(%client, "", "\c2This server has been running for \c0" @ %time @ "\c2.");
}
