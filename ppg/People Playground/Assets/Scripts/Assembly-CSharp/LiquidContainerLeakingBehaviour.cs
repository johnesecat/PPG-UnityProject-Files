using System;
using System.Collections.Generic;
using UnityEngine;

public class LiquidContainerLeakingBehaviour : MonoBehaviour, Messages.IShot, Messages.IExitShot, Messages.IUnstabbed, Messages.IRepair, Messages.IOnBeforeSerialise
{
	[Serializable]
	public struct Hole
	{
		public Vector2 LocalPos;

		public Vector2 LocalDir;

		public float Intensity;
	}

	public float DrainIntensityMultiplier = 0.1f;

	public float VisualPressureMultiplier = 1f;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public GameObject HolePrefab;

	[SkipSerialisation]
	public BloodContainer Source;

	private readonly List<Hole> holes = new List<Hole>();

	public Hole[] SerialisableHoles;

	public bool HasHoles;

	private void Start()
	{
		if (!PhysicalBehaviour)
		{
			PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
		}
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => holes.Count > 0, "repairLiquidHoles", "Repair holes", "Repairs holes in liquid container", delegate
		{
			Repair();
		}));
		if (HasHoles && SerialisableHoles != null)
		{
			Hole[] serialisableHoles = SerialisableHoles;
			for (int i = 0; i < serialisableHoles.Length; i++)
			{
				Hole hole = serialisableHoles[i];
				CreateHole(hole.LocalPos, hole.LocalDir, hole.Intensity);
			}
		}
		SerialisableHoles = null;
	}

	public void Shot(Shot shot)
	{
		Vector3 vector = base.transform.InverseTransformPoint(PhysicalBehaviour.rigidbody.ClosestPoint(shot.point + shot.normal * 0.1f));
		Vector3 vector2 = base.transform.InverseTransformDirection(shot.normal);
		CreateHole(vector, vector2, Mathf.Clamp(shot.damage, 0.2f, 5f));
	}

	public void ExitShot(Shot shot)
	{
		Shot(shot);
	}

	public void Unstabbed(Stabbing stabbing)
	{
		if (stabbing.stabber.StabCausesWound)
		{
			Vector3 vector = base.transform.InverseTransformPoint(PhysicalBehaviour.rigidbody.ClosestPoint(stabbing.point + stabbing.normal * 0.1f));
			Vector3 vector2 = base.transform.InverseTransformDirection(stabbing.normal);
			CreateHole(vector, vector2, 5f);
		}
	}

	private void CreateHole(Vector2 localPos, Vector2 localDir, float intensity)
	{
		if (intensity < 0.1f)
		{
			Debug.LogErrorFormat("Attempt to create drain hole with intensity {0}. Intensity needs to be greater than or equal to 0.1", intensity);
			return;
		}
		GameObject obj = UnityEngine.Object.Instantiate(HolePrefab, base.transform);
		obj.transform.localPosition = localPos + localDir.normalized * 0.03f;
		obj.transform.localScale = Vector3.one;
		obj.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(localDir.y, localDir.x) * 57.29578f + 90f);
		LiquidContainerHoleBehaviour component = obj.GetComponent<LiquidContainerHoleBehaviour>();
		component.Source = Source;
		component.ForceMultiplier *= VisualPressureMultiplier;
		component.DrainRate = intensity * 0.05f * DrainIntensityMultiplier;
		HasHoles = true;
		holes.Add(new Hole
		{
			LocalDir = localDir,
			LocalPos = localPos,
			Intensity = intensity
		});
	}

	public void Repair()
	{
		holes.Clear();
		HasHoles = false;
		LiquidContainerHoleBehaviour[] componentsInChildren = GetComponentsInChildren<LiquidContainerHoleBehaviour>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	public void OnBeforeSerialise()
	{
		SerialisableHoles = holes.ToArray();
	}
}
