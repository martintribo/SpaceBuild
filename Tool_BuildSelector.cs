//Tool to create a virtualBrickList for a build.
//A 'build' meaning all bricks connected to this brick (possibly within a certain range).
//Relies heavily on virtualBrickList functions.

//vbList storage example:
//$moduleList[158, 0] = aloshivblist;
//$moduleListCount[158] = 1;

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
	
	if(%col.getClassName() !$= "fxDTSBrick")
	{
		commandToClient(%client, 'bottomPrint', "You must select a brick!", 3);
		return;
	}
	
	if(getTrust(%col, %client) <= 2) //getTrust might be the wrong function
	{
		commandToClient(%client, 'bottomPrint', "You must have full trust to select a build!", 3);
		return;
	}
	
	//All checks are go - create the virtualBrickList and add the bricks!
	%vbList = new ScriptObject()
	{
		class = "virtualBrickList";
	};
	
	//store it in an array
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
	%bf.setFinishCommand(%client @ ".onBuildSelectorDone(" @ %client @ ", " @ %bf @ ");");
	%bf.search(%col, "chain", "add all", "", 1);
	
	//From here on out, we wait for the brickFinder to finish up. It'll call GameConnection::onBuildSelectorDone(%client, %bf); when it's finished, so continue there.
}

function GameConnection::onBuildSelectorDone(%client, %bf)
{
	commandToClient(%client, 'bottomPrint', "Build selected - vbList created. (num " @ $moduleListCount[%client.bl_id] @ ", " @ %vbList @ ")");
}
