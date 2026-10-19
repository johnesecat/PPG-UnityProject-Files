using System;
using UnityEngine;

public class ParticleMachineBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public bool EmitSmoke;

	public bool EmitFire;

	public bool EmitElectricity;

	[SkipSerialisation]
	public ParticleSystem SmokeParticles;

	[SkipSerialisation]
	public ParticleSystem FireParticles;

	[SkipSerialisation]
	public ParticleSystem ElectricityParticles;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		addToggleButton("smoke", () => EmitSmoke, delegate(bool v)
		{
			EmitSmoke = v;
		});
		addToggleButton("fire", () => EmitFire, delegate(bool v)
		{
			EmitFire = v;
		});
		addToggleButton("electricity", () => EmitElectricity, delegate(bool v)
		{
			EmitElectricity = v;
		});
		UpdateActivation();
		void addToggleButton(string label, Func<bool> getValue, Action<bool> setValue)
		{
			phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("pm" + label, () => (getValue() ? "Disable " : "Enable ") + label, "Toggle " + label, delegate
			{
				setValue(!getValue());
				UpdateActivation();
			}));
		}
	}

	public void UpdateActivation()
	{
		setPlaying(SmokeParticles, EmitSmoke);
		setPlaying(FireParticles, EmitFire);
		setPlaying(ElectricityParticles, EmitElectricity);
		void setPlaying(ParticleSystem ps, bool play)
		{
			if (base.enabled && Activated && play)
			{
				ps.Play();
			}
			else
			{
				ps.Stop();
			}
		}
	}

	public void Use(ActivationPropagation activation)
	{
		Activated = !Activated;
		UpdateActivation();
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	private void OnDisable()
	{
		UpdateActivation();
	}
}
