using System.Collections.Generic;

public class PhysicalBehaviourManager : BehaviourManager<PhysicalBehaviour>
{
	protected override IList<PhysicalBehaviour> GetCollection()
	{
		return Global.main.PhysicalObjectsInWorld;
	}
}
