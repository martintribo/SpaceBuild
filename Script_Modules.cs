//Note: ModuleSO should always have all info needed to recreate (render) the module
//The vbl's position and angleid should not be used independently
//If they're changed, so should the ModuleSO's position and angleId
function newModuleSO(%position, %angleId, %moduleType)
{
	%vbl = newVBL();

	%mod = new ScriptObject()
	{
		class = "ModuleSO";
		vbl = %vbl;
		moduleType = %moduleType;
		position = %position;
		angleId = %angleId;
		state = "none";
		numHatches = 0;
	};
	%vbl.module = %mod;
	%vbl.setListAngleId(%angleId);
	%vbl.recenter(%position);
	
	return %mod;
}

function ModuleSO::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.numHatches = 0;
	%obj.state = "none"; //none state basically means the module isn't represented at all in spaceyou 
	%obj.bricks = new SimSet();
	%obj.factory = new ScriptObject(ModuleSOFactory)
	{
		class = "BrickFactory";
		module = %obj;
	};
	%obj.hatchBrickSet = new SimSet();
	%obj.cleaning = false;
	//%obj.position
	//%obj.angleId
	//%obj.moduleType
	//%obj.state
}

function ModuleSO::onRemove(%this, %obj)
{
	%obj.derender();

	%obj.factory.delete();

	%obj.hatchBrickSet.delete();
}

function ModuleSO::getPosition(%obj)
{
	return %obj.position;
}

function ModuleSO::setPosition(%obj, %pos)
{
	%obj.position = %pos;

	%obj.derender();
	%obj.render();
}

function ModuleSO::getAngleId(%obj)
{
	return %obj.angleId;
}

function ModuleSO::setAngleId(%obj, %ang)
{
	%obj.angleId = %ang;

	%obj.deleteRender();
	%obj.render();
}

//Only rerenders once if both position and angle are changed
function ModuleSO::setPositionAngleId(%obj, %pos, %ang)
{
	%obj.position = %pos;
	%obj.angleId = %ang;

	%obj.derender();
	%obj.render();
}

function ModuleSO::getModuleType(%obj)
{
	return %obj.moduleType;
}

function ModuleSO::addBrick(%mod, %sb)
{
	if (%sb.isHatch())
	{
		%box = %sb.getWorldBox();
		%pos = %sb.getPosition();
		if (%sb.isHorizontalHatch())
		{
			switch (%sb.getAngleId())
			{
				case 0:
					%point = getWord(%pos, 0) SPC getWord(%box, 4) SPC getWord(%pos, 2);
				case 1:
					%point = getWord(%box, 3) SPC getWords(%pos, 1, 2);
				case 2:
					%point = getWord(%pos, 0) SPC getWord(%box, 1) SPC getWord(%pos, 2);
				case 3:
					%point = getWord(%box, 0) SPC getWords(%pos, 1, 2);
			}
			%dir = %sb.getAngleId();
		}
		else
		{
			if (%sb.isUpHatch())
			{
				%point = getWords(%pos, 0, 1) SPC getWord(%box, 5);
				%dir = 4; //up
			}
			else
			{
				%point = getWords(%pos, 0, 1) SPC getWord(%box, 2);
				%dir = 5; //down
			}
		}
		
		%sb.hatchId = %mod.numHatches;
		%vb = %mod.vbl.addRealBrick(%sb);
		%mod.addHatch(%vb, %point, %dir);
	}
	else
		%vb = %mod.vbl.addRealBrick(%sb);
	
	%sb.vBrick = %vb;
	%mod.bricks.add(%sb);
	%sb.module = %mod;
}

//Data needs to be organized so that it's possible to remove a hatch
//And the ModuleSO will be able to still keep a list of them
function ModuleSO::addHatch(%obj, %vb, %point, %dir1, %dir2)
{
	//%obj.hatches[%obj.numHatches, "point"] = %point;
	//%obj.hatches[%obj.numHatches, "direction"] = %dir1;
	%obj.vbl.addMarker("hatch" @ %obj.numHatches, %point, %dir1, %dir2);
	%obj.registerHatchVBrick(%obj.numHatches, %vb);
}

function ModuleSO::registerHatchVBrick(%obj, %hatchId, %vb)
{
	%obj.hatchVBricks[%hatchId] = %vb;
	if (%hatchId >= %obj.numHatches)
		%obj.numHatches = %hatchId + 1;
}

