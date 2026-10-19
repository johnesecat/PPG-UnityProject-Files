using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CarBehaviour : MonoBehaviour, Messages.IUse, Messages.IOnEMPHit
{
	public WheelJoint2D[] WheelJoints;

	public bool[] WheelJointState;

	public float MotorSpeed = 1200f;

	public float WheelJointStrengthMultiplier = 1f;

	[SkipSerialisation]
	public AnimationCurve AccelerationCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	private float time;

	public bool Activated;

	public bool WaterProof;

	[Space]
	public float EngineLoudness = 1f;

	[SkipSerialisation]
	public AudioClip EngineStartup;

	[SkipSerialisation]
	public AudioClip EngineShutoff;

	[SkipSerialisation]
	public AudioClip EngineLoop;

	private AudioSource engineAudioSource;

	private AudioSource engineAudioSourceSingle;

	[SkipSerialisation]
	public PhysicalBehaviour Phys;

	[HideInInspector]
	public bool IsBrakeEngaged = true;

	[HideInInspector]
	public float Health = 1f;

	public bool CollideWithChildren;

	[SkipSerialisation]
	public UnityEvent OnRepair;

	public Transform audioSourcePosition;

	private void Awake()
	{
		IsBrakeEngaged = true;
		if (audioSourcePosition == null)
		{
			audioSourcePosition = base.transform;
		}
		engineAudioSource = audioSourcePosition.gameObject.AddComponent<AudioSource>();
		engineAudioSource.clip = EngineLoop;
		engineAudioSource.minDistance = EngineLoudness;
		engineAudioSource.maxDistance = 100f;
		engineAudioSource.dopplerLevel = 0f;
		engineAudioSource.spatialBlend = 1f;
		engineAudioSource.loop = true;
		engineAudioSource.playOnAwake = false;
		engineAudioSourceSingle = audioSourcePosition.gameObject.AddComponent<AudioSource>();
		engineAudioSourceSingle.minDistance = EngineLoudness;
		engineAudioSourceSingle.maxDistance = 100f;
		engineAudioSourceSingle.dopplerLevel = 0f;
		engineAudioSourceSingle.spatialBlend = 1f;
		engineAudioSourceSingle.playOnAwake = false;
		WheelJoints = GetComponentsInChildren<WheelJoint2D>();
		WheelJointState = new bool[WheelJoints.Length];
		for (int i = 0; i < WheelJointState.Length; i++)
		{
			WheelJoints[i].breakForce = 15000f * WheelJointStrengthMultiplier;
			WheelJointState[i] = true;
		}
		if (!Phys)
		{
			Phys = GetComponent<PhysicalBehaviour>();
		}
	}

	private void Start()
	{
		Phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("reverseCar", "Reverse gear", "Reverse vehicle", delegate
		{
			ReverseGear();
		}));
		Phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("toggleBreak", () => (!IsBrakeEngaged) ? "Engage parking brakes" : "Disengage parking brakes", "Toggle parking brakes", delegate
		{
			IsBrakeEngaged = !IsBrakeEngaged;
		}));
		Phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("repairEngine", () => (!(Health > 0f)) ? "Repair engine" : "Break engine", "Repair or break the engine", delegate
		{
			Health = ((Health < 0f) ? 1 : (-1));
			if (Health < 0f && Activated)
			{
				Activated = false;
				UpdateActivated();
			}
			else
			{
				OnRepair?.Invoke();
			}
		}));
		if (!CollideWithChildren)
		{
			Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < WheelJointState.Length; i++)
			{
				if (!WheelJointState[i])
				{
					WheelJoints[i].breakForce = 0f;
				}
			}
			Collider2D[] array = componentsInChildren;
			foreach (Collider2D collider2D in array)
			{
				Collider2D[] array2 = componentsInChildren;
				foreach (Collider2D collider2D2 in array2)
				{
					if (!(collider2D.transform == collider2D2.transform))
					{
						IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(collider2D, collider2D2);
					}
				}
			}
		}
		UpdateActivated();
	}

	private void ReverseGear()
	{
		MotorSpeed = 0f - MotorSpeed;
	}

	private void OnJointBreak2D(Joint2D joint)
	{
		if (WheelJoints.Contains(joint))
		{
			int num = WheelJoints.ToList().IndexOf(joint as WheelJoint2D);
			WheelJointState[num] = false;
			joint.connectedBody.transform.SetParent(null);
		}
	}

	public void OnEMPHit()
	{
		Health = ((Health < 0f) ? 1 : (-1));
		if (Health < 0f && Activated)
		{
			Activated = false;
			UpdateActivated();
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		time += deltaTime;
		float num = 0f;
		if (Phys.Wetness > 0f && !WaterProof)
		{
			Health -= deltaTime * 0.5f;
			if (Health < 0f && Activated)
			{
				Activated = false;
				UpdateActivated();
			}
		}
		WheelJoint2D[] wheelJoints = WheelJoints;
		foreach (WheelJoint2D wheelJoint2D in wheelJoints)
		{
			if ((bool)wheelJoint2D)
			{
				wheelJoint2D.useMotor = IsBrakeEngaged || Activated;
				if (IsBrakeEngaged || Activated)
				{
					JointMotor2D motor = wheelJoint2D.motor;
					motor.motorSpeed = (Activated ? ((MotorSpeed + Phys.Charge * (float)((MotorSpeed < 0f) ? (-50) : 50)) * AccelerationCurve.Evaluate(time * 0.25f)) : 0f) * base.transform.lossyScale.x;
					num += motor.motorSpeed;
					wheelJoint2D.motor = motor;
				}
			}
		}
		engineAudioSource.volume = Mathf.Max(Mathf.Abs(num) / (float)WheelJoints.Length / Mathf.Abs(MotorSpeed), 0.2f);
	}

	public void Use(ActivationPropagation activation)
	{
		if (!(Health < 0f) && base.enabled)
		{
			if (activation.Channel == 1)
			{
				ReverseGear();
				return;
			}
			time = 0f;
			Activated = !Activated;
			UpdateActivated();
		}
	}

	private void OnDisable()
	{
		WheelJoint2D[] wheelJoints = WheelJoints;
		foreach (WheelJoint2D wheelJoint2D in wheelJoints)
		{
			if ((bool)wheelJoint2D)
			{
				wheelJoint2D.useMotor = false;
			}
		}
		engineAudioSource.Stop();
	}

	private void UpdateActivated()
	{
		if (Activated)
		{
			engineAudioSource.Play();
			engineAudioSourceSingle.PlayOneShot(EngineStartup);
		}
		else
		{
			engineAudioSource.Stop();
			engineAudioSourceSingle.PlayOneShot(EngineShutoff);
		}
	}

	public void SetHealth(float h)
	{
		Health = h;
		if (Health < 0f && Activated)
		{
			Activated = false;
			UpdateActivated();
		}
	}
}
