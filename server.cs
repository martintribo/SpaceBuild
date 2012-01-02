//This will exec all other SpaceBuild-specific files.

//Contains no-gravity player scripts (should be replaced soon)
exec("./Script_Gravity.cs");

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

//SBTick (currently just burning up script)
exec("./Script_SBTick.cs");

//Enforces rules for the server (no hammers/wrenches/wands/building in space)
exec("./Script_Rules.cs");

//Contains helper functions for testing out specific parts of the gamemode
exec("./Script_Tests.cs");
