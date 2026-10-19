using System;
using System.Collections;
using UnityEngine;

public class GlobalShrapnelEmitter : MonoBehaviour
{
	public Cartridge Cartridge;

	public LayerMask LayersToHit;

	public LayerMask WaterLayer;

	[NonSerialized]
	public BallisticsEmitter BallisticsEmitter;

	public static GlobalShrapnelEmitter Instance { get; private set; }

	public void Awake()
	{
		BallisticsEmitter = new BallisticsEmitter(this, Cartridge);
		BallisticsEmitter.RenderTracersIfTraversal = false;
		BallisticsEmitter.DoTraversal = true;
		BallisticsEmitter.MaxBallisticsIterations = 16u;
		BallisticsEmitter.RicochetChanceMultiplier = 4f;
		BallisticsEmitter.WaterLayer = WaterLayer;
		BallisticsEmitter.LayersToHit = LayersToHit;
		BallisticsEmitter.MaxTotalDistance = 30f;
		Instance = this;
	}

	public void EmitShrapnel(Vector2 position, int amount)
	{
		StartCoroutine(EmitRoutine(position, amount));
	}

	private IEnumerator EmitRoutine(Vector2 position, int amount)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForFixedUpdate();
		for (int i = 0; i < amount; i++)
		{
			BallisticsEmitter.Emit(position, UnityEngine.Random.insideUnitCircle);
		}
	}

	private void OnDestroy()
	{
		BallisticsEmitter.Dispose();
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
