function MCSlot::onAdd(%this)
{
	//create a VBL to store our bricks in
	%this.vbl = new ScriptObject()
	{
		class = "VirtualBrickList";
	};
}

function MCSlot::onRemove(%this, %obj)
{
	%this.vbl.delete();
}

function MCSlot::getPosition(%this)
{
	return %this.position;
}

//Places bricks in the VBL as-is, at this slot's position.
function MCSlot::createTemplate(%this, %vbl)
{
	%factory = new ScriptObject(MCSlotTemplateBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	%pos = %this.getPosition();
	%vbl.recenter(%pos);
	%factory.createBricksForBlid(%vbl, %this.ownerBLID);
	%factory.delete();
	
	//Cheap fix, should be removed
	%bottomCenter = getWords(%vbl.getCenter(), 0, 1) SPC getWord(%pos, 2) - %vbl.getSizeZ()/2;
	
	%supportCenter = VectorAdd(%bottomCenter, "0 0 -15.9");
	
	%supportVBL = newVBL();
	echo("adding new brick" SPC %supportCenter SPC "|" SPC %pos SPC "|" SPC %vbl.getSizeZ() SPC "vbl" SPC %vbl);
	%supportVBL.addBrick(brick64xCubeData, %supportCenter, 0, 1, 15, "", 0, 0, 1, 1, 1);
	%supportVBL.createBricks();
	%supportVBL.delete();
}

function MCSlotTemplateBrickFactory::onCreateBrick(%this, %brick)
{
	%brick.slot = %this.slot;
	%brick.isSBTemplate = true;
	%brick.setNTObjectName("spacebuildSupport"); //mark template bricks as such
}

function MCSlot::addBrick(%this, %brick)
{
	%brick.slot = %this;
	%this.vbl.addRealBrick(%brick);
}

function MCSlot::removeBrick(%this, %brick)
{
	//the VBL has no support for this
	//silly nitramtj
}

//used for loading bricks
function MCSlot::createBricks(%this)
{
	%factory = new ScriptObject(MCSlotBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	%pos = %this.getPosition();
	%vbl.recenter(%pos);
	%factory.createBricksForBlid(%vbl, %this.ownerBLID);
	
	%factory.delete();
}

function MCSlotBrickFactory::onCreateBrick(%this, %brick)
{
	%brick.slot = %this.slot;
}
