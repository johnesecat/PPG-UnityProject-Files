using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class LimbStatusViewBehaviour : MonoBehaviour
{
	[Min(0.016666668f)]
	public float UpdateInterval = 1f;

	[Space]
	public TextMeshProUGUI LimbDamageSourceStats;

	public TextMeshProUGUI BloodInformation;

	public RectTransform ConsciousnessBar;

	public RectTransform ShockBar;

	public RectTransform OxygenBar;

	public RectTransform AdrenalineBar;

	public static LimbStatusViewBehaviour Main;

	public List<LimbBehaviour> Limbs;

	private float t;

	private void Awake()
	{
		Main = this;
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		t += Time.unscaledDeltaTime;
		if (t > UpdateInterval)
		{
			t = 0f;
			Refresh();
		}
	}

	private void Refresh()
	{
		if (Limbs == null || Limbs.Count == 0 || Limbs.Any((LimbBehaviour c) => !c))
		{
			LimbDamageSourceStats.text = "";
			BloodInformation.text = "";
			base.gameObject.SetActive(value: false);
		}
		else
		{
			LimbDamageSourceStats.text = $"<b>{Limbs.Count} limbs selected</b>\n<color=#FF5555>{GetLimbState()}</color>{GetSourceStats()}";
			BloodInformation.text = GetBloodInformation();
		}
	}

	private string GetBloodInformation()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		for (int i = 0; i < Limbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = Limbs[i];
			num += Mathf.Clamp01(limbBehaviour.Health / limbBehaviour.InitialHealth);
			num2 += limbBehaviour.CirculationBehaviour.GetAmount(limbBehaviour.GetOriginalBloodType());
			num3 += limbBehaviour.CirculationBehaviour.BloodFlow;
			num4 += Mathf.Clamp01(limbBehaviour.Person.Consciousness);
			num5 += Mathf.Clamp01(limbBehaviour.Person.ShockLevel);
			num6 += Mathf.Clamp01(limbBehaviour.Person.OxygenLevel);
			num7 += Mathf.Clamp01(Mathf.Clamp01(limbBehaviour.Person.AdrenalineLevel));
			num8 += limbBehaviour.InternalTemperature;
		}
		num2 /= (float)Limbs.Count;
		num4 /= (float)Limbs.Count;
		num5 /= (float)Limbs.Count;
		num6 /= (float)Limbs.Count;
		num7 /= (float)Limbs.Count;
		num8 /= (float)Limbs.Count;
		num8 = Utils.CelsiusToPreference(num8);
		ConsciousnessBar.localScale = new Vector3(1f, num4, 1f);
		ShockBar.localScale = new Vector3(1f, num5, 1f);
		OxygenBar.localScale = new Vector3(1f, num6, 1f);
		AdrenalineBar.localScale = new Vector3(1f, num7, 1f);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("integrity");
		stringBuilder.AppendFormat("{0}%\n\n", Mathf.RoundToInt(num / (float)Limbs.Count * 100f));
		stringBuilder.AppendLine("blood amount");
		stringBuilder.AppendFormat("{0}%\n\n", Mathf.RoundToInt(num2 * 100f));
		stringBuilder.AppendLine("blood pressure");
		stringBuilder.AppendFormat("{0}%\n\n", Mathf.RoundToInt(num3 / (float)Limbs.Count * 100f));
		stringBuilder.AppendLine("internal temp.");
		stringBuilder.AppendFormat("{0} {1}", Mathf.RoundToInt(num8), Utils.GetTemperatureUnitSuffix(UserPreferenceManager.Current.TemperatureUnit));
		return stringBuilder.ToString();
	}

	private string GetLimbState()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		byte b = 1;
		for (int i = 0; i < Limbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = Limbs[i];
			flag |= limbBehaviour.CirculationBehaviour.BleedingRate > 0.5f;
			flag4 |= limbBehaviour.CirculationBehaviour.InternalBleedingIntensity > 0.1f;
			flag5 |= limbBehaviour.LungsPunctured;
			flag2 |= limbBehaviour.Numbness > 0.5f;
			flag6 |= limbBehaviour.CirculationBehaviour.WasInitiallyPumping && (!limbBehaviour.CirculationBehaviour.IsPump || !limbBehaviour.IsConsideredAlive);
			flag3 |= limbBehaviour.IsDismembered;
			if (limbBehaviour.HasBrain)
			{
				flag8 |= limbBehaviour.Person.BrainDamaged;
				flag7 |= limbBehaviour.Person.Braindead;
			}
			if (limbBehaviour.IsConsideredAlive)
			{
				if (b != byte.MaxValue)
				{
					b = 0;
				}
			}
			else if (limbBehaviour.HasBrain)
			{
				b = byte.MaxValue;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (b != 0)
		{
			stringBuilder.AppendLine("dead");
		}
		if (flag)
		{
			stringBuilder.AppendLine("bleeding");
		}
		if (flag4)
		{
			stringBuilder.AppendLine("internal bleeding");
		}
		if (flag5)
		{
			stringBuilder.AppendLine("punctured lungs");
		}
		if (flag2)
		{
			stringBuilder.AppendLine("numb");
		}
		if (flag3)
		{
			stringBuilder.AppendLine("dismembered");
		}
		if (flag6)
		{
			stringBuilder.AppendLine("heart stopped");
		}
		if (flag7)
		{
			stringBuilder.AppendLine("brain-dead");
		}
		if (flag8)
		{
			stringBuilder.AppendLine("brain damage");
		}
		return stringBuilder.ToString();
	}

	private string GetSourceStats()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		for (int i = 0; i < Limbs.Count; i++)
		{
			LimbBehaviour limbBehaviour = Limbs[i];
			num7 += Mathf.Clamp01(limbBehaviour.SkinMaterialHandler.RottenProgress) * 100f;
			num6 += Mathf.Clamp01(limbBehaviour.SkinMaterialHandler.AcidProgress) * 100f;
			num5 += Mathf.Clamp01(limbBehaviour.PhysicalBehaviour.BurnProgress) * 100f;
			if (limbBehaviour.Broken)
			{
				num4++;
			}
			num3 += limbBehaviour.CirculationBehaviour.StabWoundCount;
			num += limbBehaviour.BruiseCount;
			num2 += limbBehaviour.CirculationBehaviour.GunshotWoundCount;
		}
		num5 = Mathf.RoundToInt(num5 / (float)Limbs.Count);
		num6 = Mathf.RoundToInt(num6 / (float)Limbs.Count);
		num7 = Mathf.RoundToInt(num7 / (float)Limbs.Count);
		string text = "";
		if (num > 0)
		{
			text = text + num + " " + ((num == 1) ? "bruise" : "bruises") + "\n";
		}
		if (num3 > 0)
		{
			text = text + num3 + " stab " + ((num3 == 1) ? "wound" : "wounds") + "\n";
		}
		if (num2 > 0)
		{
			text = text + num2 + " gunshot " + ((num2 == 1) ? "wound" : "wounds") + "\n";
		}
		if (num4 > 0)
		{
			text += ((Limbs.Count == 1) ? "broken bone\n" : (num4 + " broken " + ((num4 == 1) ? "bone" : "bones") + "\n"));
		}
		if (num5 > 0f)
		{
			text = text + num5 + "% burnt\n";
		}
		if (num7 > 0f)
		{
			text = text + num7 + "% rotted\n";
		}
		if (num6 > 0f)
		{
			text = text + num6 + "% skin damage\n";
		}
		return text;
	}
}
