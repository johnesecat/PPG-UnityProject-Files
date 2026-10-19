using System;
using Linefy;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class GoreStringBehaviour : MonoBehaviour
{
	[Serializable]
	private class TissueString
	{
		public readonly float Pos1;

		public readonly float Pos2;

		public readonly float Strength;

		public float Thickness;

		public TissueString(float pos1, float pos2, float thickness, float strength)
		{
			Pos1 = pos1;
			Pos2 = pos2;
			Thickness = thickness;
			Strength = strength;
		}
	}

	[SkipSerialisation]
	public bool CreateJointOnStart;

	[SkipSerialisation]
	public Color TissueColour;

	[SkipSerialisation]
	public Rigidbody2D Other;

	[SkipSerialisation]
	public float LineWidthMultiplier = 1f;

	[SkipSerialisation]
	public float JointDampening = 0.5f;

	[SkipSerialisation]
	public float JointFreq = 4f;

	[SkipSerialisation]
	public float JointBreakThreshold = 4f;

	[SkipSerialisation]
	public BloodContainer ColourSource;

	[SkipSerialisation]
	public LineSegment OriginLine;

	[SkipSerialisation]
	public LineSegment OtherLine;

	[NonSerialized]
	[SkipSerialisation]
	public SpringJoint2D Joint;

	[SkipSerialisation]
	[Layer]
	public int LayerToDrawOn;

	[SkipSerialisation]
	public int RenderOrder = 3000;

	[SkipSerialisation]
	public float DepthOffset;

	[SkipSerialisation]
	public float ViewOffset;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public float InvincibilityTime = 0.8f;

	[ReadOnly]
	public bool HadJointWhenSerialised;

	[SkipSerialisation]
	public AudioClip[] TissueSnapClips;

	private Lines lines;

	[ReorderableList]
	private TissueString[] lineData;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		if (CreateJointOnStart || HadJointWhenSerialised)
		{
			CreateJoint();
		}
	}

	private void OnDestroy()
	{
		DestroyJoint();
	}

	private void Update()
	{
		if (lines == null || lines.disposed)
		{
			return;
		}
		if (!Other.gameObject.activeInHierarchy)
		{
			DestroyJoint();
			return;
		}
		bool flag = !float.IsInfinity(Joint.breakForce);
		Color colorB = (ColourSource ? ColourSource.GetComputedColor(TissueColour) : TissueColour);
		colorB.a = 1f;
		float num = Vector2.Distance(base.transform.TransformPoint(Joint.anchor), Other.transform.TransformPoint(Joint.connectedAnchor));
		float num2 = LineWidthMultiplier / Mathf.Max(num, 0.25f);
		for (int i = 0; i < lines.count; i++)
		{
			TissueString tissueString = lineData[i];
			if (flag && tissueString.Thickness > float.Epsilon && num / (tissueString.Strength * tissueString.Strength) > Joint.breakForce / 12f)
			{
				TryPlaySnapSound();
				tissueString.Thickness = 0f;
			}
			bool num3 = tissueString.Thickness > 0.01f;
			Line value = lines[i];
			if (num3)
			{
				value.positionA = base.transform.TransformPoint(OriginLine.GetPointAlong(tissueString.Pos1));
				value.positionB = Other.transform.TransformPoint(OtherLine.GetPointAlong(tissueString.Pos2));
				value.positionA.z = 0f;
				value.positionB.z = 0f;
				value.colorA = (value.colorB = colorB);
				value.widthA = (value.widthB = Mathf.Abs(num2 * tissueString.Thickness));
			}
			else
			{
				value.colorA = (value.colorB = Color.clear);
				value.widthA = (value.widthB = 0f);
			}
			lines[i] = value;
		}
		lines.Draw(LayerToDrawOn);
	}

	private void TryPlaySnapSound()
	{
		if (TissueSnapClips != null && TissueSnapClips.Length != 0)
		{
			if ((bool)AudioSource)
			{
				AudioSource.PlayOneShot(TissueSnapClips.PickRandom(), 0.5f);
			}
			else if ((bool)phys)
			{
				phys.PlayClipOnce(TissueSnapClips.PickRandom(), 0.5f);
			}
		}
	}

	public void CreateJoint()
	{
		if ((bool)Joint)
		{
			Debug.LogError("Tried to create gore string joint again after it has already been created");
			return;
		}
		if (!TryGetComponent<Rigidbody2D>(out var _) || !Other)
		{
			throw new Exception("Tried to create gore string joint without a rigidbody on either one or both ends");
		}
		HadJointWhenSerialised = true;
		Joint = base.gameObject.AddComponent<SpringJoint2D>();
		Joint.anchor = OriginLine.GetMidpoint();
		Joint.connectedAnchor = OtherLine.GetMidpoint();
		Joint.breakForce = float.PositiveInfinity;
		Joint.autoConfigureDistance = false;
		Joint.distance = 0.05f;
		Joint.frequency = JointFreq;
		Joint.dampingRatio = JointDampening;
		Joint.connectedBody = Other;
		StartCoroutine(Utils.DelayCoroutine(InvincibilityTime, SetBreakForce));
		int num = UnityEngine.Random.Range(5, 14);
		lineData = new TissueString[num];
		lines = new Lines(num);
		lines.transparent = true;
		lines.feather = 0f;
		lines.depthOffset = DepthOffset;
		lines.viewOffset = ViewOffset;
		lines.boundSize = -1f;
		lines.capacityChangeStep = 1;
		lines.widthMultiplier = 0.001f;
		lines.zTest = CompareFunction.Disabled;
		lines.widthMode = WidthMode.WorldspaceXY;
		lines.renderOrder = RenderOrder;
		lineData[0] = new TissueString(0.5f, 0.5f, 2f, float.PositiveInfinity);
		for (int i = 1; i < lineData.Length; i++)
		{
			lineData[i] = new TissueString(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.Range(0.5f, 2f), UnityEngine.Random.Range(0.1f, 1f));
		}
	}

	private void SetBreakForce()
	{
		if ((bool)Joint)
		{
			Joint.breakForce = JointBreakThreshold * 1.5f;
		}
	}

	public void DestroyJoint()
	{
		HadJointWhenSerialised = false;
		if (lines != null)
		{
			lines.Dispose();
			lines = null;
		}
		if ((bool)Joint)
		{
			UnityEngine.Object.Destroy(Joint);
		}
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (joint == Joint)
		{
			TryPlaySnapSound();
			DestroyJoint();
		}
	}
}
