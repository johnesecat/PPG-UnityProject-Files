using System.Linq;
using TMPro;
using UnityEngine;

public class LiquidCanisterBehaviour : BloodContainer
{
	public float Capacity = 70f;

	[SkipSerialisation]
	public AnimationCurve BuoyancyResponseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public TextMeshPro Text;

	public float TextUpdateFrequency = 1f;

	private float t;

	private PhysicalBehaviour phys;

	public override bool AllowsOverflow => false;

	public override Vector2 Limits => new Vector2(0f, Capacity);

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		UpdateText();
	}

	protected override void Update()
	{
		base.Update();
		t += Time.deltaTime;
		phys.BuoyancyModifier = 1f - BuoyancyResponseCurve.Evaluate(ScaledLiquidAmount);
		if (t > 1f / TextUpdateFrequency)
		{
			UpdateText();
		}
	}

	protected override void OnLiquidEnter(Liquid type)
	{
		base.OnLiquidEnter(type);
		UpdateText();
	}

	protected override void OnLiquidExit(Liquid type)
	{
		base.OnLiquidExit(type);
		UpdateText();
	}

	private void UpdateText()
	{
		t = 0f;
		int b = Mathf.CeilToInt(base.TotalLiquidAmount * (5f / 14f));
		int num = Mathf.CeilToInt(Capacity * (5f / 14f));
		b = Mathf.Min(num, b);
		string text = $"{b}L / {num}L";
		if (LiquidDistribution.Count > 1)
		{
			Text.text = "<i>Mixture</i>\n" + text;
			return;
		}
		if (LiquidDistribution.Count == 0)
		{
			Text.text = "<i>(empty)</i>";
			return;
		}
		Liquid liquid = LiquidDistribution.Keys.First();
		Text.text = "<i>" + Utils.EscapeRichText(liquid.GetDisplayName().Replace("\n", " ")) + "</i>\n" + text;
	}
}
