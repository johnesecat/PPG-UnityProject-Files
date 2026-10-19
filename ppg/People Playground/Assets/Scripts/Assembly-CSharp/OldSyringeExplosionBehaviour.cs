using System;
using UnityEngine;

[Obsolete]
public class OldSyringeExplosionBehaviour : MonoBehaviour
{
	public float Radius;

	public Type PoisonType;

	public float[] Fingerprint;

	public Color Colour;

	private void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = component.main;
		main.startColor = Colour;
		Collider2D[] array = new Collider2D[256];
		int num = Mathf.Min(array.Length, Physics2D.OverlapCircleNonAlloc(base.transform.position, Radius, array));
		for (int i = 0; i < num; i++)
		{
			LimbBehaviour component2 = array[i].GetComponent<LimbBehaviour>();
			if (!component2 || component2.IsAndroid)
			{
				continue;
			}
			if (component2.TryGetComponent(PoisonType, out var component3))
			{
				PoisonSpreadBehaviour poisonSpreadBehaviour = component3 as PoisonSpreadBehaviour;
				if (poisonSpreadBehaviour.AllowMultipleActivations)
				{
					poisonSpreadBehaviour.Start();
				}
			}
			else
			{
				PoisonSpreadBehaviour poisonSpreadBehaviour = component2.gameObject.AddComponent(PoisonType) as PoisonSpreadBehaviour;
				poisonSpreadBehaviour.Fingerprint = Fingerprint;
				poisonSpreadBehaviour.Limb = component2;
			}
		}
		component.Play();
	}
}
