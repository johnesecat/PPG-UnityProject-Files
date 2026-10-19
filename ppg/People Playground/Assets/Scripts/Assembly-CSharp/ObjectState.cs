using System;
using System.Collections.Generic;
using System.Linq;
using Ceras;
using UnityEngine;

[Serializable]
public class ObjectState
{
	public string SpawnableAssetName;

	public TransformPrototype Root;

	public Dictionary<string, TransformPrototype> Children;

	[SerializeField]
	[Include]
	public MonoBehaviourPrototype[] RootComponentData = new MonoBehaviourPrototype[0];

	[SerializeField]
	[Include]
	public Dictionary<string, MonoBehaviourPrototype[]> ChildComponentData = new Dictionary<string, MonoBehaviourPrototype[]>();

	public ObjectState(string spawnableAssetName, TransformPrototype root, Dictionary<string, TransformPrototype> children)
	{
		SpawnableAssetName = spawnableAssetName;
		Root = root;
		Children = children;
	}

	public ObjectState()
	{
	}

	public void PackComponentData(GameObject origin, Transform[] children)
	{
		ChildComponentData = new Dictionary<string, MonoBehaviourPrototype[]>();
		foreach (Transform transform in children)
		{
			if (!transform.GetComponent<Optout>())
			{
				string hierachyPath = Utils.GetHierachyPath(transform);
				MonoBehaviourPrototype[] value = (from c in transform.GetComponents<MonoBehaviour>()
					where IsComponentToSerialise(c)
					select new MonoBehaviourPrototype(c)).ToArray();
				ChildComponentData.Add(hierachyPath, value);
			}
		}
		RootComponentData = (from c in origin.GetComponents<MonoBehaviour>().Where(IsComponentToSerialise)
			select new MonoBehaviourPrototype(c)).ToArray();
	}

	private bool IsComponentToSerialise(MonoBehaviour c)
	{
		if (c.GetType().GetCustomAttribute<SkipSerialisationAttribute>() != null)
		{
			return false;
		}
		if (c.GetType().BaseType != typeof(Component))
		{
			if (!(c is AudioSourceTimeScaleBehaviour) && !(c is DeregisterBehaviour) && !(c is FrozenInIceBehaviour) && !(c is ContextMenuOptionComponent) && !(c is DecalControllerBehaviour) && !(c is TexturePackApplier))
			{
				return !(c is SerialiseInstructions);
			}
			return false;
		}
		return false;
	}

	public GameObject CreateNew(Vector2 worldPosition, bool flipped = false)
	{
		SpawnableAsset spawnableAsset = CatalogBehaviour.Main.GetSpawnable(SpawnableAssetName);
		if (!spawnableAsset)
		{
			spawnableAsset = Resources.Load<SpawnableAsset>("SpawnableAssets/" + SpawnableAssetName);
		}
		if (!spawnableAsset)
		{
			throw new Exception("SpawnableAsset " + SpawnableAssetName + " not found");
		}
		Vector2 relativePosition = Root.RelativePosition;
		relativePosition.x *= ((!flipped) ? 1 : (-1));
		float num = Root.RelativeRotation;
		if (flipped)
		{
			num *= -1f;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(spawnableAsset.Prefab, relativePosition + worldPosition, Quaternion.Euler(0f, 0f, num));
		gameObject.transform.localScale = Root.LocalScale;
		foreach (KeyValuePair<string, TransformPrototype> child in Children)
		{
			Transform transform = gameObject.transform.Find(child.Key);
			if (transform == null)
			{
				Debug.LogWarning("Copied transform child not found: " + child.Key);
				continue;
			}
			transform.position = gameObject.transform.TransformPoint(child.Value.RelativePosition);
			transform.eulerAngles = new Vector3(0f, 0f, child.Value.RelativeRotation + gameObject.transform.eulerAngles.z);
			transform.localScale = child.Value.LocalScale;
			if (ChildComponentData.TryGetValue(child.Key, out var value))
			{
				Reconstruct(transform.gameObject, value);
			}
		}
		Reconstruct(gameObject, RootComponentData);
		gameObject.AddComponent<TexturePackApplier>();
		gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
		gameObject.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = spawnableAsset;
		gameObject.name = spawnableAsset.name;
		CatalogBehaviour.SpawnedGameObjects.Add(gameObject);
		CatalogBehaviour.PerformMod(spawnableAsset, gameObject);
		return gameObject;
	}

	private void Reconstruct(GameObject target, MonoBehaviourPrototype[] data)
	{
		HashSet<MonoBehaviour> hashSet = new HashSet<MonoBehaviour>();
		MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviourPrototype component in data)
		{
			MonoBehaviour monoBehaviour = components.Except(hashSet).FirstOrDefault((MonoBehaviour c) => c.GetType() == component.Type);
			if (monoBehaviour == null)
			{
				monoBehaviour = (MonoBehaviour)target.AddComponent(component.Type);
			}
			try
			{
				component.InjectIntoMonoBehaviour(monoBehaviour);
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
			hashSet.Add(monoBehaviour);
		}
	}

	public void LinkReferences(GameObject target, IEnumerable<SerialisableIdentity> referencePool)
	{
		foreach (KeyValuePair<string, TransformPrototype> child in Children)
		{
			Transform transform = target.transform.Find(child.Key);
			MonoBehaviourPrototype[] value;
			if (transform == null)
			{
				Debug.LogWarning("Copied transform child not found: " + child.Key);
			}
			else if (ChildComponentData.TryGetValue(child.Key, out value))
			{
				LinkReferencesFrom(transform.gameObject, value, referencePool);
			}
		}
		LinkReferencesFrom(target, RootComponentData, referencePool);
	}

	private void LinkReferencesFrom(GameObject target, MonoBehaviourPrototype[] data, IEnumerable<SerialisableIdentity> referencePool)
	{
		HashSet<MonoBehaviour> hashSet = new HashSet<MonoBehaviour>();
		MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviourPrototype component in data)
		{
			MonoBehaviour monoBehaviour = components.Except(hashSet).FirstOrDefault((MonoBehaviour c) => c.GetType() == component.Type);
			if (!(monoBehaviour == null))
			{
				try
				{
					component.LinkReferencesToMonoBehaviour(monoBehaviour, referencePool);
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
				hashSet.Add(monoBehaviour);
			}
		}
	}
}
