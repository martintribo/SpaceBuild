//This will exec all other SpaceBuild-specific files.

//disable required add-on ui elements
JeepVehicle.uiName = "";

//First execute constants file
exec("./Script_Constants.cs");

//Space bricks (hatches)
exec("./Brick_Space.cs");

//Cargo player (not yet used, waste of datablocks)
//exec("./Player_Cargo.cs");

//Module creation facility script object, generation functions
exec("./Script_MCF.cs");
exec("./Script_MCSlot.cs");
exec("./Script_MCL_Grid.cs");

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

//Core gamemode scripts (loading up initial station, etc)
exec("./Script_Gamemode.cs");

package SpacebuildGamemode
{
	function MiniGameSO::onAdd(%obj)
	{
		Parent::onAdd(%obj);
		
		if (%obj.owner != 0)
			return;
			
		//The gamemode system cannot load the hammer, so we must do it ourselves
		%obj.StartEquip[0] = HammerItem.getId();
		
		setupSpacebuild();
	}
	
	function setSkyBox(%sky)
	{
		%sky = "Add-Ons/Gamemode_Spacebuild/sky/space.dml";
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
};

activatePackage(SpacebuildGamemode);
