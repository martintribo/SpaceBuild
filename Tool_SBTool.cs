//This tool is mostly for debugging, it allows users to interface with the gamemode
//This will be done through events later

datablock ItemData(SBToolItem : wandItem)
{
	uiName = "SB Tool";
	doColorShift = true;
	colorShiftColor = "1 0 1 1";
	image = SBToolImage;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(SBToolImage)
{
   // Basic Item properties
   shapeFile = "base/data/shapes/wand.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0; //"0.7 1.2 -0.5";
   rotation = eulerToMatrix( "0 0 10" );

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = SBToolItem;
   ammo = " ";
   projectile = SBToolProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   doColorShift = true;
   colorShiftColor = SBToolItem.colorShiftColor;

   //casing = " ";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]       = "Ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Reload";
	stateTimeoutValue[2]            = 0.05;
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
	stateSound[2]					= SBToolFireSound;

	stateName[3]			= "Reload";
	stateSequence[3]                = "Reload";
	stateAllowImageChange[3]        = false;
	stateTimeoutValue[3]            = 0.5;
	stateWaitForTimeout[3]		= true;
	stateTransitionOnTimeout[3]     = "Check";

	stateName[4]			= "Check";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.2;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	//stateSequence[5]                = "Reload";
	stateScript[5]                  = "onStopFire";


};

function SBToolImage::onFire(%this, %obj, %slot)
{
	%mouseVec = %obj.getEyeVector();
	%cameraPoint = %obj.getEyePoint();
	%selectRange = 5;
	%mouseScaled = VectorScale(%mouseVec, %selectRange);
	%rangeEnd = VectorAdd(%cameraPoint, %mouseScaled);
	%searchMasks = $TypeMasks::FxBrickAlwaysObjectType;
	%target = ContainerRayCast(%cameraPoint, %rangeEnd, %searchMasks, %client.player);
	%col = getWord(%target, 0);
	
	if(isObject(%target))
	{
		SBToolCollision(%obj, %col);
	}
	
}

function SBToolCollision(%obj, %col)
{
	%client = %obj.client;
	
	%mcf = $DebugMCF;
	
	if(!isObject(%mcf))
	{
		commandToClient(%client, 'bottomPrint', "No MCF object found - fix with \c3/SetupSpace\c0.", 5);
		return;
	}
	if(!isObject(%col) || %col.getClassName() !$= "fxDTSBrick")
	{
		commandToClient(%client, 'bottomPrint', "You must hit a brick.", 4);
		return;
	}
	if(!%client.isAdmin)
	{
		commandToClient(%client, 'bottomPrint', "You must be an admin to use this tool.", 4);
		return;
	}
	
	//if(getTrustLevel(%col, %client) < 1)
	//{
	//	commandToClient(%client, 'bottomPrint', "You must have build trust to select a build!", 3);
	//	return;
	//}
	
	if (%col.getDatablock().getId() != brick1x4x3SpaceHatchData.getId())
	{
		%mcf.scanBuild(%col);
		commandToClient(%client, 'bottomPrint', "Scanning your build. \c3Hit a hatch brick to attach your module.", 5);
	}
	else
	{
		%mod = %mcf.popModule();
		if (isObject(%mod))
		{
			commandToClient(%client, 'bottomPrint', "Attaching your module...", 2);
			%mod.attachTo(0, %col.module, %col.hatchId);
			commandToClient(%client, 'bottomPrint', "Your module was attached!", 4);
		}
	}
}

function ServerCmdSBTool(%client)
{
	if (isObject(%client.player))
	{
		%client.player.updateArm(SBToolImage);
		%client.player.mountImage(SBToolImage, 0);
	}
}

function ServerCmdSetupSpace(%client)
{
	if (%client.isAdmin)
	{
		$DebugMCF = new ScriptObject()
		{
			class = "MCFacility";
		};
	}
}

function ServerCmdFirstModule(%client, %x, %y, %z)
{
	if (%client.isAdmin)
	{
		$DebugStation = new ScriptObject()
		{
			class = "StationSO";
		};
		%mod = $DebugMCF.popModule();
		%mod.state = "Deployed"; //we're manually setting this up
		%mod.vbl.shiftBricks(%x SPC %y SPC %z); //going to change this to somethign better, but good for testing
		%mod.vbl.createBricks();
		$DebugStation.addModule(%mod);
		$stationPos = $DebugStation.getPosition();
	}
}
