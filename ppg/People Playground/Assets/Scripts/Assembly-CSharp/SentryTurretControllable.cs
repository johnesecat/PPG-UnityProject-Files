using UnityEngine;

public abstract class SentryTurretControllable : MonoBehaviour
{
	public abstract Vector2 BarrelPosition { get; }

	public abstract void Disable();
}
