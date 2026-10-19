using System;
using UnityEngine;

[Obsolete]
public class DecalRenderer : MonoBehaviour, Messages.IDecal
{
	public DecalDescriptor DecalType;

	private SpriteRenderer spriteRenderer;

	public const int MaxDecals = 32;

	public int LoopIndex;

	public int CurrentDecalAmount;

	private Vector2[] localPositionBuffer = new Vector2[32];

	private static int[] shaderPropertyIndices = new int[32]
	{
		ShaderProperties.Get("_Decal01"),
		ShaderProperties.Get("_Decal02"),
		ShaderProperties.Get("_Decal03"),
		ShaderProperties.Get("_Decal04"),
		ShaderProperties.Get("_Decal05"),
		ShaderProperties.Get("_Decal06"),
		ShaderProperties.Get("_Decal07"),
		ShaderProperties.Get("_Decal08"),
		ShaderProperties.Get("_Decal09"),
		ShaderProperties.Get("_Decal10"),
		ShaderProperties.Get("_Decal11"),
		ShaderProperties.Get("_Decal12"),
		ShaderProperties.Get("_Decal13"),
		ShaderProperties.Get("_Decal14"),
		ShaderProperties.Get("_Decal15"),
		ShaderProperties.Get("_Decal16"),
		ShaderProperties.Get("_Decal17"),
		ShaderProperties.Get("_Decal18"),
		ShaderProperties.Get("_Decal19"),
		ShaderProperties.Get("_Decal20"),
		ShaderProperties.Get("_Decal21"),
		ShaderProperties.Get("_Decal22"),
		ShaderProperties.Get("_Decal23"),
		ShaderProperties.Get("_Decal24"),
		ShaderProperties.Get("_Decal25"),
		ShaderProperties.Get("_Decal26"),
		ShaderProperties.Get("_Decal27"),
		ShaderProperties.Get("_Decal28"),
		ShaderProperties.Get("_Decal29"),
		ShaderProperties.Get("_Decal30"),
		ShaderProperties.Get("_Decal31"),
		ShaderProperties.Get("_Decal32")
	};

	private MaterialPropertyBlock properties;

	private void Start()
	{
		if (!base.transform.parent)
		{
			Debug.LogErrorFormat("{0} without a parent... This is not allowed.", "DecalRenderer");
			base.enabled = false;
			return;
		}
		for (int i = 0; i < 32; i++)
		{
			localPositionBuffer[i] = new Vector2(float.MaxValue, float.MaxValue);
		}
		properties = new MaterialPropertyBlock();
		spriteRenderer = GetComponent<SpriteRenderer>();
		UpdateContainingSprite(base.transform.parent.GetComponent<SpriteRenderer>());
		base.name = "Decal renderer for " + DecalType.name;
	}

	public void UpdateContainingSprite(SpriteRenderer container)
	{
		spriteRenderer.sprite = container.sprite;
		spriteRenderer.sortingOrder = container.sortingOrder;
		spriteRenderer.sortingLayerID = container.sortingLayerID;
		spriteRenderer.GetPropertyBlock(properties);
	}

	private void OnWillRenderObject()
	{
		if ((bool)spriteRenderer)
		{
			spriteRenderer.SetPropertyBlock(properties);
		}
	}

	public void Decal(DecalInstruction d)
	{
		if (!spriteRenderer || d.type != DecalType)
		{
			return;
		}
		Vector2 vector = base.transform.InverseTransformPoint(d.globalPosition);
		float num = DecalType.IgnoreRadius * DecalType.IgnoreRadius / 4f;
		for (int i = 0; i < CurrentDecalAmount; i++)
		{
			if ((localPositionBuffer[i] - vector).sqrMagnitude < num)
			{
				return;
			}
		}
		int num2 = UnityEngine.Random.Range(0, DecalType.Sprites.Length);
		Matrix4x4 value = default(Matrix4x4);
		value.m00 = vector.x;
		value.m01 = vector.y;
		value.m02 = 0.4f;
		value.m10 = d.colourMultiplier.r * DecalType.Color.r;
		value.m11 = d.colourMultiplier.g * DecalType.Color.g;
		value.m12 = d.colourMultiplier.b * DecalType.Color.b;
		value.m13 = d.colourMultiplier.a * DecalType.Color.a;
		value.m20 = num2;
		CurrentDecalAmount++;
		CurrentDecalAmount = Mathf.Min(CurrentDecalAmount, 32);
		properties.SetMatrix(shaderPropertyIndices[LoopIndex], value);
		properties.SetInt(ShaderProperties.Get("_DecalCount"), CurrentDecalAmount);
		localPositionBuffer[LoopIndex] = vector;
		LoopIndex++;
		LoopIndex %= 32;
	}
}
