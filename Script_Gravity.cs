$gravityTickTime = 5;
$gravityDefaultScale = "2 2 3";
$gravityDefaultMod = 0.25;
$gravityActivateHeight = 350; //270

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
			if(getWord(%obj.getPosition(), 2) >= $gravityActivateHeight)
			{
				if(!%grav.isActivated)
				{
					%grav.activate();
					%grav.isActivated = 1;
				}
				
				%trans = VectorSub(getWords(%obj.getTransform(), 0, 2), getWord(%grav.scale, 0) * 0.5 SPC getWord(%grav.scale, 1) * -0.5 SPC getWord(%grav.scale, 2) * 0);
				%grav.setTransform(%trans);
				if(%grav.isActivated && isObject(%obj.client))
					commandtoclient(%obj.client, 'bottomPrint', "\c2Your current gravity: x" @ %grav.gravityMod @ ". (Z Pos: " @ mFloor(getWord(%obj.getPosition(), 2)) @ ")", 0.2);
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

package gravityPackage
{
	function Armor::onNewDatablock(%this, %obj) //onNewDatablock
	{
		parent::onNewDatablock(%this, %obj);
		%scale = getWords(%obj.getObjectBox(), 3, 5);
		createGravityField(%obj, %scale);
	}
};
activatePackage(gravityPackage);
