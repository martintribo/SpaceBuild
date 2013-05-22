//Module Definitions
//These are basically scriptobjects acting as datablocks (storing info)

function ModuleData::getStructureVbl(%obj)
{
	if (!isObject(%obj.structureVbl))
		%obj.structureVbl = loadVBL(%obj.structureBls);
}

function ModuleData::getStorageVbl(%obj)
{
	if (!isObject(%obj.storageVbl))
		%obj.storageVbl = loadVBL(%obj.storageBls);
}

function ModuleData::getSize(%obj)
{
	return %obj.sizeX * 0.5 SPC %obj.sizeY * 0.5 SPC %obj.sizeZ * 0.2;
}

function ModuleData::getBrickSize(%obj)
{
	return %obj.sizeX SPC %obj.sizeY SPC %obj.sizeZ;
}

function ModuleData::getSizeX(%obj)
{
	return %obj.sizeX * 0.5;
}

function ModuleData::getSizeY(%obj)
{
	return %obj.sizeY * 0.5;
}

function ModuleData::getSizeZ(%obj)
{
	return %obj.sizeZ * 0.2;
}

function ModuleData::getBrickSizeX(%obj)
{
	return %obj.sizeX;
}

function ModuleData::getBrickSizeY(%obj)
{
	return %obj.sizeY;
}

function ModuleData::getBrickSizeZ(%obj)
{
	return %obj.sizeZ;
}

function ModuleData::getStorageBls(%obj)
{
	return %obj.storageBls;
}

function ModuleData::getStructureBls(%obj)
{
	return %obj.structureBls;
}

if (!isObject(Module16x32Data))
{
	new ScriptObject(Module16x32Data)
	{
		class = "ModuleData";

		type = "Default";
		//realworld coordinates
		sizeX = 16;
		sizeY = 32;
		sizeZ = 41;

		storageBls = $Spacebuild::AddOnPath @ "MCF/storage16x32.bls";
		structureBls = $Spacebuild::AddOnPath @ "MCF/structure16x32.bls";
	};

	new ScriptObject(Module16x16Data)
	{
		class = "ModuleData";

		type = "Default";
		//realworld coordinates
		sizeX = 16;
		sizeY = 16;
		sizeZ = 71;

		storageBls = $Spacebuild::AddOnPath @ "MCF/storage16x16.bls";
		structureBls = $Spacebuild::AddOnPath @ "MCF/structure16x16.bls";
	};
}

