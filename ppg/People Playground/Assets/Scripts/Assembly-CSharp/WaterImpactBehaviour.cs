using System.Collections.Generic;
using UnityEngine;

public class WaterImpactBehaviour : MonoBehaviour, Messages.IShot
{
	public GameObject Splash;

	public float VolumeDenominator = 25f;

	public bool AffectVolume = true;

	private static readonly List<ParticleSystem> buffer = new List<ParticleSystem>(4);

	public void Shot(Shot shot)
	{
		GameObject gameObject = (Splash.CompareTag("Poolable") ? PoolGenerator.Instance.RequestPrefab(Splash, shot.point) : Object.Instantiate(Splash, shot.point, Quaternion.identity));
		if (!gameObject)
		{
			return;
		}
		Vector2 normal = shot.normal;
		normal.y = 0f - Mathf.Abs(normal.y);
		gameObject.transform.up = Vector2.Reflect(normal, Vector2.up);
		int num = 1;
		float num2 = Mathf.Pow(shot.damage, 0.5f) * 0.9f;
		if (AffectVolume)
		{
			gameObject.GetComponentInChildren<AudioSource>().volume = num2 / VolumeDenominator;
		}
		gameObject.GetComponentsInChildren(buffer);
		for (int i = 0; i < buffer.Count; i++)
		{
			ParticleSystem particleSystem = buffer[i];
			if ((bool)particleSystem)
			{
				ParticleSystem.ShapeModule shape = particleSystem.shape;
				ParticleSystem.MainModule main = particleSystem.main;
				shape.radius = (float)num * 0.01f;
				main.startSizeMultiplier = (float)num * 0.15f * Random.Range(1f, 1.4f);
				main.startSpeedMultiplier = num2;
			}
		}
	}
}
