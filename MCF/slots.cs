//The purpose of the SlotSO is to simply store all the I we

function SlotSO::onAdd(%this, %obj)
{
	%obj.storageSets = new SimSet();
	%obj.storages = new SimSet();
	%obj.storageNum = 0;
	
	%obj.structureSets = new SimSet();
	%obj.structures = new SimSet();
	%obj.structureNum = 0;
}

function SlotSO::onRemove(%this, %obj)
{
	while (%obj.storages.getCount())
		%obj.storages.getObject(0).delete();
	while (%obj.storageSets.getCount())
		%obj.storageSets.getObject(0).delete();
	%obj.storageSets.delete();
	%obj.storages.delete();
	
	while (%obj.structures.getCount())
		%obj.structures.getObject(0).delete();
	while (%obj.structureSets.getCount())
		%obj.structureSets.getObject(0).delete();
	%obj.structureSets.delete();
	%obj.structures.delete();
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

function SlotSO::addStorage(%obj, %storage, %name)
{
	%type = %storage.getModuleType();

	%set = %obj.getStorageSet(%type);
	
	%set.add(%storage);
	%obj.storages.add(%storage);
	
	if (%name $= "")
	{
		%name = "Storage" @ %obj.storageNum;
		%obj.storageNum++;
		while (isObject(%obj.storages[%name]))
		{
			%name = "Storage" @ %obj.storageNum;
			%obj.storageNum++;
		}
	}
	%obj.setStorageName(%storage, %name);
	
	%storage.slot = %obj;
}

function SlotSO::removeStorage(%obj, %storage)
{
	%type = %storage.getModuleType();
	
	%set = %obj.getStorageSet(%type);
	
	%set.remove(%storage);
	%obj.storages.remove(%storage);
	
	%obj.storages[%storage.slotName] = "";
	%storage.slotName = "";
	
	%storage.slot = "";
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

function SlotSO::getStorageByName(%obj, %name)
{
	%storage = %obj.storages[%name];
	
	if (!isObject(%storage))
		%storage = 0;
	
	return %storage;
}

function SlotSO::setStorageName(%obj, %storage, %name)
{
	if (%storage.slotName !$= "")
		%obj.storages[%storage.slotName] = "";
	
	%storage.slotName = %name;
	%obj.storages[%name] = %storage;
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

function SlotSO::addStructure(%obj, %structure, %name)
{
	%type = %structure.getModuleType();

	%set = %obj.getStructureSet(%type);
	
	%set.add(%structure);
	%obj.structures.add(%structure);
	
	if (%name $= "")
	{
		%name = "Structure" @ %obj.structureNum;
		%obj.structureNum++;
		while (isObject(%obj.structures[%name]))
		{
			%name = "Structure" @ %obj.structureNum;
			%obj.structureNum++;
		}
	}
	%obj.setStructureName(%structure, %name);
	
	%structure.slot = %obj;
}

function SlotSO::removeStructure(%obj, %structure)
{
	%type = %structure.getModuleType();
	
	%set = %obj.getStructureSet(%type);
	
	%set.remove(%structure);
	%obj.structures.remove(%structure);
	
	%obj.structures[%structure.slotName] = "";
	%structure.slotName = "";
	
	%structure.slot = "";
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

function SlotSO::getStructureByName(%obj, %name)
{
	%structure = %obj.structures[%name];
	
	if (!isObject(%structure))
		%structure = 0;
	
	return %structure;
}

function SlotSO::setStructureName(%obj, %structure, %name)
{
	if (%structure.slotName !$= "")
		%obj.structures[%structure.slotName] = "";
	
	%structure.slotName = %name;
	%obj.structures[%name] = %structure;
}