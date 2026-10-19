using UnityEngine;

public class VaseDebrisBehaviour : MonoBehaviour
{
	public ParticleSystem SmallDebris;

	public SpriteRenderer[] Shards;

	public void SetSprite(Sprite vaseSprite)
	{
		ParticleSystem.ShapeModule shape = SmallDebris.shape;
		shape.texture = vaseSprite.texture;
		for (int i = 0; i < Shards.Length; i++)
		{
			Shards[i].sprite = vaseSprite;
		}
	}
}