function ModuleSO::registerHatchBrick(%obj, %hatchId, %brick)
{
	%obj.hatchBricks[%hatchId] = %brick;
	%obj.hatchBrickSet.add(%brick);
}

//This method just takes care of removing the marker and internal references
//It is assumed the vbl and brick will be deleted by the method callign this
function ModuleSO::removeHatch(%obj, %hatchId)
{
	%obj.vbl.removeMarker("hatch" @ %hatchId);
	
	for (%i = %hatchId + 1; %i < %obj.numHatches; %i++)
	{
		%obj.vbl.renameMarker("hatch" @ %i, "hatch" @ (%i - 1));
		%obj.hatchVBricks[%i - 1] = %obj.hatchVBricks[%i];
	}
	
	%obj.numHatches--;
}

function ModuleSO::getHatchType(%obj, %i)
{
	%type = 0;
	if (%i < %obj.numHatches)
	{
		%dir = %obj.vbl.getMarkerPrimary("hatch" @ %i);
		if (%dir < 4)
			%type = "horizontal";
		else if (%dir == 4)
			%type = "up";
		else
			%type = "down";
	}
	
	return %type;
}

function ModuleSO::attachTo(%obj, %objHatch, %mod, %modHatch)
{
	//besides just setting up links and etc, the states and owners of modules must be changed
	%obj.vbl.markers["hatch" @ %objHatch].alignWith(%mod.vbl.markers["hatch" @ %modHatch]);

	%obj.setPositionAngleId(%obj.vbl.getCenter(), %obj.vbl.getListAngleId());
	
	//%obj.owner.addModule(%obj); //this is going to be updated in future modification
}

function ModuleSO::setState(%obj, %state)
{
	%obj.derender();

	%obj.state = %state;

	%obj.render();
}

package ModulePack
{
	//Interesting note!
	//Support_ToolBrick must be executed before this, or this is never called!
	//TODO: Make this use a BrickFactory instead of the old way
	function virtualBrickList::onCreateBrick(%obj, %b)
	{
		if (isObject(%obj.module))
		{
			%b.module = %obj.module;
			if (%b.isHatch())
			{
				%b.setColliding(1); //it's easier for it not to be colliding on the ground, but it needs to be solid in space
				%b.setRendering(1);
				%b.setRaycasting(1);
			}
		}
		Parent::onCreateBrick(%obj, %b);
	}

	function fxDTSBrick::onRemove(%this, %obj)
	{
		if (isObject(%obj.module))
			%obj.module.onRemoveBrick(%obj);

		Parent::onRemove(%this, %obj);
	}
};

activatePackage(ModulePack);

function moduleSO::getCompatibleHatches(%obj, %type)
{
	// if (%dir < 4)
		// %type = "horizontal";
	// else if (%dir == 4)
		// %type = "down";
	// else
		// %type = "up";
	
	if (%type $= "down")
		%type = "up";
	else if (%type $= "up")
		%type = "down";
	
	%list = "";
	
	for (%i = 0; %i < %obj.numHatches; %i++)
	{
		if (%obj.getHatchType(%i) $= %type)
			%list = %list @ %i @ "\t";
	}
	
	return %list;
}

//error codes:
//1 - module does not fit bounds
//2 - module is empty/too few bricks
function ModuleSO::verifyVBL(%obj)
{
	%maxBounds = "8 16 8.2"; //16 32 12.66 to users
	%minBricks = 1;
	%allowSwitchingXandY = true;
	
	%x = %obj.vbl.getSizeX();
	%y = %obj.vbl.getSizeY();
	%z = %obj.vbl.getSizeZ();
	
	//check max bounds
	if (%allowSwitchingXandY)
	{
		//this is a bit complicated;
		//it checks that either rotation of the VBL will fit the max bounds
		%highest = getMax(getWord(%maxBounds, 0), getWord(%maxBounds, 1));
		%lowest = getMin(getWord(%maxBounds, 0), getWord(%maxBounds, 1));
		
		if (%x > %lowest)
		{
			if (%x > %highest)
				return 1;
			else
				if (%y > %lowest)
					return 1;
		}else{
			if (%y > %highest)
				return 1;
		}
	}else{
		if (%x > getWord(%maxBounds, 0))
			return 1;
		if (%y > getWord(%maxBounds, 1))
				return 1;
	}
	
	if (%z > getWord(%maxBounds, 2))
			return 1;
	
	//check min bricks
	if (%obj.vbl.getCount() < %minBricks)
		return 2;
		
	//note: functions don't return 0 by default
	return 0;
}

