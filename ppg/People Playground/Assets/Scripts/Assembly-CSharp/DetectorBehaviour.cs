using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectorBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public float RelativeVelocityThreshold;

	[SkipSerialisation]
	public LayerMask LayersToDetect;

	[SkipSerialisation]
	public bool HasToBeAlive;

	[SkipSerialisation]
	public bool HasToBeOnFire;

	[SkipSerialisation]
	public PhysicalProperties[] PropertyFilter;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public SpriteRenderer EnabledLight;

	[SkipSerialisation]
	public LineRenderer Line;

	[SkipSerialisation]
	public float BeamOffset = 0.05f;

	[HideInInspector]
	public bool TriggerOnExit = true;

	public bool ShowBeam = true;

	public float Range = 5f;

	public bool Activated;

	public const float MinRange = 1.2136364f;

	public const float MaxRange = 24.272728f;

	[SkipSerialisation]
	private static List<Utils.LaserHit> hits = new List<Utils.LaserHit>();

	private Vector3[] positions;

	private bool isDetecting;

	private SpriteRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	private bool IsFlipped => base.transform.localScale.x < 0f;

	private void Awake()
	{
		Activated = true;
		renderer = GetComponent<SpriteRenderer>();
		propertyBlock = new MaterialPropertyBlock();
		renderer.GetPropertyBlock(propertyBlock);
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void OnContinuousUpdate(float arg0)
	{
		if (isDetecting && base.enabled)
		{
			for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
			{
				ushort channel = ActivationPropagation.AllChannels[i];
				SendMessage("IsolatedContinuousActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void Start()
	{
		UpdateActivated();
		Line.useWorldSpace = true;
		Line.SetPositions(new Vector3[2]
		{
			Vector3.right * BeamOffset,
			Vector3.right * Range
		});
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		buttons.Add(new ContextMenuButton("toggleTriggerBehaviour", () => (!TriggerOnExit) ? "Double trigger" : "Single trigger", "Toggle trigger behaviour", delegate
		{
			TriggerOnExit = !TriggerOnExit;
		}));
		buttons.Add(new ContextMenuButton("setDetectorRange", "Set range", "Set detector range", delegate
		{
			Utils.OpenFloatInputDialog(Range * (220f / 267f), this, delegate(DetectorBehaviour t, float v)
			{
				t.Range = Mathf.Clamp(v * 1.2136364f, 1.2136364f, 24.272728f);
			}, "Set detector range in meters", $"Target distance in meters from {1f} m to {20f} m");
		}));
		buttons.Add(new ContextMenuButton("toggleDetectorBeam", () => (!ShowBeam) ? "Show beam" : "Hide beam", "Show or hide the detector beam", delegate
		{
			ShowBeam = !ShowBeam;
			UpdateActivated();
		}));
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivated();
		}
	}

	private void OnDisable()
	{
		Activated = false;
		Line.enabled = false;
		propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 0f);
		renderer.SetPropertyBlock(propertyBlock);
		EnabledLight.enabled = false;
	}

	private void UpdateActivated()
	{
		FixedUpdate();
		Line.enabled = Activated && ShowBeam;
		EnabledLight.enabled = Activated;
	}

	private void Update()
	{
		if (!Global.main.Paused && !Global.main.GetPausedMenu())
		{
			if (isDetecting)
			{
				propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 1f);
			}
			else
			{
				propertyBlock.SetFloat(ShaderProperties.Get("_GlowIntensity"), 0f);
			}
		}
	}

	private void FixedUpdate()
	{
		if (!Activated)
		{
			isDetecting = false;
			return;
		}
		float maximumDistance = PhysicalBehaviour.Charge + Range;
		hits.Clear();
		Utils.GetLaserEndPoint(PhysicalBehaviour.rigidbody.position, PhysicalBehaviour.rigidbody.GetRelativeVector(Vector3.right) * ((!IsFlipped) ? 1 : (-1)), ref hits, LayersToDetect, maximumDistance);
		Line.positionCount = hits.Count + 1;
		positions = new Vector3[hits.Count + 1];
		positions[0] = PhysicalBehaviour.rigidbody.position;
		for (int i = 0; i < hits.Count; i++)
		{
			positions[i + 1] = hits[i].point;
		}
		Line.SetPositions(positions);
		bool flag = FitsCriteria(hits.LastOrDefault());
		if (!isDetecting || flag)
		{
			return;
		}
		if (TriggerOnExit)
		{
			for (int j = 0; j < ActivationPropagation.AllChannels.Length; j++)
			{
				ushort channel = ActivationPropagation.AllChannels[j];
				SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
		isDetecting = false;
	}

	private bool FitsCriteria(Utils.LaserHit laserHit)
	{
		if (!laserHit.physicalBehaviour)
		{
			return false;
		}
		if (!laserHit.hit.HasValue)
		{
			return false;
		}
		RaycastHit2D value = laserHit.hit.Value;
		Debug.DrawRay(laserHit.point, laserHit.point);
		if (HasToBeAlive)
		{
			if (!AliveBehaviour.AliveByTransform.TryGetValue(value.transform.root, out var value2))
			{
				return false;
			}
			if (!value2.IsAlive())
			{
				return false;
			}
		}
		if (RelativeVelocityThreshold > 0.01f && (PhysicalBehaviour.rigidbody.GetPointVelocity(laserHit.point) - laserHit.physicalBehaviour.rigidbody.GetPointVelocity(laserHit.point)).sqrMagnitude < RelativeVelocityThreshold)
		{
			return false;
		}
		if (HasToBeOnFire && !laserHit.physicalBehaviour.OnFire)
		{
			return false;
		}
		if (PropertyFilter.Length != 0 && !PassesFilter(laserHit.physicalBehaviour))
		{
			return false;
		}
		if (!isDetecting)
		{
			for (int i = 0; i < ActivationPropagation.AllChannels.Length; i++)
			{
				ushort channel = ActivationPropagation.AllChannels[i];
				SendMessage("IsolatedActivation", new ActivationPropagation(base.transform.root, channel), SendMessageOptions.DontRequireReceiver);
			}
		}
		isDetecting = true;
		return true;
	}

	private bool PassesFilter(PhysicalBehaviour phys)
	{
		for (int i = 0; i < PropertyFilter.Length; i++)
		{
			if (PropertyFilter[i] == phys.Properties)
			{
				return true;
			}
		}
		return false;
	}

	private void OnWillRenderObject()
	{
		renderer.SetPropertyBlock(propertyBlock);
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}
}
