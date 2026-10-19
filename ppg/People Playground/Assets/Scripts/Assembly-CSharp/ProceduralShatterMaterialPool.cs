using System.Collections.Generic;
using UnityEngine;

public class ProceduralShatterMaterialPool : MonoBehaviour
{
	public Material Base;

	public Texture2D[] ShatterMasks;

	public Dictionary<Sprite, Material> Pool = new Dictionary<Sprite, Material>();

	public static ProceduralShatterMaterialPool Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public static Material Get(Sprite sprite)
	{
		if (Instance.Pool.TryGetValue(sprite, out var value))
		{
			value.SetTexture(ShaderProperties.Get("_ShardIDMask"), Instance.ShatterMasks.PickRandom());
			return value;
		}
		value = new Material(Instance.Base);
		Vector4 vector = new Vector4(0f, 0f, 1f, 1f);
		Vector2 vector2 = new Vector2(sprite.texture.width, sprite.texture.height);
		Vector2 min = Utils.GetMin(sprite.uv, SelectSqrMagnitude);
		Vector2 vector3 = new Vector2(sprite.rect.width, sprite.rect.height);
		value.SetVector(value: new Vector4(min.x, min.y, vector3.x / vector2.x, vector3.y / vector2.y), nameID: ShaderProperties.Get("_AtlasTransform"));
		value.SetTexture(ShaderProperties.Get("_ShardIDMask"), Instance.ShatterMasks.PickRandom());
		Instance.Pool.Add(sprite, value);
		return value;
	}

	private static float SelectSqrMagnitude(Vector2 v)
	{
		return v.sqrMagnitude;
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<Sprite, Material> item in Pool)
		{
			Object.Destroy(item.Value);
		}
	}
}
