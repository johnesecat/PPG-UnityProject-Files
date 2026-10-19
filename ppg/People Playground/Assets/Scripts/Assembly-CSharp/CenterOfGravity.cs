using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CenterOfGravity : MonoBehaviour
{
	private Camera camera;

	public Renderer Renderer;

	public float SizeMultiplier = 0.25f;

	private bool shouldExist;

	private void Awake()
	{
		camera = Camera.main;
	}

	private void Update()
	{
		shouldExist = SelectionController.Main.SelectedObjects.Any((PhysicalBehaviour c) => c);
		Renderer.enabled = shouldExist;
		if (shouldExist)
		{
			base.transform.position = GetPosition(SelectionController.Main.SelectedObjects);
		}
		base.transform.localScale = camera.orthographicSize * SizeMultiplier * Vector3.one;
	}

	protected virtual Vector3 GetPosition(IEnumerable<PhysicalBehaviour> phys)
	{
		Vector3 vector = Vector2.zero;
		float num = 0f;
		foreach (PhysicalBehaviour phy in phys)
		{
			if ((bool)phy)
			{
				num += phy.rigidbody.mass;
			}
		}
		foreach (PhysicalBehaviour phy2 in phys)
		{
			if ((bool)phy2)
			{
				if (phy2.rigidbody.bodyType != 0)
				{
					vector += phy2.transform.position * phy2.rigidbody.mass;
					continue;
				}
				Vector2 centerOfMass = phy2.rigidbody.centerOfMass;
				vector += phy2.transform.localToWorldMatrix.MultiplyPoint3x4(centerOfMass / phy2.transform.lossyScale) * phy2.rigidbody.mass;
			}
		}
		Vector3 result = vector / num;
		result.x = result.x.NaNFallback();
		result.y = result.y.NaNFallback();
		result.z = result.z.NaNFallback();
		return result;
	}
}
