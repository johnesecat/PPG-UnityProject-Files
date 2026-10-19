using NaughtyAttributes;
using UnityEngine;

public class ReactorConduitSetBehaviour : MonoBehaviour
{
	public Rigidbody2D[] NoCollide;

	public ReactorConduitBehaviour[] Conduits;

	[Button("Get conduits", EButtonEnableMode.Always)]
	private void Awake()
	{
		Conduits = GetComponentsInChildren<ReactorConduitBehaviour>();
	}

	private void Start()
	{
		for (int i = 0; i < NoCollide.Length; i++)
		{
			Collider2D[] componentsInChildren = NoCollide[i].GetComponentsInChildren<Collider2D>();
			for (int j = 0; j < Conduits.Length; j++)
			{
				Collider2D[] componentsInChildren2 = Conduits[j].GetComponentsInChildren<Collider2D>();
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					for (int l = 0; l < componentsInChildren2.Length; l++)
					{
						Physics2D.IgnoreCollision(componentsInChildren2[l], componentsInChildren[k]);
					}
				}
			}
		}
	}

	public ReactorA5DisplayBehaviour.ReactorPartStatus GetStatus()
	{
		int num = 0;
		bool flag = false;
		for (int i = 0; i < Conduits.Length; i++)
		{
			ReactorConduitBehaviour reactorConduitBehaviour = Conduits[i];
			num += (reactorConduitBehaviour.Broken ? 1 : 0);
			flag |= !reactorConduitBehaviour.HealthyWallJoint;
		}
		if (num >= Conduits.Length / 2)
		{
			return ReactorA5DisplayBehaviour.ReactorPartStatus.Catastrophic;
		}
		if (num > 2)
		{
			return ReactorA5DisplayBehaviour.ReactorPartStatus.Damaged;
		}
		if (flag || num > 0)
		{
			return ReactorA5DisplayBehaviour.ReactorPartStatus.Questionable;
		}
		return ReactorA5DisplayBehaviour.ReactorPartStatus.Normal;
	}
}
