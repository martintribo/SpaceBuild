//Module Definitions
//These are basically scriptobjects acting as datablocks (storing info)

if (!isObject(Module16x32Data))
{
	new ScriptObject(Module16x32Data)
	{
		type = "Default";
		//realworld coordinates
		sizeX = 8;
		sizeY = 16;
		sizeZ = 8.2;

		storageBLS = loadVBL($Spacebuild::AddOnPath @ "MCF/storage16x32.bls");
		structureBLS = loadVBL($Spacebuild::AddOnPath @ "MCF/structure16x32.bls");
	};

	new ScriptObject(Module16x32Data)
	{
		type = "Default";
		//realworld coordinates
		sizeX = 8;
		sizeY = 8;
		sizeZ = 14.2;

		storageBLS = loadVBL($Spacebuild::AddOnPath @ "MCF/storage16x16.bls");
		structureBLS = loadVBL($Spacebuild::AddOnPath @ "MCF/structure16x16.bls");
	};
}

