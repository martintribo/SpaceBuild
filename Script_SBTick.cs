$SBTickSpeed = 100;
$SBTickAutoStart = true;

$playerBurnSpeed = 55;
$playerBurnDamage = 10;
$showStationLocator = 1;
$spaceHeight = 360;

if($SBTickAutoStart)
	schedule(1000, 0, "SBTick");

function SBTick()
{
	if(isEventPending($SBTickSched))
		cancel($SBTickSched);
	$SBTickSched = schedule($SBTickSpeed, 0, "SBTick");

	$DefaultMiniGame.evalQueue.runStatements();
	
	for(%i = 0; %i < clientGroup.getCount(); %i++)
	{
		%client = clientGroup.getObject(%i);
		if(isObject(%client.player))
		{
			if(%client.player.isInSpace())
			{
				//Should be player's current gravity.
				%grav = 0.0;
				%station = $DefaultMiniGame.station;
				
				if(vectorLen(%client.player.getVelocity()) > $playerBurnSpeed && !%client.player.getObjectMount())
					playerBurnEffects(%client.player);
				else if(%client.player.getMountedImage(2) == NameToID("shuttleFlameImage"))
					removeBurnEffects(%client.player);
				
				if(isObject(%station) && %station.getPosition() !$= "" && $showStationLocator)
				{
					%stationPos = %station.getPosition(); //$stationPos; //convert to function at a later date!
					%stationPos = vectorSub(getWords(%client.player.getTransform(), 0, 2), %stationPos);
					%rx = mFloor(getWord(%stationPos, 0));
					%ry = mFloor(getWord(%stationPos, 1));
					%rz = mFloor(getWord(%stationPos, 2));
					
					%rxPositive = %rx < 0 ? %rx * -1 : %rx;
					%ryPositive = %ry < 0 ? %ry * -1 : %ry;
					%rzPositive = %rz < 0 ? %rz * -1 : %rz;

					if(%rxPositive < 200)
						%xColor = "\c2";
					else if(%rxPositive < 400)
						%xColor = "\c3";
					else
						%xColor = "\c0";
					//============================
					if(%ryPositive < 200)
						%yColor = "\c2";
					else if(%ryPositive < 400)
						%yColor = "\c3";
					else
						%yColor = "\c0";
					//============================
					if(%rzPositive < 200)
						%zColor = "\c2";
					else if(%rzPositive < 400)
						%zColor = "\c3";
					else
						%zColor = "\c0";
					
					commandToClient(%client, 'bottomPrint', "\c2Gravity: x" @ %grav @ ". \c2Altitude: " @ mFloor(getWord(%client.player.getTransform(), 2)) @ ". \c2Station: " @ %xColor @ %rx SPC %yColor @ %ry SPC %zColor @ %rz @ ".", 3);
				}else{
					commandToClient(%client, 'bottomPrint', "\c2Gravity: x" @ %grav @ ". \c2Altitude: " @ mFloor(getWord(%client.player.getTransform(), 2)) @ ".", 3);
				}
			}
		}
	}
}

function Player::isInSpace(%obj)
{
	if(getWord(%obj.getTransform(), 2) >= $spaceHeight)
		return(1);
	else
		return(0);
}

function playerBurnEffects(%player)
{
	if(%player.getMountedImage(2) != NameToID("shuttleFlameImage"))
	{
		%player.mountImage(shuttleFlameImage, 2);
		%player.burnPlayer();
	}
	
	%player.damage("", "", $playerBurnDamage);
}

function removeBurnEffects(%player)
{
	%player.burnPlayer(0.1);
	%player.unMountImage(2);
}
