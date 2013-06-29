//The purpose of the SlotSO is to simply store all the I we

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
	
	//%obj.position
	//%obj.angleId
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
		
		for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
			%obj.slotObjects.getObject(%i).setRendering(true);
	}
	else if (%obj.rendering && !%value)
	{
		%obj.rendering = false;
		
		for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
			%obj.slotObjects.getObject(%i).setRendering(false);
	}
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
		%center = %slotObj.getPosition(); // this isn't necessarily the center
		
		for (%i = 0; %i < %obj.slotObjects.getCount(); %i++)
		{
			%slotObj = %obj.slotObjects.getObject(%i);
			%oldDis = VectorSub(%slotObj.getPosition(), %oldPos);
			switch (%rotAng)
			{
				case 1:
					%newDis = getWord(%oldDis, 1) SPC -getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
				case 2:
					%newDis = -getWord(%oldDis, 1) SPC -getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
				case 3:
					%newDis = -getWord(%oldDis, 1) SPC getWord(%oldDis, 0) SPC getWord(%oldDis, 2);
			}
			
			%newPos = VectorAdd(%newDis, %center);
			%slotObj.setAngleId(%angleId);
			%slotObj.setPosition(%newPos);
		}
		
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

function isClass(%obj, %type)
{
	if (%obj.getClassName() $= %type)
		return true;
	else if (%obj.getClassName() $= "ScriptObject" && (%obj.class $= %type || %obj.superClass $= %type || %obj.getName() $= %type))
		return true;
	else
		return false;
}
