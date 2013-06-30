//The purpose of the SlotSO is to simply store all the I we

function newSlotSO(%slotType, %position, %angleId, %blid)
{
	%slot = new ScriptObject()
	{
		class = "SlotSO";
		slotType = %slotType;
		position = %position;
		angleId = %angleId;
		slotBlid = %blid;
	};
	%slot.setRendering(true);
	
	return %slot;
}

function loadSlotSO(%file)
{
	%slot = new ScriptObject()
	{
		class = "SlotSO";
	};
	
	%slot.import(%file);
	
	return %slot;
}

function SlotSO::onAdd(%this, %obj)
{
	%obj.slotObjects = new SimSet();
	%obj.slotObjNum = 0;
	
	%obj.storageSets = new SimSet();
	%obj.storages = new SimSet();
	%obj.storageNum = 0;
	
	%obj.structureSets = new SimSet();
	%obj.structures = new SimSet();
	%obj.structureNum = 0;
	
	%obj.baseBricks = new SimSet();
	%obj.baseFactory = new ScriptObject(SlotSOFactory)
	{
		class = "BrickFactory";
		slot = %obj;
	};
	
	%obj.brickSet = new SimSet();
	%obj.brickVbl = newVBL();
	%obj.brickVbl.setListAngleId(%obj.getAngleId());
	%obj.brickFactory = new ScriptObject(SlotSOBrickFactory)
	{
		class = "BrickFactory";
		slot = %obj;
	};
	
	%obj.derendering = false;
	
	%obj.setRendering(false);
	
	//%obj.position
	//%obj.angleId
	//%obj.slotType
}

function SlotSO::onRemove(%this, %obj)
{
	while (%obj.slotObjects.getCount())
		%obj.slotObjects.getObject(0).delete();
	%obj.slotObjects.delete();
	
	while (%obj.storageSets.getCount())
		%obj.storageSets.getObject(0).delete();
	%obj.storageSets.delete();
	%obj.storages.delete();
	
	while (%obj.structureSets.getCount())
		%obj.structureSets.getObject(0).delete();
	%obj.structureSets.delete();
	%obj.structures.delete();
	
	while (%obj.brickSet.getCount())
		%obj.brickSet.delete();
	%obj.brickSet.delete();
	
	%obj.brickVbl.delete();
	%obj.brickFactory.delete();
	
	while (%obj.baseBricks.getCount())
		%obj.baseBricks.getObject(0).delete();
	%obj.baseBricks.delete();
	
	%obj.baseFactory.delete();
}

function SlotSOFactory::onCreateBrick(%obj, %brick)
{
	%obj.slot.baseBricks.add(%brick);
	%brick.slot = %obj.slot;
}

function SlotSOBrickFactory::onCreateBrick(%obj, %brick)
{
	%obj.slot.brickSet.add(%brick);
	%brick.slot = %obj.slot;
	%brick.built = true;
}

function SlotSO::getSlotType(%obj)
{
	return %obj.slotType;
}

function SlotSO::isRendering(%obj)
{
	return %obj.rendering;
}

function SlotSO::setRendering(%obj, %value)
{
	if (!%obj.rendering && %value)
	{
		%obj.rendering = true;
		
		%baseVbl = %obj.slotType.getBaseVbl();
		%baseVbl.setListAngleId(%obj.getAngleId());
		%baseVbl.recenter(%obj.getPosition());
		%obj.baseFactory.createBricksForBlid(%baseVbl, %obj.getOwnerBlid());
		
		%obj.brickFactory.createBricks(%obj.brickVbl);
		
		for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
			%obj.slotObjects.getObject(%i).setRendering(true);
	}
	else if (%obj.rendering && !%value)
	{
		%obj.rendering = false;
		
		for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
			%obj.slotObjects.getObject(%i).setRendering(false);
		
		while (%obj.baseBricks.getCount())
			%obj.baseBricks.getObject(0).delete();
		
		%obj.derendering = true;
		
		while (%obj.brickSet.getCount())
			%obj.brickSet.getObject(0).delete();
		
		%obj.derendering = false;
	}
}

function SlotSO::getOwnerBlid(%obj)
{
	return %obj.slotBlid;
}

