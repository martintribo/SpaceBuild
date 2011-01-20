//Module Construction Facility
//This script object contains build generation scripts and module scanning scripts.

//DEBUG COMMAND (also untested)
function serverCmdSetUpMCF(%client)
{
	if(!%client.isAdmin)
		return;
	
	if(isObject($DebugMCF))
		$DebugMCF.delete();
	
	%obj = new ScriptObject()
	{
		class = "MCFacility";
	};
	
	$DebugMCF = %obj;
	
	%slotSizeX = getWord(%obj.moduleSlotSize, 0);
	%slotSizeY = getWord(%obj.moduleSlotSize, 1);
	%slotSizeZ = getWord(%obj.moduleSlotSize, 2);
	%MCFSizeX = getWord(%obj.MCFSize, 0);
	%MCFSizeY = getWord(%obj.MCFSize, 1);
	
	%pos = getWords(%client.player.getTransform(), 0, 2);
	//%pos = vectorAdd(%pos, "0 0 " @ (getWord(%obj.moduleSlotSize, 2) * 0.5) + 0.5);
	%pos = vectorAdd(%pos, (0.5 * %MCFSizeX * %slotSizeX) SPC (0.5 * %MCFSizeY * %slotSizeY) SPC 0);
	
	%obj.setPosition(%pos);
	
	commandToClient(%client, 'centerPrint', "Set up as \c3$DebugMCF\c0.", 4);
}

function MCFacility::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.queue = new SimSet(); //assuming objects keep order inserted in, might be wrong
	
	//set up module creation parameters
	%obj.moduleSlotSize = VectorScale("64 64 40", 0.5); //size that each module slot takes up
	%obj.MCFSize = "6 6"; //how big we get before going to the next floor, should be integers
	%obj.position = "0 0 0"; //where the center of the MCF is, should be modified with setPosition
	%obj.moduleSlotLoad = "test"; //name of the vbList .bls file to load (in the config/ directory)
	
	//and some default variables, that shouldn't be changed
	%obj.highestfloor = 0; //handled internally
}

//if %convert is true, then it'll autoconvert your units to brick units instead of world units
function MCFacility::setModuleSlotSize(%obj, %size, %dontconvert)
{
	if(!%dontconvert)
		%size = vectorScale(%size, 0.5);
	
	%obj.moduleSlotSize = %size;
	
	//possibly add some code to load the VBL and auto-correct sizing information? wouldn't be hard...later, maybe
}

function MCFacility::setMCFSize(%obj, %size)
{
	%obj.MCFSize = %size;
}

//usually you need to up this position's Z by 1/2 of moduleSlotSize's Z, or it's in the ground, if you set it at a player location
function MCFacility::setPosition(%obj, %pos)
{
	%obj.position = %pos;
}

//will get the center of the given module slot - it works!
function MCFacility::getSlotWorldCenter(%obj, %slot)
{
	%slotSizeX = getWord(%obj.moduleSlotSize, 0);
	%slotSizeY = getWord(%obj.moduleSlotSize, 1);
	%slotSizeZ = getWord(%obj.moduleSlotSize, 2);
	%MCFSizeX = getWord(%obj.MCFSize, 0);
	%MCFSizeY = getWord(%obj.MCFSize, 1);
	
	//get corner
	%pos = vectorSub(%obj.position, (0.5 * %MCFSizeX * %slotSizeX) SPC (0.5 * %MCFSizeY * %slotSizeY) SPC 0);
	
	//now find the correct x/y for module
	%pos = vectorAdd(%pos, (getWord(%slot, 0) * %slotSizeX - (0.5 * %slotSizeX)) SPC (getWord(%slot, 1) * %slotSizeY - (0.5 * %slotSizeY)));
	
	//find correct Z
	%pos = getWords(%pos, 0, 1) SPC (getWord(%obj.position, 2) + (getWord(%slot, 2) * %slotSizeZ) + (%slotSizeZ * 0.5));
	
	return(%pos);
}

//will give the bottom of the given module slot - it works!
function MCFacility::getSlotWorldBottom(%obj, %slot)
{
	%pos = %obj.getSlotWorldCenter(%slot);
	%pos = vectorSub(%pos, "0 0 " @ getWord(%obj.moduleSlotSize, 2) / 2);
}

//generate a module slot, owned by %client (optional) - offset is wrong because of vbl shifting!
function MCFacility::generateModuleSlot(%obj, %slot, %client)
{
	%vbList = new ScriptObject()
	{
		class = virtualBrickList;
	};
	%vbList.loadBLSFile("config/" @ %obj.moduleSlotLoad @ ".bls");
	
	%slotpos = %obj.getSlotWorldBottom(%slot);
	
	echo("Slot pos: " @ %slotpos);
	//%vbList.realign("down\t" @ %slotpos);
	
	%shift = vectorSub(%slotpos, getWords(%vbList.getCenter(), 0, 1) SPC %vbList.getBottomFace());
	
	%vbList.shiftBricks(%shift);
	
	%vbList.createBricks(%client);
	%vbList.delete();
	
	if(getWord(%slot, 2) > %obj.highestfloor)
		%obj.highestfloor = getWord(%slot, 2);
}

