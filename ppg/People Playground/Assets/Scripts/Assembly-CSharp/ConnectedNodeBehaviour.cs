using UnityEngine;

public class ConnectedNodeBehaviour : MonoBehaviour, Messages.IOnBeforeSerialise
{
	public bool IsRoot;

	public int Value;

	[SkipSerialisation]
	public ConnectedNodeBehaviour[] Connections;

	public bool[] SerialisableConnectedNodes;

	public bool ShouldLoadSerialisableConnections;

	[SkipSerialisation]
	public bool IsConnectedToRoot { get; private set; } = true;


	private void Awake()
	{
		NodeController.Nodes.Add(this);
	}

	private void Start()
	{
		if (!ShouldLoadSerialisableConnections || SerialisableConnectedNodes == null)
		{
			return;
		}
		for (int i = 0; i < Connections.Length; i++)
		{
			if (!SerialisableConnectedNodes[i])
			{
				Connections[i] = null;
			}
		}
	}

	private void SetToNull(ConnectedNodeBehaviour otherNode)
	{
		for (int i = 0; i < Connections.Length; i++)
		{
			if (Connections[i] == otherNode)
			{
				Connections[i] = null;
				break;
			}
		}
	}

	public void DisconnectFrom(ConnectedNodeBehaviour otherNode)
	{
		otherNode.SetToNull(this);
		SetToNull(otherNode);
	}

	public void DisconnectFromEverything()
	{
		for (int i = 0; i < Connections.Length; i++)
		{
			ConnectedNodeBehaviour connectedNodeBehaviour = Connections[i];
			if ((bool)connectedNodeBehaviour)
			{
				connectedNodeBehaviour.SetToNull(this);
			}
		}
		for (int j = 0; j < Connections.Length; j++)
		{
			Connections[j] = null;
		}
	}

	public void ResetValue()
	{
		IsConnectedToRoot = false;
		Value = (IsRoot ? 1000000 : 0);
	}

	public void RootPropagation()
	{
		if (IsRoot)
		{
			propagateValue(this);
		}
		void propagateValue(ConnectedNodeBehaviour me)
		{
			me.IsConnectedToRoot = me.IsRoot || me.Value > 0;
			for (int i = 0; i < me.Connections.Length; i++)
			{
				ConnectedNodeBehaviour connectedNodeBehaviour = me.Connections[i];
				if ((bool)connectedNodeBehaviour && connectedNodeBehaviour.Value < me.Value)
				{
					connectedNodeBehaviour.Value = me.Value - 1;
					propagateValue(connectedNodeBehaviour);
				}
			}
		}
	}

	public bool IsConnectedTo(ConnectedNodeBehaviour other)
	{
		if (other == null)
		{
			return false;
		}
		for (int i = 0; i < Connections.Length; i++)
		{
			ConnectedNodeBehaviour connectedNodeBehaviour = Connections[i];
			if ((bool)connectedNodeBehaviour && connectedNodeBehaviour == other)
			{
				return true;
			}
		}
		return false;
	}

	private void OnDestroy()
	{
		DisconnectFromEverything();
		NodeController.Nodes.Remove(this);
	}

	public void OnBeforeSerialise()
	{
		ShouldLoadSerialisableConnections = true;
		SerialisableConnectedNodes = new bool[Connections.Length];
		for (int i = 0; i < Connections.Length; i++)
		{
			SerialisableConnectedNodes[i] = Connections[i] != null;
		}
	}
}
