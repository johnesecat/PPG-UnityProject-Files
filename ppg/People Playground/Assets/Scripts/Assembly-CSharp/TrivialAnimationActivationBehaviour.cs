using UnityEngine;

public class TrivialAnimationActivationBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public TrivialAnimationBehaviour AnimationBehaviour;

	private PhysicalBehaviour phys;

	private int heat;

	private float time;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Update()
	{
		if (phys.IsBeingUsedContinuously())
		{
			heat = 2;
		}
		heat--;
		if (heat > 0)
		{
			time += Time.deltaTime / AnimationBehaviour.Duration;
		}
		else
		{
			time -= Time.deltaTime / AnimationBehaviour.Duration;
		}
		time = Mathf.Clamp01(time);
		AnimationBehaviour.ForceSetProgress(time);
	}
}
