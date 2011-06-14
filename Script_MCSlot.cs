function MCSlot::getPosition(%this)
{
	return %this.position;
}

//Places bricks in the VBL as-is, at this slot's position.
function MCSlot::createTemplate(%this, %vbl)
{
	%pos = %this.getPosition();
	%vbl.recenter(%pos);
	%vbl.createBricks();
}
