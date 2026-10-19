using System.Collections;
using UnityEngine;

public class FreezeBehaviour : MonoBehaviour
{
	private Rigidbody2D rb;

	private Vector2 velocity;

	private float angularVelocity;

	private GameObject selectionOutlineObject;

	private SpriteRenderer spriteRenderer;

	private MaterialPropertyBlock propertyBlock;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		velocity = rb.velocity;
		angularVelocity = rb.angularVelocity;
		FreezeStackController.RequestFreeze(rb);
		if (UserPreferenceManager.Current.ShowOutlines && (!TryGetComponent<PhysicalBehaviour>(out var component) || component.HasOutline))
		{
			CreateOutlineObject();
		}
	}

	private void CreateOutlineObject()
	{
		selectionOutlineObject = new GameObject("Outline");
		selectionOutlineObject.AddComponent<Optout>();
		selectionOutlineObject.layer = 16;
		spriteRenderer = selectionOutlineObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "Top";
		spriteRenderer.sortingOrder = 2147483646;
		spriteRenderer.transform.SetParent(base.transform, worldPositionStays: false);
		spriteRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
		spriteRenderer.sharedMaterial = Global.main.FrozenOutlineMaterial;
		propertyBlock = new MaterialPropertyBlock();
		spriteRenderer.GetPropertyBlock(propertyBlock);
		Vector2 vector = new Vector2(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);
		Vector2 min = Utils.GetMin(spriteRenderer.sprite.uv, (Vector2 v) => v.sqrMagnitude);
		Vector2 vector2 = new Vector2(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height);
		Vector4 value = new Vector4(min.x, min.y, vector2.x / vector.x, vector2.y / vector.y);
		propertyBlock.SetVector("_AtlasTransform", value);
		spriteRenderer.SetPropertyBlock(propertyBlock);
		StartCoroutine(Disappear());
	}

	private IEnumerator Disappear()
	{
		yield return new WaitForSecondsRealtime(1f);
		spriteRenderer.enabled = false;
	}

	private void OnDestroy()
	{
		if ((bool)rb)
		{
			FreezeStackController.RequestUnfreeze(rb);
			rb.velocity = velocity;
			rb.angularVelocity = angularVelocity;
		}
		if ((bool)selectionOutlineObject)
		{
			Object.Destroy(selectionOutlineObject);
		}
	}
}
