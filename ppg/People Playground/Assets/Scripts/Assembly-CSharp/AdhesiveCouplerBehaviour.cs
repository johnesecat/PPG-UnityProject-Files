using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdhesiveCouplerBehaviour : MonoBehaviour, Messages.IUse, Messages.IOnAfterDeserialise, Messages.IOnBeforeSerialise
{
	public class Connection
	{
		public GameObject Other;

		public LineRenderer Line;

		public FixedJoint2D Joint;

		public Vector2 OtherLocalPoint;

		public float Time;

		public float TimeVariation = UnityEngine.Random.Range(0.95f, 2f);
	}

	public float StickDistance;

	[SkipSerialisation]
	public AudioClip OnStick;

	[SkipSerialisation]
	public AudioClip OnUnstick;

	[SkipSerialisation]
	public GameObject SlimeSplatPrefab;

	[SkipSerialisation]
	public GameObject GooLineRendererPrefab;

	[SkipSerialisation]
	public LayerMask LayerMask;

	[SkipSerialisation]
	public AnimationCurve WidthAnimationCurve;

	[SkipSerialisation]
	public float WidthAnimationDuration = 0.5f;

	[SkipSerialisation]
	public Color FakeLiquidColor;

	[SkipSerialisation]
	public LiquidContainerController LiquidContainer;

	[SkipSerialisation]
	public GameObject KillZone;

	private bool stuck;

	private PhysicalBehaviour phys;

	private List<Connection> connections = new List<Connection>();

	private float fakeLiquidAmount = 1f;

	private static Collider2D[] buffer = new Collider2D[32];

	[HideInInspector]
	public Guid[] SerialisableState;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		if ((bool)LiquidContainer)
		{
			LiquidContainer.Color = FakeLiquidColor;
		}
		SetStickDistance(StickDistance);
		List<ContextMenuButton> buttons = phys.ContextMenuOptions.Buttons;
		ContextMenuButton[] array = new ContextMenuButton[1];
		ContextMenuButton contextMenuButton = (array[0] = new ContextMenuButton("couplerStickDistance", "Set stick distance", "Set the distance the coupler can stick to objects", delegate
		{
			Utils.OpenFloatInputDialog(StickDistance, this, delegate(AdhesiveCouplerBehaviour w, float v)
			{
				w.SetStickDistance(v);
			}, "Set stick distance", "Distance");
		})
		{
			LabelWhenMultipleAreSelected = "Set stick distance"
		});
		buttons.AddRange(array);
	}

	public void SetStickDistance(float v)
	{
		StickDistance = v;
		Vector2 vector = 2f * v * Vector2.one;
		KillZone.transform.localScale = vector;
		KillZone.GetComponent<IgnoreParentSize>().DesiredSize = vector;
	}

	public void Use(ActivationPropagation propagation)
	{
		if (stuck)
		{
			Unstick();
		}
		else
		{
			Stick();
		}
	}

	public void Stick()
	{
		if (stuck)
		{
			return;
		}
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, StickDistance, buffer, LayerMask);
		if (num <= 1)
		{
			return;
		}
		phys.PlayClipOnce(OnStick, 0.75f);
		UnityEngine.Object.Instantiate(SlimeSplatPrefab, base.transform.position, Quaternion.identity);
		Transform root = base.transform.root;
		for (int i = 0; i < num; i++)
		{
			Collider2D collider2D = buffer[i];
			if (collider2D.transform.root != root)
			{
				StickTo(collider2D);
			}
		}
		stuck = true;
	}

	public void Unstick()
	{
		if (stuck)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				Connection connection = connections[i];
				UnityEngine.Object.Destroy(connection.Line);
				UnityEngine.Object.Destroy(connection.Joint);
			}
			connections.Clear();
			phys.PlayClipOnce(OnUnstick, 0.75f);
			stuck = false;
		}
	}

	private void StickTo(Collider2D collider)
	{
		if ((bool)collider.attachedRigidbody && !Physics2D.GetIgnoreCollision(collider, phys.colliders[0]))
		{
			Vector2 vector = collider.ClosestPoint(base.transform.position);
			UnityEngine.Object.Instantiate(SlimeSplatPrefab, vector, Quaternion.identity);
			vector = collider.transform.InverseTransformPoint(vector);
			FixedJoint2D fixedJoint2D = base.gameObject.AddComponent<FixedJoint2D>();
			fixedJoint2D.autoConfigureConnectedAnchor = true;
			fixedJoint2D.connectedBody = collider.attachedRigidbody;
			fixedJoint2D.connectedAnchor = vector;
			GameObject gameObject = UnityEngine.Object.Instantiate(GooLineRendererPrefab, collider.transform, worldPositionStays: false);
			Connection connection = new Connection
			{
				Other = collider.gameObject,
				Joint = fixedJoint2D,
				Line = gameObject.GetComponent<LineRenderer>(),
				OtherLocalPoint = vector
			};
			connection.Line.positionCount = 2;
			connection.Line.useWorldSpace = true;
			connections.Add(connection);
		}
	}

	public void OnAfterDeserialise(List<GameObject> gms)
	{
		IEnumerable<SerialisableIdentity> source = gms.SelectMany((GameObject c) => c.GetComponentsInChildren<SerialisableIdentity>());
		int i;
		for (i = 0; i < SerialisableState.Length; i++)
		{
			SerialisableIdentity serialisableIdentity = source.FirstOrDefault((SerialisableIdentity s) => s.UniqueIdentity == SerialisableState[i]);
			if ((bool)serialisableIdentity && serialisableIdentity.TryGetComponent<Collider2D>(out var component))
			{
				StickTo(component);
			}
		}
		stuck = connections.Any();
		SerialisableState = null;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < connections.Count; i++)
		{
			Connection connection = connections[i];
			if (!connection.Other || !connection.Line || !connection.Joint)
			{
				connections.RemoveAt(i);
				i--;
				phys.PlayClipOnce(OnUnstick, 0.75f);
				continue;
			}
			Vector3 vector = connection.Other.transform.TransformPoint(connection.OtherLocalPoint);
			Vector3 vector2 = position;
			vector2 += (vector - vector2).normalized * 0.2f;
			connection.Line.SetPosition(0, vector2);
			connection.Line.SetPosition(1, Vector3.Lerp(vector2, vector, Mathf.Clamp01(connection.Time / 0.1f)));
			connection.Line.widthMultiplier = WidthAnimationCurve.Evaluate(Mathf.Clamp01(connection.Time / WidthAnimationDuration));
			connection.Time += Time.deltaTime * connection.TimeVariation;
		}
		if (connections.Count == 0)
		{
			stuck = false;
		}
		if ((bool)LiquidContainer)
		{
			fakeLiquidAmount += Time.deltaTime * (float)((!stuck) ? 1 : (-1)) * 4f;
			fakeLiquidAmount = Mathf.Clamp01(fakeLiquidAmount);
			LiquidContainer.FillPercentage = 0.5f * fakeLiquidAmount;
		}
	}

	private void OnDestroy()
	{
		foreach (Connection connection in connections)
		{
			UnityEngine.Object.Destroy(connection.Line);
		}
	}

	public void OnBeforeSerialise()
	{
		SerialisableState = connections.Where((Connection c) => (bool)c.Other && (bool)c.Joint).Select(delegate(Connection v)
		{
			if (v.Other.TryGetComponent<SerialisableIdentity>(out var component))
			{
				return component.UniqueIdentity;
			}
			Debug.LogWarning("Adhesive coupler connection with invalid or non-existent ID");
			return default(Guid);
		}).ToArray();
	}
}
