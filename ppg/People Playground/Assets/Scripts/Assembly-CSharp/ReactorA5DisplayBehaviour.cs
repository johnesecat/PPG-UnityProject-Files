using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ReactorA5DisplayBehaviour : MonoBehaviour
{
	public enum ReactorPartStatus
	{
		Normal,
		Questionable,
		Damaged,
		Catastrophic
	}

	public ReactorCoreBehaviour Core;

	public TextMeshPro TextMesh;

	public Material DiagnosticsMaterial;

	public ReactorConduitSetBehaviour Wire1;

	public ReactorConduitSetBehaviour Wire2;

	public ReactorConduitSetBehaviour Wire3;

	public ReactorConduitSetBehaviour Wire4;

	public AudioClip AlertBeep;

	private Color wire1Col = Color.white;

	private Color wire2Col = Color.white;

	private Color wire3Col = Color.white;

	private Color wire4Col = Color.white;

	private Color vacuumChamberCol = Color.white;

	private Color connectorCol = Color.white;

	private Color baseCol = Color.white;

	private PhysicalBehaviour phys;

	private FixedIntervalDistributor alertBeepClock = new FixedIntervalDistributor();

	private FixedIntervalDistributor updateDisplayClock = new FixedIntervalDistributor();

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		alertBeepClock.RateHz = 2f;
		updateDisplayClock.RateHz = 10f;
	}

	private void Update()
	{
		for (int i = 0; i < updateDisplayClock.CalculateCycleCount(Time.deltaTime); i++)
		{
			UpdateDisplayText();
		}
		ReactorPartStatus status = Wire1.GetStatus();
		ReactorPartStatus status2 = Wire2.GetStatus();
		ReactorPartStatus status3 = Wire3.GetStatus();
		ReactorPartStatus status4 = Wire4.GetStatus();
		wire1Col = GetColorForStatus(status);
		wire2Col = GetColorForStatus(status2);
		wire3Col = GetColorForStatus(status3);
		wire4Col = GetColorForStatus(status4);
		vacuumChamberCol = GetColorForStatus(ReactorPartStatus.Normal);
		connectorCol = GetColorForStatus(ReactorPartStatus.Normal);
		baseCol = GetColorForStatus(ReactorPartStatus.Normal);
		ReactorPartStatus worstStatus = (ReactorPartStatus)Math.Max((int)status, Math.Max((int)status2, Math.Max((int)status3, (int)status4)));
		PlayAlertNoise(worstStatus);
	}

	private void UpdateDisplayText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string temperatureUnitSuffix = Utils.GetTemperatureUnitSuffix(UserPreferenceManager.Current.TemperatureUnit);
		stringBuilder.AppendLine("<align=center>CURRENT REACTOR DATA</align>");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("TEMPERATURE: {0}{1}\n", Math.Round(Utils.CelsiusToPreference(Core.InternalTemperature), 2), temperatureUnitSuffix);
		stringBuilder.AppendFormat("COOLING PUMPS: {0}%\n", Mathf.RoundToInt(Core.CoolingPower * 100f));
		stringBuilder.AppendFormat("CONTROL ROD DEPTH: {0}%\n", Mathf.RoundToInt((1f - Core.ReactorPowerTargetRatio) * 100f));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("REACTOR IS {0}\n", Core.Criticality.ToString().ToUpperInvariant());
		stringBuilder.AppendFormat("POWER OUTPUT: {0} E\n", Mathf.RoundToInt(Core.PowerOutput));
		stringBuilder.AppendLine("- - - -");
		if (Core.State == ReactorCoreBehaviour.ReactorState.LowTemp && Time.time % 2f > 1f)
		{
			stringBuilder.AppendFormat("<color=orange>LOW TEMPERATURE</color>\n");
		}
		if (Core.State == ReactorCoreBehaviour.ReactorState.HighTemp && Time.time % 2f > 1f)
		{
			stringBuilder.AppendFormat("<color=orange>HIGH TEMPERATURE</color>\n");
		}
		if (Core.State == ReactorCoreBehaviour.ReactorState.Meltdown && Time.time % 1f > 0.5f)
		{
			stringBuilder.AppendFormat("<color=red>CATASTROPHIC TEMPERATURE</color>\n");
		}
		TextMesh.text = stringBuilder.ToString();
	}

	private void PlayAlertNoise(ReactorPartStatus worstStatus)
	{
		switch (worstStatus)
		{
		default:
			return;
		case ReactorPartStatus.Normal:
			return;
		case ReactorPartStatus.Questionable:
			alertBeepClock.RateHz = 0.5f;
			break;
		case ReactorPartStatus.Damaged:
			alertBeepClock.RateHz = 1f;
			break;
		case ReactorPartStatus.Catastrophic:
			alertBeepClock.RateHz = 4f;
			break;
		}
		for (int i = 0; i < alertBeepClock.CalculateCycleCount(Time.deltaTime); i++)
		{
			phys.PlayClipOnce(AlertBeep, 0.1f);
		}
	}

	private void OnWillRenderObject()
	{
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_Wire1"), wire1Col);
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_Wire2"), wire2Col);
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_Wire3"), wire3Col);
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_Wire4"), wire4Col);
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_VacuumChamber"), vacuumChamberCol);
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_Connector"), connectorCol);
		DiagnosticsMaterial.SetColor(ShaderProperties.Get("_Base"), baseCol);
	}

	private static float Square(float x)
	{
		return (Mathf.Sin((float)Math.PI * x) > 0f) ? 1 : 0;
	}

	public static Color GetColorForStatus(ReactorPartStatus status)
	{
		switch (status)
		{
		case ReactorPartStatus.Questionable:
			return Color.Lerp(Color.white, Color.yellow, Square(Time.time));
		case ReactorPartStatus.Damaged:
			return Color.Lerp(Color.yellow, Color.red, Square(Time.time * 4f));
		case ReactorPartStatus.Catastrophic:
			return Color.Lerp(Color.clear, Color.red, Square(Time.time * 9f));
		default:
			return Color.white;
		}
	}
}
