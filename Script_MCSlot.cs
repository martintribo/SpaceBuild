function MCSlot::onAdd(%this)
{
	//create a VBL to store our bricks in
	%this.builtBricks = new SimSet();
	%this.templateBricks = new SimSet();
	
	//For player saved module loading
	%this.selectedSave = 0;
	
}

function MCSlot::onRemove(%this, %obj)
{
	while (%this.builtBricks.getCount())
		%this.builtBricks.getObject(0).delete();
	while (%this.templateBricks.getCount())
		%this.templateBricks.getObject(0).delete();
	%this.builtBricks.delete();
	%this.templateBricks.delete();
}

function MCSlot::getOwnerBLID(%this)
{
	return %this.ownerBLID;
}

function MCSlot::getOwnerName(%this)
{
	return %this.ownerName;
}

function MCSlot::getSize(%this)
{
	return %this.size;
}

function MCSlot::getPosition(%this)
{
	return %this.position;
}

function MCSlot::getCenter(%this)
{
	return VectorAdd(%this.getPosition(), 0 SPC 0 SPC getWord(%this.size, 2) * 0.5);
}

function MCSlot::getLastActive(%this)
{
	//if the player is in the server, update to now
	if(isObject(findClientByBLID(%this.getOwnerBLID())))
	{
		%this.lastActive = getDateTime();
	}
	
	return %this.lastActive;
}

