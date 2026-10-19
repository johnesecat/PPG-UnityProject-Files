using UnityEngine;

public struct Shot
{
	public Vector2 normal;

	public Vector2 point;

	public float damage;

	public Cartridge cartridge;

	public bool CanCrush;

	public bool triggersExplosiveOverride;

	public Shot(Vector2 normal, Vector2 point, float damage, bool triggerExplosiveOverride = true, Cartridge cartridge = null)
	{
		this.normal = normal;
		this.point = point;
		this.damage = damage;
		triggersExplosiveOverride = triggerExplosiveOverride;
		this.cartridge = cartridge;
		CanCrush = true;
	}
}
