//Places bricks in the VBL as-is, at this slot's position.
function MCSlot::createTemplate(%vbl)
{
	%pos = %this.getPosition();
	%vbl.shiftBricks(%pos);
	%vbl.createBricks();
}
