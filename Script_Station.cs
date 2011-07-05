function StationSO::onAdd(%this, %obj)
{
	//%obj.base
	
	%obj.modules = new SimSet();
}

function StationSO::onRemove(%this, %obj)
{
	while (%obj.modules.getCount())
		%obj.modules.getObject(0).delete();
	
	%obj.modules.delete();
}

function StationSO::addModule(%obj, %mod)
{
	if (!isObject(%obj.base))
	{
		%obj.base = %mod;
	}
	
	%mod.owner = %obj;
	%obj.modules.add(%mod);
}

function StationSO::getPosition(%obj)
{
	if (isObject(%obj.base))
		return %obj.base.getPosition();
}

function StationSO::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	for (%m = 0; %m < %obj.modules.getCount(); %m++)
	{
		%mod = %obj.modules.getObject(%m);
		%modPath = %path @ "/" @ %name @ "_mod" @ %m @ ".mod";
		%mod.export(%modPath);
		%f.writeLine("mod" TAB %modPath);
	}
	%f.close();
	%f.delete();
}

function StationSO::import(%obj, %file)
{
	%f = new FileObject();
	
	%f.openForRead(%file);
	
	while (!%f.isEOF())
	{
		%line = %f.readLine();	
		%numFields = getFieldCount(%line);
		for (%i = 0; %i < %numFiels; %i++)
			%fields[%i] = getField(%line, %i);
		%obj.addModule(loadModuleSO(%fields[1]));
	}
	
	%f.close();
	%f.delete();
}

function StationSO::createBricks(%obj)
{
	for (%i = 0; %i < %obj.modules.getCount(); %i++)
	{
		%mod = %obj.modules.getObject(%i);
		%mod.vbl.createBricks();
	}
}












