function MCSlot::onAdd(%this)
{
	//create a VBL to store our bricks in
	%this.builtBricks = new SimSet();
	%this.vbl = new ScriptObject()
	{
		class = "VirtualBrickList";
	};
}

function MCSlot::onRemove(%this, %obj)
{
	%this.vbl.delete();
	while (%this.builtBricks.getCount())
		%this.builtBricks.getObject(0).delete();
	%this.builtBricks.delete();
}

function MCSlot::getPosition(%this)
{
	return %this.position;
}

//Places bricks in the VBL as-is, at this slot's position.
function MCSlot::createTemplate(%this, %vbl)
{
	%factory = new ScriptObject(MCSlotTemplateBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	%pos = %this.getPosition();
	%vbl.recenter(%pos);
	%factory.createBricksForBlid(%vbl, %this.ownerBLID);

	
	//Cheap fix, should be removed
	%bottomCenter = getWords(%vbl.getCenter(), 0, 1) SPC getWord(%pos, 2) - %vbl.getSizeZ()/2;
	
	%supportCenter = VectorAdd(%bottomCenter, "0 0 -15.9");
	
	%supportVBL = newVBL();
	%supportVBL.addBrick(brick64xCubeData, %supportCenter, 0, 1, 15, "", 0, 0, 1, 1, 1);
	%factory.createBricks(%supportVBL);
	%supportVBL.delete();
	%factory.delete();
}

function MCSlotTemplateBrickFactory::onCreateBrick(%this, %brick)
{
	%brick.slot = %this.slot;
	%brick.isSBTemplate = true;
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

//used for loading bricks
function MCSlot::createBricks(%this)
{
	%factory = new ScriptObject(MCSlotBrickFactory)
	{
		class = "BrickFactory";
		slot = %this;
	};
	%vbl = %this.vbl;
	%pos = %this.getPosition();
	//%vbl.recenter(%pos);
	%factory.createBricksForBlid(%vbl, %this.ownerBLID);
	
	%factory.delete();
}

function MCSlotBrickFactory::onCreateBrick(%this, %brick)
{
	%this.slot.addBrick(%brick);
}

function MCSlot::readyVbl(%obj)
{
	%obj.vbl.delete();
	%obj.vbl = newVBL();
	%obj.vbl.addSet(%obj.builtBricks);
}

//*****************************************************************
//Nitramtj - Not in use yet
function ModuleSO::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%f.writeLine("ownerBLID" TAB %obj.state);
	
	%f.writeLine("ownerName" TAB %obj.numHatches);
	%saveVbl = newVBL();
	%saveVbl.addSet(%obj.builtBricks);
	
	%vblPath = %path @ "/" @ %name @ "_vbl" @ %m @ ".bls";
	%obj.vbl.exportBLSFile(%vblPath);
	%f.writeLine("vbl" TAB %vblPath);
		
	%saveVbl.delete();
	%f.close();
	%f.delete();
}

function ModuleSO::import(%obj, %file)
{
	%f = new FileObject();
	%f.openForRead(%file);
	
	%obj.state = getField(%f.readLine(), 1);
	%obj.numHatches = getField(%f.readLine(), 1);
	%obj.vbl.loadBLSFile(getField(%f.readLine(), 1));
	%obj.vbl.createBricks();
	
	%f.close();
	%f.delete();
}

function MCSlotLoadFactory::onCreateBrick(%obj, %brick)
{
	//bricks are not autoadded because they are created through onLoadPlant
	%obj.slot.addRealBrick(%brick);
}

//*****************************************************************