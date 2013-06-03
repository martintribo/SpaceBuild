//This will exec all other SpaceBuild-specific files.

//disable required add-on ui elements
JeepVehicle.uiName = "";

//First execute constants file
exec("./Script_Constants.cs");

//EvalQueue, temporary solution to events limiting object creation
exec("./Script_EvalQueue.cs");

//Space bricks (hatches)
exec("./Brick_Space.cs");

//Cargo player (not yet used, waste of datablocks)
//exec("./Player_Cargo.cs");

//Module Types, used my MCF and Modules
exec("./Module_Types.cs");

//Module creation facility script object, generation functions
exec("./Script_MCF.cs");
exec("./Script_MCSlot.cs");

//Modules script object
exec("./Script_Modules.cs");

//Station script object
exec("./Script_Station.cs");

//SBTool, for interfacing with the gamemode
exec("./Tool_SBTool.cs");

//Space Shuttle (also execs Particle_Shuttle.cs)
exec("./Vehicle_SpaceShuttle.cs");

//SBTick (currently burning up and station locator)
exec("./Script_SBTick.cs");

//Contains no-gravity player scripts (should be replaced soon)
exec("./Script_Gravity.cs");

//Enforces rules for the server (no hammers/wrenches/wands/building in space)
exec("./Script_Rules.cs");

//Events for interacting with the MCF
exec("./Event_Space.cs");

//Contains helper functions for testing out specific parts of the gamemode
exec("./Script_Tests.cs");

//SpaceBuild player, that can easily navigate in a zero-gravity environment (right click)
exec("./Player_Space.cs");

exec("./MCF/base.cs");

//Core gamemode scripts (loading up initial station, etc)
exec("./Script_Gamemode.cs");

//Autosave the station and MCF
exec("./Script_Autosave.cs");

//RTB Registration
if(isFile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	if(!$RTB::RTBR_ServerControl_Hook)
		exec("Add-Ons/System_ReturnToBlockland/hooks/serverControl.cs");
	
	RTB_registerPref("Max Module Saves", $Spacebuild::Name, "$Spacebuild::Prefs::MaxModuleSaves", "int 0 9999", $Spacebuild::AddOnFolderName, 30, 0, 1);
}
else
{
	$Spacebuild::Prefs::MaxModuleSaves = 9;
}



	

package SpacebuildGamemode
{
	function MiniGameSO::onAdd(%obj)
	{
		Parent::onAdd(%obj);
		
		if (%obj.owner != 0)
			return;
			
		//The gamemode system cannot load the hammer, so we must do it ourselves
		%obj.StartEquip[0] = HammerItem.getId();
		
		schedule("20000", 0, "setupSpacebuild", %obj);
	}
	
	function setSkyBox(%sky)
	{
		%sky = "Add-Ons/Sky_SpaceBuild/SpaceBuild.dml";
		Parent::setSkyBox(%sky);
	}
	
	//Normally making the ground transparent seems to make the bottom texture render, but we want it to anyways in this case
	function EnvGuiServer::SetAdvancedMode()
	{
		Parent::SetAdvancedMode();
		
		Sky.noRenderBans = 1;
		Sky.renderBottomTexture = 1;
		
		Sky.setScale("1 1 1");
	}
	
	//teleport the player to the module slot on spawn if they have one
	function GameConnection::spawnPlayer(%client)
	{
		parent::spawnPlayer(%client);
		
		if($DefaultMinigame.mcf.findSlotByBLID(%client.bl_id) != -1)
			%client.teleportToModuleSlot();
	}
	
	function GameConnection::onClientLeaveGame(%client)
	{
		%slot = $DefaultMinigame.mcf.findSlotByBLID(%client.bl_id);
		
		if(%slot != -1)
		{
			if(%slot.isEmpty())
			{
				$DefaultMinigame.mcf.deleteSlot(%slot); //if the player didn't build anything, we can just get rid of the slot when they leave
			}else{
				%slot.getLastActive(); //force the lastActive property of the slot to be refreshed to now
			}
		}
		
		parent::onClientLeaveGame(%client);
	}
	
	//(RTB also packages this function, so I'm pretty sure it's the right one to use, though it may be meant for client scripts)
	function disconnectedCleanup()
	{
		if(isEventPending($SBTickSched))
			cancel($SBTickSched);
		if(isEventPending($gravityTickSched))
			cancel($gravityTickSched);
		if(isEventPending($SBAutosaveTick))
			cancel($SBAutosaveTick);
		
		parent::disconnectedCleanup();
	}
	
	function MiniGameSO::pickSpawnpoint(%obj, %client)
	{
		if (isObject(%obj.spawns) && %obj.spawns.getCount())
		{
			%id = getRandom(%obj.spawns.getCount() - 1);
			return %obj.spawns.getObject(%id).spacebuildSpawnPoint();
		}
		else
			return Parent::pickSpawnpoint(%obj, %client);
	}
	
	function fxDTSBrick::spacebuildSpawnPoint(%this)
	{
		%point	= %this.getPosition();
		%trans	= %point SPC getWords(%this.getTransform(), 3, 6);
		
		return %trans;
	}
};

activatePackage(SpacebuildGamemode);
