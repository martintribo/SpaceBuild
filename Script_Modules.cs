function newModuleSO(%vbl)
{
	if (!isObject(%vbl))
		%vbl = newVBL(1);
	return new ScriptObject()
	{
		class = "ModuleSO";
		vbl = %vbl;
	};
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
}

function ModuleSO::addHatch(%obj, %point, %dir1, %dir2)
{
	%obj.hatches[%obj.numHatches, "point"] = %point;
	%obj.hatches[%obj.numHatches, "direction"] = %dir1;
	%obj.vbl.addMarker("Hatch" @ %obj.numHatches, %point, %dir1, %dir2);
	%obj.numHatches++;
}

function ModuleSO::attachTo(%obj, %objHatch, %mod, %modHatch)
{
	//besides just setting up links and etc, the states and owners of modules must be changed
	%obj.vbl.markers[%objHatch].alignWith(%mod.vbl.markers[%modHatch]);
	%obj.vbl.createBricks();
	%obj.setPosition(%obj.vbl.getCenter());
	%obj.deploy();
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
	%obj.setState(%state);
	%obj.setPosition(%pos);
}

function ModuleSO::getPosition(%obj)
{
	//this eventually needs to change depending on the state
	if(%state $= "cargo")
		return %obj.cargoPlayer.getPosition();
	else
		return %obj.vbl.getCenter();
}

function ModuleSO::setPosition(%obj, %pos)
{
	%obj.vbl.recenter(%pos);
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
