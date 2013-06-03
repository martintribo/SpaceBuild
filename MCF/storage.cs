//storage radius: 4.5 8.5 4.6
//module radius: 4 8 4.1
//module center is .3 world units above storage's center
//module bottom is 3.8 world units below storage's center
//module corners are 4 and 8 from center
function newModuleStorage(%moduleType, %position, %angleId, %blid)
{
	if (%blid $= "")
		%blid = $SpaceBuild::SpawnBLID;

	return new ScriptObject()
	{
		class = "ModuleStorage";
		moduleType = %moduleType;
		position = %position;
		angleId = %angleId;
		blid = %blid;
	};
}

function loadModuleStorage(%file)
{
	%storage = new ScriptObject()
	{
		class = "ModuleStorage";
		moduleType = Module16x32Data; //to keep error from happening
	};

	%storage.import(%file);

	return %storage;
}

function ModuleStorage::onAdd(%this, %obj)
{
	//%obj.position
	//%obj.angleId
	
	%obj.norender = false;
	
	if (%obj.moduleType $= "")
		error("Storage Declared without module type!");
	else
		%obj.moduleOffset = "0 0" SPC %obj.moduleType.getStorageVbl().getSizeZ()/2 - 0.2 - %obj.moduleType.getSizeZ()/2;

	%obj.structureBricks = new SimSet();

	%obj.factory = new ScriptObject(ModuleStorageFactory)
	{
		class = "BrickFactory";
		brickGroup = %obj.structureBricks;
	};

	%obj.render();
}

function ModuleStorage::onRemove(%this, %obj)
{
	%obj.derender();

	%obj.structureBricks.delete();

	//The module that this storage is holding should be handled by the code deleting the structure..
}

function ModuleStorage::render(%obj)
{
	if (!%obj.norender)
	{
		%vbl = %obj.moduleType.getStorageVbl();

		%vbl.setListAngleId(%obj.angleId);
		%vbl.recenter(%obj.position);

		%obj.factory.createBricksForBlid(%vbl, %obj.blid);

		if (isObject(%obj.module))
			%obj.module.setState("bricks");
	}
}

function ModuleStorage::setStorageBlid(%obj, %blid)
{
	%obj.storageBlid = %blid;
}

function ModuleStorage::getStorageBlid(%obj)
{
	return %obj.storageBlid;
}

function ModuleStorage::getModuleType(%obj)
{
	return %obj.moduleType;
}

function ModuleStorageFactory::onCreateBrick(%obj, %brick)
{
	%obj.brickGroup.add(%brick);
}

function ModuleStorage::derender(%obj)
{
	while (%obj.structureBricks.getCount())
		%obj.structureBricks.getObject(0).delete();
	
	if (isObject(%obj.module))
		%obj.module.setState("none");
}

function ModuleStorage::setRendering(%obj, %on)
{
	%on = !%on;

	if (%on != %obj.norender)
	{
		%obj.norender = %on;

		if (%obj.norender)
			%obj.derender();
		else
			%obj.render();
	}
}

function ModuleStorage::getPosition(%obj)
{
	return %obj.position;
}

function ModuleStorage::setPosition(%obj, %pos)
{
	%obj.derender();

	%obj.position = %pos;

	if (isObject(%obj.module))
		%obj.module.setPosition(VectorAdd(%pos, %obj.moduleOffset));

	%obj.render();
}

function ModuleStorage::getAngleId(%obj)
{
	return %obj.angleId;
}

function ModuleStorage::setAngleId(%obj, %angleId)
{
	%obj.derender();
	
	%obj.angleId = %angleId;
	
	if (isObject(%obj.module))
		%obj.module.setAngleId(%angleId);

	%obj.render();
}

function ModuleStorage::setModule(%obj, %module)
{
	if (!isObject(%module))
		error("Attempted to store module that does not exist!");

	if (%module.getModuleType().getId() != %obj.getModuleType().getId())
		error("Module of incorrect type being put in storage!");
	
	if (isObject(%obj.module))
	{
		error("Module being added to storage already containing module!");
		%obj.module.setState("none");
	}

	%obj.module = %module;

	%obj.module.setState("none");
	%obj.module.setAngleId(%obj.angleId);
	%obj.module.setPosition(VectorAdd(%obj.position, %obj.moduleOffset));
	%obj.module.setState("bricks");
}

function ModuleStorage::loadModule(%obj, %file)
{
	%mod = loadModuleSO(%file);

	if (%module.getModuleType().getId() != %obj.getModuleType().getId())
		error("Module of incorrect type being put in storage!");
	
	%obj.setModule(%mod);
}

function ModuleStorage::deleteModule(%obj, %module)
{
	%obj.module.delete();
}

function ModuleStorage::clearModule(%obj, %module)
{
	%obj.module.setState("none");
	%obj.module = "";
}

function ModuleStorage::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%f.writeLine("type" TAB %obj.moduleType.getName());
	%f.writeLine("moduleOffset" TAB %obj.moduleOffset);
	%f.writeLine("position" TAB %obj.position);
	%f.writeLine("angleId" TAB %obj.angleId);
	%f.writeLine("blid" TAB %obj.storageBlid);

	%mod = %obj.module;
	if (isObject(%mod))
	{
		%modPath = %path @ "/" @ %name @ "_mod" @ ".mod";
		%mod.export(%modPath);
		%f.writeLine("mod" TAB %modPath);
	}

	%f.close();
	%f.delete();
}

function ModuleStorage::import(%obj, %file)
{
	%f = new FileObject();
	
	%f.openForRead(%file);

	%obj.moduleType = getField(%f.readLine(), 1).getId();
	%obj.moduleOffset = getField(%f.readLine(), 1);
	%obj.position = getField(%f.readLine(), 1);
	%obj.angleId = getField(%f.readLine(), 1);
	%obj.storageBlid = getField(%f.readLine(), 1);
	
	if (!%f.isEOF())
		%obj.module = loadModuleSO(getField(%f.readLine(), 1));

	%obj.render();
	
	%f.close();
	%f.delete();
}
