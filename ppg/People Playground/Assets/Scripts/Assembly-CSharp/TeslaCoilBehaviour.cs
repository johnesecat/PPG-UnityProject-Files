using System.Collections.Generic;
using UnityEngine;

public class TeslaCoilBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject bolt;

	public uint BoltCount = 2u;

	public bool Activated;

	[SkipSerialisation]
	public GameObject Wh;

	[SkipSerialisation]
	public PhysicalBehaviour physicalBehaviour;

	private GameObject[] bolts;

	private HashSet<ImmobilityFieldBehaviour> whCapturers = new HashSet<ImmobilityFieldBehaviour>();

	[SkipSerialisation]
	public AudioSource audioSource;

	[SkipSerialisation]
	public AudioClip[] sparkClips;

	private bool whReady;

	private GameObject instantiatedWhO;

	public bool unfit;

	private void Awake()
	{
		bolts = new GameObject[BoltCount];
		for (int i = 0; i < BoltCount; i++)
		{
			GameObject gameObject = Object.Instantiate(bolt, base.transform.position, Quaternion.identity, base.transform);
			gameObject.name += i;
			gameObject.transform.localPosition = new Vector3(0f, 1f, 0f);
			gameObject.GetComponent<TeslaCoilLightningBoltBehaviour>().parent = this;
			bolts[i] = gameObject;
		}
	}

	private void Start()
	{
		UpdateBolts();
	}

	private void OnAfterDeserialise()
	{
		unfit = true;
	}

	private void Use()
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateBolts();
		}
	}

	private void OnDisable()
	{
		Activated = false;
		audioSource.Stop();
		GameObject[] array = bolts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		if ((bool)instantiatedWhO)
		{
			Object.Destroy(instantiatedWhO);
		}
		whReady = false;
	}

	private void FixedUpdate()
	{
		whReady = CalculateWhReady();
		if (whReady)
		{
			AffectOpposingNode();
		}
		else if ((bool)instantiatedWhO)
		{
			Object.Destroy(instantiatedWhO);
		}
	}

	private bool CalculateWhReady()
	{
		if (unfit || !Activated || !base.enabled || whCapturers.Count < 2 || physicalBehaviour.Charge < 25f)
		{
			return false;
		}
		int num = 0;
		foreach (ImmobilityFieldBehaviour whCapturer in whCapturers)
		{
			if ((bool)whCapturer && whCapturer.Activated && whCapturer.PhysicalBehaviour.Charge > 25f)
			{
				num++;
			}
			if (num >= 2)
			{
				return true;
			}
		}
		return false;
	}

	private void AffectOpposingNode()
	{
		if (unfit)
		{
			return;
		}
		Collider2D collider2D = Physics2D.OverlapPoint(base.transform.TransformPoint(new Vector2(0f, 6f)));
		if ((bool)collider2D && collider2D.TryGetComponent<TeslaCoilBehaviour>(out var component) && component.whReady)
		{
			if (!component.instantiatedWhO && !instantiatedWhO)
			{
				instantiatedWhO = Object.Instantiate(Wh, (base.transform.position + component.transform.position) / 2f, Quaternion.identity);
				return;
			}
			if ((bool)instantiatedWhO)
			{
				instantiatedWhO.transform.position = (base.transform.position + component.transform.position) / 2f;
			}
			physicalBehaviour.rigidbody.AddTorque(Random.Range(-1f, 1f) * 500f);
			physicalBehaviour.rigidbody.AddForce((base.transform.position - component.transform.position).normalized * 320f);
		}
		else if ((bool)instantiatedWhO)
		{
			Object.Destroy(instantiatedWhO);
		}
	}

	private void OnImmobilityCapture(ImmobilityFieldBehaviour sender)
	{
		whCapturers.Add(sender);
	}

	private void OnImmobilityRelease(ImmobilityFieldBehaviour sender)
	{
		whCapturers.Remove(sender);
	}

	public void PlaySparkSound()
	{
		audioSource.PlayOneShot(sparkClips.PickRandom());
	}

	private void UpdateBolts()
	{
		if (Activated)
		{
			audioSource.Play();
			GameObject[] array = bolts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
		else
		{
			audioSource.Stop();
			GameObject[] array = bolts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}

	private void OnDestroy()
	{
		if ((bool)instantiatedWhO)
		{
			Object.Destroy(instantiatedWhO);
		}
	}
}
