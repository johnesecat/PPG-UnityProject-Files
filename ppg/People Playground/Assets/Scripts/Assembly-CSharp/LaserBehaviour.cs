using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour, Messages.IOnAfterDeserialise, Messages.IUse
{
	public Vector2 barrelPosition;

	public Vector2 barrelDirection;

	public Color UserSetColour = new Color(0f, 1f, 0f);

	[SkipSerialisation]
	[GradientUsage(true)]
	public Gradient LaserColourGradient;

	[SkipSerialisation]
	public float Range = 5000f;

	[SkipSerialisation]
	public LayerMask LayersToHit;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public ParticleSystem SmokeParticles;

	[SkipSerialisation]
	public ParticleSystem ImpactParticles;

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	[HideInInspector]
	public bool Activated;

	[Space]
	[SkipSerialisation]
	public bool Attachment;

	[NonSerialized]
	internal int ConnectedEggs;

	private float timer;

	private float pseudoCharge;

	private Vector3[] vertices;

	[SkipSerialisation]
	private bool isFit = true;

	[SkipSerialisation]
	private Utils.LaserHit finalHit;

	private const float ExplosionPeriod = 0.12f;

	private static List<Utils.LaserHit> hits = new List<Utils.LaserHit>();

	private HoverboxBehaviour hv;

	private GameObject e;

	[SerializeField]
	private GameObject ep;

	public Vector2 BarrelPosition
	{
		get
		{
			if (!PhysicalBehaviour.rigidbody)
			{
				return base.transform.TransformPoint(barrelPosition * base.transform.lossyScale);
			}
			return PhysicalBehaviour.rigidbody.GetRelativePoint(barrelPosition * base.transform.lossyScale);
		}
	}

	public Vector2 BarrelDirection => (PhysicalBehaviour.rigidbody ? PhysicalBehaviour.rigidbody.GetRelativeVector(barrelDirection) : ((Vector2)base.transform.TransformDirection(barrelDirection))) * ((!IsFlipped) ? 1 : (-1));

	private bool IsFlipped => base.transform.localScale.x < 0f;

	private void Start()
	{
		PhysicalBehaviour.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => !ColorpickerDialogBehaviour.IsOpen, "setLaserColour", "Set laser colour", "Change the colour of this object", delegate
		{
			Utils.OpenColourInputDialog(UserSetColour, "Pick a colour", "Set laser colour", delegate(LaserBehaviour obj, Color c)
			{
				Color.RGBToHSV(c, out var H, out var S, out var V);
				c = Color.HSVToRGB(H, S, Mathf.Max(0.01f, V));
				obj.UserSetColour = c;
			});
		}));
		LineRenderer.useWorldSpace = true;
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled && !Attachment)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	public void UpdateActivation()
	{
		if (Activated)
		{
			UpdateLaser();
			ImpactParticles.Play();
			SmokeParticles.Play();
			LineRenderer.enabled = true;
		}
		else
		{
			ImpactParticles.Stop();
			SmokeParticles.Stop();
			LineRenderer.enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (Activated)
		{
			FireLaser();
		}
		if (!isFit)
		{
			return;
		}
		if (Activated && ConnectedEggs > 0 && (bool)hv && finalHit.hit.HasValue && finalHit.rayDepth == 3 && (bool)finalHit.physicalBehaviour && finalHit.physicalBehaviour.Properties.Softness > float.Epsilon && hv.PhysicalBehaviour.rigidbody.velocity.magnitude > 5f)
		{
			if (!e)
			{
				e = UnityEngine.Object.Instantiate(ep, finalHit.point, Quaternion.identity);
			}
			else
			{
				e.transform.position = finalHit.point;
			}
			pseudoCharge = 30f;
			finalHit.physicalBehaviour.rigidbody.AddForceAtPosition(hv.PhysicalBehaviour.rigidbody.velocity, finalHit.point);
		}
		else
		{
			pseudoCharge = 0f;
			if ((bool)e)
			{
				UnityEngine.Object.Destroy(e);
			}
		}
		if (ConnectedEggs > 0 && !hv)
		{
			if ((bool)e)
			{
				UnityEngine.Object.Destroy(e);
			}
			ConnectedEggs = 0;
		}
	}

	private void FireLaser()
	{
		if (UpdateLaser())
		{
			float num = Mathf.Max(PhysicalBehaviour.Charge, pseudoCharge);
			if (num > 2f)
			{
				finalHit.physicalBehaviour.Temperature += Time.deltaTime * num / finalHit.physicalBehaviour.GetHeatCapacity();
				finalHit.physicalBehaviour.Ignite(finalHit.point);
				ImpactParticles.transform.position = finalHit.point;
				SmokeParticles.transform.position = finalHit.point;
				if (num > 50f)
				{
					finalHit.physicalBehaviour.SendMessage("Slice", SendMessageOptions.DontRequireReceiver);
					finalHit.physicalBehaviour.SendMessage("Break", Vector2.zero, SendMessageOptions.DontRequireReceiver);
				}
				if (num > 150f)
				{
					finalHit.physicalBehaviour.BurnProgress = 1f;
					if (num > 700f)
					{
						timer += Time.deltaTime;
						if (timer > 0.12f)
						{
							ExplosionCreator.CreateExplosionWithWater(WaterBehaviour.IsPointUnderWater(finalHit.point), new ExplosionCreator.ExplosionParameters(6u, finalHit.point, 1f, 3f, createFx: true));
							timer = 0f;
						}
					}
				}
			}
			ParticleSystem.EmissionModule emission = SmokeParticles.emission;
			emission.rateOverTimeMultiplier = 10f * Mathf.Min(num, 5f);
			if (num > 0.1f)
			{
				finalHit.physicalBehaviour.BurnProgress += Time.deltaTime * num * 0.01f;
			}
			ParticleSystem.MainModule main = ImpactParticles.main;
			main.startSpeedMultiplier = Mathf.Min(0.5f + num / 50f, 3f);
			ParticleSystem.EmissionModule emission2 = ImpactParticles.emission;
			emission2.rateOverTimeMultiplier = Mathf.Min(num, 50f);
			finalHit.physicalBehaviour.SendMessage("OnLasered", new Messages.ILasered.LaserArgs(this, finalHit), SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			ParticleSystem.EmissionModule emission3 = ImpactParticles.emission;
			emission3.rateOverTimeMultiplier = 0f;
			ParticleSystem.EmissionModule emission4 = SmokeParticles.emission;
			emission4.rateOverTimeMultiplier = 0f;
		}
	}

	private bool UpdateLaser()
	{
		float num = PhysicalBehaviour.Charge / 1000f;
		Color userSetColour = UserSetColour;
		userSetColour.a = 1f;
		userSetColour *= 0.02f * Mathf.Clamp(PhysicalBehaviour.Charge, 1f, 50f);
		Color color = ((num > float.Epsilon) ? Color.Lerp(userSetColour, LaserColourGradient.Evaluate(num), num) : userSetColour);
		LineRenderer.startColor = color;
		LineRenderer.endColor = color;
		ParticleSystem.MainModule main = ImpactParticles.main;
		main.startColor = color;
		Vector2 vector = BarrelPosition;
		Vector2 vector2 = BarrelDirection;
		hits.Clear();
		Utils.GetLaserEndPoint(vector, vector2, ref hits, LayersToHit, Range);
		if (hits.Count != 0)
		{
			vertices = new Vector3[hits.Count + 1];
			vertices[0] = vector;
			LineRenderer.positionCount = hits.Count + 1;
			for (int i = 0; i < hits.Count; i++)
			{
				vertices[i + 1] = hits[i].point;
			}
			finalHit = hits.Last();
			LineRenderer.SetPositions(vertices);
			return finalHit.physicalBehaviour;
		}
		LineRenderer.positionCount = 2;
		vertices = new Vector3[2];
		vertices[0] = vector;
		vertices[1] = vector + vector2 * Range;
		LineRenderer.SetPositions(vertices);
		return false;
	}

	private void OnDisable()
	{
		Activated = false;
		ImpactParticles.Stop();
		SmokeParticles.Stop();
		LineRenderer.enabled = false;
		UnityEngine.Object.Destroy(e);
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(e);
	}

	internal void ConnectEgg(HoverboxBehaviour a)
	{
		hv = a;
	}

	public void OnAfterDeserialise(List<GameObject> gameObjects)
	{
		isFit = false;
	}
}
