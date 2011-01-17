//Module Construction Facility
//This needs to be able to store modules

function MCFacility::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.queue = new SimSet(); //assuming objects keep order inserted in, might be wrong
	
	//set up module slot size, bounds of MCF, MCF position, default module slot vblist to load
	%obj.moduleSlotSize = "32 32 32"; //size that each module slot takes up
	%obj.MCFSize = "6 6"; //how big we get before going to the next floor
	%obj.position = "0 0 0"; //where the center of the MCF is
	%obj.moduleSlotLoad = ""; //name of the vbList file to load
}

function MCFacility::setModuleSlotSize(%obj, %size)
{
	%obj.moduleSlotSize = %size;
}

function MCFacility::setMCFSize(%obj, %size)
{
	%obj.MCFSize = %size;
}

function MCFacility::setPosition(%obj, %pos)
{
	%obj.position = %pos;
}

//will get the center of the given module slot
function MCFacility::getSlotWorldCenter(%obj, %slot)
{
	%pos = getWord(%obj.moduleSlotSize, 0) * getWord(%obj.MCFSize, 0) SPC getWord(%obj.moduleSlotSize, 1) * getWord(%obj.MCFSize, 1) SPC getWord(%obj.moduleSlotSize, 2); //max size
	%pos = vectorScale(%pos, 0.5); //half of max size
	%pos = vectorSub(%obj.position, %pos); //get corner
	
	
	%slotpos = getWord(%obj.moduleSlotSize, 0) * getWord(%slot, 0) SPC getWord(%obj.moduleSlotSize, 1) * getWord(%slot, 1) SPC getWord(%obj.moduleSlotSize, 2) * getWord(%slot, 2);
	%slotpos = vectorAdd(%slotpos, %pos); //this should be the top left corner of the slot
	
	return(%slotpos);
}

//will give the bottom of the given module slot
function MCFacility::getSlotWorldBottom(%obj, %slot)
{
	%pos = %obj.getSlotWorldCenter(%slot);
	%pos = vectorSub(%pos, "0 0 " @ getWord(%obj.moduleSlotSize, 2) / 2);
}

//generate a module slot, owned by %client (optional)
//this is pretty much entirely psuedo-code but with correct syntax
function MCFacility::generateModuleSlot(%obj, %slot, %client)
{
	%vbList = new ScriptObject()
	{
		class = virtualBrickList;
	};
	%vbList.loadBLSFile("config/" @ %obj.moduleSlotLoad @ ".bls");
	
	%slotpos = %obj.getSlotWorldBottom(%slot);
	%vbList.shiftBricks(vectorSub(%slotpos, %vbList.getBottomFace()));
	
	%vbList.createBricks(%client);
	
	%vbList.delete();
}

function MCFacility::buildNextFloor(%obj)
{

}

function MCFacility::deleteModuleSlot(%obj, %slot)
{
	
}

function MCFacility::firstFreeSlot(%obj)
{
	
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