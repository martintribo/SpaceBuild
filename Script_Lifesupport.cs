//$spaceHeight - this is when you enter 'space' (defined in Script_Gravity)
$defaultOxygen = 100;					//how much air you have when you spawn
$oxygenGainRate = 2;				//how much air you gain per tick
$oxygenLossRate = 1;				//how much air you lose per tick

package lifeSupportPackage
{
	function Armor::onNewDatablock(%this, %obj) //onNewDatablock
	{
		parent::onNewDatablock(%this, %obj);
		%obj.oxygenLevel = $defaultOxygen;
	}
};
activatePackage(gravityPackage);

function Player::isInSpace(%obj)
{
	if(getWord(%obj.getTransform(), 2) >= $spaceHeight)
		return(1);
	else
		return(0);
}

function Player::setOxygenLevel(%obj, %amount)
{
	%obj.oxygenLevel = %amount;
	//update oxygen ui here - just bottomprint as a placeholder
	commandToClient(%obj.client, 'bottomPrint', "Your oxygen: " @ %amount, 10);
}

function Player::getOxygenLevel(%obj)
{
	return(%obj.oxygenLevel);
}

function Player::inOxygenEnvironment(%obj)
{
	//to do
}

//Should be called by SBTick or merged with it
function lifeSupportTick()
{
	for(%i = 0; %i < clientGroup.getCount(); %i++)
	{
		%client = clientGroup.getObject(%i);
		%isSafe = %client.player.inOxygenEnvironment();
		%oxy = %client.player.getOxygenLevel();
		
		if(%isSafe && %client.player.oxygenLevel < $defaultOxygen)
		{
			%client.player.setOxygenLevel(%oxy + $oxygenGainRate);
		}
		if(%client.player.isInSpace())
		{
			if(!%isSafe)
				%client.player.setOxygenLevel(%oxy - $oxygenLossRate);
		}
	}
}
