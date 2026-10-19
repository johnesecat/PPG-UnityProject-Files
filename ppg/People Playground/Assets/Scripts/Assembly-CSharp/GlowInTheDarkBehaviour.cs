using UnityEngine;

public class GlowInTheDarkBehaviour : MonoBehaviour
{
	private GameObject glow;

	private LightSprite lightSprite;

	private void Start()
	{
		glow = new GameObject("glowing child with laser eyes", typeof(Optout));
		glow.transform.SetParent(base.transform);
		glow.transform.localPosition = default(Vector3);
		glow.transform.localRotation = default(Quaternion);
		glow.transform.localScale = Vector3.one;
		float radius = 5f;
		if (TryGetComponent<PhysicalBehaviour>(out var component))
		{
			radius = Mathf.Sqrt(component.ObjectArea) * 3f;
		}
		lightSprite = ModAPI.CreateLight(glow.transform, new Color(0.08f, 0.62f, 0.28f), radius, 0.25f);
	}

	private void OnDestroy()
	{
		if ((bool)lightSprite)
		{
			Object.Destroy(lightSprite.gameObject);
		}
		if ((bool)glow)
		{
			Object.Destroy(glow);
		}
	}
}
