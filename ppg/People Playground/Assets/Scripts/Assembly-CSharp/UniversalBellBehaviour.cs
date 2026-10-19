using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UniversalBellBehaviour : MonoBehaviour, Messages.IUse
{
	public int BellClipIndex;

	[SkipSerialisation]
	public AudioSource ClipSource;

	[SkipSerialisation]
	public Gradient ColourGradient;

	[SkipSerialisation]
	public TextMeshPro DetailViewLabel;

	[SkipSerialisation]
	public float ClapperVelocityInfluence = 100f;

	[SkipSerialisation]
	public float ClapperIntensity;

	private SpriteRenderer spriteRenderer;

	[SerializeField]
	[SkipSerialisation]
	private SpriteRenderer clapperSpriteRenderer;

	private PhysicalBehaviour phys;

	private Transform clapperTransform;

	private float clapperRot;

	private float clapperRotVel;

	private Vector2 previousVelocity;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		phys = GetComponent<PhysicalBehaviour>();
		clapperTransform = clapperSpriteRenderer.transform;
		DetailViewLabel.transform.eulerAngles = default(Vector3);
		phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton("bellSetNote", "Set bell note", "Set bell note", delegate
		{
			if (SelectionController.Main.SelectedObjects.Count > 1)
			{
				UniversalBellBehaviour component3;
				IEnumerable<PhysicalBehaviour> source = SelectionController.Main.SelectedObjects.Where((PhysicalBehaviour t) => t.TryGetComponent<UniversalBellBehaviour>(out component3));
				if (!source.Any() || source.First().gameObject != base.gameObject)
				{
					return;
				}
			}
			UniversalBellBehaviour component2;
			PhysicalBehaviour[] copy = SelectionController.Main.SelectedObjects.Where((PhysicalBehaviour t) => t.TryGetComponent<UniversalBellBehaviour>(out component2)).ToArray();
			PianoDialogBox.Show(GetBellClip(), delegate(BellClip c)
			{
				int num = Global.main.BellClips.BellClips.FindIndex((BellClip d) => d.Name == c.Name);
				if (num == -1)
				{
					UISoundBehaviour.Main.Error();
					DialogBoxManager.Notification("Could not find the selected piano key...");
				}
				else
				{
					for (int i = 0; i < copy.Length; i++)
					{
						if (copy[i].TryGetComponent<UniversalBellBehaviour>(out var component))
						{
							component.SetBellIndex(num);
						}
					}
				}
			});
		}));
		phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => BellClipIndex < Global.main.BellClips.BellClips.Count - 1, "bellIncrementNote", () => "Set to " + Global.main.BellClips.BellClips[BellClipIndex + 1].GetDisplayName(shorten: true), "Increment bell note", delegate
		{
			SetBellIndex(BellClipIndex + 1);
			Play();
		}));
		phys.ContextMenuOptions.Buttons.Add(new ContextMenuButton(() => BellClipIndex > 0, "bellDecrementNote", () => "Set to " + Global.main.BellClips.BellClips[BellClipIndex - 1].GetDisplayName(shorten: true), "Decrement bell note", delegate
		{
			SetBellIndex(BellClipIndex - 1);
			Play();
		}));
		SetBellIndex(BellClipIndex);
	}

	public void SetBellIndex(int i)
	{
		BellClipIndex = i;
		BellClip bellClip = GetBellClip();
		Color color3 = (clapperSpriteRenderer.color = (spriteRenderer.color = ColourGradient.Evaluate((float)i / (float)Global.main.BellClips.BellClips.Count)));
		if (phys.OverrideImpactSounds == null || phys.OverrideImpactSounds.Length != 1)
		{
			phys.OverrideImpactSounds = new AudioClip[1] { bellClip.Clip };
		}
		else
		{
			phys.OverrideImpactSounds[0] = bellClip.Clip;
		}
		phys.OverrideShotSounds = phys.OverrideImpactSounds;
		DetailViewLabel.text = bellClip.GetDisplayName(shorten: true);
	}

	private void FixedUpdate()
	{
		clapperRotVel -= base.transform.InverseTransformVector(phys.rigidbody.velocity - previousVelocity).x * ClapperVelocityInfluence;
		previousVelocity = phys.rigidbody.velocity;
	}

	private void Update()
	{
		DetailViewLabel.transform.eulerAngles = default(Vector3);
		clapperRotVel = Mathf.Lerp(clapperRotVel, 0f, Utils.GetLerpFactorDeltaTime(0.9f, Time.deltaTime));
		if (clapperRot < -24f)
		{
			clapperRot = -24f;
			doBounce();
		}
		else if (clapperRot > 24f)
		{
			clapperRot = 24f;
			doBounce();
		}
		float num = Physics2D.gravity.magnitude * phys.rigidbody.gravityScale;
		if (num > float.Epsilon)
		{
			Vector2 vector = Physics2D.gravity / num;
			Vector3 vector2 = base.transform.InverseTransformDirection(vector);
			float num2 = Mathf.Atan2(vector2.x, 0f - vector2.y) * 57.29578f;
			clapperRotVel += num / 9.81f * Time.timeScale * Mathf.Clamp(num2 - clapperRot, -90f, 90f) * Mathf.Max(Vector2.Dot(vector2, Vector2.down), 0.05f);
		}
		clapperRot += clapperRotVel * Time.deltaTime;
		clapperTransform.localEulerAngles = new Vector3(0f, 0f, clapperRot);
		void doBounce()
		{
			clapperRotVel = Mathf.Lerp(clapperRotVel, phys.rigidbody.angularVelocity, 0.2f);
			clapperRotVel *= -0.25f;
			if (phys.rigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				phys.rigidbody.angularVelocity *= 0.9f;
			}
		}
	}

	public BellClip GetBellClip()
	{
		return Global.main.BellClips.BellClips[BellClipIndex];
	}

	public void Use(ActivationPropagation activation)
	{
		Play();
	}

	public void Play()
	{
		ClipSource.PlayOneShot(GetBellClip().Clip);
		clapperRotVel += ClapperIntensity;
		phys.rigidbody.AddTorque(ClapperIntensity * 0.005f);
	}
}
