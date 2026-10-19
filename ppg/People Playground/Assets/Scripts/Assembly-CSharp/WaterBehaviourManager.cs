using System.Collections.Generic;

public class WaterBehaviourManager : BehaviourManager<WaterBehaviour>
{
	protected override void FixedUpdate()
	{
		for (int i = 0; i < Global.main.PhysicalObjectsInWorld.Count; i++)
		{
			Global.main.PhysicalObjectsInWorld[i].underwaterMarkings = 0;
		}
		base.FixedUpdate();
		for (int j = 0; j < Global.main.PhysicalObjectsInWorld.Count; j++)
		{
			PhysicalBehaviour physicalBehaviour = Global.main.PhysicalObjectsInWorld[j];
			physicalBehaviour.IsUnderWater = physicalBehaviour.underwaterMarkings > 0;
		}
	}

	protected override IList<WaterBehaviour> GetCollection()
	{
		return WaterBehaviour.waters;
	}
}
