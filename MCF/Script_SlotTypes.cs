//Slot Types
//These are basically ScriptObjects acting as DataBlocks (storing info)

function SlotType::getBaseVbl(%obj)
{
	if (!isObject(%obj.baseVbl))
		%obj.baseVbl = loadVBL(%obj.baseBls);
	
	return %obj.baseVbl;
}

function SlotType::getSize(%obj)
{
	return %obj.sizeX * 0.5 SPC %obj.sizeY * 0.5 SPC %obj.sizeZ * 0.2;
}

function SlotType::getBrickSize(%obj)
{
	return %obj.sizeX SPC %obj.sizeY SPC %obj.sizeZ;
}

function SlotType::getSizeX(%obj)
{
	return %obj.sizeX * 0.5;
}

function SlotType::getSizeY(%obj)
{
	return %obj.sizeY * 0.5;
}

function SlotType::getSizeZ(%obj)
{
	return %obj.sizeZ * 0.2;
}

function SlotType::getBrickSizeX(%obj)
{
	return %obj.sizeX;
}

function SlotType::getBrickSizeY(%obj)
{
	return %obj.sizeY;
}

function SlotType::getBrickSizeZ(%obj)
{
	return %obj.sizeZ;
}

function SlotType::getBaseBls(%obj)
{
	return %obj.baseBls;
}

if (!isObject(Slot64x64Type))
{
	new ScriptObject(Slot64x64Type)
	{
		class = "SlotType";

		type = "Default";
		//realworld coordinates
		sizeX = 64;
		sizeY = 64;
		sizeZ = 1;

		baseBls = $Spacebuild::AddOnPath @ "MCF/slotBase64x64.bls";
	};
}