function SlotSO::setOwnerBlid(%obj, %blid)
{
	%wasRendering = %obj.isRendering();
	%obj.setRendering(false);
	
	%obj.slotBlid = %blid;
	
	if (%wasRendering)
		%obj.setRendering(true);
}

function SlotSO::getPosition(%obj)
{
	return %obj.position;
}

function SlotSO::setPosition(%obj, %newPos)
{
	%wasRendering = %obj.isRendering();
	%obj.setRendering(false);
	
	%oldPos = %obj.getPosition();
	
	for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
	{
		%slotObj = %obj.slotObjects.getObject(%i);
		
		%dis = VectorSub(%slotObj.getPosition(), %oldPos);
		%slotObjPos = VectorAdd(%dis, %newPos);
		
		%slotObj.setPosition(%slotObjPos);
	}
	
	%vbl = %obj.brickVbl;
	%dis = VectorSub(%vbl.getCenter(), %oldPos);
	%vblPos = VectorAdd(%dis, %newPos);
	%vbl.recenter(%vblPos);
	
	%obj.position = %newPos;
	
	if (%wasRendering)
		%obj.setRendering(true);
}

function SlotSO::getAngleId(%obj)
{
	return %obj.angleId;
}

function SlotSO::setAngleId(%obj, %angleId)
{
	//first normalize the angleId
	if (%angleId < 0)
		%angleId = 4 - (mAbs(%angleId) % 4);
	else
		%angleId %= 4;
	
	%curAng = %obj.getAngleId();
	
	if (%curAng != %angleId)
	{
		%wasRendering = %obj.isRendering();
		%obj.setRendering(false);
		
		if (%curAng > %angleId)
			%curAng -= 4;
		
		%rotAng = %angleId - %curAng;
		%center = %obj.getPosition(); // this isn't necessarily the center
		
		for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
		{
			%slotObj = %obj.slotObjects.getObject(%i);
			%oldDis = VectorSub(%slotObj.getPosition(), %center);
			%oldAng = %slotObj.getAngleId();
			switch (%rotAng)
			{
				case 1:
					%newDis = getWord(%oldDis, 1) SPC -getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
				case 2:
					%newDis = -getWord(%oldDis, 0) SPC -getWord(%oldDis, 1) SPC getWord(%oldDis, 2);
				case 3:
					%newDis = -getWord(%oldDis, 1) SPC getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
			}
			
			%newPos = VectorAdd(%newDis, %center);
			%newAng = (%oldAng + %rotAng) % 4;
			%slotObj.setAngleId(%newAng);
			%slotObj.setPosition(%newPos);
		}
		
		%vbl = %obj.brickVbl;
		%oldDis = VectorSub(%vbl.getCenter(), %center);
		switch (%rotAng)
		{
			case 1:
				%newDis = getWord(%oldDis, 1) SPC -getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
			case 2:
				%newDis = -getWord(%oldDis, 0) SPC -getWord(%oldDis, 1) SPC getWord(%oldDis, 2);
			case 3:
				%newDis = -getWord(%oldDis, 1) SPC getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
		}
		
		%newPos = VectorAdd(%newDis, %center);
		%vbl.setListAngleId(%angleId);
		%vbl.recenter(%newPos);
		
		%obj.angleId = %angleId;
		
		if (%wasRendering)
			%obj.setRendering(true);
	}
}

//General SlotObject Code

function SlotSO::setSlotObjectName(%obj, %slotObj, %name)
{
	if (%slotObj.slotName !$= "")
		%obj.slotObjects[%slotObj.slotName] = "";
	
	%obj.slotObjects[%name] = %slotObj;
	%slotObj.slotName = %name;
}

function SlotSO::addSlotObject(%obj, %slotObj, %name)
{
	%slotObj.slot = %obj;
	%obj.slotObjects.add(%slotObj);
	
	if (%obj.isRendering())
		%slotObj.setRendering(true);
	else
		%obj.setRendering(false);
	
	if (isClass(%slotObj, "ModuleStorage"))
		%obj.onAddStorage(%slotObj, %name);
	else if (isClass(%slotObj, "ModuleStructure"))
		%obj.onAddStructure(%slotObj, %name);
	else
		%obj.onAddBasicSlotObject(%slotObj, %name);
}

