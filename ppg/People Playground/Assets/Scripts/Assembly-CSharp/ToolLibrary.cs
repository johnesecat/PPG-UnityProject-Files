using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ToolLibrary : MonoBehaviour
{
	[Serializable]
	public struct Tool
	{
		public string Name;

		[TextArea(3, 16)]
		public string Description;

		public Sprite Icon;

		public string Type;

		public string ToSortBy;

		public string Parent;

		public Tool(string name, string description, Sprite icon, string type, string parent = null)
		{
			Name = name;
			Description = description;
			Icon = icon;
			Type = type;
			Parent = parent;
			ToSortBy = name;
		}

		public override bool Equals(object obj)
		{
			if (obj is Tool)
			{
				Tool tool = (Tool)obj;
				return Type == tool.Type;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 2049151605 + EqualityComparer<string>.Default.GetHashCode(Type);
		}

		public static bool operator ==(Tool left, Tool right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Tool left, Tool right)
		{
			return !(left == right);
		}
	}

	[ReorderableList]
	public List<Tool> Tools;

	[ReorderableList]
	public List<Tool> Powers;

	public static UnityEvent OnCollectionChange = new UnityEvent();

	public static HashSet<Type> ModdedTypes = new HashSet<Type>();

	public static ToolLibrary Instance { get; private set; }

	public void BroadcastCollectionChange()
	{
		OnCollectionChange.Invoke();
	}

	private void Awake()
	{
		Instance = this;
	}
}
