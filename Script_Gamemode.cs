function setupSpacebuild(%mg)
{
	%mg.evalQueue = new ScriptObject()
	{
		class = "EvalQueue";
	};
	
	loadRunway();
	createSpaceObjects(%mg);
}

function createSpaceObjects(%mg)
{
	%so = new ScriptObject(MCFacility);
	%mg.station = new ScriptObject(StationSO);
	%mg.mcf = new ScriptObject(MCFacility);
	%mg.mcf.setPosition($Spacebuild::MCFPosition);
	
	%mcl = new ScriptObject($Spacebuild::DefaultMCL);
	%mg.mcf.setMCL(%mcl);
	
	%defaultStationPath = $Spacebuild::SavePath @ $Spacebuild::StationFile;
	%defaultMCFPath = $Spacebuild::SavePath @ $Spacebuild::MCFFile;
	
	if (isFile(%defaultStationPath))
		%mg.station.import(%defaultStationPath);
	else
		createDefaultStation(%mg);
	
	if (isFile(%defaultMCFPath))
		%mg.mcf.import(%defaultMCFPath);
		
	$SpacebuildAutoloadComplete = true;
}

//temporary fix
function loadRunway()
{
	%runwayBLS = $Spacebuild::AddOnPath @ $Spacebuild::RunwayFile;
	%vbl = newVBL();
	%vbl.loadBLSFile(%runwayBLS);
	%vbl.createBricks();
	%vbl.delete();
}

function createDefaultStation(%mg)
{
	%defaultStationPath = $Spacebuild::SavePath @ $Spacebuild::StationFile;
	
	%vbl = newVBL();
	%vbl.loadBLSFile($Spacebuild::AddOnPath @ $Spacebuild::StarterModuleFile);
	%mg.mcf.scanVBL(%vbl);
	if (!%verificationError) //%verificationError is not defined?
	{
		%mod = %mg.mcf.popModule();
		%mod.state = "Deployed"; //Shouldn't have to manually do this
		error("Mod vbl count: " @ %mod.vbl.getCount());
		%mod.vbl.shiftBricks($Spacebuild::StationDisplacement);
		%mod.vbl.createBricks();
		%mg.station.addModule(%mod);
		
		%mg.station.export(%defaultStationPath);
	}
	%vbl.delete();
}
