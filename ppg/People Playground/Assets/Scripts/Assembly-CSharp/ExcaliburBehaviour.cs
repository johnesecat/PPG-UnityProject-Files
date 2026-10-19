using UnityEngine;

public class ExcaliburBehaviour : MonoBehaviour, Messages.IUse, Messages.ILodged
{
	[SkipSerialisation]
	public GameObject FireBallPrefab;

	[SkipSerialisation]
	public Transform SpawnPoint;

	[SkipSerialisation]
	public float SpellForce;

	[Space]
	[SkipSerialisation]
	public Transform SpellChargeGlow;

	[SkipSerialisation]
	public ParticleSystem SpellChargeParticleSystem;

	[SkipSerialisation]
	public AnimationCurve SpellChargeCurve;

	[SkipSerialisation]
	public AudioSource ChargeAudioSource;

	private float chargeTime;

	private bool performingSpell;

	private PhysicalBehaviour phys;

	private void Awake()
	{
		phys = GetComponent<PhysicalBehaviour>();
	}

	public void Use(ActivationPropagation prop)
	{
		if (!performingSpell)
		{
			performingSpell = true;
			chargeTime = 0f;
			SpellChargeParticleSystem.Emit(12);
			ChargeAudioSource.Play();
		}
	}

	private void CastSpell()
	{
		GameObject gameObject = Object.Instantiate(FireBallPrefab, SpawnPoint.position, Quaternion.identity);
		gameObject.GetComponent<Rigidbody2D>().AddForce(base.transform.up * SpellForce, ForceMode2D.Impulse);
		Collider2D[] componentsInChildren = base.transform.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D b in componentsInChildren)
		{
			IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(gameObject.GetComponent<Collider2D>(), b);
		}
		performingSpell = false;
		SpellChargeGlow.localScale = Vector2.zero;
	}

	public void Update()
	{
		if (performingSpell)
		{
			chargeTime += Time.deltaTime * 2f;
			SpellChargeGlow.localScale = 2f * SpellChargeCurve.Evaluate(chargeTime) * Vector2.one;
			if (chargeTime >= 1f)
			{
				CastSpell();
			}
		}
	}

	public void Lodged(Stabbing stabbing)
	{
		if ((bool)stabbing.victim && !stabbing.victim.transform.root.CompareTag("World"))
		{
			if (stabbing.victim.TryGetComponent<DestroyableBehaviour>(out var component))
			{
				component.Break();
			}
			if (stabbing.victim.TryGetComponent<DamagableMachineryBehaviour>(out var component2))
			{
				component2.ForceBreak();
			}
			if (stabbing.victim.TryGetComponent<LimbBehaviour>(out var component3) && component3.IsAndroid)
			{
				component3.SkinMaterialHandler.AcidProgress = Mathf.Max(component3.SkinMaterialHandler.AcidProgress, 0.6f);
				component3.Damage(component3.Health + 1f);
			}
			else
			{
				stabbing.victim.transform.root.BroadcastMessage("OnEMPHit", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
