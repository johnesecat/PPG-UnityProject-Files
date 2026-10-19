using System.Collections.Generic;
using UnityEngine;

public class ModGlobals : MonoBehaviour
{
	public Material[] Materials;

	public GameObject[] ParticleEffects;

	public CatalogData Catalog;

	private static bool alreadyRan;

	internal static Dictionary<string, Material> LoadedMaterials = new Dictionary<string, Material>();

	internal static Dictionary<string, GameObject> LoadedParticleEffects = new Dictionary<string, GameObject>();

	internal static CatalogData MainCatalog;

	private void Awake()
	{
		if (!alreadyRan)
		{
			alreadyRan = true;
			MainCatalog = Catalog;
			injectInDictionary<Material>(Materials, LoadedMaterials);
			injectInDictionary<GameObject>(ParticleEffects, LoadedParticleEffects);
		}
		void injectInDictionary<T>(T[] obj, Dictionary<string, T> dict) where T : Object
		{
			foreach (T val in obj)
			{
				dict.Add(val.name, val);
			}
		}
	}
}
