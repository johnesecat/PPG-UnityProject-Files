using UnityEngine;

public class WrenchBehaviour : MonoBehaviour
{
	public float MinImpulse = 3f;

	public float HealingPower = 0.2f;

	public float DamagePointHealingPower = 0.2f;

	public float BulletHoleHealingPower = 0.2f;

	public float BurnProgressHealingPower = 0.2f;

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.GetContact(0).normalImpulse < MinImpulse)
		{
			return;
		}
		if (Random.value > 0.75f)
		{
			collision.transform.SendMessageUpwards("Repair", SendMessageOptions.DontRequireReceiver);
		}
		LimbBehaviour component = collision.transform.GetComponent<LimbBehaviour>();
		if ((bool)component && component.IsAndroid)
		{
			CirculationBehaviour circulationBehaviour = component.CirculationBehaviour;
			component.Health = Mathf.Lerp(component.Health, component.InitialHealth, HealingPower);
			component.HealBone();
			circulationBehaviour.HealBleeding();
			circulationBehaviour.ClearLiquid();
			circulationBehaviour.AddLiquid(component.GetOriginalBloodType(), 1f);
			circulationBehaviour.BloodFlow = 1f;
			circulationBehaviour.IsPump = circulationBehaviour.WasInitiallyPumping;
			if (component.HasBrain)
			{
				component.Person.Braindead = false;
				component.Person.BrainDamaged = false;
			}
			component.Person.Consciousness = Mathf.Lerp(component.Person.Consciousness, 1f, 0.5f);
			component.PhysicalBehaviour.BurnProgress *= 1f - BurnProgressHealingPower;
			for (int i = 0; i < component.SkinMaterialHandler.damagePoints.Length; i++)
			{
				component.SkinMaterialHandler.damagePoints[i].z -= DamagePointHealingPower;
			}
			component.SkinMaterialHandler.Sync();
		}
	}
}
