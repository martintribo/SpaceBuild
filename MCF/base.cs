//Module Construction Facility

function MCF::onAdd(%this, %obj)
{
	%obj.setupQueue();
}

function MCF::onRemove(%this, %obj)
{
	%obj.removeQueue();
}

exec("./queue.cs");
exec("./events.cs");
exec("./building.cs");
exec("./constructors.cs");

