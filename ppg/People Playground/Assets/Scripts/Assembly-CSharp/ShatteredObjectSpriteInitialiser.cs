using UnityEngine;

public class ShatteredObjectSpriteInitialiser : MonoBehaviour
{
	[SkipSerialisation]
	public ShatteredObjectGenerator BoneGenerator;

	[SkipSerialisation]
	public ShatteredObjectGenerator FleshGenerator;

	[SkipSerialisation]
	public ShatteredObjectGenerator SkinGenerator;

	public void UpdateSprites(in LimbSpriteCache.LimbSprites sprites)
	{
		if ((bool)GetComponent<SkinMaterialHandler>())
		{
			if ((bool)BoneGenerator && (bool)sprites.Bone)
			{
				BoneGenerator.Sprite = sprites.Bone;
			}
			if ((bool)FleshGenerator && (bool)sprites.Flesh)
			{
				FleshGenerator.Sprite = sprites.Flesh;
			}
			if ((bool)SkinGenerator && (bool)sprites.Skin)
			{
				SkinGenerator.Sprite = sprites.Skin;
			}
		}
	}
}