function MCFacility::deleteModuleSlot(%obj, %slot)
{
	
}

function MCFacility::slotOccupied(%obj, %slot)
{
	
}

function MCFacility::firstFreeSlot(%obj)
{
	for(%z = 0; %z < %obj.highestfloor; %z++)
	{
		for(%x = 0; %x < getWord(%obj.MCFSize, 0); %x++)
		{
			for(%y = 0; %y < getWord(%obj.MCFSize, 1); %y++)
			{
				if(!%obj.slotOccupied(%x SPC %y SPC %z))
					return %x SPC %y SPC %z;
			}
		}
	}
}



function MCFacility::scanBuild(%obj, %brick)
{
	//need to just initialize the scan here
	%mod = newModuleSO();
	%bf = new ScriptObject()
	{
		class = "BrickFinder";
	};
	%bf.setOnSelectCommand(%obj @ ".onFoundBrick(%sb, " @ %mod @ ");");
	%bf.setFinishCommand(%obj @ ".onFinishedFinding(" @ %mod @ ", " @ %bf @ ");");
	%bf.search(%brick, "chain", "all", "spaceSupport", 0);
}

function MCFacility::onFoundBrick(%obj, %sb, %mod)
{
	echo("%sb = " @ %sb);
	if (%sb.isHatch())
	{
		%box = %sb.getWorldBox();
		%pos = %sb.getPosition();
		if (%sb.isHorizontalHatch())
		{
			switch (%sb.getAngleId())
			{
				case 0:
					%point = getWord(%pos, 0) SPC getWord(%box, 4) SPC getWord(%pos, 2);
				case 1:
					%point = getWord(%box, 3) SPC getWords(%pos, 1, 2);
				case 2:
					%point = getWord(%pos, 0) SPC getWord(%box, 1) SPC getWord(%pos, 2);
				case 3:
					%point = getWord(%box, 0) SPC getWords(%pos, 1, 2);
			}
			%dir = %sb.getAngleId();
		}
		else
		{
			if (%sb.isUpHatch())
			{
				%point = getWords(%pos, 0, 1) SPC getWord(%box, 5);
				%dir = 4; //up
			}
			else
			{
				%point = getWords(%pos, 0, 1) SPC getWord(%box, 2);
				%dir = 5; //down
			}
			
		}
		
		%sb.setColor(%mod.numHatches);
		%sb.hatchId = %mod.numHatches;
		%mod.addHatch(%point, %dir);
	}
	%mod.addBrick(%sb);
}

function MCFacility::onFinishedFinding(%obj, %mod, %bf)
{
	//delete the Brick Finder and make a new module with that vbl
	echo("onfinishedfinding" SPC %mod SPC %bf);
	%obj.addToQueue(%mod);
	%bf.delete();
}

function MCFacility::addToQueue(%obj, %mod)
{
	echo("add to queue" SPC %mod);
	%obj.queue.add(%mod);
}

function MCFacility::peekModule(%obj, %i)
{
	if (%i $= "")
		%i = 0;
	%mod = 0;
	if (%i < %obj.queue.getCount())
		%mod = %obj.queue.getObject(%i);
	return %mod;
}

function MCFacility::popModule(%obj)
{
	%mod = false;
	if (%obj.queue.getCount() > 0)
	{
		%mod = %obj.queue.getObject(0);
		%obj.queue.remove(%mod);
	}
	
	return %mod;
}

function MCFacility::debugAttach(%obj)
{
	%obj.queue.getObject(1).attachTo(%obj.queue.getObject(0), "hatch1", "hatch0");
}


//so we can save hatchbricks
addCustSave("SPACEHATCH");
function virtualBrickList::cs_addReal_SPACEHATCH(%obj, %num, %brick)
{
	if (%brick.hatchId !$= "") %obj.virBricks[%num, "SPACEHATCH"] = %brick.hatchId;
	else %obj.virBricks[%num, "SPACEHATCH"] = "";
}

function virtualBrickList::cs_create_SPACEHATCH(%obj, %num, %brick)
{
	if (%obj.virBricks[%num, "SPACEHATCH"] !$= "")
		%brick.hatchId = %obj.virBricks[%num, "SPACEHATCH"];
}

function virtualBrickList::cs_save_SPACEHATCH(%obj, %num, %file)
{
	if (%brick.hatchId !$= "")
		%file.writeLine("+-SPACEHATCH" SPC %obj.virBricks[%num, "SPACEHATCH"]);
}

function virtualBrickList::cs_load_SPACEHATCH(%obj, %num, %addData, %addInfo, %addArgs, %line)
{
	%obj.virBricks[%num, "SPACEHATCH"] = %addInfo;
}

function bfSpaceSupport(%brick)
{
	if (%brick.getName() $= "_spaceSupport")
		return 1;
	return 0;
}

addBFType("spaceSupport", "bfSpaceSupport");