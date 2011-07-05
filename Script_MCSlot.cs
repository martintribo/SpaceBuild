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
}