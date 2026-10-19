using UnityEngine;

public class VisualDeletableDetachedToolBehaviour : Hover
{
	public SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected override Bounds GetVisualBounds()
	{
		return spriteRenderer.bounds;
	}

	private void Update()
	{
		CheckMouseInput();
	}

	public override void OnMouseOverlapEvent(bool overlap)
	{
		base.OnMouseOverlapEvent(overlap);
		if (overlap && UserPreferenceManager.Current.ShowOutlines)
		{
			spriteRenderer.color = Color.red;
			spriteRenderer.gameObject.layer = LayerMask.NameToLayer("ScreenUI");
		}
		else
		{
			spriteRenderer.color = Color.white;
			spriteRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	public override void OnUserDelete()
	{
		Object.Destroy(base.gameObject);
	}

	protected override bool IsMouseInsideCollider()
	{
		return spriteRenderer.bounds.Contains(Global.main.MousePosition);
	}
}
