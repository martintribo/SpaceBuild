function MCSlot::getPosition(%this)
{
	return %this.position;
}

//Places bricks in the VBL as-is, at this slot's position.
function MCSlot::createTemplate(%this, %vbl)
{
	%factory = new ScriptObject(MCSlotBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	%pos = %this.getPosition();
	%vbl.recenter(%pos);
	//%vbl.createBricks();
	%factory.createBricks(%vbl);
	%factory.delete();
}

function MCSlotBrickFactory::onCreateBrick(%obj, %brick)
{
	%brick.slot = %obj.slot;
	%brick.setNTObjectName("spacebuildSupport");
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