function MCL_Grid::onAdd(%this)
{
	//absolute maximum number of slots we should ever have; usually never reached
	//this is required so the MCF knows how many slots to go through in fors
	%this.maxSlots = 5000;

	//Set some properties (used by this MCLayout, not mandatory)
	%this.sizeX = 64; //X length of template
	%this.sizeY = 64; //Y length of template
	%this.sizeZ = 64; //Z length of template
	
	%this.countX = 6; //how many columns of slots to have
	%this.countY = 6; //how many rows of slots to have
	
	//Load VBL that we're going to use for the template
	%this.templateVBL = new ScriptObject()
	{
		class = "VirtualBrickList";
	};
	
	%this.templateVBL.loadBLSFile("./MCL_Grid_Template.bls");
	
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
	%i = %num;
	%x = 0;
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
	
	%pos = (%x * %this.sizeX) SPC (%y * %this.sizeY) SPC (%z * %this.sizeZ);
	%pos = vectorAdd(%pos, %startPos);
	return(%pos);
}
