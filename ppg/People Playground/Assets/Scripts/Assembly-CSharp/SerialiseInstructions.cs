using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SerialiseInstructions : MonoBehaviour
{
	public SpawnableAsset OriginalSpawnableAsset;

	public Transform[] RelevantTransforms;

	private void Start()
	{
		Transform[] source = base.transform.GetComponentsInChildren(typeof(Transform), includeInactive: true).Cast<Transform>().ToArray();
		RelevantTransforms = source.Where((Transform c) => !c.gameObject.GetComponent<Optout>() && c != base.transform).ToArray();
		if (!GetComponent<SerialisableIdentity>())
		{
			base.gameObject.AddComponent<SerialisableIdentity>().Regenerate();
		}
		Transform[] relevantTransforms = RelevantTransforms;
		foreach (Transform transform in relevantTransforms)
		{
			if (!transform.GetComponent<SerialisableIdentity>())
			{
				transform.gameObject.AddComponent<SerialisableIdentity>().Regenerate();
			}
		}
	}

	public ObjectState GetCurrentState(Vector3 origin)
	{
		Dictionary<string, TransformPrototype> dictionary = new Dictionary<string, TransformPrototype>();
		Transform[] relevantTransforms = RelevantTransforms;
		foreach (Transform transform in relevantTransforms)
		{
			if ((bool)transform)
			{
				Vector2 relativePosition = base.transform.InverseTransformPoint(transform.position);
				float relativeRotation = transform.eulerAngles.z - base.transform.eulerAngles.z;
				string hierachyPath = Utils.GetHierachyPath(transform);
				if (dictionary.ContainsKey(hierachyPath))
				{
					throw new Exception(hierachyPath + " duplicate found");
				}
				dictionary.Add(hierachyPath, new TransformPrototype(relativePosition, relativeRotation, transform.localScale));
			}
		}
		ObjectState objectState = new ObjectState(OriginalSpawnableAsset.name, new TransformPrototype(base.transform.position - origin, base.transform.eulerAngles.z, base.transform.localScale), dictionary);
		objectState.PackComponentData(base.gameObject, RelevantTransforms);
		return objectState;
	}
}
