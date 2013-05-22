function newModuleSO(%moduleType, %vbl)
{
	if (!isObject(%vbl))
		%vbl = newVBL(1);
	%mod = new ScriptObject()
	{
		class = "ModuleSO";
		vbl = %vbl;
		moduleType = %moduleType;
	};
	%vbl.module = %mod;
	
	return %mod;
}

function ModuleSO::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.numHatches = 0;
	%obj.state = "none"; //none state basically means the module isn't represented at all in spaceyou 
	//%obj.moduleType
	//%obj.state
	//%obj.owner
}

function ModuleSO::getType(%obj)
{
	return %obj.moduleType;
}

function ModuleSO::addBrick(%obj, %brick)
{
	%obj.vbl.addRealBrick(%brick);
	//if (%brick.getDatablock().getId() == brick1x4x3SpaceHatchData.getId()) This part is done by the scan function, revise?
	//	%obj.addHatch();
}

function ModuleSO::addHatch(%obj, %point, %dir1, %dir2)
{
	//%obj.hatches[%obj.numHatches, "point"] = %point;
	//%obj.hatches[%obj.numHatches, "direction"] = %dir1;
	%obj.vbl.addMarker("Hatch" @ %obj.numHatches, %point, %dir1, %dir2);
	%obj.numHatches++;
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
	//%obj.vbl.createBricks();
	
	%obj.owner = %mod.owner; //update ownership, change?
	%obj.owner.addModule(%obj);
	%obj.deploy();
}

function ModuleSO::deploy(%obj)
{
	%obj.setState("Deployed");
	%obj.vbl.createBricksNoOwner();
}

function ModuleSO::setState(%obj, %state)
{
	//the position can be calculated differently depending on the state, make sure it stays the same
	%pos = %obj.getPosition();
	%obj.state = %state;
	%obj.setPosition(%pos);
}

function ModuleSO::getPosition(%obj)
{
	//this eventually needs to change depending on the state
	if(%obj.state $= "cargo")
		return %obj.cargoPlayer.getPosition();
	else
		return %obj.vbl.getCenter();
}

function ModuleSO::setPosition(%obj, %pos)
{
	%obj.vbl.recenter(%pos);
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

function moduleSO::createCargoPlayer(%module)
{
	%cargo = new Player()
	{
		datablock = playerCargo;
		moduleSO = %module;
		//owner = %client;
	};
	
	%cargo.setTransform(%module.getPosition());
	%cargo.setScale("1 1 1"); //make this proportional to build size later on (?)
	
	%module.cargoPlayer = %cargo;
	%module.setState("cargo");
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
	%obj.vbl.createBricksForBLID($Spacebuild::StationBLID);
	
	//new field that doesn't exist in older files
	if (!%f.isEOF())
		%obj.moduleType = getField(%f.readLine(), 1);
	else
		%obj.moduleType = Module16x32Data; //all old modules are 16x32
	
	%f.close();
	%f.delete();
}

function loadModuleSO(%file)
{
	%mod = newModuleSO();
	%mod.import(%file);
	return %mod;
}

//***********************************************************
//* Scanning code
//***********************************************************
function ModuleSO::scanVBL(%mod, %vbl, %mcfs)
{
	for (%i = 0; %i < %vbl.getCount(); %i++)
		%mod.scanModuleBrick(%vbl.getVirtualBrick(%i));
	
	%mcfs.onFinish(%mod);
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
	%mod.scanModuleBrick(%sb);
}

//
function ModuleSO::scanModuleBrick(%mod, %sb)
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
		%mod.addHatch(%point, %dir);
	}
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

