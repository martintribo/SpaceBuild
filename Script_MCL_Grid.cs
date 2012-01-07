function MCL_Grid::onAdd(%this)
{
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
	
	%this.templateVBL.loadBLSFile("add-ons/Gamemode_SpaceBuild/MCL_Grid_Template.bls");
	
	//Loop through template and set all names to _spacebuildSupport here
}

function MCL_Grid::getMCF(%this)
{
	return %this.mcfacility;
}

function MCL_Grid::createSlot(%this, %num, %client)
{
	//find position for this number
	%pos = %this.numberToPosition(%num);
	
	%slot = new ScriptObject()
	{
		class = "MCSlot";
		number = %num;
		position = %pos;
		ownerBLID = %client.bl_id;
		ownerName = %client.getName();
	};
	%slot.createTemplate(%this.templateVBL);
	
	%this.setSlot(%num, %slot);
	
	return %slot;
}

//these just re-route to MCF functions
function MCL_Grid::getSlot(%this, %num)
{
	return %this.getMCF().getSlot(%num);
}

function MCL_Grid::setSlot(%this, %num, %slot)
{
	%this.getMCF().setSlot(%num, %slot);
}

function MCL_Grid::nextFreeSlot(%this)
{
	return %this.getMCF().nextFreeSlot();
}

//Returns the position of a slot number (center of it)
function MCL_Grid::numberToPosition(%this, %num)
{
	%startPos = %this.getMCF().getPosition();
	%x = %num;
	%y = 0;
	%z = 0;
	
	//hardcoded for now; needs to take in to account conversions
	%sizeX = 32;
	%sizeY = 32;
	%sizeZ = 14;
	
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
	
	%pos = (%x * (%sizeX + %this.paddingX)) SPC (%y * (%sizeY + %this.paddingY)) SPC (%z * (%sizeZ + %this.paddingZ));
	%pos = vectorAdd(%pos, %startPos);
	return(%pos);
}
