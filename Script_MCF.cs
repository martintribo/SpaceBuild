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

function MCFacility::getSlotWorldPos(%obj, %slot)
{
	%pos = getWord(%obj.moduleSlotSize, 0) * getWord(%obj.MCFSize, 0) SPC getWord(%obj.moduleSlotSize, 1) * getWord(%obj.MCFSize, 1) SPC getWord(%obj.moduleSlotSize, 2); //max size
	%pos = vectorScale(%pos, 0.5); //half of max size 
	%pos = vectorSub(%obj.position, %pos); //subtract it from MCF position (get corner)
	
	%slotpos = getWord(%obj.moduleSlotSize, 0) * getWord(%slot, 0) SPC getWord(%obj.moduleSlotSize, 1) * getWord(%slot, 1) SPC getWord(%obj.moduleSlotSize, 2) * getWord(%slot, 2);
	%slotpos = vectorAdd(%slotpos, %pos); //this should be the top left corner of the slot
	
	return(%slotpos);
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
	
	//world coordinate format but without offset of MCF position
	//%slotpos = getWord(%obj.moduleSlotSize, 0) * getWord(%slot, 0) SPC getWord(%obj.moduleSlotSize, 1) * getWord(%slot, 1) SPC getWord(%obj.moduleSlotSize, 2) * getWord(%slot, 2);
	//%slotpos = vectorAdd(%slotpos, %obj.getTopLeftCorner()); //this should be the top left corner of the slot
	%slotpos = %obj.getSlotWorldPos(%slot);
	
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
	if (%sb.getDatablock().getId() == brick1x4x3SpaceHatchData.getId())
	{
		%box = %sb.getWorldBox();
		%pos = %sb.getPosition();
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
		%sb.setColor(%mod.numHatches);
		%sb.hatchId = %mod.numHatches;
		%mod.addHatch(%point, %sb.getAngleId());
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