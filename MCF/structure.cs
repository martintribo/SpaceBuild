function newModuleStructure(%moduleType, %position, %angleId, %blid)
{
	if (%blid $= "")
		%blid = $SpaceBuild::SpawnBLID;

	%struct = new ScriptObject()
	{
		class = "ModuleStructure";
		moduleType = %moduleType;
		position = %position;
		angleId = %angleId;
		blid = %blid;
	};
	
	%mod = newModuleSO(%struct.getModuleCenter(), %struct.getAngleId(), %struct.getModuleType());
	
	%struct.setModule(%mod);
	
	return %struct;
}

function loadModuleStructure(%file)
{
	%structure = new ScriptObject()
	{
		class = "ModuleStructure";
		moduleType = Module16x32Data; //to keep error from happening
	};

	%structure.import(%file);

	return %structure;
}

function ModuleStructure::onAdd(%this, %obj)
{
	//%obj.position
	//%obj.angleId
	
	%obj.norender = false;
	
	if (%obj.moduleType $= "")
		error("Structure Declared without module type!");
	else
		%obj.moduleOffset = "0 0" SPC %obj.moduleType.getStructureVbl().getSizeZ()/2 - 0.2 - %obj.moduleType.getSizeZ()/2;

	%obj.structureBricks = new SimSet();

	%obj.factory = new ScriptObject(ModuleStructureFactory)
	{
		class = "BrickFactory";
		brickGroup = %obj.structureBricks;
		structure = %obj;
	};

	%obj.render();
}

function ModuleStructure::onRemove(%this, %obj)
{
	%obj.derender();

	%obj.structureBricks.delete();

	%obj.module.delete();
}

function ModuleStructure::render(%obj)
{
	if (!%obj.norender)
	{
		%vbl = %obj.moduleType.getStructureVbl();

		%vbl.setListAngleId(%obj.angleId);
		%vbl.recenter(%obj.position);

		%obj.factory.createBricksForBlid(%vbl, %obj.structureBlid);

		if (isObject(%obj.module))
			%obj.module.setState("bricks");
	}
}

function ModuleStructure::setOwnerBlid(%obj, %blid)
{
	%obj.structureBlid = %blid;
	if (%obj.isRendering())
	{
		%obj.derender();
		%obj.render();
	}
}

function ModuleStructure::getStructureBlid(%obj)
{
	return %obj.structureBlid;
}

function ModuleStructure::getModuleType(%obj)
{
	return %obj.moduleType;
}

function ModuleStructureFactory::onCreateBrick(%obj, %brick)
{
	%obj.brickGroup.add(%brick);
	%brick.structure = %obj.structure;
}

function ModuleStructure::derender(%obj)
{
	while (%obj.structureBricks.getCount())
		%obj.structureBricks.getObject(0).delete();
	
	if (isObject(%obj.module))
		%obj.module.setState("none");
}

function ModuleStructure::isRendering(%obj)
{
	return !%obj.norender;
}

function ModuleStructure::setRendering(%obj, %on)
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

function ModuleStructure::getPosition(%obj)
{
	return %obj.position;
}

function ModuleStructure::setPosition(%obj, %pos)
{
	%wasRendering = %obj.isRendering();
	%obj.derender();

	%obj.position = %pos;

	if (isObject(%obj.module))
		%obj.module.setPosition(%obj.getModuleCenter());

	if (%wasRendering)
		%obj.render();
}

function ModuleStructure::getModule(%obj)
{
	return %obj.module;
}

function ModuleStructure::getAngleId(%obj)
{
	return %obj.angleId;
}

function ModuleStructure::setAngleId(%obj, %angleId)
{
	%angleId %= 4;
	if (%angleId == %obj.angleId)
		return;
	
	%wasRendering = %obj.isRendering();
	%obj.derender();
	
	%obj.angleId = %angleId;
	
	if (isObject(%obj.module))
		%obj.module.setAngleId(%angleId);

	if (%wasRendering)
		%obj.render();
}

function ModuleStructure::setModule(%obj, %module)
{
	if (!isObject(%module))
		error("Attempted to store module that does not exist!");

	if (%module.getModuleType().getId() != %obj.getModuleType().getId())
		error("Module of incorrect type being put in structure!");
	
	if (isObject(%obj.module))
	{
		error("Module being added to structure already containing module!");
		%obj.module.setState("none");
	}

	%obj.module = %module;
	%module.structure = %obj;

	%obj.module.setState("none");
	%obj.module.setAngleId(%obj.angleId);
	%obj.module.setPosition(%obj.getModuleCenter());
	%obj.module.setState("bricks");
}

function ModuleStructure::loadModule(%obj, %file)
{
	%mod = loadModuleSO(%file);

	if (%module.getModuleType().getId() != %obj.getModuleType().getId())
		error("Module of incorrect type being put in structure!");
	
	%obj.setModule(%mod);
}

function ModuleStructure::deleteModule(%obj, %module)
{
	%obj.module.delete();
	%mod = newModuleSO(%obj.getModuleCenter(), %obj.getAngleId(), %obj.getModuleType());
	%obj.setModule(%mod);
}

function ModuleStructure::clearModule(%obj, %module)
{
	%obj.module.setState("none");
	%obj.module = "";
	%mod = newModuleSO(%obj.getModuleCenter(), %obj.getAngleId(), %obj.getModuleType());
	%obj.setModule(%mod);
}