function SlotSO::removeSlotObject(%obj, %slotObj)
{
	%obj.slotObjects[%slotObj.slotName] = "";
	%slotObj.slotName = "";
	%slotObj.slot = "";
	
	if (isClass(%slotObj, "ModuleStorage"))
		%obj.onRemoveStorage(%slotObj, %name);
	else if (isClass(%slotObj, "ModuleStructure"))
		%obj.onRemoveStructure(%slotObj, %name);
	
	//Not needed yet
	//else
	//	%obj.onRemoveBasicSlotObject(%slotObj, %name);
}

function SlotSO::onAddBasicSlotObject(%obj, %slotObj, %name)
{
	if (%name $= "")
	{
		%name = "SlotObject" @ %obj.slotObjNum;
		%obj.slotObjNum++;
		while (isObject(%obj.slotObjects[%name]))
		{
			%name = "SlotObject" @ %obj.slotObjNum;
			%obj.slotObjNum++;
		}
	}
	%obj.setSlotObjectName(%storage, %name);
}

function SlotSO::getSlotObjectByName(%obj, %name)
{
	if (isObject(%obj.slotObjects[%name]))
		return %obj.slotObjects[%name];
	else
		return 0;
}

//Storage Related Code

function SlotSO::addStorageSet(%obj, %type)
{
	%name = %type.getName();
	%set = new SimSet();
	%obj.storageSets.add(%set);
	%obj.storageSets[%name] = %set;
	%set.name = %name;
	
	return %set;
}

function SlotSO::getStorageSet(%obj, %type)
{
	%name = %type.getName();
	
	if (!isObject(%obj.storageSets[%name]))
		%obj.addStorageSet(%type);
	
	return %obj.storageSets[%name];
}

function SlotSO::onAddStorage(%obj, %storage, %name)
{
	%type = %storage.getModuleType();

	%set = %obj.getStorageSet(%type);
	
	%set.add(%storage);
	%obj.storages.add(%storage);
	
	if (%name $= "")
	{
		%name = "Storage" @ %obj.storageNum;
		%obj.storageNum++;
		while (isObject(%obj.slotObjects[%name]))
		{
			%name = "Storage" @ %obj.storageNum;
			%obj.storageNum++;
		}
	}
	%obj.setSlotObjectName(%storage, %name);
}

function SlotSO::onRemoveStorage(%obj, %storage)
{
	%type = %storage.getModuleType();
	
	%set = %obj.getStorageSet(%type);
	
	%set.remove(%storage);
	%obj.storages.remove(%storage);
}

function SlotSO::getNumStorages(%obj, %type)
{
	if (%type !$= "")
		%set = %obj.getStorageSet(%type);
	else
		%set = %obj.storages;
	
	return %set.getCount();
}

function SlotSO::getStorage(%obj, %i, %type)
{
	if (%type !$= "")
		%set = %obj.getStorageSet(%type);
	else
		%set = %obj.storages;
	
	return %set.getObject(%i);
}

//Structure Code

function SlotSO::addStructureSet(%obj, %type)
{
	%name = %type.getName();
	%set = new SimSet();
	%obj.structureSets.add(%set);
	%obj.structureSets[%name] = %set;
	%set.name = %name;
	
	return %set;
}

function SlotSO::getStructureSet(%obj, %type)
{
	%name = %type.getName();
	
	if (!isObject(%obj.structureSets[%name]))
		%obj.addStructureSet(%type);
	
	return %obj.structureSets[%name];
}

function SlotSO::onAddStructure(%obj, %structure, %name)
{
	%type = %structure.getModuleType();

	%set = %obj.getStructureSet(%type);
	
	%set.add(%structure);
	%obj.structures.add(%structure);
	
	if (%name $= "")
	{
		%name = "Structure" @ %obj.structureNum;
		%obj.structureNum++;
		while (isObject(%obj.slotObjects[%name]))
		{
			%name = "Structure" @ %obj.structureNum;
			%obj.structureNum++;
		}
	}
	%obj.setSlotObjectName(%structure, %name);
}

