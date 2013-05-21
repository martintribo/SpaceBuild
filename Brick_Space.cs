datablock fxDTSBrickData(brick16x32ModuleData)
{
	brickFile = "./16x32Module.blb";
	category = "Special";
	subCategory = "SpaceBuild";
	uiName = "16x32 Module";
	iconName = "Add-Ons/Brick_V15/2x2 Corner";
};
datablock fxDTSBrickData(brick16x16ModuleData)
{
	brickFile = "./16x16Module.blb";
	category = "Special";
	subCategory = "SpaceBuild";
	uiName = "16x16 Module";
	iconName = "Add-Ons/Brick_V15/2x2 Corner";
};
datablock fxDTSBrickData(brick16x32ModuleStorageData)
{
	brickFile = "./16x32ModuleStorage.blb";
	category = "Special";
	subCategory = "SpaceBuild";
	uiName = "16x32 Module Storage";
	iconName = "Add-Ons/Brick_V15/2x2 Corner";
};
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


//This feels unnecessary, but required to work with virtual bricks of hatches
function VirtualBrick::isHatch(%obj)
{
	if (%obj.isHorizontalHatch() || %obj.isVerticalHatch())
		return true;
	else
		return false;
}

function VirtualBrick::isHorizontalHatch(%obj)
{
	if (%obj.getDatablock().getId() == brick1x4x3SpaceHatchData.getId())
		return true;
	else
		return false;
}

function VirtualBrick::isVerticalHatch(%obj)
{
	if (%obj.isDownHatch() || %obj.isUpHatch())
		return true;
	else
		return false;
}

function VirtualBrick::isDownHatch(%obj)
{
	if (%obj.getDatablock().getId() == brick4x4fDownSpaceHatchData.getId())
		return true;
	else
		return false;
}

function VirtualBrick::isUpHatch(%obj)
{
	if (%obj.getDatablock().getId() == brick4x4fTopSpaceHatchData.getId())
		return true;
	else
		return false;
}

//so we can save hatchbricks
addCustSave("SPACEHATCH");
function virtualBrickList::cs_addReal_SPACEHATCH(%obj, %vb, %brick)
{
	if (%brick.hatchId !$= "")
	{
		%vb.hatchId = %brick.hatchId;
		%vb.props["SPACEHATCH"] = true;
	}
	else
	{
		%vb.hatchId = "";
		%vb.props["SPACEHATCH"] = "";
	}
}

function virtualBrickList::cs_create_SPACEHATCH(%obj, %vb, %brick)
{
	if (%vb.hatchId !$= "")
		%brick.hatchId = %vb.hatchId;
}

function virtualBrickList::cs_save_SPACEHATCH(%obj, %vb, %file)
{
	if (%vb.hatchId !$= "")
	{
		%file.writeLine("+-SPACEHATCH" SPC %vb.hatchId);
	}
}

function virtualBrickList::cs_load_SPACEHATCH(%obj, %vb, %addData, %addInfo, %addArgs, %line)
{
	%vb.hatchId = %addInfo;
	%vb.props["SPACEHATCH"] = true;
}

function bfSpaceSupport(%brick)
{
	if (%brick.getName() $= "_spacebuildSupport")
		return 1;
	return 0;
}

addBFType("spaceSupport", "bfSpaceSupport");

