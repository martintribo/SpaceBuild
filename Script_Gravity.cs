//This script should be replaced by Nitramtj's physical zone auto maker thing as soon as possible.
//The messaging commands will be replaced with Script_SBTick.cs.

$gravityTickTime = 5;
$gravityDefaultScale = "2 2 3";
$gravityDefaultMod = 0.25;
$spaceHeight = 360;

//$stationPos = ""; //define this manually ingame!

if(!isObject(gravityFieldGroup))
{
	new SimGroup(gravityFieldGroup);
}

function gravityTick()
{
	for(%i = 0; %i < gravityFieldGroup.getCount(); %i++)
	{
		%grav = gravityFieldGroup.getObject(%i);
		%obj = %grav.gravObject;
		if(!isObject(%obj))
		{
			%grav.delete();
		}else{
			if(getWord(%obj.getPosition(), 2) >= $spaceHeight)
			{
				if(!%grav.isActivated)
				{
					%grav.activate();
					%grav.isActivated = 1;
				}
				
				%trans = VectorSub(getWords(%obj.getTransform(), 0, 2), getWord(%grav.scale, 0) * 0.5 SPC getWord(%grav.scale, 1) * -0.5 SPC getWord(%grav.scale, 2) * 0);
				%grav.setTransform(%trans);
				
				if($stationPos !$= "")
				{
				//Calculate space station guage
				%stationPos = $stationPos; //convert to function at a later date
				%stationPos = vectorSub(getWords(%obj.getTransform(), 0, 2), %stationPos);
				%x = mFloor(getWord(%stationPos, 0));
				%y = mFloor(getWord(%stationPos, 1));
				%z = mFloor(getWord(%stationPos, 2));
				
				if(mAbs(%x) < 200)
					%xColor = "\c2";
				if(mAbs(%x) < 400 && mAbs(%x) > 200)
					%xColor = "\c3";
				if(mAbs(%x) > 400)
					%xColor = "\c0";
				//============================
				if(mAbs(%y) < 200)
					%yColor = "\c2";
				if(mAbs(%y) < 400 && mAbs(%y) > 200)
					%yColor = "\c3";
				if(mAbs(%y) > 400)
					%yColor = "\c0";
				//============================
				if(mAbs(%z) < 200)
					%zColor = "\c2";
				if(mAbs(%z) < 400 && mAbs(%z) > 200)
					%zColor = "\c3";
				if(mAbs(%z) > 400)
					%zColor = "\c0";
				
				if(%grav.isActivated && isObject(%obj.client))
					commandToClient(%obj.client, 'bottomPrint', "\c2Gravity: x" @ %grav.gravityMod @ ". Altitude: " @ mFloor(getWord(%obj.getTransform(), 2)) @ ". \c6Station: " @ %xColor @ %x SPC %yColor @ %y SPC %zColor @ %z @ ".", 5);
				}else{
					if(%grav.isActivated && isObject(%obj.client))
						commandToClient(%obj.client, 'bottomPrint', "\c2Gravity: x" @ %grav.gravityMod @ ". Altitude: " @ mFloor(getWord(%obj.getTransform(), 2)) @ ".", 5);
				}
			}else{
				if(%grav.isActivated)
				{
					%grav.deactivate();
					%grav.isActivated = 0;
				}
			}
		}
	}
	
	if(isEventPending($gravityTickSched))
		cancel($gravityTickSched);
	
	if(gravityFieldGroup.getCount() > 0)
		$gravityTickSched = schedule($gravityTickTime, 0, "gravityTick");
}

function createGravityField(%obj, %scale)
{
	if(!%scale)
		%scale = $gravityDefaultScale;
	
	if(isObject(%obj.gravField))
		%obj.gravField.delete();
	
	%grav = new PhysicalZone(){
		 position = "0 0 0";
		 rotation = "1 0 0 0";
		 scale = %scale;
		 velocityMod = "1";
		 gravityMod = $gravityDefaultMod;
		 extraDrag = "0";
		 isWater = "0";
		 waterViscosity = "40";
		 waterDensity = "1";
		 waterColor = "0.200000 0.600000 0.600000 0.300000";
		 appliedForce = "0 0 0";
		 polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
	};
	
	%grav.gravObject = %obj;
	%grav.deactivate();
	%grav.isActivated = 0;
	%obj.gravField = %grav;
	
	gravityFieldGroup.add(%grav);
	
	if(!isEventPending($gravityTickSched))
		gravityTick();
}

//A duplicate from Script_Lifesupport, which isn't execed yet. Remove later.
function Armor::isInSpace(%obj)
{
	if(getWord(%obj.getTransform(), 2) >= $spaceHeight)
		return(1);
	else
		return(0);
}

package gravityPackage
{
	function Armor::onNewDatablock(%this, %obj) //onNewDatablock
	{
		parent::onNewDatablock(%this, %obj);
		%scale = getWords(%obj.getObjectBox(), 3, 5);
		createGravityField(%obj, %scale);
	}
	//function Armor::onTrigger(%this, %obj, %trig, %val)
	//{
	//	if(%val && %trig == 3 && %obj.isInSpace())
	//	{
	//		%obj.setVelocity(vectorAdd(%obj.getVelocity(), "0 0 -1.5"));
	//	}
	//}
};
activatePackage(gravityPackage);
