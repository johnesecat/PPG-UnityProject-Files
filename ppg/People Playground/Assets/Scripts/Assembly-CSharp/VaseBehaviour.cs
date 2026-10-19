using UnityEngine;

public class VaseBehaviour : DestroyableBehaviour
{
	private SpriteRenderer spriteRenderer;

	protected override void Awake()
	{
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void OnDebrisCreated(GameObject createdDebris, Vector2 velocity)
	{
		createdDebris.GetComponent<VaseDebrisBehaviour>().SetSprite(spriteRenderer.sprite);
		base.OnDebrisCreated(createdDebris, velocity);
	}
}
