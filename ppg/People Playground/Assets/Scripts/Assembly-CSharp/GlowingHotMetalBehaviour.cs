using UnityEngine;

[RequireComponent(typeof(PhysicalBehaviour))]
public class GlowingHotMetalBehaviour : MonoBehaviour
{
	private PhysicalBehaviour physicalBehaviour;

	private SpriteRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	public float UpperTemperatureBound = 1000f;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		renderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
		if (renderer.sharedMaterial.name != "GlowingMetal")
		{
			Debug.LogWarning("Incorrect material set on " + base.gameObject.name);
		}
	}

	private void OnWillRenderObject()
	{
		if (base.enabled)
		{
			if (!renderer.sprite.packed)
			{
				propertyBlock.SetTexture("_MainTex", renderer.sprite.texture);
			}
			float num = Mathf.Clamp01(physicalBehaviour.Temperature / UpperTemperatureBound);
			propertyBlock.SetFloat(ShaderProperties.Get("_Progress"), num * num);
			renderer.SetPropertyBlock(propertyBlock);
		}
	}
}
