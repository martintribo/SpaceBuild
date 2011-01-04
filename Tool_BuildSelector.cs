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
	
	//This will find all bricks connected to %col.
	%bf = new ScriptObject()
	{
		class = "BrickFinder";
	};
	
	%bf.setOnSelectCommand(%client @ ".onBuildSelectorBrickFound(%sb);");
	%bf.setFinishCommand(%client @ ".onBuildSelectorDone(" @ %bf @ ");");
	%bf.search(%col, "chain", "all", "", 1);
	
	//From here on out, we wait for the brickFinder to finish up. It'll call GameConnection::onBuildSelectorDone(%client, %bf) when it's finished, so continue there.
	//Every time the brickFinder finds a brick, it calls GameConnection::onBuildSelectorBrickFound(%client, %sb), which keeps the adding-to-vbList lag-free.
}

function GameConnection::onBuildSelectorBrickFound(%client, %sb)
{
	%vbList = $moduleList[%client.bl_id, $moduleListCount[%client.bl_id] - 1];
	
	//Add the found brick to the vbList.
	%vbList.addRealBrick(%sb);
	//Then, delete the brick.
	//%sb.delete();
}

function GameConnection::onBuildSelectorDone(%client, %bf)
{
	%vbList = $moduleList[%client.bl_id, $moduleListCount[%client.bl_id] - 1];
	
	//Delete the real bricks that the brickFinder found
	%bf.schedule(10, "delete");
	
	//the vbList is populated - now, create a cargo player for a physical representation of it
	%cargo = new Player()
	{
		datablock = playerCargo;
		virtualBrickList = %vbList;
		owner = %client;
	};
	%cargo.setTransform(%vbList.getCenter());
	%cargo.setScale("1 1 1"); //make this proportional to build size later on (?)
	%vbList.cargoObject = %cargo;
	
	commandToClient(%client, 'bottomPrint', "Build packaged; cargo created.", 5);
}

function expandCargoPlayer(%obj, %client)
{
	%vbList = %obj.virtualBrickList;
	
	%vbList.shiftBricks(vectorSub(%client.player.getPosition(), %vbList.getCenter()));
	%vbList.createBricks();
	
	%obj.removeBody(); //delete the cargo in an interesting way (yay particles!)
	%vbList.delete(); //the vbList's job is done - it can die now :D
	
	if(isObject(%client))
		commandToClient(%client, 'bottomPrint', "Build expanded!", 4);
}

function serverCmdExpandCargo(%client)
{
	%mouseVec = %client.player.getEyeVector();
	%cameraPoint = %client.player.getEyePoint();
	%selectRange = 25;
	%mouseScaled = VectorScale(%mouseVec, %selectRange);
	%rangeEnd = VectorAdd(%cameraPoint, %mouseScaled);
	%searchMasks = $TypeMasks::PlayerObjectType;
	%target = ContainerRayCast(%cameraPoint, %rangeEnd, %searchMasks, %client.player);
	%target = getWord(%target, 0);
	
	if(!isObject(%target))
		return;
	if(!isObject(%target.virtualBrickList))
		return;
	
	expandCargoPlayer(%target, %client);
}
