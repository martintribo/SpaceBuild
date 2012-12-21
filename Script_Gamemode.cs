function setupSpacebuild()
{
	createSpaceObjects();

	
	
	

}

function createSpaceObjects()
{
	%mg = $DefaultMiniGame;
	
	%mg.station = new ScriptObject(StationSO);
	%mg.mcf = new ScriptObject(MCFacility);
	%mg.mcf.setPosition($Spacebuild::MCFPosition);
	
	%mcl = new ScriptObject($Spacebuild::DefaultMCL);
	%mg.mcf.setMCL(%mcl);
	
	%defaultStation = $Spacebuild::SavePath @ $Spacebuild::StationFile;
	%defaultMCF = $Spacebuild::SavePath @ $Spacebuild::MCFFile;
	
	if (isFile(%defaultStation))
		%mg.station.import(%defaultStation);
	if (isFile(%defaultMCF))
		%mg.mcf.import(%defaultMCF);
}