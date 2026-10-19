using System.Collections.Generic;

public class LimbBehaviourManager : BehaviourManager<LimbBehaviour>
{
	public static readonly List<LimbBehaviour> Limbs = new List<LimbBehaviour>();

	private void Awake()
	{
		Limbs.Clear();
	}

	protected override IList<LimbBehaviour> GetCollection()
	{
		return Limbs;
	}
}
