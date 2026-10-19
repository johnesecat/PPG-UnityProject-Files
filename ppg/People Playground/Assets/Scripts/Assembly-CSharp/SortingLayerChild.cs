using NaughtyAttributes;
using UnityEngine;

[SkipSerialisation]
public class SortingLayerChild : MonoBehaviour
{
	public enum SortBehaviour
	{
		Behind,
		Same,
		Above
	}

	private SpriteRenderer spriteRenderer;

	private LineRenderer lineRenderer;

	private ParticleSystemRenderer particleSystem;

	private SpriteRenderer parentRenderer;

	public SortBehaviour Behaviour = SortBehaviour.Same;

	[ShowIf("OffsetIsApplicable")]
	public int Offset = 1;

	private bool OffsetIsApplicable()
	{
		return Behaviour != SortBehaviour.Same;
	}

	private void Awake()
	{
		OnTransformParentChanged();
		spriteRenderer = GetComponent<SpriteRenderer>();
		lineRenderer = GetComponent<LineRenderer>();
		particleSystem = GetComponent<ParticleSystemRenderer>();
	}

	private void OnWillRenderObject()
	{
		if (!parentRenderer)
		{
			return;
		}
		if ((bool)spriteRenderer)
		{
			switch (Behaviour)
			{
			case SortBehaviour.Above:
				spriteRenderer.sortingLayerID = parentRenderer.sortingLayerID;
				spriteRenderer.sortingOrder = parentRenderer.sortingOrder + Offset;
				break;
			case SortBehaviour.Same:
				spriteRenderer.sortingLayerID = parentRenderer.sortingLayerID;
				spriteRenderer.sortingOrder = parentRenderer.sortingOrder;
				break;
			case SortBehaviour.Behind:
				spriteRenderer.sortingLayerID = parentRenderer.sortingLayerID;
				spriteRenderer.sortingOrder = parentRenderer.sortingOrder - Offset;
				break;
			}
		}
		if ((bool)lineRenderer)
		{
			switch (Behaviour)
			{
			case SortBehaviour.Above:
				lineRenderer.sortingLayerID = parentRenderer.sortingLayerID;
				lineRenderer.sortingOrder = parentRenderer.sortingOrder + Offset;
				break;
			case SortBehaviour.Same:
				lineRenderer.sortingLayerID = parentRenderer.sortingLayerID;
				lineRenderer.sortingOrder = parentRenderer.sortingOrder;
				break;
			case SortBehaviour.Behind:
				lineRenderer.sortingLayerID = parentRenderer.sortingLayerID;
				lineRenderer.sortingOrder = parentRenderer.sortingOrder - Offset;
				break;
			}
		}
		if ((bool)particleSystem)
		{
			switch (Behaviour)
			{
			case SortBehaviour.Above:
				particleSystem.sortingLayerID = parentRenderer.sortingLayerID;
				particleSystem.sortingOrder = parentRenderer.sortingOrder + Offset;
				break;
			case SortBehaviour.Same:
				particleSystem.sortingLayerID = parentRenderer.sortingLayerID;
				particleSystem.sortingOrder = parentRenderer.sortingOrder;
				break;
			case SortBehaviour.Behind:
				particleSystem.sortingLayerID = parentRenderer.sortingLayerID;
				particleSystem.sortingOrder = parentRenderer.sortingOrder - Offset;
				break;
			}
		}
	}

	private void OnTransformParentChanged()
	{
		if ((bool)base.transform.parent)
		{
			parentRenderer = base.transform.parent.GetComponent<SpriteRenderer>();
		}
		else
		{
			parentRenderer = null;
		}
	}
}
