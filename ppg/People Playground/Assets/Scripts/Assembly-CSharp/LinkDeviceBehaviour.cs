using System;
using UnityEngine;

public abstract class LinkDeviceBehaviour : Hover
{
	public Vector2 LocalOffset;

	public Vector2 OtherLocalOffset;

	public PhysicalBehaviour Other;

	[SkipSerialisation]
	protected LineRenderer lineRenderer;

	[SkipSerialisation]
	protected PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	protected Material initialMaterial;

	[SkipSerialisation]
	protected SpriteRenderer fromSpriteRenderer;

	[SkipSerialisation]
	protected SpriteRenderer toSpriteRenderer;

	protected virtual void Awake()
	{
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	protected virtual void Start()
	{
		if (!Other)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		string text = Guid.NewGuid().ToString();
		GameObject gameObject = new GameObject("link source " + text);
		gameObject.AddComponent<Optout>();
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localPosition = LocalOffset;
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		lineRenderer.sharedMaterial = (initialMaterial = GetWireMaterial());
		lineRenderer.widthMultiplier = GetWireWidth();
		Color color2 = (lineRenderer.startColor = (lineRenderer.endColor = GetWireColor()));
		fromSpriteRenderer = PrepareSpriteRendererOrder(PhysicalBehaviour, gameObject);
		GameObject gameObject2 = new GameObject("link target " + text);
		gameObject2.AddComponent<Optout>();
		gameObject2.transform.SetParent(Other.transform);
		gameObject2.transform.localPosition = OtherLocalOffset;
		toSpriteRenderer = PrepareSpriteRendererOrder(Other, gameObject2);
		gameObject.AddComponent<VisualDeletableDetachedToolBehaviour>();
		gameObject2.AddComponent<VisualDeletableDetachedToolBehaviour>();
		ModAPI.InvokeLinkCreated(this, this);
		AfterInitialise();
	}

	private void Update()
	{
		if (Global.main.GetPausedMenu())
		{
			return;
		}
		if (!fromSpriteRenderer || !toSpriteRenderer)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		lineRenderer.enabled = Global.main.ShowLimbStatus;
		if (Global.main.ShowLimbStatus)
		{
			lineRenderer.SetPosition(0, base.transform.TransformPoint(LocalOffset));
			lineRenderer.SetPosition(1, Other.transform.TransformPoint(OtherLocalOffset));
		}
		CheckMouseInput();
	}

	public override void OnMouseOverlapEvent(bool overlap)
	{
		base.OnMouseOverlapEvent(overlap);
		if (overlap && UserPreferenceManager.Current.ShowOutlines)
		{
			lineRenderer.sharedMaterial = Resources.Load<Material>("Materials/DeleteWire");
			lineRenderer.gameObject.layer = LayerMask.NameToLayer("ScreenUI");
		}
		else
		{
			lineRenderer.sharedMaterial = initialMaterial;
			lineRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	private SpriteRenderer PrepareSpriteRendererOrder(PhysicalBehaviour phys, GameObject container)
	{
		SpriteRenderer component = phys.GetComponent<SpriteRenderer>();
		int sortingLayerID = component.sortingLayerID;
		int sortingOrder = component.sortingOrder;
		SpriteRenderer spriteRenderer = container.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerID = sortingLayerID;
		spriteRenderer.sortingOrder = sortingOrder + 1;
		spriteRenderer.sprite = GetDeviceSprite();
		container.AddComponent<ExistInDetailView>();
		return spriteRenderer;
	}

	protected abstract Sprite GetDeviceSprite();

	protected abstract float GetWireWidth();

	protected abstract Color GetWireColor();

	protected abstract Material GetWireMaterial();

	protected abstract void AfterInitialise();

	protected override Bounds GetVisualBounds()
	{
		return lineRenderer.bounds;
	}

	public override void OnUserDelete()
	{
	}

	protected override void OnDestroy()
	{
		ModAPI.InvokeLinkDestroyed(this, this);
		base.OnDestroy();
		if ((bool)fromSpriteRenderer)
		{
			UnityEngine.Object.Destroy(fromSpriteRenderer.gameObject);
		}
		if ((bool)toSpriteRenderer)
		{
			UnityEngine.Object.Destroy(toSpriteRenderer.gameObject);
		}
	}

	protected override bool IsMouseInsideCollider()
	{
		if (!lineRenderer || !lineRenderer.enabled)
		{
			return false;
		}
		bool useWorldSpace = lineRenderer.useWorldSpace;
		Vector3 vector = Global.main.MousePosition;
		if (!useWorldSpace)
		{
			vector = lineRenderer.transform.InverseTransformPoint(vector);
		}
		for (int i = 0; i < lineRenderer.positionCount - 1; i++)
		{
			Vector3 position = lineRenderer.GetPosition(i);
			Vector3 position2 = lineRenderer.GetPosition(i + 1);
			if (Utils.SqrdDistanceFromPointToLineSegment(vector, position, position2) <= 0.0016f)
			{
				return true;
			}
		}
		return false;
	}
}
