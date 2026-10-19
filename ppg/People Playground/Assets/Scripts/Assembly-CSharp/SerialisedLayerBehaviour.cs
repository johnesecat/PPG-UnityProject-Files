using UnityEngine;

public class SerialisedLayerBehaviour : MonoBehaviour
{
	private PhysicalBehaviour phys;

	public OptionalInt SortingLayerID;

	public OptionalInt SortingOrder;

	public static readonly float NearbySearchRadius = 10f;

	[SkipSerialisation]
	public (int id, int order) OriginalSorting = (0, 0);

	public static (int id, int order) GetSortingFor(PhysicalBehaviour phys)
	{
		if ((bool)phys.SortingGroup)
		{
			return (phys.SortingGroup.sortingLayerID, phys.SortingGroup.sortingOrder);
		}
		if ((bool)phys.spriteRenderer)
		{
			return (phys.spriteRenderer.sortingLayerID, phys.spriteRenderer.sortingOrder);
		}
		return (0, 0);
	}

	public static void SetSortingFor(PhysicalBehaviour phys, int sortingLayerId, int sortingOrder)
	{
		if ((bool)phys.SortingGroup)
		{
			phys.SortingGroup.sortingLayerID = sortingLayerId;
			phys.SortingGroup.sortingOrder = sortingOrder;
		}
		else if ((bool)phys.spriteRenderer)
		{
			phys.spriteRenderer.sortingLayerID = sortingLayerId;
			phys.spriteRenderer.sortingOrder = sortingOrder;
		}
	}

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		OriginalSorting = GetSortingFor(phys);
	}

	private void Start()
	{
		SetSortingFor(phys, SortingLayerID.Active ? SortingLayerID.Value : OriginalSorting.id, SortingOrder.Active ? SortingOrder.Value : OriginalSorting.order);
	}

	public void SetOrder(int layerId, int order)
	{
		SetOrder(new OptionalInt(layerId), new OptionalInt(order));
	}

	public void SetOrder(OptionalInt layerId, OptionalInt order)
	{
		SortingLayerID = layerId;
		SortingOrder = order;
		SetSortingFor(phys, layerId.Active ? layerId.Value : OriginalSorting.id, order.Active ? order.Value : OriginalSorting.order);
	}

	public void MoveLayer(int delta)
	{
		(int, int) sortingFor = GetSortingFor(phys);
		int sortingOrder;
		PhysicalBehaviour result2;
		PhysicalBehaviour result3;
		SortingLayer layer2;
		if (delta > 0)
		{
			if (FindTopmostAround(base.transform.position, sortingFor.Item1, out var result, out sortingOrder) && result.gameObject == base.gameObject && SortingLayers.GetLayerAbove(SortingLayer.GetLayerValueFromID(sortingFor.Item1), out var layer))
			{
				if (FindBottommostAround(base.transform.position, layer.id, out result2, out var sortingOrder2))
				{
					SetOrder(layer.id, sortingOrder2 + 1);
				}
				else
				{
					SetOrder(layer.id, 0);
				}
				return;
			}
		}
		else if (delta < 0 && FindBottommostAround(base.transform.position, sortingFor.Item1, out result3, out sortingOrder) && result3.gameObject == base.gameObject && SortingLayers.GetLayerUnder(SortingLayer.GetLayerValueFromID(sortingFor.Item1), out layer2))
		{
			if (FindTopmostAround(base.transform.position, layer2.id, out result2, out var sortingOrder3))
			{
				SetOrder(layer2.id, sortingOrder3 - 1);
			}
			else
			{
				SetOrder(layer2.id, 0);
			}
			return;
		}
		SetOrder(sortingFor.Item1, sortingFor.Item2 + delta);
	}

	public static bool FindTopmostAround(Vector2 point, int sortingLayerId, out PhysicalBehaviour result, out int sortingOrder)
	{
		int num = int.MinValue;
		result = null;
		sortingOrder = num;
		foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNearPositionAccurate(point, NearbySearchRadius))
		{
			if ((bool)item)
			{
				(int, int) sortingFor = GetSortingFor(item);
				if (sortingFor.Item1 == sortingLayerId && sortingFor.Item2 > num)
				{
					result = item;
					num = (sortingOrder = sortingFor.Item2);
				}
			}
		}
		return result != null;
	}

	public static bool FindBottommostAround(Vector2 point, int sortingLayerId, out PhysicalBehaviour result, out int sortingOrder)
	{
		int num = int.MaxValue;
		result = null;
		sortingOrder = num;
		foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNearPositionAccurate(point, NearbySearchRadius))
		{
			if ((bool)item)
			{
				(int, int) sortingFor = GetSortingFor(item);
				if (sortingFor.Item1 == sortingLayerId && sortingFor.Item2 < num)
				{
					result = item;
					num = (sortingOrder = sortingFor.Item2);
				}
			}
		}
		return result != null;
	}

	public void SendToBack()
	{
		(int id, int order) sortingFor = GetSortingFor(phys);
		int num = sortingFor.id;
		int num2 = SortingLayer.GetLayerValueFromID(num);
		int num3 = sortingFor.order;
		Transform root = base.transform.root;
		foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNearPositionAccurate(base.transform.position, NearbySearchRadius))
		{
			if ((bool)item && !(item.transform.root == root))
			{
				(int, int) sortingFor2 = GetSortingFor(item);
				int layerValueFromID = SortingLayer.GetLayerValueFromID(sortingFor2.Item1);
				if (layerValueFromID < num2)
				{
					num2 = layerValueFromID;
					(num, _) = sortingFor2;
				}
				if (num == sortingFor2.Item1)
				{
					num3 = Mathf.Min(num3, sortingFor2.Item2);
				}
			}
		}
		SetOrder(num, num3 - 1);
	}

	public void BringToFront()
	{
		(int id, int order) sortingFor = GetSortingFor(phys);
		int num = sortingFor.id;
		int num2 = SortingLayer.GetLayerValueFromID(num);
		int num3 = sortingFor.order;
		Transform root = base.transform.root;
		foreach (PhysicalBehaviour item in Global.main.GetPhysicsObjectsNearPositionAccurate(base.transform.position, NearbySearchRadius))
		{
			if ((bool)item && !(item.transform.root == root))
			{
				(int, int) sortingFor2 = GetSortingFor(item);
				int layerValueFromID = SortingLayer.GetLayerValueFromID(sortingFor2.Item1);
				if (layerValueFromID > num2)
				{
					num2 = layerValueFromID;
					(num, _) = sortingFor2;
				}
				if (num == sortingFor2.Item1)
				{
					num3 = Mathf.Max(num3, sortingFor2.Item2);
				}
			}
		}
		SetOrder(num, num3 + 1);
	}
}