//Places bricks in the template VBL at this slot's position.
//Will also name the bricks in the template spacebuildSupport and add them to %this.templateBricks (see MCSlotTemplateBrickFactory::onCreateBrick())
function MCSlot::createTemplate(%this, %vbl)
{
	%factory = new ScriptObject(MCSlotTemplateBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	
	%pos = %this.getPosition();
	
	%center = vectorAdd(%pos, 0 SPC 0 SPC (%vbl.getSizeZ() / 2));
	
	%vbl.recenter();
	%vbl.shiftBricks(%center);

	%factory.createBricksForBlid(%vbl, %this.ownerBLID);
	%factory.delete();
}

function MCSlotTemplateBrickFactory::onCreateBrick(%this, %brick)
{
	%brick.slot = %this.slot;
	%brick.isSBTemplate = true;
	%this.slot.templateBricks.add(%brick);
	%brick.setNTObjectName("spacebuildSupport"); //mark template bricks as such
	
	if (%brick.isSaveSlotPrint)
		%this.slot.savePrintBrick = %brick;
}

function MCSlot::addBrick(%this, %brick)
{
	%brick.slot = %this;
	%this.builtBricks.add(%brick);
}

function MCSlot::removeBrick(%this, %brick)
{
	%this.builtBricks.remove(%brick);
}

function MCSlotBrickFactory::onCreateBrick(%this, %brick)
{
	%this.slot.addBrick(%brick);
}

function MCSlot::saveBuiltBricks(%this, %path)
{
	%vbl = newVBL();
	%vbl.addSet(%this.builtBricks);
	
	%vbl.shiftBricks(vectorScale(%this.getPosition(), -1));
	
	%vbl.exportBLSFile(%path);
	
	%vbl.delete();
}

function MCSlot::loadBuiltBricks(%this, %path)
{
	%vbl = newVBL();
	
	%vbl.loadBLSFile(%path);
	
	%vbl.shiftBricks(%this.getPosition());
	
	%factory = new ScriptObject(MCSlotLoadFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	
	//should probably not be no owner
	%factory.createBricksNoOwner(%vbl, %this.ownerBLID);
	%factory.delete();
	
	%vbl.delete();
}

function MCSlotLoadFactory::onCreateBrick(%this, %brick)
{
	//bricks are not autoadded to builtBricks because they are created through onLoadPlant, so we just add them here
	%this.slot.addBrick(%brick);
}

function MCSlot::getCurrentSaveSlot(%obj)
{
	return %obj.selectedSave;
}

function MCSlot::nextSaveSlot(%obj)
{
	%obj.selectedSave++;
	if (%obj.selectedSave >= $Spacebuild::Prefs::MaxModuleSaves)
		%obj.selectedSave = 0;
	
	%obj.updateSavePrintBrick();
}

function MCSlot::prevSaveSlot(%obj)
{
	%obj.selectedSave--;
	if (%obj.selectedSave < 0)
		%obj.selectedSave = $Spacebuild::Prefs::MaxModuleSaves - 1;
	
	%obj.updateSavePrintBrick();
}

function MCSlot::updateSavePrintBrick(%obj)
{
	if (isObject(%obj.savePrintBrick))
	{
		%printId = $printNameTable["Letters/" @ (%obj.selectedSave + 1)];
		%obj.savePrintBrick.setPrint(%printId);
	}
}

//only clears built bricks, not the structure!
function MCSlot::clearBuiltBricks(%obj)
{
	while (%obj.builtBricks.getCount())
	{
		%brick = %obj.builtBricks.getObject(0);
		%obj.removeBrick(%brick); //for the future, if we want to add callbacks
		%brick.delete();
	}
}

function MCSlot::loadBuiltBricksInSaveSlot(%obj)
{
	%obj.clearBuiltBricks();
	
	%blid = %obj.ownerBLID;
	
	%path = $Spacebuild::SavePath @ "Players/" @ %blid @ "/" @ %obj.selectedSave;
	
	//normally would check if the file exists, but there isn't a one line way to do that and you get no errors for trying to load a nonexisting file..
	%obj.loadBuiltBricks(%path);
}

function MCSlot::saveBuiltBricksInSaveSlot(%obj)
{
	%blid = %obj.ownerBLID;
	%path = $Spacebuild::SavePath @ "Players/" @ %blid @ "/" @ %obj.selectedSave;
	
	%obj.saveBuiltBricks(%path);
}

function MCSlot::export(%this, %filePath)
{
	%path = filePath(%filePath);
	%fileName = fileBase(%filePath);
	
	%file = new FileObject();
	%file.openForWrite(%filePath);
	
	%file.writeLine(%this.getOwnerBLID());
	%file.writeLine(%this.getOwnerName());
	%file.writeLine(%this.getSize());
	%file.writeLine(%this.getCurrentSaveSlot());
	%file.writeLine(%this.getLastActive());
	%file.writeLine(%this.modulesAccepted);
	
	%file.close();
	%file.delete();
	
	%this.saveBuiltBricks(%path @ "/" @ %fileName @ "_bricks.bls");
}

function MCSlot::import(%this, %filePath, %templateVBL)
{
	%path = filePath(%filePath);
	%fileName = fileBase(%filePath);
	
	%file = new FileObject();
	%file.openForRead(%filePath);
	
	%this.ownerBLID = %file.readLine();
	%this.ownerName = %file.readLine();
	%this.size = %file.readLine();
	%this.selectedSave = %file.readLine();
	%this.lastActive = %file.readLine();
	%this.modulesAccepted = %file.readLine();

	%file.close();
	%file.delete();	
	
	//this should really be refactored so MCSlot::createTemplate infers the VBL (and has no arguments), probably through MCSlot::getTemplateVBL() or something,
	//but we're too close to launch to break old stuff now
	%this.createTemplate(%templateVBL);
	%this.loadBuiltBricks(%path @ "/" @ %fileName @ "_bricks.bls");
}

//so we can save hatchbricks
addCustSave("SAVESLOTPRINT");

function virtualBrickList::cs_save_SAVESLOTPRINT(%obj, %vb, %file)
{
	if (%vb.props["SAVESLOTPRINT"] !$= "")
		%file.writeLine("+-SAVESLOTPRINT" SPC %vb.hatchId);
}

function virtualBrickList::cs_load_SAVESLOTPRINT(%obj, %vb, %addData, %addInfo, %addArgs, %line)
{
	%vb.props["SAVESLOTPRINT"] = true;
}

function virtualBrickList::cs_create_SAVESLOTPRINT(%obj, %vb, %brick)
{
	if (%vb.props["SAVESLOTPRINT"] !$= "")
		%brick.isSaveSlotPrint = true;
}

function virtualBrickList::cs_addReal_SAVESLOTPRINT(%obj, %vb, %brick)
{
	if (%brick.isSaveSlotPrint !$= "")
		%vb.props["SAVESLOTPRINT"] = true;
	else
		%vb.props["SAVESLOTPRINT"] = "";
}
