//$spaceHeight - this is when you enter 'space' (defined in Script_Gravity)
$defaultOxygen = 100;					//how much air you have when you spawn
$oxygenChangeRate = 1;				//how much air you gain/lose per tick
$lifeSupportTickTime = 500;		//how much time between lifesupport ticks (in ms)

package lifeSupportPackage
{
	function Armor::onNewDatablock(%this, %obj) //onNewDatablock
	{
		parent::onNewDatablock(%this, %obj);
		%obj.oxygenLevel = $defaultOxygen;
	}
};
activatePackage(gravityPackage);

function Armor::isInSpace(%obj)
{
	if(getWord(%obj.getTransform(), 2) >= $spaceHeight)
		return(1);
	else
		return(0);
}

function Armor::setOxygenLevel(%obj, %amount)
{
	%obj.oxygenLevel = %amount;
	//update oxygen ui here - just bottomprint as a placeholder
	commandToClient(%obj.client, 'bottomPrint', "Your oxygen: " @ %amount, 10);
}

function Armor::getOxygenLevel(%obj)
{
	return(%obj.oxygenLevel);
}

function Armor::inOxygenEnvironment(%obj)
{
	//to do
}

function lifeSupportTick()
{
	for(%i = 0; %i < clientGroup.getCount(); %i++)
	{
		%client = clientGroup.getObject(%i);
		%isSafe = %client.player.inOxygenEnvironment();
		%oxy = %client.player.getOxygenLevel();
		
		if(%isSafe && %client.player.oxygenLevel < $defaultOxygen)
		{
			%client.player.setOxygenLevel(%oxy + $oxygenChangeRate);
		}
		if(%client.player.isInSpace())
		{
			if(!%isSafe)
				%client.player.setOxygenLevel(%oxy - $oxygenChangeRate);
		}
	}
	
	if(isEventPending($lifeSupportSched))
		cancel($lifeSupportSched);
	
	$lifeSupportSched = schedule($lifeSupportTickTime, 0, "lifeSupportTick");
}
