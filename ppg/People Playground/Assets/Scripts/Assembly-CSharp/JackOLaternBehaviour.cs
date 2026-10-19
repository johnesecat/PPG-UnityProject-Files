using System.Linq;
using UnityEngine;

public class JackOLaternBehaviour : MonoBehaviour
{
	public Sprite[] GlowingSprites;

	public SpriteRenderer ObjectSprite;

	public SpriteRenderer LightSprite;

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		LightSprite.enabled = GlowingSprites.Contains(ObjectSprite.sprite);
	}
}
