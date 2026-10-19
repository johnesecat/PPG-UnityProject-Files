using UnityEngine;

[ExecuteAlways]
public class ResizableSliceBehaviour : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private MaterialPropertyBlock propertyBlock;

	private MaterialPropertyBlock propertyBlock2;

	private PhysicalBehaviour physicalBehaviour;

	private Vector3 lastScale;

	private void Awake()
	{
		propertyBlock = new MaterialPropertyBlock();
		propertyBlock2 = new MaterialPropertyBlock();
		physicalBehaviour = GetComponent<PhysicalBehaviour>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.GetPropertyBlock(propertyBlock);
		lastScale = new Vector3(float.MaxValue, 0f);
	}

	private void OnWillRenderObject()
	{
		if (!(Vector2.SqrMagnitude(lastScale - base.transform.localScale) < float.Epsilon))
		{
			lastScale = base.transform.localScale;
			spriteRenderer.GetPropertyBlock(propertyBlock);
			propertyBlock.SetVector(ShaderProperties.Get("_ObjectScale"), new Vector3(Mathf.Abs(base.transform.localScale.x), Mathf.Abs(base.transform.localScale.y), 1f));
			spriteRenderer.SetPropertyBlock(propertyBlock);
			if ((bool)physicalBehaviour && (bool)physicalBehaviour.outlineSpriteRenderer)
			{
				physicalBehaviour.outlineSpriteRenderer.GetPropertyBlock(propertyBlock2);
				propertyBlock2.SetVector(ShaderProperties.Get("_ObjectScale"), base.transform.localScale);
				physicalBehaviour.outlineSpriteRenderer.SetPropertyBlock(propertyBlock2);
			}
			else
			{
				physicalBehaviour = GetComponent<PhysicalBehaviour>();
				physicalBehaviour.RefreshOutline();
			}
		}
	}
}
