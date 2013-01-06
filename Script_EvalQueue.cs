function EvalQueue::onAdd(%this, %obj)
{
	%obj.statementCount = 0;
}

function EvalQueue::addStatement(%obj, %statement)
{
	//protect from duplicate statements being called multiple times
	for (%i = 0; %i < %obj.statementCount; %i++)
	{
		if (%statement $= %obj.statements[%i])
			return;
	}
	%obj.statements[%obj.statementCount] = %statement;
	%obj.statementCount++;
}

function EvalQueue::runStatements(%obj)
{
	for (%i = 0; %i < %obj.statementCount; %i++)
		eval(%obj.statements[%i]);
	
	%obj.statementCount = 0;
}
