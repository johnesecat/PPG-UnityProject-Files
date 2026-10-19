using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SortingLayers
{
	public static readonly int Bottom = SortingLayer.GetLayerValueFromName("Bottom");

	public static readonly int Background = SortingLayer.GetLayerValueFromName("Background");

	public static readonly int Default = SortingLayer.GetLayerValueFromName("Default");

	public static readonly int Foreground = SortingLayer.GetLayerValueFromName("Foreground");

	public static readonly int Top = SortingLayer.GetLayerValueFromName("Top");

	public static readonly IList<SortingLayer> All = SortingLayer.layers;

	public static readonly IList<KeyValuePair<int, SortingLayer>> SortedByDepth = (from layer in All
		select new KeyValuePair<int, SortingLayer>(SortingLayer.GetLayerValueFromID(layer.id), layer) into layer
		orderby layer.Key
		select layer).ToList();

	public static bool GetLayerAbove(int layervalue, out SortingLayer layer)
	{
		for (int i = 0; i < SortedByDepth.Count; i++)
		{
			KeyValuePair<int, SortingLayer> keyValuePair = SortedByDepth[i];
			if (keyValuePair.Key == layervalue)
			{
				if (i == SortedByDepth.Count - 1)
				{
					layer = keyValuePair.Value;
					return false;
				}
				layer = SortedByDepth[i + 1].Value;
				return true;
			}
		}
		layer = default(SortingLayer);
		return false;
	}

	public static bool GetLayerUnder(int layervalue, out SortingLayer layer)
	{
		for (int i = 0; i < SortedByDepth.Count; i++)
		{
			KeyValuePair<int, SortingLayer> keyValuePair = SortedByDepth[i];
			if (keyValuePair.Key == layervalue)
			{
				if (i == 0)
				{
					layer = keyValuePair.Value;
					return false;
				}
				layer = SortedByDepth[i - 1].Value;
				return true;
			}
		}
		layer = default(SortingLayer);
		return false;
	}

	public static bool TryGetLayerIdForLayerValue(int layervalue, out SortingLayer layer)
	{
		for (int i = 0; i < All.Count; i++)
		{
			layer = All[i];
			if (SortingLayer.GetLayerValueFromID(layer.id) == layervalue)
			{
				return true;
			}
		}
		layer = default(SortingLayer);
		return false;
	}
}
