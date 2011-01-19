//Contains shuttle flame emitter/image for mounting.
exec("./Particle_Shuttle.cs");

datablock WheeledVehicleData(SpaceShuttleVehicle)
{
	category = "Vehicles";
	displayName = " ";
	shapeFile = "Add-ons/Vehicle_Jeep/jeep.dts"; //"~/data/shapes/skivehicle.dts"; //
	emap = true;
	minMountDist = 3;

	numMountPoints = 7;
	mountThread[0] = "sit";
	mountThread[1] = "sit";
	mountThread[2] = "sit";
	mountThread[3] = "sit";
	mountThread[4] = "sit";
	mountThread[5] = "root";
	mountThread[6] = "root";
	mountThread[7] = "sit";

	maxDamage = 200.00;
	destroyedLevel = 200.00;
	speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;

	massCenter = "0 0 0";
	//massBox = "2 5 1";

	maxSteeringAngle = 0.9785;  // Maximum steering angle, should match animation
	integration = 4;           // Force integration time: TickSec/Rate
	tireEmitter = VehicleTireEmitter; // All the tires use the same dust emitter

	// 3rd person camera settings
	cameraRoll = false;         // Roll the camera with the vehicle
	cameraMaxDist = 13;         // Far distance from vehicle
	cameraOffset = 7.5;        // Vertical offset from camera mount point
	cameraLag = 0.0;           // Velocity lag of camera
	cameraDecay = 0.75;        // Decay per sec. rate of velocity lag
	cameraTilt = 0.4;
	collisionTol = 0.1;        // Collision distance tolerance
	contactTol = 0.1;

	useEyePoint = false;	

	defaultTire	= jeepTire;
	defaultSpring	= jeepSpring;
	//flatTire	= jeepFlatTire;
	//flatSpring	= jeepFlatSpring;

	numWheels = 4;

	// Rigid Body
	mass = 300;
	density = 5.0;
	drag = 1.6;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;        // Impacts over this invoke the script callback
	softImpactSpeed = 10;       // Play SoftImpact Sound
	hardImpactSpeed = 15;      // Play HardImpact Sound
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000; //4000;       // Engine power
	engineBrake = 2000;         // Braking when throttle is 0
	//brakeTorque = 50000;        // When brakes are applied
	brakeTorque = 0;
	maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	rollForce		= 2500;
	yawForce		= 2500;
	pitchForce		= 2500;
	rotationalDrag		= 0.2;

	// Advanced Steering
	steeringAutoReturn = true;
	steeringAutoReturnRate = 0.9;
	steeringAutoReturnMaxSpeed = 10;
	//steeringUseStrafeSteering = true;
	//steeringStrafeSteeringRate = 0.1;

	// Energy
	maxEnergy = 100;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;

	splash = vehicleSplash;
	splashVelocity = 4.0;
	splashAngle = 67.0;
	splashFreqMod = 300.0;
	splashVelEpsilon = 0.60;
	bubbleEmitTime = 1.4;
	splashEmitter[0] = vehicleFoamDropletsEmitter;
	splashEmitter[1] = vehicleFoamEmitter;
	splashEmitter[2] = vehicleBubbleEmitter;
	mediumSplashSoundVelocity = 10.0;   
	hardSplashSoundVelocity = 20.0;   
	exitSplashSoundVelocity = 5.0;

	//mediumSplashSound = "";
	//hardSplashSound = "";
	//exitSplashSound = "";

	// Sounds
	//   jetSound = ScoutThrustSound;
	//engineSound = idleSound;
	//squealSound = skidSound;
	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;
	//wheelImpactSound = slowImpactSound;

	//   explosion = VehicleExplosion;
	justcollided = 0;

	uiName = "Space Shuttle";
	rideable = true;
	lookUpLimit = 0.65;
	lookDownLimit = 0.45;

	paintable = true;

	damageEmitter[0] = VehicleBurnEmitter;
	damageEmitterOffset[0] = "0.0 0.0 0.0 ";
	damageLevelTolerance[0] = 0.99;

	damageEmitter[1] = VehicleBurnEmitter;
	damageEmitterOffset[1] = "0.0 0.0 0.0 ";
	damageLevelTolerance[1] = 1.0;

	numDmgEmitterAreas = 1;

	initialExplosionProjectile = jeepExplosionProjectile;
	initialExplosionOffset = 0;         //offset only uses a z value for now

	burnTime = 4000;

	finalExplosionProjectile = jeepFinalExplosionProjectile;
	finalExplosionOffset = 0.5;          //offset only uses a z value for now

	minRunOverSpeed    = 4;   //how fast you need to be going to run someone over (do damage)
	runOverDamageScale = 8;   //when you run over someone, speed * runoverdamagescale = damage amt
	runOverPushScale   = 1.2; //how hard a person you're running over gets pushed

	//protection for passengers
	protectPassengersBurn   = false;  //protect passengers from the burning effect of explosions?
	protectPassengersRadius = true;  //protect passengers from radius damage (explosions) ?
	protectPassengersDirect = false; //protect passengers from direct damage (bullets) ?

	steeringUseStrafeSteering = false;

	horizontalSurfaceForce	= 10;   // Horizontal center "wing" (provides "bite" into the wind for climbing/diving and turning)
	verticalSurfaceForce	= 10; 
};

function SpaceShuttleVehicle::onTrigger(%this, %obj, %trig, %val)
{
	if (%trig == 2)
	{
		if (%val)
		{
			%obj.launchTick();
			//Mount a nice emitter.
			//%obj.mountImage(shuttleFlameImage, 7);
		}else{
			if (isEventPending(%obj.launchTick))
			{
				cancel(%obj.launchTick);
				//Unmount the emitter.
				//%obj.unMountImage(7);
			}
		}
	}
}

function WheeledVehicle::launchTick(%obj)
{
	if (isEventPending(%obj.launchTick))
		cancel(%obj.launchTick);
	%vec = %obj.getForwardVector();
	%obj.applyImpulse(%obj.getPosition(), VectorScale(%vec, 2000));
	%obj.launchTick = %obj.schedule(100, "launchTick");
}

package shuttleTrust
{
	function getTrustLevel(%obj, %other, %a1, %a2)
	{
		if(%obj.getDataBlock() != 0 && %obj.getDataBlock().getName() $= "SpaceShuttleVehicle")
			return 1;
		
		if(%other.getDataBlock() != 0 && %other.getDataBlock().getName() $= "SpaceShuttleVehicle")
			return 1;
		
		Parent::getTrustLevel(%obj, %other, %a1, %a2);
	}
	
	//This disabled trust for passengers, but only people with trust can get in as drivers. Not quite ideal, but may be useful in the future.
	// function getTrustLevel(%obj, %other, %a1, %a2)
	// {
		// if(%obj.getDataBlock() != 0 && %obj.getDataBlock().getName() $= "SpaceShuttleVehicle" && isObject(%obj.getMountedObject(0)))
			// return 1;
		
		// if(%other.getDataBlock() != 0 && %other.getDataBlock().getName() $= "SpaceShuttleVehicle" && isObject(%other.getMountedObject(0)))
			// return 1;
		
		// Parent::getTrustLevel(%obj, %other, %a1, %a2);
	// }
};

activatePackage(shuttleTrust);