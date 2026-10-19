using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ReactorCoreBehaviour : MonoBehaviour
{
	public enum ReactorCriticality
	{
		Subcritical,
		Critical,
		Supercritical
	}

	public enum ReactorState
	{
		Idle,
		LowTemp,
		HighTemp,
		Meltdown
	}

	private const string LowTemperatureAlarmGroup = "LowTemperatureAlarmGroup";

	private const string HighTempAlarmGroup = "HighTempAlarmGroup";

	private const string MeltdownAlarmGroup = "MeltdownAlarmGroup";

	public float MeltdownTemperatureThreshold = 10000f;

	public float HighTemperatureThreshold = 5000f;

	public float LowTemperatureWarningThreshold = 1000f;

	public float MeltdownRequiredTime = 15f;

	[Range(0f, 1f)]
	public float CoolingPower = 0.5f;

	public float InternalTemperature = 2500f;

	[Range(1f, 16384f)]
	public float InsulationPower = 25f;

	[Range(0f, 1f)]
	public float ReactorPowerTargetRatio = 1f;

	public float HeatUpIntensity = 100f;

	public float CoolIntensity = 100f;

	public float TimeVariationSpeed = 1f;

	public float TimeVariationIntensity = 1f;

	public float CoolingPowerRatio = 1f;

	public float ReferenceOutputPower = 1000000f;

	public float OutputPowerSmoothing = 50f;

	[Foldout("LowTemperatureAlarmGroup")]
	public GameObject[] LowTempAlarmGameObjects;

	[Foldout("LowTemperatureAlarmGroup")]
	public MonoBehaviour[] LowTempAlarmBehaviours;

	[Foldout("HighTempAlarmGroup")]
	public GameObject[] HighTempAlarmGameObjects;

	[Foldout("HighTempAlarmGroup")]
	public MonoBehaviour[] HighTempAlarmBehaviours;

	[Foldout("MeltdownAlarmGroup")]
	public GameObject[] MeltdownAlarmGameObjects;

	[Foldout("MeltdownAlarmGroup")]
	public MonoBehaviour[] MeltdownAlarmBehaviours;

	public ReactorConduitSetBehaviour[] EnergyConduits;

	public DoAtTime MeltdownSequence;

	public GameObject[] DisableOnMeltdown;

	public UnityEvent OnMeltdownStart;

	public PhysicalBehaviour[] OutputNodes;

	private PhysicalBehaviour phys;

	private ReactorState lastState;

	private float powerOutput;

	private float lastPoweroutput;

	private float meltdownTime;

	private FixedIntervalDistributor criticalityClock = new FixedIntervalDistributor();

	private bool meltdownStarted;

	private bool hasMeltedDown;

	public ReactorState State;

	public ReactorCriticality Criticality = ReactorCriticality.Critical;

	public float PowerOutput => powerOutput;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
		criticalityClock.RateHz = 8f;
	}

	private void Start()
	{
		InternalTemperature = (LowTemperatureWarningThreshold + HighTemperatureThreshold) / 2f;
		ManageActivationGroups(force: true);
	}

	private void FixedUpdate()
	{
		float b = CalculatePowerOutput();
		powerOutput = Mathf.Lerp(powerOutput, b, 1f / Mathf.Max(OutputPowerSmoothing, 1f));
		for (int i = 0; i < criticalityClock.CalculateCycleCount(Time.fixedDeltaTime); i++)
		{
			CalculateCriticality();
		}
		ControlTemperature();
		Diffuse();
		ManageActivationGroups(force: false);
		if (OutputNodes != null)
		{
			for (int j = 0; j < OutputNodes.Length; j++)
			{
				OutputNodes[j].Charge = Mathf.Max(OutputNodes[j].Charge, powerOutput);
			}
		}
		if (InternalTemperature > MeltdownTemperatureThreshold && (bool)MeltdownSequence && !hasMeltedDown)
		{
			if (!meltdownStarted)
			{
				meltdownStarted = true;
				OnMeltdownStart?.Invoke();
			}
			meltdownTime += Time.fixedDeltaTime;
			if (meltdownTime > MeltdownRequiredTime)
			{
				if (DisableOnMeltdown != null)
				{
					GameObject[] disableOnMeltdown = DisableOnMeltdown;
					for (int k = 0; k < disableOnMeltdown.Length; k++)
					{
						disableOnMeltdown[k].SetActive(value: false);
					}
				}
				MeltdownSequence.enabled = true;
				hasMeltedDown = true;
			}
			else
			{
				phys.Temperature += Time.fixedDeltaTime * 9f;
				CameraShakeBehaviour.main.Shake(Time.fixedDeltaTime * 15f, base.transform.position, 0.1f);
			}
		}
		else
		{
			meltdownStarted = false;
			meltdownTime = 0f;
		}
	}

	private void CalculateCriticality()
	{
		float num = powerOutput / lastPoweroutput;
		if (num < 0.99f)
		{
			Criticality = ReactorCriticality.Subcritical;
		}
		else if (num > 1.01f)
		{
			Criticality = ReactorCriticality.Supercritical;
		}
		else
		{
			Criticality = ReactorCriticality.Critical;
		}
		lastPoweroutput = powerOutput;
	}

	public void ForceMeltdown()
	{
		InternalTemperature = MeltdownTemperatureThreshold * 2f;
		CoolingPowerRatio = 0f;
		ReactorPowerTargetRatio = 1f;
	}

	private void ManageActivationGroups(bool force)
	{
		State = ReactorState.Idle;
		if (InternalTemperature < LowTemperatureWarningThreshold)
		{
			State = ReactorState.LowTemp;
		}
		else if (InternalTemperature > MeltdownTemperatureThreshold)
		{
			State = ReactorState.Meltdown;
		}
		else if (InternalTemperature > HighTemperatureThreshold)
		{
			State = ReactorState.HighTemp;
		}
		if (lastState != State || force)
		{
			lastState = State;
			SynchroniseAlarmObjects(LowTempAlarmGameObjects, LowTempAlarmBehaviours, State == ReactorState.LowTemp);
			SynchroniseAlarmObjects(HighTempAlarmGameObjects, HighTempAlarmBehaviours, State == ReactorState.HighTemp);
			SynchroniseAlarmObjects(MeltdownAlarmGameObjects, MeltdownAlarmBehaviours, State == ReactorState.Meltdown);
		}
	}

	private static void SynchroniseAlarmObjects(GameObject[] gms, MonoBehaviour[] bvhs, bool active)
	{
		for (int i = 0; i < gms.Length; i++)
		{
			if (gms[i].activeSelf != active)
			{
				gms[i].SetActive(active);
			}
		}
		for (int j = 0; j < bvhs.Length; j++)
		{
			if (bvhs[j].enabled != active)
			{
				bvhs[j].enabled = active;
			}
		}
	}

	private void ControlTemperature()
	{
		float num = (Mathf.Clamp01(Mathf.PerlinNoise(-578.2346f, Time.time * TimeVariationSpeed)) * 2f - 1f) * TimeVariationIntensity;
		InternalTemperature += num * Time.fixedDeltaTime;
		InternalTemperature += ReactorPowerTargetRatio * HeatUpIntensity * Time.fixedDeltaTime;
		float b = CoolIntensity * Mathf.Clamp01(CoolingPower * CoolingPowerRatio / 2f) * Time.fixedDeltaTime * Mathf.Clamp01((InternalTemperature + 273.15f) / 100f);
		InternalTemperature -= Mathf.Max(0f, b);
		InternalTemperature = Mathf.Max(InternalTemperature, -273.15f);
	}

	public void IncreaseCooling()
	{
		CoolingPower = Mathf.Clamp01(CoolingPower + 0.1f);
	}

	public void DecreaseCooling()
	{
		CoolingPower = Mathf.Clamp01(CoolingPower - 0.1f);
	}

	public void IncreasePower()
	{
		ReactorPowerTargetRatio = Mathf.Clamp01(ReactorPowerTargetRatio + 0.1f);
	}

	public void DecreasePower()
	{
		ReactorPowerTargetRatio = Mathf.Clamp01(ReactorPowerTargetRatio - 0.1f);
	}

	public void DecrementCoolingPower(float ratio = 0.5f)
	{
		CoolingPowerRatio -= ratio;
	}

	private float CalculatePowerOutput()
	{
		if (hasMeltedDown)
		{
			return 0f;
		}
		float num = 1f;
		if (EnergyConduits != null)
		{
			ReactorConduitSetBehaviour[] energyConduits = EnergyConduits;
			for (int i = 0; i < energyConduits.Length; i++)
			{
				switch (energyConduits[i].GetStatus())
				{
				case ReactorA5DisplayBehaviour.ReactorPartStatus.Questionable:
					num *= 0.7f;
					continue;
				case ReactorA5DisplayBehaviour.ReactorPartStatus.Damaged:
					num *= 0.2f;
					continue;
				case ReactorA5DisplayBehaviour.ReactorPartStatus.Catastrophic:
					break;
				default:
					continue;
				}
				num *= 0f;
				break;
			}
		}
		float num2 = Mathf.Sqrt(Mathf.Clamp01(ReactorPowerTargetRatio + Random.Range(-0.1f, 0.1f))) * ReferenceOutputPower;
		if (float.IsNaN(num2) || float.IsInfinity(num2))
		{
			num2 = ReferenceOutputPower;
		}
		return Mathf.Max(0f, num2) * num;
	}

	private void Diffuse()
	{
		float internalTemperature = InternalTemperature;
		float temperature = phys.Temperature;
		float t = Time.fixedDeltaTime * 10f / Mathf.Clamp(InsulationPower, 1f, 16384f);
		InternalTemperature = Mathf.Lerp(internalTemperature, temperature, t);
		phys.Temperature = Mathf.Lerp(temperature, internalTemperature, t);
		if (!meltdownStarted)
		{
			phys.Temperature = Mathf.Lerp(phys.Temperature, PhysicalBehaviour.AmbientTemperature, 0.01f);
		}
	}
}