function SlotSO::onRemoveStructure(%obj, %structure)
{
	%type = %structure.getModuleType();
	
	%set = %obj.getStructureSet(%type);
	
	%set.remove(%structure);
	%obj.structures.remove(%structure);
}

function SlotSO::getNumStructures(%obj, %type)
{
	if (%type !$= "")
		%set = %obj.getStructureSet(%type);
	else
		%set = %obj.structures;
	
	return %set.getCount();
}

function SlotSO::getStructure(%obj, %i, %type)
{
	if (%type !$= "")
		%set = %obj.getStructureSet(%type);
	else
		%set = %obj.structures;
	
	return %set.getObject(%i);
}

//End Structure Code

function isClass(%obj, %type)
{
	if (%obj.getClassName() $= %type)
		return true;
	else if (%obj.getClassName() $= "ScriptObject" && (%obj.class $= %type || %obj.superClass $= %type || %obj.getName() $= %type))
		return true;
	else
		return false;
}

function ModuleSO::getSlot(%obj)
{
	if (isObject(%obj.slot))
		return %obj.slot;
	else
		return 0;
}

function ModuleStructure::getSlot(%obj)
{
	if (isObject(%obj.slot))
		return %obj.slot;
	else
		return 0;
}

function ModuleStorage::getSlot(%obj)
{
	if (isObject(%obj.slot))
		return %obj.slot;
	else
		return 0;
}

function fxDTSBrick::getSlot(%obj)
{
	//oh boy
	if (isObject(%obj.structure))
		return %obj.structure.getSlot();
	else if (isObject(%obj.storage))
		return %obj.storage.getSlot();
	else if (isObject(%obj.module))
		return %obj.module.getSlot();
	else if (isObject(%obj.slot))
		return %obj.slot;
	else
		return 0;
}

//checks if brick is completely within bounds
function SlotSO::brickWithinBounds(%obj, %brick)
{
	%brickBox = %brick.getWorldBox();
	%slotBox = %obj.getWorldBox();
	
	for (%i = 0; %i < 2; %i++)
		if (getWord(%brickBox, %i) < getWord(%slotBox, %i) || getWord(%brickBox, %i + 3) > getWord(%slotBox, %i + 3))
			return false;
	
	return true;
}

//checks if brick is completely out of bounds
function SlotSO::brickOutOfBounds(%obj, %brick)
{
	%brickBox = %brick.getWorldBox();
	%slotBox = %obj.getWorldBox();
	
	for (%i = 0; %i < 2; %i++)
		if (getWord(%brickBox, %i) > getWord(%slotBox, %i + 3) || getWord(%brickBox, %i + 3) < getWord(%slotBox, %i))
			return true;
	
	return false;
}

function SlotSO::getWorldBox(%obj, %brick)
{
	%ang = %obj.getAngleId() % 2;
	%slotRad = VectorAdd(VectorScale(%obj.getSlotType().getSize(), 0.5), "0.05 0.05 0.05");
	%slotCen = %obj.getPosition();
	
	if (%ang)
		%slotRad = getWord(%slotRad, 1) SPC getWord(%slotRad, 0) SPC getWord(%slotRad, 2);
	
	%min = VectorSub(%slotCen, %slotRad);
	%max = VectorAdd(%slotCen, %slotRad);
	
	return %min SPC %max;
}