function ModuleSO::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%f.writeLine("STATE" TAB %obj.state);
	
	%f.writeLine("HATCHES" TAB %obj.numHatches);
	//for (%h = 0; %h < %obj.numHatches; %h++)
	//	%f.writeLine("HATCH" TAB %obj.hatches[%obj.numHatches, "point"] TAB %obj.hatches[%obj.numHatches, "direction"]);
	
	%vblPath = %path @ "/" @ %name @ "_vbl" @ %m @ ".bls";
	%obj.vbl.exportBLSFile(%vblPath);
	%f.writeLine("vbl" TAB %vblPath);
	%f.writeLine("moduleType" TAB %obj.moduleType.getName());
	%f.writeLine("position" TAB %obj.position);
	%f.writeLine("angleId" TAB %obj.angleId);
		
	%f.close();
	%f.delete();
}

function ModuleSO::import(%obj, %file)
{
	%f = new FileObject();
	%f.openForRead(%file);
	
	%obj.state = getField(%f.readLine(), 1);

	//Deployed state is deprecated
	if (%obj.state $= "Deployed")
		%obj.state = "bricks";

	%obj.numHatches = getField(%f.readLine(), 1);
	%obj.vbl.loadBLSFile(getField(%f.readLine(), 1));
	
	//new fields that doesn't exist in older files
	if (!%f.isEOF())
		%obj.moduleType = getField(%f.readLine(), 1);
	else
		%obj.moduleType = Module16x32Data; //all old modules are 16x32

	if (!%f.isEOF())
		%obj.position = getField(%f.readLine(), 1);
	else
		%obj.position = %obj.vbl.getCenter();
	
	if (!%f.isEOF())
		%obj.angleId = getField(%f.readLine(), 1);
	else
		%obj.angleId = %obj.vbl.getListAngleId();
	
	%f.close();
	%f.delete();

	%obj.render();
}

function loadModuleSO(%file)
{
	%mod = newModuleSO();
	%mod.import(%file);
	return %mod;
}

function ModuleSO::render(%obj)
{
	if (%obj.state $= "bricks")
	{
		%obj.vbl.setListAngleId(%obj.angleId);
		%obj.vbl.recenter(%obj.position);
		%obj.factory.createBricksForBLID(%obj.vbl, $Spacebuild::StationBLID); //Might want to do offset of blids instead of a station blid
	}
}

function ModuleSOFactory::onCreateBrick(%obj, %brick)
{
	%obj.module.bricks.add(%brick);
}

function ModuleSO::derender(%obj)
{
	if (%obj.state $= "bricks")
	{
		%obj.cleaning = true;
		while (%obj.bricks.getCount())
			%obj.bricks.getObject(0).delete();
		%obj.cleaning = false;
	}
}

function ModuleSO::onRemoveBrick(%obj, %brick)
{
	if (!%obj.cleaning)
		%obj.removeBrick(%brick);
}

function ModuleSO::removeBrick(%obj, %brick)
{
	if (%brick.hatchId)
		%obj.removeHatch(%brick.hatchId);
	
	%brick.vBrick.delete();

}

//***********************************************************
//* Scanning code
//***********************************************************
function ModuleSO::scanVBL(%mod, %vbl)
{
	for (%i = 0; %i < %vbl.getCount(); %i++)
		%mod.addBrick(%vbl.getVirtualBrick(%i));
}
//Starts the scanning script
//creates a new module, sets up scanning object
//module
function ModuleSO::scanBuild(%mod, %brick, %mcfs)
{
	//need to just initialize the scan here
	%bf = new ScriptObject()
	{
		class = "BrickFinder";
	};
	%bf.setOnSelectCommand(%mod @ ".onFoundBrick(%sb);");
	%bf.setFinishCommand(%mod @ ".onFinishedFinding(" @ %bf @ ", " @ %mcfs @ ");");
	%bf.search(%brick, "chain", "all", "spaceSupport", 0);
}

function ModuleSO::onFoundBrick(%mod, %sb)
{
	%mod.addBrick(%sb);
}

//cleans up brick finder and calls scanning 
function ModuleSO::onFinishedFinding(%mod, %bf, %mcfs)
{
	if (isObject(%mcfs))
		%mcfs.onFinishScan(%mod); //technically %mod doesn't need to be sent, but it is sent for ease of use

	//delete the Brick Finder
	%bf.schedule(0, "delete");
}

//***********************************************************
//* End scanning code
//***********************************************************

