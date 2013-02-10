function MCF::setupQueue()
{
	%this.queue = new SimSet();
}

function MCF::removeQueue()
{
	for (%i = 0; %i < %this.queue.getCount(); %i++)
		%this.queue.getObject(0).delete();
	
	%this.queue.delete();
}

