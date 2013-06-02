//storage radius: 4.5 8.5 4.6
//module radius: 4 8 4.1
//module center is .3 world units above storage's center
//module bottom is 3.8 world units below storage's center
//module corners are 4 and 8 from center
function getStorage16x32Vbl()
{
	if (!isObject(storage16x32Vbl))
	{
		%vbl = new ScriptObject(storage16x32Vbl)
		{
			class = "VirtualBrickList";
		};
		%vbl.loadBLSFile($Spacebuild::AddOnPath @ "MCF/storage16x32.bls");
	}
	else
		%vbl = storage16x32Vbl.getId();
	
	return %vbl;
}

function Storage16x32::onAdd(%this, %obj)
{
	//%obj.position
	%obj.bricksCreated = false;
	%obj.factory = new ScriptObject(Storage16x32Factory)
	{
		class = "BrickFactory";
	};
	%obj.structureBricks = new SimSet();
}

function Storage16x32::onRemove(%this, %obj)
{
	%obj.deleteBricks();
	%obj.moduleBricks.delete();
	%Obj.structureBricks.delete();
	%obj.factory.delete();
}

function Storage16x32::createBricks(%obj, %blid)
{
	if (!%obj.bricksCreated || %obj.blid != %blid)
	{
		%obj.deleteBricks(%obj);
		%vbl = getStorage16x32Vbl();
		%vbl.recenter(%obj.getPosition());

		%factory.brickGroup = %obj.structureBricks;
		%factory.createBricksForBlid(%vbl, %blid);

		//if (%obj.moduleVbl

		%vbl.createBricksForBlid(%blid);
		%obj.bricksCreated = true;
	}
}

function Storage16x32::setPosition(%obj)
{
	if (%obj.position !$= %pos)
	{
		%obj.position = %pos;

		if (%obj.bricksCreated == true)
			%obj.createBricks();
	}
}

function Storage16x32::setCorner(%obj)
{
	if (%obj.position !$= %pos)
	{
		%obj.position		
	}
}

function Storage16x32::getPosition(%obj)
{
	return %obj.position;
}

function Storage16x32Factory::onCreateBrick(%obj, %brick)
{
	%obj.brickGroup.add(%brick);
}

function Storage16x32::deleteBricks(%obj)
{
	while (%obj.moduleBricks.getCount())
		%obj.moduleBricks.getObject(0).delete();
	while (%obj.structureBricks.getCount())
		%obj.structureBricks.getObject(0).delete();
	
	%obj.bricksCreated = false;
}

