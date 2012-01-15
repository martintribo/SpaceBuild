datablock fxDTSBrickData(brick1x4x3SpaceHatchData)
{
	brickFile = "./1x4x3SpaceHatch.blb";
	category = "Special";
	subCategory = "SpaceBuild";
	uiName = "Horizontal Space Hatch";
	iconName = "Add-Ons/Brick_V15/2x2 Corner";
};

datablock fxDTSBrickData(brick4x4fDownSpaceHatchData)
{
	brickFile = "./4x4fBottomSpaceHatch.blb";
	category = "Special";
	subCategory = "SpaceBuild";
	uiName = "Bottom Space Hatch";
	iconName = "Add-Ons/Brick_V15/2x2 Corner";
};

datablock fxDTSBrickData(brick4x4fTopSpaceHatchData)
{
	brickFile = "./4x4fTopSpaceHatch.blb";
	category = "Special";
	subCategory = "SpaceBuild";
	uiName = "Top Space Hatch";
	iconName = "Add-Ons/Brick_V15/2x2 Corner";
};

function fxDTSBrick::isHatch(%obj)
{
	if (%obj.isHorizontalHatch() || %obj.isVerticalHatch())
		return true;
	else
		return false;
}

function fxDTSBrick::isHorizontalHatch(%obj)
{
	if (%obj.getDatablock().getId() == brick1x4x3SpaceHatchData.getId())
		return true;
	else
		return false;
}

function fxDTSBrick::isVerticalHatch(%obj)
{
	if (%obj.isDownHatch() || %obj.isUpHatch())
		return true;
	else
		return false;
}

function fxDTSBrick::isDownHatch(%obj)
{
	if (%obj.getDatablock().getId() == brick4x4fDownSpaceHatchData.getId())
		return true;
	else
		return false;
}

function fxDTSBrick::isUpHatch(%obj)
{
	if (%obj.getDatablock().getId() == brick4x4fTopSpaceHatchData.getId())
		return true;
	else
		return false;
}
