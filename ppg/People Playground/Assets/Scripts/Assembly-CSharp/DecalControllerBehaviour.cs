using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DecalControllerBehaviour : MonoBehaviour, Messages.IDecal
{
	public DecalDescriptor DecalDescriptor;

	[NonSerialized]
	[SkipSerialisation]
	public GameObject decalHolder;

	[NonSerialized]
	[SkipSerialisation]
	private bool dirty;

	[HideInInspector]
	public List<Vector2> localDecalPositions = new List<Vector2>();

	private SpriteMask spriteMask;

	private (int id, int order) originalSortingLayer;

	private void Awake()
	{
		dirty = false;
	}

	private void Start()
	{
		if (!DecalDescriptor)
		{
			UnityEngine.Object.Destroy(this);
			Debug.LogError("Decal controller has an invalid DecalDescriptor!");
		}
	}

	public void Clear()
	{
		localDecalPositions.Clear();
		if (!dirty)
		{
			return;
		}
		if ((bool)decalHolder)
		{
			foreach (Transform item in decalHolder.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			UnityEngine.Object.Destroy(decalHolder);
		}
		dirty = false;
	}

	private void CreateContainer()
	{
		if ((bool)DecalDescriptor)
		{
			dirty = true;
			string n = DecalDescriptor.name + " container";
			Transform transform = base.transform.Find(n);
			if ((bool)transform)
			{
				decalHolder = transform.gameObject;
			}
			else
			{
				decalHolder = new GameObject(n);
			}
			decalHolder.transform.SetParent(base.transform);
			decalHolder.transform.localPosition = Vector3.zero;
			decalHolder.transform.localScale = Vector3.one;
			decalHolder.transform.localEulerAngles = Vector3.zero;
			decalHolder.GetOrAddComponent<Optout>();
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			originalSortingLayer = (component.sortingLayerID, component.sortingOrder);
			SortingGroup orAddComponent = decalHolder.GetOrAddComponent<SortingGroup>();
			orAddComponent.sortingLayerID = originalSortingLayer.id;
			orAddComponent.sortingOrder = component.sortingOrder + 1;
			spriteMask = decalHolder.GetOrAddComponent<SpriteMask>();
			spriteMask.sprite = component.sprite;
			spriteMask.sortingOrder = component.sortingOrder + 1;
		}
	}

	private bool IsNearOtherDecal(Vector2 localPosition)
	{
		float num = DecalDescriptor.IgnoreRadius * DecalDescriptor.IgnoreRadius;
		Vector3 vector = base.transform.TransformPoint(localPosition);
		foreach (Vector2 localDecalPosition in localDecalPositions)
		{
			if ((vector - base.transform.TransformPoint(localDecalPosition)).sqrMagnitude < num)
			{
				return true;
			}
		}
		return false;
	}

	public void Decal(DecalInstruction instruction)
	{
		if (!UserPreferenceManager.Current.Decals || DecalDescriptor != instruction.type)
		{
			return;
		}
		if (!dirty)
		{
			CreateContainer();
		}
		if (!decalHolder)
		{
			return;
		}
		Vector2 vector = base.transform.InverseTransformPoint(instruction.globalPosition);
		if (IsNearOtherDecal(vector))
		{
			return;
		}
		GameObject gameObject = new GameObject("decal");
		float num = instruction.size * UnityEngine.Random.Range(1f, 1.2f);
		gameObject.isStatic = true;
		gameObject.transform.SetParent(decalHolder.transform, worldPositionStays: true);
		gameObject.transform.localScale = decalHolder.transform.InverseTransformVector(num * Vector3.one);
		gameObject.transform.localRotation = Quaternion.LookRotation(Vector3.forward, decalHolder.transform.InverseTransformDirection(Vector3.zero));
		gameObject.transform.localPosition = vector;
		SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = DecalDescriptor.Sprites.PickRandom();
		spriteRenderer.color = DecalDescriptor.Color * instruction.colourMultiplier;
		if (UserPreferenceManager.Current.GorelessMode)
		{
			Color color = spriteRenderer.color;
			spriteRenderer.color = Utils.ChangeRedToOrange(in color);
		}
		spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		spriteRenderer.sortingLayerID = originalSortingLayer.id;
		spriteRenderer.sortingOrder = originalSortingLayer.order + 1;
		if (DecalDescriptor.WillDrip)
		{
			int minInclusive = Mathf.Min(DecalDescriptor.MaxDripCount, DecalDescriptor.MinDripCount);
			int maxExclusive = Mathf.Max(DecalDescriptor.MaxDripCount, DecalDescriptor.MinDripCount);
			for (int i = 0; i <= UnityEngine.Random.Range(minInclusive, maxExclusive); i++)
			{
				GameObject obj = new GameObject("streak");
				obj.transform.SetParent(gameObject.transform);
				obj.transform.localPosition = UnityEngine.Random.insideUnitCircle * num / 2f;
				obj.transform.localScale = UnityEngine.Random.Range(0.02f, 0.05f) * Vector3.one;
				SpriteRenderer spriteRenderer2 = obj.AddComponent<SpriteRenderer>();
				spriteRenderer2.sprite = DecalDescriptor.DripParticle;
				spriteRenderer2.color = spriteRenderer.color;
				spriteRenderer2.maskInteraction = spriteRenderer.maskInteraction;
				spriteRenderer2.sortingLayerID = spriteRenderer.sortingLayerID;
				spriteRenderer2.sortingOrder = spriteRenderer.sortingOrder;
				spriteRenderer2.drawMode = SpriteDrawMode.Sliced;
				obj.AddComponent<DecalDripStreakBehaviour>();
			}
		}
		localDecalPositions.Add(vector);
	}
}
