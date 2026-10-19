using UnityEngine;

public class EnergyWireBehaviour : DistanceJointWireBehaviour
{
	private (AntennaReceiverBehaviour j, PhysicalBehaviour phys) receiver;

	private (AntennaBehaviour a, PhysicalBehaviour phys) antenna;

	private bool hasAntenna;

	private bool hasEggs;

	private (HoverboxBehaviour a, LaserBehaviour b) eggs;

	protected override void Created()
	{
		base.Created();
		SetAntennaObjects();
		SetEggs();
	}

	private void SetEggs()
	{
		if ((bool)otherPhysicalBehaviour && (bool)physicalBehaviour)
		{
			if (physicalBehaviour.TryGetComponent<HoverboxBehaviour>(out eggs.a))
			{
				hasEggs = otherPhysicalBehaviour.TryGetComponent<LaserBehaviour>(out eggs.b);
			}
			else if (otherPhysicalBehaviour.TryGetComponent<HoverboxBehaviour>(out eggs.a))
			{
				hasEggs = physicalBehaviour.TryGetComponent<LaserBehaviour>(out eggs.b);
			}
			if (hasEggs)
			{
				eggs.b.ConnectedEggs++;
				eggs.b.ConnectEgg(eggs.a);
			}
		}
	}

	protected override void Tick()
	{
		if ((bool)otherPhysicalBehaviour && (bool)physicalBehaviour)
		{
			TransferEnergy();
			if (hasAntenna)
			{
				TransferAntennaSignal();
			}
			SetWireColours();
		}
	}

	private void SetAntennaObjects()
	{
		if ((bool)otherPhysicalBehaviour && (bool)physicalBehaviour)
		{
			if (physicalBehaviour.TryGetComponent<AntennaBehaviour>(out antenna.a))
			{
				hasAntenna = otherPhysicalBehaviour.TryGetComponent<AntennaReceiverBehaviour>(out receiver.j);
			}
			else if (otherPhysicalBehaviour.TryGetComponent<AntennaBehaviour>(out antenna.a))
			{
				hasAntenna = physicalBehaviour.TryGetComponent<AntennaReceiverBehaviour>(out receiver.j);
			}
			if (hasAntenna)
			{
				receiver.phys = receiver.j.GetComponent<PhysicalBehaviour>();
				antenna.phys = antenna.a.GetComponent<PhysicalBehaviour>();
				receiver.j.ConnectedAntennae++;
				receiver.j.Connect();
			}
		}
	}

	private void TransferAntennaSignal()
	{
		if (!receiver.j || !antenna.a)
		{
			if ((bool)receiver.j)
			{
				receiver.j.ConnectedAntennae--;
			}
			antenna = default((AntennaBehaviour, PhysicalBehaviour));
			receiver = default((AntennaReceiverBehaviour, PhysicalBehaviour));
		}
		else
		{
			float signalStrength = antenna.a.GetSignalStrength();
			receiver.phys.Charge += signalStrength;
			receiver.j.SignalStrength = Mathf.Lerp(receiver.j.SignalStrength, signalStrength, Time.fixedDeltaTime * 10f);
		}
	}

	private void TransferEnergy()
	{
		Utils.TransferEnergyFixedRate(physicalBehaviour, otherPhysicalBehaviour);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)receiver.j)
		{
			receiver.j.ConnectedAntennae--;
		}
		if ((bool)eggs.b)
		{
			eggs.b.ConnectedEggs--;
		}
	}

	protected virtual void SetWireColours()
	{
		if (physicalBehaviour.GetChargeWithWireResistance() > 0.001f)
		{
			lineRenderer.startColor = Color.LerpUnclamped(WireColor, Color.cyan, Random.value * physicalBehaviour.GetChargeWithWireResistance());
			lineRenderer.endColor = Color.LerpUnclamped(WireColor, Color.cyan, Random.value * physicalBehaviour.GetChargeWithWireResistance());
		}
		else
		{
			lineRenderer.startColor = WireColor;
			lineRenderer.endColor = WireColor;
		}
	}
}
