using UnityEngine;
using UnityEngine.Animations;

public class PinBehaviour : Hover, Messages.ISlice
{
	public float BreakingThreshold = float.PositiveInfinity;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	public PhysicalBehaviour Other;

	public Vector2 LocalAnchor;

	[SkipSerialisation]
	public Transform SpriteChild;

	[SkipSerialisation]
	public SpriteRenderer SpriteRenderer;

	[SkipSerialisation]
	public HingeJoint2D Joint;

	[SkipSerialisation]
	private ParentConstraint parentConstraint;

	public bool UsedToHaveConnectedBody;

	[HideInInspector]
	public string PinSpritePath = "Sprites/Pin";

	private float initialBreakForce = float.PositiveInfinity;

	public bool AttachedToWall => Other;

	private void Awake()
	{
		PhysicalBehaviour = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		CreateJoint();
		ModAPI.InvokePinCreated(this, this);
	}

	protected void CreateJoint()
	{
		Joint = base.gameObject.AddComponent<HingeJoint2D>();
		Joint.breakForce = Utils.CalculateBreakForceForCable(Joint, BreakingThreshold);
		initialBreakForce = Joint.breakForce;
		Joint.anchor = LocalAnchor;
		if (AttachedToWall)
		{
			Joint.connectedBody = Other.rigidbody;
		}
		GameObject gameObject = new GameObject("Pin");
		SpriteChild = gameObject.transform;
		SpriteChild.position = base.transform.TransformPoint(LocalAnchor);
		SpriteChild.localScale = new Vector3(1f, 1f, 1f);
		parentConstraint = gameObject.AddComponent<ParentConstraint>();
		parentConstraint.AddSource(new ConstraintSource
		{
			sourceTransform = base.transform,
			weight = 1f
		});
		SyncChildPos();
		parentConstraint.constraintActive = true;
		Utils.GetTopMostLayer(Other ? Other.GetComponent<SpriteRenderer>() : null, GetComponent<SpriteRenderer>(), out var layerId, out var sortingOrder);
		SpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		SpriteRenderer.sortingLayerID = layerId;
		SpriteRenderer.sortingOrder = sortingOrder + 1;
		SpriteRenderer.sprite = Resources.Load<Sprite>(PinSpritePath);
		gameObject.AddComponent<Optout>();
	}

	private void SyncChildPos()
	{
		parentConstraint.SetTranslationOffset(0, LocalAnchor * base.transform.lossyScale);
	}

	private void Update()
	{
		if (Global.main.GetPausedMenu())
		{
			return;
		}
		bool flag = (bool)Other && Other.isDisintegrated;
		if ((Joint.connectedBody == null && UsedToHaveConnectedBody) || flag)
		{
			Object.Destroy(this);
			return;
		}
		SyncChildPos();
		if ((bool)SpriteChild)
		{
			CheckMouseInput();
		}
	}

	private void FixedUpdate()
	{
		if (initialBreakForce != float.PositiveInfinity)
		{
			float num = 0f;
			if ((bool)PhysicalBehaviour)
			{
				num = PhysicalBehaviour.BurnProgress;
			}
			if ((bool)Other)
			{
				num = Mathf.Max(num, Other.BurnProgress);
			}
			Joint.breakForce = initialBreakForce * (1f - num);
		}
	}

	public override void OnMouseOverlapEvent(bool overlap)
	{
		base.OnMouseOverlapEvent(overlap);
		if (overlap && UserPreferenceManager.Current.ShowOutlines)
		{
			SpriteRenderer.color = Color.red;
			SpriteRenderer.gameObject.layer = LayerMask.NameToLayer("ScreenUI");
		}
		else
		{
			SpriteRenderer.color = Color.white;
			SpriteRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	private void OnJointBreak2D(Joint2D broken)
	{
		if (broken == Joint)
		{
			Object.Destroy(this);
		}
	}

	protected override void OnDestroy()
	{
		ModAPI.InvokePinDestroyed(this, this);
		base.OnDestroy();
		if ((bool)SpriteChild)
		{
			Object.Destroy(SpriteChild.gameObject);
		}
		Object.Destroy(Joint);
		Object.Destroy(this);
	}

	private void OnDisable()
	{
		if ((bool)SpriteChild)
		{
			SpriteChild.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		if ((bool)SpriteChild)
		{
			SpriteChild.gameObject.SetActive(value: true);
		}
	}

	public virtual void Slice()
	{
		Joint.breakForce = 0f;
		Joint.breakTorque = 0f;
	}

	protected override Bounds GetVisualBounds()
	{
		return SpriteRenderer.bounds;
	}

	public override void OnUserDelete()
	{
	}

	protected override bool IsMouseInsideCollider()
	{
		return Vector2.SqrMagnitude(SpriteRenderer.transform.position - Global.main.MousePosition) < 0.005102041f;
	}
}
