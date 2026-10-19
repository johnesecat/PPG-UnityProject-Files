using UnityEngine;
using UnityEngine.Events;

public class ActOnShot : MonoBehaviour, Messages.IShot
{
	[SkipSerialisation]
	public float ShotDamageThreshold;

	[SkipSerialisation]
	public float CartridgeDamageThreshold;

	[SkipSerialisation]
	public float Chance = 1f;

	[SkipSerialisation]
	public UnityEvent Actions;

	public void Shot(Shot shot)
	{
		if (shot.damage >= ShotDamageThreshold && (!shot.cartridge || shot.cartridge.Damage >= CartridgeDamageThreshold) && Random.value <= Chance)
		{
			Actions.Invoke();
		}
	}
}
