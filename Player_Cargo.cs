if(!isObject(cargoGroup))
{
	new simGroup(cargoGroup);
}

//Animations for the cargo playertype (really just no animation, but this fixes some 'no animation exists!' errors)
datablock TSShapeConstructor(cargoPlayerDts)
{
	baseShape  = "./cargoPlayer.dts";
	sequence0  = "./cargo_root.dsq root";
	sequence1  = "./cargo_root.dsq run";
	sequence2  = "./cargo_root.dsq walk";
	sequence3  = "./cargo_root.dsq back";
	sequence4  = "./cargo_root.dsq side";
	sequence5  = "./cargo_root.dsq crouch";
	sequence6  = "./cargo_root.dsq crouchRun";
	sequence7  = "./cargo_root.dsq crouchBack";
	sequence8  = "./cargo_root.dsq crouchSide";
	sequence9  = "./cargo_root.dsq look";
	sequence10 = "./cargo_root.dsq headside";
	sequence11 = "./cargo_root.dsq headUp";
	sequence12 = "./cargo_root.dsq jump";
	sequence13 = "./cargo_root.dsq standjump";
	sequence14 = "./cargo_root.dsq fall";
	sequence15 = "./cargo_root.dsq land";
	sequence16 = "./cargo_root.dsq armAttack";
	sequence17 = "./cargo_root.dsq armReadyLeft";
	sequence18 = "./cargo_root.dsq armReadyRight";
	sequence19 = "./cargo_root.dsq armReadyBoth";
	sequence20 = "./cargo_root.dsq spearready";  
	sequence21 = "./cargo_root.dsq spearThrow";
	sequence22 = "./cargo_root.dsq talk";  
	sequence23 = "./cargo_root.dsq death1"; 
	sequence24 = "./cargo_root.dsq shiftUp";
	sequence25 = "./cargo_root.dsq shiftDown";
	sequence26 = "./cargo_root.dsq shiftAway";
	sequence27 = "./cargo_root.dsq shiftTo";
	sequence28 = "./cargo_root.dsq shiftLeft";
	sequence29 = "./cargo_root.dsq shiftRight";
	sequence30 = "./cargo_root.dsq rotCW";
	sequence31 = "./cargo_root.dsq rotCCW";
	sequence32 = "./cargo_root.dsq undo";
	sequence33 = "./cargo_root.dsq plant";
	sequence34 = "./cargo_root.dsq sit";
	sequence35 = "./cargo_root.dsq wrench";
  sequence36 = "./cargo_root.dsq activate";
  sequence37 = "./cargo_root.dsq activate2";
  sequence38 = "./cargo_root.dsq leftrecoil";
};

datablock PlayerData(PlayerCargo : PlayerStandardArmor)
{
	shapeFile = "./cargoPlayer.dts";
	boundingBox			= "1 1 1";				//vectorScale("1 1 1", 4);
  crouchBoundingBox	= "1 1 1";			//vectorScale("1 1 1", 4);
	
	jumpForce = 0;
	canJet = 0;
	
	maxForwardCrouchSpeed = "0";
	maxSideCrouchSpeed = "0";
	maxBackwardCrouchSpeed = "0";
	
	maxForwardSpeed = "0";
	maxSideSpeed = "0";
	maxBackwardSpeed = "0";
	
	maxDamage = 300;
	
	uiName = ""; //we don't want this player to be selectable - it does nothing, so that would be stupid
};

//Cargo players need to be in a group so they are affected by 
function PlayerCargo::onAdd(%this, %obj)
{
	cargoGroup.add(%obj);
}
