//Module Construction Facility
//This script object contains build generation scripts and module scanning scripts.

function MCFacility::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.queue = new SimSet(); //assuming objects keep order inserted in, might be wrong


	//MCLayout code!
	//
	//absolute maximum number of slots we should ever have; usually never reached
	//this is required so the MCF knows how many slots to go through in fors
	%this.maxSlots = 5000;
	
	%this.countX = 6; //how many columns of slots to have
	%this.countY = 6; //how many rows of slots to have
	
	//how much space to leave between slots on each axis
	%this.paddingX = 0;
	%this.paddingY = 0;
	%this.paddingZ = 16;
	
	//Load VBL that we're going to use for the template
	%this.templateVBL = new ScriptObject()
	{
		class = "VirtualBrickList";
	};
	
	%this.templateVBL.loadBLSFile("add-ons/Gamemode_SpaceBuild/MCF_Grid_Template.bls");

}

function MCFacility::setPosition(%this, %pos)
{
	%this.position = %pos;
}

function MCFacility::getPosition(%this)
{
	return %this.position;
}

function MCFacility::createSlot(%this, %num, %client)
{
	if(%num $= "" || !isObject(%client))
	{
		error("MCF cannot create slot - no slot number or client specified!");
		return;
	}
	
	//find position for this number
	%pos = %this.numberToPosition(%num);
	
	%slot = new ScriptObject()
	{
		class = "MCSlot";
		number = %num;
		position = %pos;
		ownerBLID = %client.bl_id;
		ownerName = %client.name;
		size = %this.templateVBL.getSizeX() SPC %this.templateVBL.getSizeY() SPC %this.templateVBL.getSizeZ();
	};
	%slot.createTemplate(%this.templateVBL);
	
	%this.setSlot(%num, %slot);
	
	return %slot;
}

function MCFacility::createSlotForClient(%this, %client)
{
	%slot = %this.nextFreeSlot();
	
	if(%slot != -1)
		return %this.createSlot(%slot, %client);
	else
		return -1;
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
	for(%i = 0; %i < %this.maxSlots; %i++)
	{
		if(!isObject(%this.getSlot(%i)))
			return(%i);
	}
	
	return -1;
}

//nicely unregisters slot and deletes it
function MCFacility::deleteSlot(%this, %slotobj)
{
	%this.setSlot(%slotobj.slotNum, 0);
	%slotobj.delete();
}

function MCFacility::purgeEmptySlots(%this)
{
	for(%i = 0; %i < %this.maxSlots; %i++)
	{
		%slot = %this.getSlot(%i);
		if(isObject(%slot) && %slot.isEmpty())
		{
			%this.deleteSlot(%slot);
		}
	}
}

function MCFacility::findSlotByBLID(%this, %blid)
{
	for(%i = 0; %i < %this.maxSlots; %i++)
	{
		if(%this.getSlot(%i).ownerBLID == %blid)
			return %this.getSlot(%i);
	}
	
	return -1;
}

function MCFacility::findSlotByName(%this, %name)
{
	for(%i = 0; %i < %this.maxSlots; %i++)
	{
		if(%this.getSlot(%i).ownerName $= %name)
			return %this.getSlot(%i);
	}
	
	return -1;
}

function MCFacility::scanVBL(%obj, %vbl)
{
	echo("in scanVBL " @ %obj);
	%mod = newModuleSO(%vbl.getCenter(), 0, Module16x32Data);
	echo("The module from scanVBL is " @ %mod);
	
	%mod.scanVBL(%vbl);

	%obj.addToQueue(%mod);
}

function MCFacility::scanBuild(%obj, %brick)
{
	%mod = newModuleSO(Module16x32Data);

	%mod.scanBuild(%brick, %obj);
}

//Called by the ModuleSO
function MCFacility::onFinishScan(%obj, %mod)
{
	%verificationError = %mod.verifyVBL();
	switch (%verificationError) //this should report errors in a better way, directly to the user
	{
		case 1: //module does not fit bounds
			error("SpaceBuild module verification error - does not fit within maximum module bounds!");
		case 2: //module is empty/doesn't have min bricks
			error("SpaceBuild module verification error - module is empty/has too few bricks!");
	}
	
	if (%verificationError != 0)
		%mod.schedule(0, "delete");
	else
		%obj.addToQueue(%mod);
}

function MCFacility::addToQueue(%obj, %mod)
{
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
	//maybe remove these modules from the queue, and then set their states to "bricks"
}

//Returns the position of a slot number (center of it)
function MCFacility::numberToPosition(%this, %num)
{
	%startPos = %this.getPosition();
	%x = %num;
	%y = 0;
	%z = 0;
	
	while(%x >= %this.countX)
	{
		%y++;
		%x -= %this.countX;
	}
	
	while(%y >= %this.countY)
	{
		%z++;
		%y -= %this.countY;
	}
	
	%pos = (%x * (%this.templateVBL.getSizeX() + %this.paddingX)) SPC (%y * (%this.templateVBL.getSizeY() + %this.paddingY)) SPC (%z * (%this.templateVBL.getSizeZ() + %this.paddingZ));
	%pos = vectorAdd(%pos, %startPos);
	return(%pos);
}

//save format is:
//position
//slotNum^slotSaveName
function MCFacility::export(%obj, %filePath)
{
	%path = filePath(%filePath);
	%fileName = fileBase(%filePath);
	
	%file = new FileObject();
	%file.openForWrite(%filePath);
	
	%file.writeLine(%obj.getPosition()); //first line is always MCF position
	
	for(%i = 0; %i < %obj.maxSlots; %i++)
	{
		
		if(isObject(%obj.slot[%i]))
		{
			%slotSaveName = %fileName @ "_slot" @ %i @ ".mcslot";
			%slotSavePath = %path @ "/" @ %slotSaveName;
			
			%file.writeLine(%i TAB %slotSaveName);
			%obj.slot[%i].export(%slotSavePath);
		}
	}
	
	%file.close();
	%file.delete();
}

function MCFacility::import(%obj, %filePath)
{
	%path = filePath(%filePath);
	%fileName = fileBase(%filePath);
	
	%file = new FileObject();
	%file.openForRead(%filePath);
	
	%obj.setPosition(%file.readLine()); //first line is always MCF position
	
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		
		%slotNum = getField(%line, 0);
		%slotSaveName = getField(%line, 1);
		%slotSavePath = %path @ "/" @ %slotSaveName;
		
		%slotSO = new ScriptObject()
		{
			class = "MCSlot";
			number = %slotNum;
			position = %obj.numberToPosition(%slotNum);
		};
		
		%slotSO.import(%slotSavePath, %obj.templateVBL);
		
		//register slotSO with this MCF
		%obj.setSlot(%slotNum, %slotSO);
	}
	
	%file.close();
	%file.delete();
}

