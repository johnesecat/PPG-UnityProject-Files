using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Decal Descriptor")]
public class DecalDescriptor : ScriptableObject
{
	public Color Color = Color.white;

	public float IgnoreRadius = 0.2f;

	public Sprite[] Sprites;

	public int MinDripCount;

	public int MaxDripCount;

	[ShowIf("WillDrip")]
	public Sprite DripParticle;

	internal bool WillDrip => MaxDripCount > 0;
}
