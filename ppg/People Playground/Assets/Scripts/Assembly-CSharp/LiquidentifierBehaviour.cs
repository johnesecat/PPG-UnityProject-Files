using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class LiquidentifierBehaviour : BloodContainer, Messages.IUse
{
	[SkipSerialisation]
	public AudioClip FlushSound;

	[SkipSerialisation]
	public AudioClip[] Clips;

	[SkipSerialisation]
	public SpriteRenderer[] Lights;

	[SkipSerialisation]
	public TextMeshPro TextMesh;

	[SkipSerialisation]
	public TextMeshPro StateMesh;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public LiquidContainerController LiquidContainer;

	private bool isBusy;

	public override Vector2 Limits => new Vector2(0f, 7f);

	public override bool AllowsOverflow => false;

	public void Use(ActivationPropagation p)
	{
		if (base.enabled)
		{
			if (p.Channel == 1)
			{
				StartCoroutine(FlushRoutine());
			}
			else
			{
				StartCoroutine(IdentificationRoutine());
			}
		}
	}

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => !isBusy, "flushLiquidentifier", "Flush", "Drain the container of its contents", delegate
		{
			StartCoroutine(FlushRoutine());
		}));
	}

	private IEnumerator IdentificationRoutine()
	{
		if (isBusy)
		{
			yield break;
		}
		isBusy = true;
		for (int j = 0; j < Lights.Length; j++)
		{
			Lights[j].enabled = true;
		}
		PhysicalBehaviour.PlayClipOnce(Clips.PickRandom());
		TextMesh.text = "INITIALISING...";
		yield return new WaitForSeconds(1.123f);
		TextMesh.text = "SCANNING...";
		for (int i = 0; i < 30; i++)
		{
			for (int k = 0; k < Lights.Length; k++)
			{
				Lights[k].enabled = (double)Random.value > 0.5;
			}
			yield return new WaitForSeconds(0.0682f);
		}
		StringBuilder stringBuilder = new StringBuilder();
		DeleteEmptyLiquidEntries();
		stringBuilder.Append("<align=\"center\"><#fff>LIQUID CONTENTS\n</align></color>\n\n");
		if (base.TotalLiquidAmount > float.Epsilon)
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<Liquid, RefFloat> item in LiquidDistribution.OrderByDescending((KeyValuePair<Liquid, RefFloat> l) => l.Value.Raw))
			{
				float num3 = item.Value.Raw / base.TotalLiquidAmount;
				int num4 = ((num2 == LiquidDistribution.Count - 1) ? (100 - num) : Mathf.RoundToInt(num3 * 100f));
				if (num4 > 0)
				{
					stringBuilder.Append(num4);
					stringBuilder.Append("% ");
					stringBuilder.Append(item.Key.GetDisplayName());
					stringBuilder.AppendLine();
					num += num4;
					num2++;
				}
			}
		}
		else
		{
			stringBuilder.Append("<#222>(EMPTY)");
		}
		for (int m = 0; m < Lights.Length; m++)
		{
			Lights[m].enabled = false;
		}
		TextMesh.text = stringBuilder.ToString();
		isBusy = false;
	}

	private IEnumerator FlushRoutine()
	{
		if (!isBusy)
		{
			isBusy = true;
			PhysicalBehaviour.PlayClipOnce(FlushSound);
			TextMesh.text = "FLUSHING...";
			float startAmount = base.TotalLiquidAmount;
			for (float timeSpent = 0f; (base.TotalLiquidAmount >= float.Epsilon || timeSpent <= FlushSound.length) && timeSpent < 5f; timeSpent += Time.deltaTime)
			{
				Drain(startAmount * Time.deltaTime / 1.521f);
				yield return new WaitForEndOfFrame();
			}
			DeleteEmptyLiquidEntries();
			TextMesh.text = "Flushed :)";
			isBusy = false;
		}
	}

	protected override void Update()
	{
		base.Update();
		StateMesh.text = (float)Mathf.CeilToInt(base.TotalLiquidAmount * (5f / 14f) * 10f) * 0.1f + " LITER";
		if ((bool)LiquidContainer)
		{
			LiquidContainer.FillPercentage = Mathf.Clamp01(ScaledLiquidAmount);
			LiquidContainer.Color = GetComputedColor();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].enabled = false;
		}
		if ((bool)PhysicalBehaviour.MainAudioSource)
		{
			PhysicalBehaviour.MainAudioSource.Stop();
		}
		isBusy = false;
		TextMesh.text = "ABRUPT SHUTDOWN RECOVERY";
	}
}
