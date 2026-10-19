using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComponentTagContainer : MonoBehaviour
{
	[Serializable]
	public struct TaggedComponent
	{
		public Component Component;

		public string Tag;

		public override bool Equals(object obj)
		{
			if (obj is TaggedComponent)
			{
				TaggedComponent taggedComponent = (TaggedComponent)obj;
				if (EqualityComparer<Component>.Default.Equals(Component, taggedComponent.Component))
				{
					return Tag == taggedComponent.Tag;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (951617459 * -1521134295 + EqualityComparer<Component>.Default.GetHashCode(Component)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
		}

		public static bool operator ==(TaggedComponent left, TaggedComponent right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TaggedComponent left, TaggedComponent right)
		{
			return !(left == right);
		}
	}

	[SkipSerialisation]
	public TaggedComponent[] TaggedComponents;

	[ContextMenu("Add all joints :)")]
	private void AddAllJoints()
	{
		if (TaggedComponents == null)
		{
			TaggedComponents = new TaggedComponent[0];
		}
		Joint2D[] components = base.gameObject.GetComponents<Joint2D>();
		int num = TaggedComponents.Length;
		Array.Resize(ref TaggedComponents, TaggedComponents.Length + components.Length);
		for (int i = 0; i < components.Length; i++)
		{
			TaggedComponents[num + i] = new TaggedComponent
			{
				Component = components[i],
				Tag = components[i].GetType().Name + i
			};
		}
	}

	public bool HasTag(string tag)
	{
		return TaggedComponents.Any((TaggedComponent t) => t.Tag == tag);
	}

	public TaggedComponent GetComponentWithTag(string tag)
	{
		return TaggedComponents.FirstOrDefault((TaggedComponent t) => t.Tag == tag);
	}

	public IEnumerable<TaggedComponent> GetAllComponentsWithTag(string tag)
	{
		return TaggedComponents.Where((TaggedComponent t) => t.Tag == tag);
	}

	public bool TryGetTagFrom(Component comp, out string tag)
	{
		tag = null;
		TaggedComponent[] taggedComponents = TaggedComponents;
		for (int i = 0; i < taggedComponents.Length; i++)
		{
			TaggedComponent taggedComponent = taggedComponents[i];
			if (taggedComponent.Component == comp)
			{
				tag = taggedComponent.Tag;
				return true;
			}
		}
		return false;
	}
}
