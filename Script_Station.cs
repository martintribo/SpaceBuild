function StationSO::onAdd(%this, %obj)
{
	//%obj.headModule
	
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
	if (!isObject(%obj.headModule))
		%obj.headModule = %mod;
	
	%obj.modules.add(%mod);
}

function StationSO::removeModule(%obj, %mod)
{
	%obj.modules.remove(%mod);

	if (isObject(%mod) && %obj.headModule.getId() == %mod.getId())
	{
		if (%obj.modules.getCount())
			%obj.headModule = %obj.modules.getObject(0);
		else
			%obj.headModule = 0;
	}
}

function StationSO::getPosition(%obj)
{
	if (isObject(%obj.headModule))
		return %obj.headModule.getPosition();
}

function StationSO::export(%obj, %file)
{
	%name = fileBase(%file);
	%path = filePath(%file);
	%f = new FileObject();
	
	%f.openForWrite(%file);
	
	%mod = %obj.headModule;
	if (isObject(%mod))
	{
		%modPath = %path @ "/" @ %name @ "_head" @ ".mod";
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
		for (%i = 0; %i < %numFields; %i++)
			%fields[%i] = getField(%line, %i);
		if (%fields[0] $= "mod")
		{
			%headModule = loadModuleSO(%fields[1], %obj);
			%obj.addModule(%headModule);
			%obj.headModule = %headModule;
		}
	}
	
	%f.close();
	%f.delete();
}

