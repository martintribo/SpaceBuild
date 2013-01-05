function SpaceBuildAutosave()
{
	if($SpacebuildAutoloadComplete)
	{
		%defaultStationPath = $Spacebuild::SavePath @ $Spacebuild::StationFile;
		%defaultMCFPath = $Spacebuild::SavePath @ $Spacebuild::MCFFile;
		
		commandToAll('centerPrint', "\c6Autosaving SpaceBuild...", 50);
		
		echo(getDateTime() @ " - AUTOSAVING STATION...");
		$DefaultMinigame.station.export(%defaultStationPath);
		echo(getDateTime() @ " - AUTOSAVING MCF...");
		$DefaultMinigame.mcf.export(%defaultMCFPath);
		echo(getDateTime() @ " - AUTOSAVES COMPLETE.");
		
		commandToAll('centerPrint', "\c6Autosaving SpaceBuild...\c3done\c6!", 2);
	}
	
	if(!isEventPending($SBAutosaveTick))
		$SBAutosaveTick = schedule($Spacebuild::AutosaveTime, 0, "SpaceBuildAutosave");
}

$SBAutosaveTick = schedule($Spacebuild::AutosaveTime, 0, "SpaceBuildAutosave");