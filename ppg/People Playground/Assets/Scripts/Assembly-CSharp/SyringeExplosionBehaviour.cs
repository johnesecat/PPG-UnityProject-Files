using System.Collections.Generic;
using UnityEngine;

public class SyringeExplosionBehaviour : MonoBehaviour
{
	public float Radius;

	public IEnumerable<Liquid> Liquids;

	public float Amount = 0.1f;

	public Color Colour;

	private static readonly Collider2D[] results = new Collider2D[64];

	private void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = component.main;
		main.startColor = Colour;
		int num = Mathf.Min(results.Length, Physics2D.OverlapCircleNonAlloc(base.transform.position, Radius, results));
		float amount = Amount / (float)num * 3f;
		for (int i = 0; i < num; i++)
		{
			LimbBehaviour component2 = results[i].GetComponent<LimbBehaviour>();
			if (!component2 || component2.IsAndroid)
			{
				continue;
			}
			foreach (Liquid liquid in Liquids)
			{
				component2.CirculationBehaviour.AddLiquid(liquid, amount);
			}
		}
		component.Play();
	}
}
