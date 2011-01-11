datablock AudioProfile(SBToolExplosionSound)
{
   filename    = "./sound/arrowHit.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(SBToolFireSound)
{
   filename    = "./sound/bowFire.wav";
   description = AudioClosest3d;
   preload = true;
};


//arrow trail
datablock ParticleData(SBToolTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 200;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/dot";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]	= "1 1 1 0.2";
	colors[1]	= "1 1 1 0.0";
	sizes[0]	= 0.2;
	sizes[1]	= 0.01;
	times[0]	= 0.0;
	times[1]	= 1.0;
};

datablock ParticleEmitterData(SBToolTrailEmitter)
{
   ejectionPeriodMS = 2;
   periodVarianceMS = 0;

   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;

   ejectionOffset = 0;

   thetaMin         = 0.0;
   thetaMax         = 90.0;  

   particles = SBToolTrailParticle;
};

//effects
datablock ParticleData(SBToolExplosionParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = 0.1;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 300;
	textureName          = "base/data/particles/chunk";
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]     = "0.9 0.9 0.6 0.9";
	colors[1]     = "0.9 0.5 0.6 0.0";
	sizes[0]      = 0.25;
	sizes[1]      = 0.0;
};

datablock ParticleEmitterData(SBToolExplosionEmitter)
{
   ejectionPeriodMS = 7;
   periodVarianceMS = 0;
   ejectionVelocity = 5;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SBToolExplosionParticle";
};

datablock ExplosionData(SBToolExplosion)
{
   //explosionShape = "";
	soundProfile = SBToolExplosionSound;

   lifeTimeMS = 150;

   particleEmitter = SBToolExplosionEmitter;
   particleDensity = 10;
   particleRadius = 0.2;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = false;
   camShakeFreq = "10.0 11.0 10.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 0.5;
   camShakeRadius = 10.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 2;
   lightStartColor = "0.3 0.6 0.7";
   lightEndColor = "0 0 0";
};


//projectile
datablock ProjectileData(SBToolProjectile)
{
   //projectileShapeName = "./shapes/arrow.dts";

   directDamage        = 0;
   directDamageType    = $DamageType::ArrowDirect;

   radiusDamage        = 0;
   damageRadius        = 0;
   radiusDamageType    = $DamageType::ArrowDirect;

   explosion           = SBToolExplosion;
   //particleEmitter     = SBToolTrailEmitter;

   muzzleVelocity      = 50;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 100;
   fadeDelay           = 70;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = false;
   gravityMod = 0.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";
};

datablock ItemData(SBToolItem : wandItem)
{
	uiName = "SB Tool";
	doColorShift = true;
	colorShiftColor = "0.1 0.7 0.05 1.000";
	image = SBToolImage;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(SBToolImage)
{
   // Basic Item properties
   shapeFile = "base/data/shapes/wand.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0; //"0.7 1.2 -0.5";
   rotation = eulerToMatrix( "0 0 10" );

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = SBToolItem;
   ammo = " ";
   projectile = SBToolProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   doColorShift = true;
   colorShiftColor = SBToolItem.colorShiftColor;

   //casing = " ";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]       = "Ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Reload";
	stateTimeoutValue[2]            = 0.05;
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
	stateSound[2]					= SBToolFireSound;

	stateName[3]			= "Reload";
	stateSequence[3]                = "Reload";
	stateAllowImageChange[3]        = false;
	stateTimeoutValue[3]            = 0.5;
	stateWaitForTimeout[3]		= true;
	stateTransitionOnTimeout[3]     = "Check";

	stateName[4]			= "Check";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.2;
	stateAllowImageChange[5]        = false;
	stateWaitForTimeout[5]		= true;
	//stateSequence[5]                = "Reload";
	stateScript[5]                  = "onStopFire";


};

function SBToolProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	%mcf = $DebugMCF;
	
	%client = %obj.client;
	
	if(!isObject(%col) || %col.getClassName() !$= "fxDTSBrick" || !%client.isAdmin)
		return;
	
	//if(getTrustLevel(%col, %client) < 1)
	//{
	//	commandToClient(%client, 'bottomPrint', "You must have trust to select a build!", 3);
	//	return;
	//}
	if (%col.getDatablock().getId() != brick1x4x3SpaceHatchData.getId())
		%mcf.scanBuild(%col);
	else
	{
		%mod = %mcf.popModule();
		if (isObject(%mod))
		{
			%mod.attachTo(0, %col.module, %col.hatchId);
		}
	}
}

function ServerCmdSBTool(%client)
{
	if (isObject(%client.player))
	{
		%client.player.updateArm(SBToolImage);
		%client.player.mountImage(SBToolImage, 0);
	}
}
