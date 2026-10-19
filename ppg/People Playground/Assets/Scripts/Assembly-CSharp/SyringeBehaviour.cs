using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SyringeBehaviour : BloodContainer, Messages.IShot, Messages.IExitShot, Messages.IOnFragmentHit, Messages.IBreak, Messages.IUse, Messages.ILodged, Messages.IDislodged
{
	public bool Finite;

	public bool CanToggleInfinite = true;

	public PressureDirection PressureMode;

	public bool NewlySpawned = true;

	[SkipSerialisation]
	public float TransferRate = 0.01f;

	private SpriteRenderer spriteRenderer;

	[SkipSerialisation]
	public HashSet<BloodContainer> pushTargets = new HashSet<BloodContainer>();

	private Liquid spawnLiquid;

	private MaterialPropertyBlock materialProperty;

	public override Vector2 Limits => new Vector2(0f, 1.4f);

	public override PressureDirection Pressure => PressureMode;

	public override bool AllowsOverflow => false;

	protected virtual void Awake()
	{
		materialProperty = new MaterialPropertyBlock();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.GetPropertyBlock(materialProperty);
	}

	protected virtual void Start()
	{
		AddButtons();
		if (GetLiquidID() != null)
		{
			spawnLiquid = Liquid.GetLiquid(GetLiquidID());
			if (NewlySpawned)
			{
				AddLiquid(spawnLiquid, Limits.y);
			}
		}
		foreach (PhysicalBehaviour.Penetration penetration in GetComponent<PhysicalBehaviour>().penetrations)
		{
			if (penetration.Victim.TryGetComponent<BloodContainer>(out var component))
			{
				pushTargets.Add(component);
			}
		}
		NewlySpawned = false;
	}

	protected void AddButtons()
	{
		List<ContextMenuButton> buttons = GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons;
		buttons.Add(new ContextMenuButton(() => Pressure != 0 && Finite, "setToPush", "Set to push", "Set syringe to push mode", delegate
		{
			PressureMode = PressureDirection.Push;
		}));
		buttons.Add(new ContextMenuButton(() => Pressure != PressureDirection.Pull && Finite, "setToPull", "Set to pull", "Set syringe to pull mode", delegate
		{
			PressureMode = PressureDirection.Pull;
		}));
		buttons.Add(new ContextMenuButton(() => Pressure != PressureDirection.None && Finite, "setToIdle", "Set to idle", "Set syringe to idle mode", delegate
		{
			PressureMode = PressureDirection.None;
		}));
		buttons.Add(new ContextMenuButton(() => CanToggleInfinite, "toggleInifniteSyringe", () => (!Finite) ? "Disable infinite source" : "Enable infinite source", "Toggle syringe infinite", delegate
		{
			Finite = !Finite;
			PressureMode = PressureDirection.Push;
		}));
	}

	public virtual void Use(ActivationPropagation a)
	{
		switch (a.Channel)
		{
		default:
		{
			PressureDirection pressureDirection = (PressureMode = ((PressureMode == PressureDirection.Push) ? PressureDirection.Pull : PressureDirection.Push));
			break;
		}
		case 1:
			PressureMode = PressureDirection.Push;
			break;
		case 2:
			if (Finite)
			{
				PressureMode = PressureDirection.Pull;
			}
			break;
		}
	}

	public virtual void Lodged(Stabbing stabbing)
	{
		if ((bool)stabbing.victim && stabbing.victim.TryGetComponent<BloodContainer>(out var component))
		{
			pushTargets.Add(component);
		}
	}

	public virtual void Dislodged(PhysicalBehaviour.Penetration penetration)
	{
		if (penetration != null && (bool)penetration.Victim && penetration.Victim.TryGetComponent<BloodContainer>(out var component))
		{
			pushTargets.Remove(component);
		}
	}

	protected virtual void FixedUpdate()
	{
		if (!Finite && GetLiquidID() != null)
		{
			AddLiquid(spawnLiquid, Limits.y - GetAmount(spawnLiquid));
		}
		foreach (BloodContainer pushTarget in pushTargets)
		{
			BloodContainer other = pushTarget;
			if ((bool)other)
			{
				BloodWireBehaviour.AveragePressure(Time.fixedDeltaTime, this, other);
				if (!Finite || MeasuredPressure > other.MeasuredPressure)
				{
					push();
				}
				else if (Finite)
				{
					pull();
				}
			}
			void pull()
			{
				if (!(other.TotalLiquidAmount <= other.LowerLimit) && !(base.TotalLiquidAmount >= base.UpperLimit))
				{
					other.TransferTo(TransferRate, this);
				}
			}
			void push()
			{
				if (!(base.TotalLiquidAmount <= base.LowerLimit) && !(other.TotalLiquidAmount >= other.UpperLimit))
				{
					if (GetLiquidID() != null)
					{
						CirculationBehaviour circulationBehaviour = other as CirculationBehaviour;
						if ((object)circulationBehaviour != null)
						{
							if (circulationBehaviour.GetAmount(spawnLiquid) < 0.5f)
							{
								TransferTo(TransferRate, other);
							}
						}
						else
						{
							TransferTo(TransferRate, other);
						}
					}
					else
					{
						TransferTo(TransferRate, other);
					}
				}
			}
		}
	}

	public abstract string GetLiquidID();

	public virtual void ExitShot(Shot shot)
	{
		BreakSyringe();
	}

	public virtual void Shot(Shot shot)
	{
		StartCoroutine(WaitAFrame());
		IEnumerator WaitAFrame()
		{
			yield return new WaitForSeconds(0.05f);
			BreakSyringe();
		}
	}

	public virtual void Break(Vector2 velocity)
	{
		BreakSyringe();
	}

	public virtual void OnFragmentHit(float force)
	{
		BreakSyringe();
	}

	protected virtual void BreakSyringe()
	{
		if (base.TotalLiquidAmount > float.Epsilon && LiquidDistribution.Count > 0)
		{
			SyringeExplosionBehaviour component = Object.Instantiate(Resources.Load<GameObject>("Prefabs/SyringeExplosion"), base.transform.position, Quaternion.identity).GetComponent<SyringeExplosionBehaviour>();
			component.Colour = GetComputedColor();
			component.Liquids = LiquidDistribution.Keys;
			component.Amount = base.TotalLiquidAmount / (float)LiquidDistribution.Count;
			Object.Destroy(base.gameObject);
		}
	}

	protected virtual void OnWillRenderObject()
	{
		materialProperty.SetColor(ShaderProperties.Get("_LiquidColour"), GetColor());
		switch (PressureMode)
		{
		case PressureDirection.Push:
			materialProperty.SetFloat(ShaderProperties.Get("_Direction"), 1f);
			break;
		case PressureDirection.Pull:
			materialProperty.SetFloat(ShaderProperties.Get("_Direction"), -1f);
			break;
		default:
			materialProperty.SetFloat(ShaderProperties.Get("_Direction"), 0f);
			break;
		}
		spriteRenderer.SetPropertyBlock(materialProperty);
	}

	private Color GetColor()
	{
		Color computedColor = GetComputedColor();
		computedColor.a = Mathf.Clamp01(ScaledLiquidAmount);
		return computedColor;
	}
}
