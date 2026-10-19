using UnityEngine;

public class GorseEyeBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public Rect Container;

	[SkipSerialisation]
	public LimbBehaviour Limb;

	[SkipSerialisation]
	public SpriteRenderer Renderer;

	[SkipSerialisation]
	public float SmallRandomRadius = 0.02f;

	[SkipSerialisation]
	public Vector2 LargeRange = new Vector2(0.7f, 1f);

	[SkipSerialisation]
	public Vector2 SmallRange = new Vector2(0.7f, 1f);

	private float largeInterval;

	private float smallInterval;

	private float largeTimer;

	private float smallTimer;

	private bool isRendering = true;

	private void Awake()
	{
		largeInterval = Utils.RandomBetween(LargeRange);
		smallInterval = Utils.RandomBetween(SmallRange);
	}

	private void Update()
	{
		if (Limb.Person.Consciousness < 0.5f || !Limb.IsConsideredAlive)
		{
			if (isRendering)
			{
				isRendering = false;
				Renderer.enabled = isRendering;
			}
			return;
		}
		if (!isRendering)
		{
			isRendering = true;
			Renderer.enabled = isRendering;
		}
		if (!Limb.Frozen)
		{
			largeTimer += Time.deltaTime;
			smallTimer += Time.deltaTime;
		}
		if (largeTimer > largeInterval)
		{
			base.transform.localPosition = Container.position + new Vector2(Random.value * Container.size.x, Random.value * Container.size.y);
			largeInterval = Utils.RandomBetween(LargeRange);
			largeTimer = 0f;
		}
		if (smallTimer > smallInterval && largeTimer > largeInterval / 6f)
		{
			base.transform.localPosition += (Vector3)Random.insideUnitCircle * SmallRandomRadius;
			smallInterval = Utils.RandomBetween(SmallRange);
			smallTimer = 0f;
		}
	}
}