package SlotSOPackage
{
//need to verify that brick is only touching one slot
//and that it is within slot's horizontal bounds
function fxDTSBrick::onPlant(%obj)
{
	%ret = Parent::onPlant(%obj);
	
	//don't want to add bricks that belong to structure's module
	if (isObject(%obj.getStructure()))
		return %ret;
	
	//code done this way so the logic is in one place
	//since bricks placed on the structure, but inside the module bounds are allowed
	//it's possible that the brick can be touching two structures, so a list must be kept instead of a single variable
	%numSlots = 0;
	%foundOtherBrick = false;
	
	for (%d = 0; %d < %obj.getNumDownBricks(); %d++)
	{
		%brick = %obj.getDownBrick(%d);
		%slot = %brick.getSlot();
		
		if (isObject(%slot) && !%foundSlots[%slot])
		{
			%slots[%numSlots] = %slot;
			%numSlots++;
			%foundSlots[%slot] = true;
		}
	}
	for (%u = 0; %u < %obj.getNumUpBricks(); %u++)
	{
		%brick = %obj.getUpBrick(%u);
		%slot = %brick.getSlot();
		
		if (isObject(%slot) && !%foundSlots[%slot])
		{
			%slots[%numSlots] = %slot;
			%numSlots++;
			%foundSlots[%slot] = true;
		}
	}
	if (%numSlots > 1)
	{
		%obj.killBrick();
			if (isObject(%obj.client))
				commandToClient(%obj.client, 'centerPrint', "Bricks cannot go between slots!", 3);
	}
	else if (%numSlots == 1)
	{
		%slot = %slots[0];
		if (%slot.brickWithinBounds(%obj))
			%slot.addBrick(%obj);
		else if (!%slot.brickOutOfBounds(%obj))
		{
			%obj.killBrick();
			if (isObject(%obj.client))
				commandToClient(%obj.client, 'centerPrint', "Bricks must be placed completely within the slot!", 3);
		}
	}
	
	return %ret;
}

function fxDTSBrick::onRemove(%this, %obj)
{
	if (isObject(%obj.slot) && %obj.built)
		%obj.slot.onRemoveBrick(%obj);

	Parent::onRemove(%this, %obj);
}
};

function SlotSO::addBrick(%obj, %brick)
{
	%obj.brickSet.add(%brick);
	%obj.brickVbl.addRealBrick(%brick);
	%brick.slot = %obj;
	%brick.built = true;
}

function SlotSO::onRemoveBrick(%obj, %brick)
{
	if (!%obj.derendering)
		%obj.removeBrick(%brick);
}

function SlotSO::removeBrick(%obj, %brick)
{
	%brick.vBrick.delete();
	%obj.brickSet.remove(%brick);
	%obj.slot = "";
}
activatePackage("SlotSOPackage");

function SlotSO::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%f.writeLine("type" TAB %obj.slotType.getName());
	%f.writeLine("position" TAB %obj.position);
	%f.writeLine("angleId" TAB %obj.angleId);
	%f.writeLine("rendering" TAB %obj.rendering);
	%f.writeLine("blid" TAB %obj.slotBlid);

	%brickPath = %path @ "/" @ %name @ "_builtBricks" @ ".bls";
	%obj.brickVbl.exportBLSFile(%brickPath);
	%f.writeLine("builtBricks" TAB %brickPath);
	
	for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
	{
		%slotObj = %obj.slotObjects.getObject(%i);
		%scriptClass = %slotObj.getScriptClass();
		
		%slotObjPath = %path @ "/" @ %name @ "_slotObj" @ %i @ ".txt";
		%slotObj.export(%slotObjPath);
		%f.writeLine("slotObj" TAB %scriptClass TAB %slotObjPath);
	}

	%f.close();
	%f.delete();
}

function SlotSO::import(%obj, %file)
{
	%obj.setRendering(false);
	
	%f = new FileObject();
	%f.openForRead(%file);
	
	%obj.slotType = getField(%f.readLine(), 1);
	%obj.position = getField(%f.readLine(), 1);
	%obj.angleId = getField(%f.readLine(), 1);
	%rendering = getField(%f.readLine(), 1);
	%obj.slotBlid = getField(%f.readLine(), 1);
	
	%obj.brickVbl.loadBLSFile(getField(%f.readLine(), 1));

	while (!%f.isEOF())
	{
		%line = %f.readLine();
		%scriptClass = getField(%line, 1);
		%slotObjFile = getField(%line, 2);
		
		//this could be dangerous
		//the scriptClass can be fetched from the slot object's name if there is no class
		//if the player could influence an object's name that didn't have a class, they could
		//modify some of the name of the method being called
		//since text is appended to the name, no critical methods should be able to be called
		%slotObj = call(%scriptClass @ "_" @ "import", %slotObjFile);
		%obj.addSlotObject(%slotObj);
	}
	
	%f.close();
	%f.delete();

	%obj.setRendering(%rendering);
}

//this might not be the best way to determine the object's class
function ScriptObject::getScriptClass(%obj)
{
	if (%obj.class !$= "")
		return %obj.class;
	else if (%obj.getName() !$= "")
		return %obj.getName();
}
