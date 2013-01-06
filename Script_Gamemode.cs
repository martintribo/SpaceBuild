function setupSpacebuild(%mg)
{
	%mg.evalQueue = new ScriptObject()
	{
		class = "EvalQueue";
	};
	
	loadSpawn(%mg);
	createSpaceObjects(%mg);
	startTicks();
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

function loadSpawn(%mg)
{
	%factory = new ScriptObject(SpacebuildSpawnBrickFactory)
	{
		class = "BrickFactory";
		minigame = %mg;
		spawnDB = brickSpawnPointData.getId();
	};
	%runwayBLS = $Spacebuild::AddOnPath @ $Spacebuild::RunwayFile;
	%vbl = newVBL();
	%vbl.loadBLSFile(%runwayBLS);
	
	%mg.spawns = new SimSet();
	%factory.createBricksForBlid(%vbl, $Spacebuild::SpawnBLID);
	
	%vbl.delete();
	
	%factory.delete();
}

function SpacebuildSpawnBrickFactory::onCreateBrick(%this, %brick)
{
	if (%this.spawnDB == %brick.getDatablock().getId())
		%this.minigame.spawns.add(%brick);
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

function startTicks()
{
	//gravityTick will start itself once someone spawns
	SBTick();
	$SBAutosaveTick = schedule($Spacebuild::AutosaveTime, 0, "SpaceBuildAutosave"); //(don't autosave right away)
}
