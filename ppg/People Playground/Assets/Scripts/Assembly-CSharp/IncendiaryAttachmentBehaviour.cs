using UnityEngine;

public class IncendiaryAttachmentBehaviour : FirearmAttachmentBehaviour
{
	[Range(0f, 100f)]
	public int FireChance = 10;

	public override void OnConnect()
	{
	}

	public override void OnDisconnect()
	{
	}

	public override void OnFire()
	{
	}

	public override void OnHit(BallisticsEmitter.CallbackParams args)
	{
		if (!Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(args.HitObject.transform, out var value))
		{
			return;
		}
		if (Random.value <= (float)FireChance / 100f)
		{
			value.Ignite(args.Position);
			if (value.OnFire)
			{
				value.BurnIntensity = 0.5f;
			}
		}
		value.BurnProgress += 0.001f * FirearmBehaviour.Cartridge.Damage / (1.5f + Mathf.Max(0.01f, value.BurnProgress));
		value.Temperature += 0.05f / value.GetHeatCapacity() * 2f * (FirearmBehaviour.Cartridge.Damage / 10f);
	}
}
