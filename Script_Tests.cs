//This function, given a brick, sets up a station and adds a module to it, which
//is scanned in from the given brick.
function testSave(%brick, %x, %y, %z, %file)
{
	setupSpace();
	%mcf = $DebugMCF;
	%mcf.scanBuild(%brick);
	firstModule(%x, %y, %z);
	%station = $DebugStation;
	%station.export(%file);
}

function ServerCmdTestSpaceSave(%client, %file)
{
	if (%client.isAdmin)
	{
		if (%file $= "")
			%file = "DebugSpaceSave";
		
		%file = "saves/Spacebuild/" @ %file @ ".sbs";
		
		%brick = %client.wrenchBrick;
		if (isObject(%brick))
		{
			testSave(%brick, 0, 0, 100, %file);
			messageClient(%client, '', "\c2Test Save done.");
		}
		else
			messageClient(%client, '', "\c2Error: Brick does not exist.");
	}
}