function ModuleStructure::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%f.writeLine("type" TAB %obj.moduleType.getName());
	%f.writeLine("moduleOffset" TAB %obj.moduleOffset);
	%f.writeLine("position" TAB %obj.position);
	%f.writeLine("angleId" TAB %obj.angleId);
	%f.writeLine("blid" TAB %obj.structureBlid);

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

function ModuleStructure::import(%obj, %file)
{
	%f = new FileObject();
	
	%f.openForRead(%file);

	%obj.moduleType = getField(%f.readLine(), 1).getId();
	%obj.moduleOffset = getField(%f.readLine(), 1);
	%obj.position = getField(%f.readLine(), 1);
	%obj.angleId = getField(%f.readLine(), 1);
	%obj.structureBlid = getField(%f.readLine(), 1);
	
	if (!%f.isEOF())
		%obj.module = loadModuleSO(getField(%f.readLine(), 1));

	%obj.render();
	
	%f.close();
	%f.delete();
}

//Brick Handling Code
function ModuleStructure::addBrick(%obj, %brick)
{
	//just mark the brick as being part of this structure, and then add it to the module
	
	%obj.module.addBrick(%brick);
}

//Can't think of a situation where this is used, but for completeness it's included
function ModuleStructure::removeBrick(%obj, %brick)
{
	if (isObject(%obj.module))
		%obj.module.removeBrick(%brick);
}

function fxDTSBrick::getStructure(%obj)
{
	if (isObject(%obj.module) && isObject(%obj.module.getStructure()))
		return %obj.module.getStructure();
	else if (isObject(%brick.structure))
		return %brick.structure;
	else
		return 0;
}
package ModuleStructurePackage
{
function fxDTSBrick::onPlant(%obj)
{	
	//code done this way so the logic is in one place
	//since bricks placed on the structure, but inside the module bounds are allowed
	//it's possible that the brick can be touching two structures, so a list must be kept instead of a single variable
	%numStructures = 0;
	%foundOtherBrick = false;
	
	for (%d = 0; %d < %obj.getNumDownBricks(); %d++)
	{
		%brick = %obj.getDownBrick(%d);
		%structure = %brick.getStructure();
		if (isObject(%structure))
		{
			%structures[%numStructures] = %structure;
			%numStructures++;
		}
		else
			%foundOtherBrick = true;
	}
	for (%u = 0; %u < %obj.getNumUpBricks(); %u++)
	{
		%brick = %obj.getUpBrick(%u);
		%structure = %brick.getStructure();
		
		if (isObject(%structure))
		{
			%structures[%numStructures] = %structure;
			%numStructures++;
		}
		else
			%foundOtherBrick = true;
	}
	
	//doesn't check if structure is in the list multiple times
	for (%s = 0; %s < %numStructures; %s++)
	{
		%structure = %structures[%s];
		if (%structure.brickWithinBounds(%obj))
		{
			if (%foundOtherBrick)
			{
				%obj.killBrick();
				if (isObject(%obj.client))
					commandToClient(%obj.client, 'centerPrint', "Module bricks cannot touch other bricks!", 3);
				break;
			}
			
			%structure.addBrick(%obj);
			break;
		}
		else if (!%structure.brickOutOfBounds(%obj))
		{
			%obj.killBrick(); //pray that no scumbag will submit their module before the brick is destroyed
			if (isObject(%obj.client))
				commandToClient(%obj.client, 'centerPrint', "Module bricks must fit within build structure!", 3);
			break;
		}
	}
	
	return Parent::onPlant(%obj);
}
};
activatePackage("ModuleStructurePackage");

function ModuleStructure::getModuleCenter(%obj)
{
	return VectorAdd(%obj.position, %obj.moduleOffset);
}
//checks if brick is completely within bounds
function ModuleStructure::brickWithinBounds(%obj, %brick)
{
	%brickBox = %brick.getWorldBox();
	%modBox = %obj.getModuleBox();
	
	for (%i = 0; %i < 3; %i++)
		if (getWord(%brickBox, %i) < getWord(%modBox, %i) || getWord(%brickBox, %i + 3) > getWord(%modBox, %i + 3))
			return false;
			
	return true;
}

//checks if brick is completely out of bounds
function ModuleStructure::brickOutOfBounds(%obj, %brick)
{
	%brickBox = %brick.getWorldBox();
	%modBox = %obj.getModuleBox();
	
	for (%i = 0; %i < 3; %i++)
		if (getWord(%brickBox, %i) > getWord(%modBox, %i + 3) || getWord(%brickBox, %i + 3) < getWord(%modBox, %i))
			return true;
	
	return false;
}

function ModuleStructure::getModuleBox(%obj, %brick)
{
	%ang = %obj.getAngleId() % 2;
	%modRad = VectorAdd(VectorScale(%obj.getModuleType().getSize(), 0.5), "0.05 0.05 0.05");
	%modCen = %obj.getModuleCenter();
	
	if (%ang)
		%modRad = getWord(%modRad, 1) SPC getWord(%modRad, 0) SPC getWord(%modRad, 2);
	
	%min = VectorSub(%modCen, %modRad);
	%max = VectorAdd(%modCen, %modRad);
	
	return %min SPC %max;
}

//convenience method
function ModuleSO::getStructure(%obj)
{
	if (isObject(%obj.structure) && %obj.structure.getModule().getId() == %obj.getId())
		return %obj.structure;
	else
		return 0;
}

function fxDTSBrick::getStructure(%obj)
{
	if (isObject(%obj.module))
		return %obj.module.getStructure();
	else if (isObject(%obj.structure))
		return %obj.structure;
	else
		return 0;
}