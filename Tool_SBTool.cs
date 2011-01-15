//This tool is mostly for debugging, it allows users to interface with the gamemode
//This will be done through events later

datablock ItemData(SBToolItem : wandItem)
{
	uiName = "SB Tool";
	doColorShift = true;
	colorShiftColor = "0.8 0 0.8 1";
	image = SBToolImage;
};

datablock ShapeBaseImageData(SBToolImage : wandImage)
{
	item = SBToolItem;
	doColorShift = true;
	colorShiftColor = SBToolItem.colorShiftColor;
};

function SBToolImage::onFire(%this, %obj, %slot)
{
	%types = ($TypeMasks::FxBrickAlwaysObjectType);
	%col = containerRaycast(%obj.getEyePoint(), VectorAdd(VectorScale(%obj.getEyeVector(), 8), %obj.getEyePoint()), %types);
	%col = getWord(%col, 0);
	
	if(isObject(%col))
		SBToolCollision(%obj, %col);
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
