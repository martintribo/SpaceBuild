datablock ParticleData(shuttleFlameParticle)
{
   dragCoefficient      = 0;
   windCoefficient     = 0;
   gravityCoefficient   = 0;
   inheritedVelFactor   = 0.0;
   constantAcceleration = 0.0;
   spinRandomMin = -90;
   spinRandomMax = 90;
   lifetimeMS           = 500;
   lifetimeVarianceMS   = 0;
   textureName          = "base/data/particles/cloud";
   colors[0]     = "1 1 1 1";
   colors[1]     = "0.2 0.2 1 0";
   sizes[0]      = 0.1;
   sizes[1]      = 0.05;
};

datablock ParticleEmitterData(shuttleFlameEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "shuttleFlameParticle";
};

datablock ShapeBaseImageData(shuttleFlameImage)
{
  shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = 7;
  rotation = "1 0 0 -90";

	stateName[0]					= "Ready";
	stateTransitionOnTimeout[0]		= "FireA";
	stateTimeoutValue[0]			= 0.01;

	stateName[1]					= "FireA";
	stateTransitionOnTimeout[1]		= "Done";
	stateWaitForTimeout[1]			= true;
	stateTimeoutValue[1]			= 10000;
	stateEmitter[1]					= burnEmitterA; //shuttleFlameEmitter;
	stateEmitterTime[1]				= 10000;

	stateName[2]					= "Done";
	stateTransitionOnTimeout[2]		= "FireA";
	stateTimeoutValue[2]			= 0.01;
	
	//stateScript[2]					= "onDone";
};

//Makes the particle stick with moving objects better..
burnParticleA.inheritedVelFactor = 0.725;

//function shuttleFlameImage1::onDone(%this,%obj,%slot)
//{
//	%obj.unMountImage(%slot);
//	%obj.mountImage(%slot, shuttleFlameImage1);
//}
