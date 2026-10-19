using UnityEngine;

public abstract class CanShoot : MonoBehaviour
{
	public abstract Vector2 BarrelPosition { get; }

	public abstract void Shoot();
}
