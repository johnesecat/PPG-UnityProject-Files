using System;
using UnityEngine;
using UnityEngine.Animations;

public class LimbStatusBehaviour : MonoBehaviour
{
	public LimbBehaviour limb;

	[Space]
	public Sprite DeadSprite;

	public Sprite BarSprite;

	public Transform Bar;

	public float BarMaxWidth = 9f;

	[Space]
	public ParentConstraint ParentConstraint;

	public SpriteRenderer SpriteRenderer;

	private const float SpritePixelSize = 35f;

	private void Awake()
	{
		Global.main.LimbStatusToggled += Main_LimbStatusToggled;
		base.transform.localPosition = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		base.transform.SetParent(null);
		base.gameObject.SetActive(Global.main.ShowLimbStatus);
	}

	private void Main_LimbStatusToggled(object sender, bool e)
	{
		if (!limb.PhysicalBehaviour.isDisintegrated)
		{
			base.gameObject.SetActive(e);
		}
	}

	private void Start()
	{
		limb.PhysicalBehaviour.OnDisintegration += PhysicalBehaviour_OnDisintegration;
		ParentConstraint.AddSource(new ConstraintSource
		{
			sourceTransform = limb.transform,
			weight = 1f
		});
		ParentConstraint.rotationAxis = Axis.None;
		ParentConstraint.locked = true;
		ParentConstraint.constraintActive = true;
		if (limb.PhysicalBehaviour.isDisintegrated)
		{
			PhysicalBehaviour_OnDisintegration(limb.PhysicalBehaviour, EventArgs.Empty);
		}
	}

	private void PhysicalBehaviour_OnDisintegration(object sender, EventArgs e)
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		Global.main.LimbStatusToggled -= Main_LimbStatusToggled;
		limb.PhysicalBehaviour.OnDisintegration -= PhysicalBehaviour_OnDisintegration;
	}

	private void Update()
	{
		if (SpriteRenderer.isVisible && !limb.PhysicalBehaviour.isDisintegrated)
		{
			if (limb.IsConsideredAlive)
			{
				SpriteRenderer.sprite = BarSprite;
				float num = Mathf.Clamp01(limb.Health / limb.InitialHealth) * Mathf.Clamp01(limb.CirculationBehaviour.BloodFlow) * BarMaxWidth / 35f;
				Bar.localScale = new Vector3(num, 0.028571429f);
				Bar.localPosition = new Vector3((num - BarMaxWidth / 35f) / 2f, 0f);
				Bar.gameObject.SetActive(value: true);
			}
			else
			{
				SpriteRenderer.sprite = DeadSprite;
				Bar.gameObject.SetActive(value: false);
			}
		}
	}
}
