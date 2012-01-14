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
