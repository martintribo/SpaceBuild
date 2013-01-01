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

function MCSlot::getPosition(%this)
{
	return %this.position;
}

function MCSlot::getCenter(%this)
{
	return VectorAdd(%this.getPosition(), 0 SPC 0 SPC getWord(%this.size, 2) * 0.5);
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
}

function MCSlotLoadFactory::onCreateBrick(%this, %brick)
{
	//bricks are not autoadded to builtBricks because they are created through onLoadPlant, so we just add them here
	%this.slot.addBrick(%brick);
}


function MCSlot::nextSave(%obj)
{
	%obj.selectedSave++;
	if (%obj.selectedSave >= $Spacebuild::Prefs::MaxModuleSaves)
		%obj.selectedSave = 0;
	
	%obj.updateSavePrintBrick();
}

function MCSlot::prevSave()
{
	%obj.selectedSave--;
	if (%obj.selectedSave < 0)
		%obj.selectedSave = $Spacebuild::Prefs::MaxModuleSaves - 1;
	
	%obj.updateSavePrintBrick();
}

function MCSlot::updateSaveBrick(%obj)
{
	if (isObject(%obj.savePrintBrick))
	{
		//update print
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
	%obj.import(%path);
}

function MCSlot::saveBuiltBricksInSaveSlot(%obj)
{
	%blid = %obj.ownerBLID;
	%path = $Spacebuild::SavePath @ "Players/" @ %blid @ "/" @ %obj.selectedSave;
	
	%obj.export(%path);
}