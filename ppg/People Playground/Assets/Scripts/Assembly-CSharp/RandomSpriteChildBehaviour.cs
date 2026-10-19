using UnityEngine;

public class RandomSpriteChildBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Sprite[] Sprites = new Sprite[0];

	private SpriteRenderer spriteRenderer;

	private RandomSpriteBehaviour parent;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		parent = GetComponentInParent<RandomSpriteBehaviour>();
		if ((bool)parent)
		{
			parent.OnAfterChange.AddListener(SyncSprite);
		}
	}

	private void SyncSprite()
	{
		spriteRenderer.sprite = Sprites[parent.chosenIndex];
	}
}
