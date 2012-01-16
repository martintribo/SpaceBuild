datablock PlayerData(PlayerSpace : PlayerStandardArmor)
{
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

	uiName = "SpaceBuild Player";
	showEnergyBar = false;
};

package SpaceBuildPlayerPackage {
	function PlayerSpace::onTrigger(%this, %obj, %trigger, %val)
	{
		if(%trigger == 4 && %val && %obj.isInSpace())
		{
			//do a raycast
			%mouseVec = %obj.getEyeVector();
			%cameraPoint = %obj.getEyePoint();
			%selectRange = 18;
			%mouseScaled = VectorScale(%mouseVec, %selectRange);
			%rangeEnd = VectorAdd(%cameraPoint, %mouseScaled);
			%searchMasks = $TypeMasks::FxBrickObjectType;
			%target = ContainerRayCast(%cameraPoint, %rangeEnd, %searchMasks, %obj);
			
			%brick = getWord(%target, 0);
			if(isObject(%brick))
			{
				%pos = getWords(%target, 1, 3);
				
				//push toward pos
				%vel = %obj.getVelocity();
				%addVel = vectorSub(%pos, %obj.getPosition());
				%addVel = vectorNormalize(%addVel);
				
				//make sure this push won't cause fall damage death
				%maxSpeed = %this.minImpactSpeed - vectorLen(%vel);
				%addVel = vectorScale(%addVel, mClamp(4, 0, %maxSpeed)); //this 4 is the strength
				
				%obj.setVelocity(vectorAdd(%vel, %addVel));
			}
		}
		
		parent::onTrigger(%this, %obj, %trigger, %val);
	}
};
activatePackage(SpaceBuildPlayerPackage);