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
	if (%brick.getDatablock().getId() == brick1x4x3SpaceHatchData.getId())
		%obj.addHatch();
}

function ModuleSO::addHatch(%obj, %point, %dir1, %dir2)
{
	//%obj.hatches[%obj.numHatches, "point"] = %point;
	//%obj.hatches[%obj.numHatches, "direction"] = %dir1;
	%obj.vbl.addMarker("Hatch" @ %obj.numHatches, %point, %dir1, %dir2);
	%obj.numHatches++;
}

function ModuleSO::attachTo(%obj, %objHatch, %mod, %modHatch)
{
	echo("got in attach");
	//besides just setting up links and etc, the states and owners of modules must be changed
	%obj.vbl.markers["hatch" @ %objHatch].alignWith(%mod.vbl.markers["hatch" @ %modHatch]);
	echo("got through alignwith");
	//%obj.vbl.createBricks();
	%obj.setPosition(%obj.vbl.getCenter());
	echo("got through set position");
	%obj.owner = %mod.owner; //update ownership, change?
	%obj.deploy();
	echo("got through deploy");
}

function ModuleSO::deploy(%obj)
{
	%obj.setState("Deployed");
	%obj.vbl.createBricks();
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
	function virtualBrickList::onCreateBrick(%obj, %b)
	{
		if (%b.getDatablock().getId() == brick1x4x3SpaceHatchData.getId())
		{
			echo("createbrick");
			%b.module = %obj.module;
			%b.setColliding(1); //it's easier for it not to be colliding on the ground, but it needs to be solid in space
		}
		Parent::onCreateBrick(%obj, %b);
	}
};

activatePackage(ModulePack);

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