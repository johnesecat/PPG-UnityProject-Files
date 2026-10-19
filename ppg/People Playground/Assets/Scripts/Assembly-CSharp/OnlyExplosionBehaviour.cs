using UnityEngine;

public class OnlyExplosionBehaviour : MonoBehaviour
{
	public ExplosionCreator.ExplosionParameters ExplosionParameters;

	public bool ExplodeOnStart;

	private void Start()
	{
		if (ExplodeOnStart)
		{
			Explode();
		}
	}

	public void Explode()
	{
		ExplosionParameters.Position = base.transform.position;
		ExplosionCreator.Explode(ExplosionParameters);
	}
}
