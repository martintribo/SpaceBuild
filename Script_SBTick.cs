$SBTickSpeed = 100;
$SBTickAutoStart = 1;

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
	
	for(%i = 0; %i < clientGroup.getCount(); %i++)
	{
		%client = clientGroup.getObject(%i);
		if(isObject(%client.player))
		{
			if(%client.player.isInSpace())
			{
				//Should be player's current gravity.
				%grav = 0.0;
				
				if(vectorLen(%client.player.getVelocity()) > $playerBurnSpeed && !%client.player.getObjectMount())
					playerBurnEffects(%client.player);
				else if(%client.player.getMountedImage(2) == NameToID("shuttleFlameImage"))
					removeBurnEffects(%client.player);
				
				if(isObject($DebugStation) && $DebugStation.getPosition() !$= "" && $showStationLocator)
				{
					%stationPos = $DebugStation.getPosition(); //$stationPos; //convert to function at a later date!
					%stationPos = vectorSub(getWords(%client.player.getTransform(), 0, 2), %stationPos);
					%rx = mFloor(getWord(%stationPos, 0));
					%ry = mFloor(getWord(%stationPos, 1));
					%rz = mFloor(getWord(%stationPos, 2));
					
					if(%rx < 200)
						%xColor = "\c2";
					if(%rx < 400 && %x > 200)
						%xColor = "\c3";
					if(%rx > 400)
						%xColor = "\c0";
					//============================
					if(%ry < 200)
						%yColor = "\c2";
					if(%ry < 400 && %y > 200)
						%yColor = "\c3";
					if(%ry > 400)
						%yColor = "\c0";
					//============================
					if(%rz < 200)
						%zColor = "\c2";
					if(%rz < 400 && %z > 200)
						%zColor = "\c3";
					if(%rz > 400)
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
