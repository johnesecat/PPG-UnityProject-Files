using UnityEngine;

public class PhysicsGunBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public AudioClip StartSound;

	[SkipSerialisation]
	public AudioClip EndSound;

	[SkipSerialisation]
	public SmoothLineControllerBehaviour LineRenderer;

	[SkipSerialisation]
	public Transform LineStart;

	[SkipSerialisation]
	public Transform LineTarget;

	[SkipSerialisation]
	public LayerMask LayersToHit;

	[SkipSerialisation]
	public LayerMask LayersToGrab;

	[SkipSerialisation]
	public float MaxRange;

	private PhysicalBehaviour phys;

	[HideInInspector]
	public PhysicalBehaviour Target;

	private Joint2D joint;

	private void Awake()
	{
		LineRenderer.Initialise();
		LineRenderer.gameObject.SetActive(value: false);
		phys = GetComponent<PhysicalBehaviour>();
	}

	private void Start()
	{
		SetTarget(Target);
	}

	private void StopRay()
	{
		SetTarget(null);
		LineRenderer.gameObject.SetActive(value: false);
		AudioSource.Stop();
		AudioSource.PlayOneShot(EndSound);
	}

	private void Update()
	{
		if (phys.StartedBeingUsedContinuously())
		{
			if ((bool)Target)
			{
				LineTarget.position = Target.transform.position;
			}
			LineRenderer.UpdateLine();
			LineRenderer.gameObject.SetActive(value: true);
			AudioSource.Play();
			AudioSource.PlayOneShot(StartSound);
		}
		if (phys.IsBeingUsedContinuously() && !Target)
		{
			QueryTarget();
			LineRenderer.UpdateLine();
		}
		if (phys.StoppedBeingUsedContinuously())
		{
			StopRay();
		}
		if ((bool)Target)
		{
			LineTarget.position = Target.transform.position;
			if (Target.isDisintegrated)
			{
				SetTarget(null);
			}
		}
	}

	private void OnDisable()
	{
		SetTarget(null);
		LineRenderer.gameObject.SetActive(value: false);
		AudioSource.Stop();
	}

	private void QueryTarget()
	{
		Vector3 normalized = (base.transform.right * base.transform.localScale.x).normalized;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(LineStart.transform.position, normalized, MaxRange, LayersToHit);
		if (!raycastHit2D.transform)
		{
			LineTarget.position = LineStart.transform.position + normalized * MaxRange;
		}
		else if (LayersToGrab.HasLayer(raycastHit2D.transform.gameObject.layer))
		{
			if (Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(raycastHit2D.transform, out var value))
			{
				SetTarget(value);
			}
		}
		else
		{
			LineTarget.position = raycastHit2D.point;
		}
	}

	private void SetTarget(PhysicalBehaviour p)
	{
		Target = p;
		if (!p)
		{
			Object.Destroy(joint);
			return;
		}
		LineTarget.position = p.transform.position;
		RelativeJoint2D relativeJoint2D = base.gameObject.AddComponent<RelativeJoint2D>();
		relativeJoint2D.connectedBody = p.rigidbody;
		relativeJoint2D.maxForce = 1200f;
		relativeJoint2D.maxTorque = 300f;
		relativeJoint2D.enableCollision = true;
		joint = relativeJoint2D;
	}
}
