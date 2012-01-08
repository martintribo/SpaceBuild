function MCSlot::onAdd(%this)
{
	//create a VBL to store our bricks in
	%this.vbl = new ScriptObject()
	{
		class = "VirtualBrickList";
	};
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
	%factory.createBricks(%vbl);
	%factory.delete();
}

function MCSlotTemplateBrickFactory::onCreateBrick(%this, %brick)
{
	%brick.slot = %this.slot;
	%brick.setNTObjectName("spacebuildSupport"); //mark template bricks as such
	
	%this.addBrick(%brick); //add the template bricks to be part of our VBL
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

function MCSlot::createBricks(%this)
{
	%factory = new ScriptObject(MCSlotBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	%pos = %this.getPosition();
	%vbl.recenter(%pos);
	%factory.createBricks(%vbl);
	%factory.delete();
}

function MCSlotBrickFactory::onCreateBrick(%this, %brick)
{
	%brick.slot = %this.slot;
}

function fxDTSBrick::findSlot(%obj)
{
	%slot = 0;
	if (%obj.getNumDownBricks())
		%slot = %obj.getDownBrick(0).slot;
	else if (%obj.getNumUpBricks())
		%slot = %obj.getUpBrick(0).slot;
	
	return %slot;
}

function fxDTSBrick::isInSlot(%obj)
{
	
}