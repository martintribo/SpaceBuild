function newModuleSO(%vbl)
{
	if (!isObject(%vbl))
		%vbl = newVBL(1);
	%mod = new ScriptObject()
	{
		class = "ModuleSO";
		vbl = %vbl;
	};
	%vbl.module = %mod;
	
	return %mod;
}

function ModuleSO::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.numHatches = 0;
	%obj.state = "none"; //none state basically means the module isn't represented at all in spaceyou 
	//%obj.state
	//%obj.owner`
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
	%obj.setPosition(%obj.vbl.getCenter());
	
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
		if (%obj.getHatchType(%i) $= %type)
			%list = %list @ %i @ "\t";
	
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
	%obj.vbl.createBricksNoOwner();
	
	%f.close();
	%f.delete();
}

function loadModuleSO(%file)
{
	%mod = newModuleSO();
	%mod.import(%file);
	return %mod;
}












