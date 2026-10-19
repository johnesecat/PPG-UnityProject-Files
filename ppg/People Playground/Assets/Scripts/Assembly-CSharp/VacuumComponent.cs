using UnityEngine;

public class VacuumComponent : MonoBehaviour
{
	public Collider2D Collider;

	public ContactFilter2D Filter;

	private Collider2D[] buffer = new Collider2D[32];

	private void FixedUpdate()
	{
		for (int i = 0; i < Collider.OverlapCollider(Filter, buffer); i++)
		{
			Collider2D collider2D = buffer[i];
			if (!Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(collider2D.transform, out var value))
			{
				continue;
			}
			value.Extinguish();
			value.BroadcastMessage("OnInsideVacuum", SendMessageOptions.DontRequireReceiver);
			if (value.TryGetComponent<LimbBehaviour>(out var component))
			{
				component.Wince(25f);
				if (component.SkinMaterialHandler.AcidProgress < 0.4f)
				{
					component.SkinMaterialHandler.AcidProgress += Time.deltaTime * 0.2f;
				}
				if (component.RoughClassification == LimbBehaviour.BodyPart.Head)
				{
					component.Person.AddPain(25f);
					component.Person.OxygenLevel -= Time.deltaTime * 3f;
				}
			}
		}
	}
}
