using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
	public static readonly HashSet<ConnectedNodeBehaviour> Nodes = new HashSet<ConnectedNodeBehaviour>();

	private void Awake()
	{
		Nodes.Clear();
	}

	private void Update()
	{
		foreach (ConnectedNodeBehaviour node in Nodes)
		{
			if ((bool)node)
			{
				node.ResetValue();
			}
		}
		foreach (ConnectedNodeBehaviour node2 in Nodes)
		{
			if ((bool)node2)
			{
				node2.RootPropagation();
			}
		}
	}
}
