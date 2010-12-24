//Tool to create a virtualBrickList for a build.
//A 'build' meaning all bricks connected to this brick (possibly within a certain range).
//Relies heavily on virtualBrickList functions.

//vbList storage example:
//$moduleList[158, 0] = aloshivblist;
//$moduleListCount[158] = 1;

function buildSelectorImage::onFire(%this, %obj)
{
	%types = ($TypeMasks::FxBrickAlwaysObjectType);
	%col = containerRaycast(%obj.getEyePoint(), VectorAdd(VectorScale(%obj.getEyeVector(), 8), %obj.getEyePoint()), %types);
	%col = getWord(%col, 0);
	buildSelectorProjectile::onCollision(%this, %obj, %col);
}

datablock ItemData(buildSelectorItem : wandItem)
{
	uiName = "Build Selector";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";
	image = buildSelectorImage;
};

datablock ShapeBaseImageData(buildSelectorImage : wandImage)
{
	item = buildSelectorItem;
	doColorShift = True;
	colorShiftColor = "0.471 0.471 0.471 1.000";
};

function buildSelectorProjectile::onCollision(%this, %obj, %col)
{
	%client = %obj.client;
	
	if(!isObject(%col) || %col.getClassName() !$= "fxDTSBrick")
	{
		commandToClient(%client, 'bottomPrint', "You must select a brick!", 3);
		return;
	}
	
	if(getTrustLevel(%col, %client) < 2)
	{
		commandToClient(%client, 'bottomPrint', "You must have full trust to select a build!", 3);
		return;
	}
	
	//All checks are go - create the virtualBrickList...
	%vbList = new ScriptObject()
	{
		class = "virtualBrickList";
	};
	%vbList.addRealBrick(%col); //add the first selected brick
	
	//store the virtualBrickList in an array...
	if($moduleListCount[%client.bl_id] $= "")
		$moduleListCount[%client.bl_id] = 0;
	
	$moduleList[%client.bl_id, $moduleListCount[%client.bl_id]] = %vbList;
	$moduleListCount[%client.bl_id]++;
	
	
	
	//This will find all bricks connected to %col and add them to the list.
	//------Should this be a seperate function?
	%bf = new ScriptObject()
	{
		class = "BrickFinder";
	};
	
	//%bf.setOnSelectCommand(%client @ ".onBuildSelected(%sb);");
	%bf.setFinishCommand(%client @ ".onBuildSelectorDone(" @ %bf @ ");");
	%bf.search(%col, "chain", "all", "", 1);
	
	//From here on out, we wait for the brickFinder to finish up. It'll call GameConnection::onBuildSelectorDone(%client, %bf); when it's finished, so continue there.
}

function GameConnection::onBuildSelectorDone(%client, %bf)
{
	%vbList = $moduleList[%client.bl_id, $moduleListCount[%client.bl_id] - 1];
	
	
	//*****THIS SHOULD BE ASYNC******
	//add the bricks from the brickFinder to the vbList
	for(%i = 0; %i < %bf.foundBricks.getCount(); %i++)
	{
		%vbList.addRealBrick(%bf.foundBricks.getObject(%i));
	}
	//*******************************
	
	commandToClient(%client, 'bottomPrint', "Build selected - vbList created and added to. (num " @ $moduleListCount[%client.bl_id] - 1 @ ", ID: " @ %vbList @ ")", 8);
	%bf.schedule(10, "delete");
}

function serverCmdCopyBuild(%client, %num)
{
	if(!isObject($moduleList[%client.bl_id, %num]))
	{
		commandToClient(%client, 'bottomPrint', "Invalid build number.", 4);
		return;
	}
	
	%vbList = $moduleList[%client.bl_id, %num];
	
	commandToClient(%client, 'bottomPrint', "Copying bricks...", 5);
	
	%vbList.shiftBricks(vectorSub(%client.player.getPosition(), %vbList.getCenter()));
	%vbList.createBricks();
	
	commandToClient(%client, 'bottomPrint', "\c2Copy successful!", 5);
}
