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