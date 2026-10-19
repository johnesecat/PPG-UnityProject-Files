using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class MonoBehaviourPrototype
{
	[Serializable]
	public struct ComponentReferenceLink
	{
		public Guid UniqueIdentity;

		public Type ComponentType;

		public ComponentReferenceLink(Guid uniqueIdentity, Type componentType = null)
		{
			UniqueIdentity = uniqueIdentity;
			ComponentType = componentType;
		}
	}

	public bool Enabled;

	public Type Type;

	public Dictionary<string, object> storedFields = new Dictionary<string, object>();

	public Dictionary<string, object> storedProperties = new Dictionary<string, object>();

	public Dictionary<string, ComponentReferenceLink> storedReferenceFields = new Dictionary<string, ComponentReferenceLink>();

	private const BindingFlags memberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public MonoBehaviourPrototype()
	{
	}

	public MonoBehaviourPrototype(MonoBehaviour original)
	{
		Enabled = original.enabled;
		Type = original.GetType();
		FieldInfo[] fields = Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyInfo[] properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		StoreFields(original, fields);
		StoreProperties(original, properties);
	}

	private bool ShouldSerialiseField(FieldInfo info)
	{
		if (info.IsPrivate)
		{
			return false;
		}
		if (info.GetCustomAttribute<ObsoleteAttribute>() != null)
		{
			return false;
		}
		if (info.GetCustomAttribute<NonSerializedAttribute>() != null)
		{
			return false;
		}
		if (info.GetCustomAttribute<SkipSerialisationAttribute>() != null)
		{
			return false;
		}
		return true;
	}

	private bool ShouldSerialiseProperty(PropertyInfo info)
	{
		if (!Utils.IsSerialisableType(info.PropertyType))
		{
			return false;
		}
		if (info.SetMethod?.IsPrivate ?? true)
		{
			return false;
		}
		if (!info.CanRead || !info.CanWrite)
		{
			return false;
		}
		if (info.GetCustomAttribute<ObsoleteAttribute>() != null)
		{
			return false;
		}
		if (info.GetCustomAttribute<NonSerializedAttribute>() != null)
		{
			return false;
		}
		if (info.GetCustomAttribute<SkipSerialisationAttribute>() != null)
		{
			return false;
		}
		return true;
	}

	private void StoreFields(MonoBehaviour original, FieldInfo[] fields)
	{
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!ShouldSerialiseField(fieldInfo))
			{
				continue;
			}
			if (!Utils.IsSerialisableType(fieldInfo.FieldType))
			{
				try
				{
					if (typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
					{
						Component component = fieldInfo.GetValue(original) as Component;
						if ((bool)component)
						{
							SerialisableIdentity component2 = component.GetComponent<SerialisableIdentity>();
							if ((bool)component2)
							{
								ComponentReferenceLink value = new ComponentReferenceLink(component2.UniqueIdentity, fieldInfo.FieldType);
								storedReferenceFields.Add(fieldInfo.Name, value);
							}
						}
					}
					else
					{
						if (!typeof(GameObject).IsAssignableFrom(fieldInfo.FieldType))
						{
							continue;
						}
						GameObject gameObject = fieldInfo.GetValue(original) as GameObject;
						if ((bool)gameObject)
						{
							SerialisableIdentity component3 = gameObject.GetComponent<SerialisableIdentity>();
							if ((bool)component3)
							{
								storedReferenceFields.Add(fieldInfo.Name, new ComponentReferenceLink(component3.UniqueIdentity));
							}
						}
						continue;
					}
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
				continue;
			}
			try
			{
				object obj = fieldInfo.GetValue(original);
				if (obj != null)
				{
					if (!(obj is string))
					{
						obj = obj.ShallowClone();
					}
					storedFields.Add(fieldInfo.Name, obj);
				}
			}
			catch (Exception message2)
			{
				Debug.LogWarning(message2);
			}
		}
	}

	private void StoreProperties(MonoBehaviour original, PropertyInfo[] properties)
	{
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (!ShouldSerialiseProperty(propertyInfo) || !propertyInfo.CanRead || !propertyInfo.CanWrite)
			{
				continue;
			}
			try
			{
				object value = propertyInfo.GetValue(original);
				if (value != null)
				{
					storedProperties.Add(propertyInfo.Name, value);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}
	}

	public void LinkReferencesToMonoBehaviour(MonoBehaviour target, IEnumerable<SerialisableIdentity> referencePool)
	{
		ComponentReferenceLink link;
		foreach (KeyValuePair<string, ComponentReferenceLink> storedReferenceField in storedReferenceFields)
		{
			try
			{
				link = storedReferenceField.Value;
				FieldInfo field = Type.GetField(storedReferenceField.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null || !ShouldSerialiseField(field))
				{
					continue;
				}
				SerialisableIdentity serialisableIdentity = referencePool.FirstOrDefault((SerialisableIdentity s) => s.UniqueIdentity == link.UniqueIdentity);
				if (!serialisableIdentity)
				{
					string[] obj = new string[6]
					{
						field.Name,
						" at ",
						target?.ToString(),
						"\nNo object with identity ",
						null,
						null
					};
					Guid uniqueIdentity = link.UniqueIdentity;
					obj[4] = uniqueIdentity.ToString();
					obj[5] = " found";
					Debug.LogWarning(string.Concat(obj));
				}
				else if (link.ComponentType == null)
				{
					field.SetValue(target, serialisableIdentity.gameObject);
				}
				else
				{
					Component component = serialisableIdentity.GetComponent(link.ComponentType);
					if ((bool)component)
					{
						field.SetValue(target, component);
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}
	}

	public void InjectIntoMonoBehaviour(MonoBehaviour target)
	{
		if (Type != target.GetType())
		{
			throw new Exception("Cannot map objects of different types\nattempt to map " + Type.Name + " onto " + target.GetType().Name);
		}
		target.enabled = Enabled;
		foreach (KeyValuePair<string, object> storedField in storedFields)
		{
			try
			{
				object obj = storedField.Value;
				FieldInfo field = Type.GetField(storedField.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (!(field == null) && obj != null && ShouldSerialiseField(field))
				{
					if (!(obj is string))
					{
						obj = obj.ShallowClone();
					}
					field.SetValue(target, obj);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Cannot set field " + storedField.Key + " on " + target.GetType().Name + ": " + ex);
			}
		}
		foreach (KeyValuePair<string, object> storedProperty in storedProperties)
		{
			try
			{
				object value = storedProperty.Value;
				PropertyInfo property = Type.GetProperty(storedProperty.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (!(property == null) && ShouldSerialiseProperty(property) && value != null)
				{
					property.SetValue(target, value);
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("Cannot set property " + storedProperty.Key + " on " + target.GetType().Name + ": " + ex2);
			}
		}
	}
}
