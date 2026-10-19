using System;
using System.Collections;
using UnityEngine;

public class BloodWireBehaviour : DistanceJointWireBehaviour
{
	private CirculationBehaviour limbA;

	private CirculationBehaviour limbB;

	private BloodContainer containerA;

	private BloodContainer containerB;

	private MaterialPropertyBlock materialPropertyBlock;

	private float flow;

	private Color color;

	private CardiopulmonaryBypassMachineBehaviour cardiopulmonaryBypass;

	private HeartMonitorBehaviour heartMonitor;

	private CirculationBehaviour heartLimb;

	public const float TransferRate = 2f;

	[SkipSerialisation]
	public bool CanTransferBlood
	{
		get
		{
			if ((bool)containerA && (bool)containerB && containerA.AllowsTransfer)
			{
				return containerB.AllowsTransfer;
			}
			return false;
		}
	}

	[SkipSerialisation]
	public bool AreBothLimbs { get; private set; }

	[SkipSerialisation]
	public bool IsHeartRateWire { get; private set; }

	[SkipSerialisation]
	public bool IsCardiopulmonaryBypassWire { get; private set; }

	protected override void Created()
	{
		base.Created();
		color = Color.clear;
		materialPropertyBlock = new MaterialPropertyBlock();
		lineRenderer.GetPropertyBlock(materialPropertyBlock);
		if (!physicalBehaviour || !otherPhysicalBehaviour || physicalBehaviour == otherPhysicalBehaviour)
		{
			AreBothLimbs = false;
			materialPropertyBlock.SetFloat(ShaderProperties.Get("_Flow"), 0f);
			materialPropertyBlock.SetColor(ShaderProperties.Get("_BloodColor"), Color.magenta);
			lineRenderer.SetPropertyBlock(materialPropertyBlock);
			return;
		}
		containerA = physicalBehaviour.GetComponent<BloodContainer>();
		containerB = otherPhysicalBehaviour.GetComponent<BloodContainer>();
		limbA = physicalBehaviour.GetComponent<CirculationBehaviour>();
		limbB = otherPhysicalBehaviour.GetComponent<CirculationBehaviour>();
		heartMonitor = physicalBehaviour.GetComponent<HeartMonitorBehaviour>();
		if (!heartMonitor)
		{
			heartMonitor = otherPhysicalBehaviour.GetComponent<HeartMonitorBehaviour>();
		}
		cardiopulmonaryBypass = physicalBehaviour.GetComponent<CardiopulmonaryBypassMachineBehaviour>();
		if (!cardiopulmonaryBypass)
		{
			cardiopulmonaryBypass = otherPhysicalBehaviour.GetComponent<CardiopulmonaryBypassMachineBehaviour>();
		}
		IsHeartRateWire = heartMonitor;
		IsCardiopulmonaryBypassWire = (bool)cardiopulmonaryBypass && ((bool)limbA || (bool)limbB) && (limbA ? limbA.WasInitiallyPumping : limbB.WasInitiallyPumping) && !(limbA ? limbA.Limb.IsAndroid : limbB.Limb.IsAndroid);
		string s;
		if (IsCardiopulmonaryBypassWire)
		{
			cardiopulmonaryBypass.PlayValidSound();
			cardiopulmonaryBypass.ValidConnectionCount++;
			cardiopulmonaryBypass.UpdateStatusText();
		}
		else if ((bool)cardiopulmonaryBypass)
		{
			cardiopulmonaryBypass.PlayInvalidSound();
			CirculationBehaviour circulationBehaviour = (limbA ? limbA : limbB);
			s = string.Empty;
			if (!circulationBehaviour)
			{
				s = "<color=red>INORGANIC</color>";
			}
			else if (circulationBehaviour.Limb.IsAndroid)
			{
				s = "<color=red>ARTIFICIAL</color>";
			}
			else if (!circulationBehaviour.WasInitiallyPumping)
			{
				s = "<color=red>NO HEART DETECTED</color>";
			}
			if (!string.IsNullOrEmpty(s))
			{
				cardiopulmonaryBypass.StartCoroutine(flash());
			}
			else
			{
				cardiopulmonaryBypass.UpdateStatusText();
			}
		}
		if (IsHeartRateWire)
		{
			if ((bool)limbA && !limbA.Limb.IsAndroid)
			{
				heartLimb = limbA;
			}
			else if ((bool)limbB && !limbB.Limb.IsAndroid)
			{
				heartLimb = limbB;
			}
			else
			{
				IsHeartRateWire = false;
			}
		}
		AreBothLimbs = (bool)limbA && (bool)limbB && !limbA.Limb.IsAndroid && !limbB.Limb.IsAndroid;
		materialPropertyBlock.SetFloat(ShaderProperties.Get("_Flow"), 0f);
		materialPropertyBlock.SetColor(ShaderProperties.Get("_BloodColor"), Color.black);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
		IEnumerator flash()
		{
			cardiopulmonaryBypass.StatusText.text = s;
			yield return new WaitForSeconds(3f);
			if ((bool)cardiopulmonaryBypass)
			{
				cardiopulmonaryBypass.UpdateStatusText();
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		ProcessCardiopulmonaryBypassWire();
	}

	private void ProcessCardiopulmonaryBypassWire()
	{
		if (IsCardiopulmonaryBypassWire)
		{
			float num = Mathf.Clamp01(cardiopulmonaryBypass.GetFinalIntensity());
			CirculationBehaviour circulationBehaviour = (limbA ? limbA : limbB);
			if (circulationBehaviour.Limb.NodeBehaviour.IsConnectedToRoot)
			{
				circulationBehaviour.Limb.Person.OxygenLevel = Mathf.Max(0.5f * num, circulationBehaviour.Limb.Person.OxygenLevel);
			}
			circulationBehaviour.ArtificialHeartbeat = cardiopulmonaryBypass.TargetHeartrate * num;
			circulationBehaviour.BloodFlow = Mathf.Max(circulationBehaviour.BloodFlow, num);
		}
	}

	protected override void Tick()
	{
		if (IsHeartRateWire)
		{
			SendHeartRate();
		}
		float transferRate = Time.fixedDeltaTime * 2f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (CanTransferBlood)
		{
			Transfer(containerA, containerB, 1f, transferRate);
		}
		AveragePressure(fixedDeltaTime, containerA, containerB);
		if (AreBothLimbs)
		{
			float bloodFlow = limbA.BloodFlow;
			float bloodFlow2 = limbB.BloodFlow;
			float b = 0.5f * (bloodFlow + bloodFlow2);
			if (limbA.AllowPressureTransfer && limbB.AllowPressureTransfer)
			{
				limbA.BloodFlow = Mathf.Lerp(bloodFlow, b, fixedDeltaTime);
				limbB.BloodFlow = Mathf.Lerp(bloodFlow2, b, fixedDeltaTime);
			}
		}
		flow = Mathf.Lerp(flow, 0f, 0.1f);
		color = Color.Lerp(color, Color.clear, 0.1f);
	}

	private void Transfer(BloodContainer a, BloodContainer b, float visualDir, float transferRate)
	{
		if (a.MeasuredPressure > b.MeasuredPressure)
		{
			TransferLiquid(transferRate, a, b, visualDir);
		}
		else if (b.MeasuredPressure > a.MeasuredPressure)
		{
			TransferLiquid(transferRate, b, a, 0f - visualDir);
		}
	}

	private void SendHeartRate()
	{
		if (heartLimb.Limb.PhysicalBehaviour.isDisintegrated || physicalBehaviour.isDisintegrated)
		{
			heartMonitor.IsConnected = false;
			return;
		}
		heartMonitor.IsConnected = true;
		heartMonitor.ConnectedLimb = heartLimb.Limb;
	}

	private void OnWillRenderObject()
	{
		materialPropertyBlock.SetFloat(ShaderProperties.Get("_Flow"), Mathf.Clamp(flow, -1f, 1f));
		materialPropertyBlock.SetColor(ShaderProperties.Get("_BloodColor"), color);
		lineRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	private void TransferLiquid(float rate, BloodContainer from, BloodContainer to, float visualDirection)
	{
		if (rate < 0f)
		{
			throw new Exception("Rate can't be negative");
		}
		if (from.TotalLiquidAmount > from.LowerLimit && to.TotalLiquidAmount < to.UpperLimit && Mathf.Abs(from.TransferTo(rate, to)) > float.Epsilon)
		{
			from.DeleteEmptyLiquidEntries();
			to.DeleteEmptyLiquidEntries();
			flow = Mathf.Lerp(flow, visualDirection, rate);
			color = Color.Lerp(color, from.GetComputedColor(), rate);
		}
	}

	internal static void AveragePressure(float rate, BloodContainer from, BloodContainer to)
	{
		if ((bool)from && (bool)to)
		{
			if (rate < 0f)
			{
				throw new Exception("Rate can't be negative");
			}
			float measuredPressure = from.MeasuredPressure;
			float measuredPressure2 = to.MeasuredPressure;
			float b = 0.5f * (measuredPressure + measuredPressure2);
			if (from.AllowPressureTransfer && to.AllowPressureTransfer)
			{
				from.MeasuredPressure = Mathf.Lerp(measuredPressure, b, rate);
				to.MeasuredPressure = Mathf.Lerp(measuredPressure2, b, rate);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (IsHeartRateWire && (bool)heartMonitor)
		{
			heartMonitor.IsConnected = false;
		}
		if (IsCardiopulmonaryBypassWire && (bool)cardiopulmonaryBypass)
		{
			(limbA ? limbA : limbB).ArtificialHeartbeat = 0f;
			cardiopulmonaryBypass.ValidConnectionCount--;
			cardiopulmonaryBypass.UpdateActivation();
		}
	}
}
