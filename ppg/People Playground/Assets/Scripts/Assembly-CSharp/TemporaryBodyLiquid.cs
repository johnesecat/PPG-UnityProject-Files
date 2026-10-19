using UnityEngine;

public abstract class TemporaryBodyLiquid : Liquid
{
	public virtual float RemovalChancePerSecond { get; } = 0.8f;


	public virtual bool ShouldCallOnEnterEveryUpdate => true;

	public override void OnUpdate(BloodContainer c)
	{
		CirculationBehaviour circulationBehaviour = c as CirculationBehaviour;
		if ((object)circulationBehaviour != null)
		{
			if (Random.value < RemovalChancePerSecond * RemovalChancePerSecond)
			{
				c.RemoveLiquid(this, c.GetAmount(this));
			}
			if (ShouldCallOnEnterEveryUpdate)
			{
				OnEnterLimb(circulationBehaviour.Limb);
			}
		}
	}
}
