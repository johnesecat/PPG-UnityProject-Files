using UnityEngine;

[RequireComponent(typeof(PhysicalBehaviour))]
public class BurnableSpriteBehaviour : MonoBehaviour
{
	private PhysicalBehaviour physicalBehaviour;

	private SpriteRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		renderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
		if (renderer.sharedMaterial.name != "BurnableSpriteShader")
		{
			Debug.LogWarning("Incorrect material set on " + base.gameObject.name);
		}
	}

	private void OnWillRenderObject()
	{
		if (base.enabled)
		{
			propertyBlock.SetTexture(ShaderProperties.Get("_MainTex"), renderer.sprite.texture);
			float num = Mathf.Clamp01(physicalBehaviour.BurnProgress);
			propertyBlock.SetFloat(ShaderProperties.Get("_Progress"), num * num);
			renderer.SetPropertyBlock(propertyBlock);
		}
	}
}
