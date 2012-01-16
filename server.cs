//This will exec all other SpaceBuild-specific files.

//First of all, Support_ToolBrick is required
%error = ForceRequiredAddOn("Support_ToolBrick");

if (%error == $Error::AddOn_Disabled)
{
	//Doesn't matter, Support_ToolBrick has nothing visible to the clients
}

if (%error == $Error::AddOn_NotFound)
{
	//we don't have the jeep, so we're screwed
	error("ERROR: Gamemode_Spacebuild - required add-on Support_ToolBrick not found");
}
else
{

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
}
