public class ExplosiveAttachmentBehaviour : FirearmAttachmentBehaviour
{
	[SkipSerialisation]
	public ExplosionCreator.ExplosionParameters ExplosionParams;

	private ExplosionCreator.ExplosionParameters adjustedParams;

	public override void OnConnect()
	{
		adjustedParams = ExplosionParams;
		adjustedParams.Range *= FirearmBehaviour.Cartridge.Damage / 10f;
		adjustedParams.FragmentForce *= FirearmBehaviour.Cartridge.Damage / 10f;
		adjustedParams.EffectSize = GetSizeForDamage(FirearmBehaviour.Cartridge.Damage);
		FirearmBehaviour.BallisticsEmitter.ExplosiveRoundParams = adjustedParams;
		FirearmBehaviour.BallisticsEmitter.ExplosiveRound = true;
		FirearmBehaviour.BallisticsEmitter.ExplodeAtHitPoint = true;
	}

	public static ExplosionCreator.EffectSize GetSizeForDamage(float dmg)
	{
		if (dmg > 100f)
		{
			return ExplosionCreator.EffectSize.Large;
		}
		if (dmg > 50f)
		{
			return ExplosionCreator.EffectSize.Medium;
		}
		return ExplosionCreator.EffectSize.Small;
	}

	public override void OnDisconnect()
	{
		FirearmBehaviour.BallisticsEmitter.ExplosiveRound = false;
		FirearmBehaviour.BallisticsEmitter.ExplodeAtHitPoint = false;
	}

	public override void OnFire()
	{
	}

	public override void OnHit(BallisticsEmitter.CallbackParams args)
	{
	}
}
