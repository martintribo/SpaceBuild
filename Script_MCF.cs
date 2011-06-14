//Module Construction Facility
//This script object contains build generation scripts and module scanning scripts.

function MCFacility::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.queue = new SimSet(); //assuming objects keep order inserted in, might be wrong
}

function MCFacility::setPosition(%this, %pos)
{
	%this.position = %pos;
}

function MCFacility::getPosition(%this)
{
	return %this.position;
}

function MCFacility::setMCL(%this, %mcl)
{
	%this.mclayout = %mcl;
	%mcl.mcfacility = %this;
}

function MCFacility::getMCL(%this)
{
	return %this.mclayout;
}

function MCFacility::createSlotForClient(%this, %client)
{
	%this.getMCL().createSlot(%this.getMCL().nextFreeSlot(), %client);
}

function MCFacility::getSlot(%this, %num)
{
	return %this.slot[%num];
}

function MCFacility::setSlot(%this, %num, %obj)
{
	%this.slot[%num] = %obj;
}

function MCFacility::nextFreeSlot(%this)
{
	for(%i = 0; %i < %this.getMCL().maxSlots; %i++)
	{
		if(!isObject(%this.getSlot(%i)))
			return(%i);
	}
	
	return -1;
}

//Bonus!
function MCFacility::findSlotByBLID(%this, %blid)
{
	for(%i = 0; %i < %this.getMCL().maxSlots; %i++)
	{
		if(%this.getSlot(%i).ownerBLID == %blid)
			return %this.getSlot(%i);
	}
}

//Bonus!
function MCFacility::findSlotByName(%this, %name)
{
	for(%i = 0; %i < %this.getMCL().maxSlots; %i++)
	{
		if(%this.getSlot(%i).ownerName $= %name)
			return %this.getSlot(%i);
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