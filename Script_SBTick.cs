$SBTickSpeed = 400;
$SBTickAutoStart = 1;

$playerBurnSpeed = 40;
$playerBurnDamage = 15;
$showStationLocator = 0;

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
				%grav = 0.25;
				
				if(vectorLen(%client.player.getVelocity()) > $playerBurnSpeed && !%client.player.getObjectMount())
					playerBurnEffects(%client.player);
				else if(%client.player.getMountedImage(2) == NameToID("shuttleFlameImage"))
					removeBurnEffects(%client.player);
				
				if($stationPos !$= "" && $showStationLocator)
				{
					%stationPos = $stationPos; //convert to function at a later date!
					%stationPos = vectorSub(getWords(%client.player.getTransform(), 0, 2), %stationPos);
					%rx = mFloor(getWord(%stationPos, 0));
					%ry = mFloor(getWord(%stationPos, 1));
					%rz = mFloor(getWord(%stationPos, 2));
					
					%x = mAbs(%x);
					%y = mAbs(%y);
					%z = mAbs(%z);
					
					if(%x < 200)
						%xColor = "\c2";
					if(%x < 400 && %x > 200)
						%xColor = "\c3";
					if(%x > 400)
						%xColor = "\c0";
					//============================
					if(%y < 200)
						%yColor = "\c2";
					if(%y < 400 && %y > 200)
						%yColor = "\c3";
					if(%y > 400)
						%yColor = "\c0";
					//============================
					if(%z < 200)
						%zColor = "\c2";
					if(%z < 400 && %z > 200)
						%zColor = "\c3";
					if(%z > 400)
						%zColor = "\c0";
					
					commandToClient(%client, 'bottomPrint', "\c2Gravity: x" @ %grav @ ". Altitude: " @ %rz @ ". \c6Station: " @ %xColor @ %rx SPC %yColor @ %ry SPC %zColor @ %rz @ ".", 5);
				}else{
					commandToClient(%client, 'bottomPrint', "\c2Gravity: x" @ %grav @ ". Altitude: " @ mFloor(getWord(%client.player.getTransform(), 2)) @ ".", 5);
				}
			}
		}
	}
}

//Copied from Script_LifeSupport.cs, but that isn't execed yet, so put it here for now (as a duplicate).
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
