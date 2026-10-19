using UnityEngine;

public class ToggleableMirrorBehaviour : MonoBehaviour
{
	private MaterialPropertyBlock propertyBlock;

	private SpriteRenderer spriteRenderer;

	private PhysicalBehaviour physicalBehaviour;

	private void Awake()
	{
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		spriteRenderer.GetPropertyBlock(propertyBlock);
	}

	private void FixedUpdate()
	{
		propertyBlock.SetFloat(ShaderProperties.Get("_Progress"), physicalBehaviour.Charge);
		physicalBehaviour.ReflectsLasers = (double)physicalBehaviour.Charge < 0.67;
		physicalBehaviour.AbsorbsLasers = physicalBehaviour.ReflectsLasers;
	}

	private void OnWillRenderObject()
	{
		spriteRenderer.SetPropertyBlock(propertyBlock);
	}
}
