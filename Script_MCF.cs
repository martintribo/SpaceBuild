//Module Construction Facility
//This needs to be able to store modules

function MCFacility::onAdd(%this, %obj)
{
	//create the queue for modules
	%obj.queue = new SimSet(); //assuming objects keep order inserted in, might be wrong
}

function MCFacility::scanBuild(%obj, %brick)
{
	//need to just initialize the scan here
	%mod = newModuleSO();
	%bf = new ScriptObject()
	{
		class = "BrickFinder";
	};
	%bf.setOnSelectCommand(%obj @ ".onFoundBrick(%sb, " @ %mod @ ");");
	%bf.setFinishCommand(%obj @ ".onFinishedFinding(" @ %mod @ ", " @ %bf @ ");");
	%bf.search(%brick, "chain", "all", "", 1);
}

function MCFacility::onFoundBrick(%obj, %sb, %mod)
{
	echo("%sb = " @ %sb);
	if (%sb.getDatablock().getId() == brick1x4x3SpaceHatchData.getId())
	{
		%box = %sb.getWorldBox();
		%pos = %sb.getPosition();
		switch (%sb.getAngleId())
		{
			case 0:
				%point = getWord(%pos, 0) SPC getWord(%box, 4) SPC getWord(%pos, 2);
			case 1:
				%point = getWord(%box, 3) SPC getWords(%pos, 1, 2);
			case 2:
				%point = getWord(%pos, 0) SPC getWord(%box, 1) SPC getWord(%pos, 2);
			case 3:
				%point = getWord(%box, 0) SPC getWords(%pos, 1, 2);
		}
		%sb.setColor(%mod.numHatches);
		%mod.addHatch(%point, %sb.getAngleId());
	}
	%mod.addBrick(%sb);
}

function MCFacility::onFinishedFinding(%obj, %mod, %bf)
{
	//delete the Brick Finder and make a new module with that vbl
	echo("onfinishedfinding" SPC %mod SPC %bf);
	%obj.addToQueue(%mod);
	%bf.delete();
}

function MCFacility::addToQueue(%obj, %mod)
{
	echo("add to queue" SPC %mod);
	%obj.queue.add(%mod);
}