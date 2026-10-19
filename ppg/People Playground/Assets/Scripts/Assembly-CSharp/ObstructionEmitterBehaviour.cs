using System.Collections.Generic;
using UnityEngine;

public class ObstructionEmitterBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public float MaximumDistance = 10f;

	[SkipSerialisation]
	public LineRenderer LineRenderer;

	[SkipSerialisation]
	public EdgeCollider2D EdgeCollider;

	[SkipSerialisation]
	public AudioSource AudioSource;

	[SkipSerialisation]
	public LayerMask HitLayers;

	private readonly List<Vector2> vertices = new List<Vector2>
	{
		default(Vector2),
		default(Vector2)
	};

	private float lastDist = float.MaxValue;

	private void Start()
	{
		UpdateActivation();
	}

	public void Use(ActivationPropagation activation)
	{
		if (base.enabled)
		{
			Activated = !Activated;
			UpdateActivation();
		}
	}

	private void UpdateActivation()
	{
		if (Activated)
		{
			CalculateShape();
		}
		EdgeCollider.enabled = Activated;
		LineRenderer.enabled = Activated;
		if (Activated)
		{
			AudioSource.Play();
			AudioSource.time = AudioSource.clip.length * Random.value;
		}
		else
		{
			AudioSource.Stop();
		}
	}

	private void CalculateShape()
	{
		RaycastHit2D raycastHit2D = Physics2D.CircleCast(base.transform.position, 0.05f, base.transform.up, MaximumDistance, HitLayers);
		SetNewDistance(raycastHit2D.transform ? raycastHit2D.distance : MaximumDistance);
	}

	private void FixedUpdate()
	{
		if (Activated)
		{
			CalculateShape();
		}
	}

	private void OnEnable()
	{
		UpdateActivation();
	}

	private void OnDisable()
	{
		EdgeCollider.enabled = false;
		LineRenderer.enabled = false;
	}

	private void SetNewDistance(float distance)
	{
		if (!((double)Mathf.Abs(distance - lastDist) <= 0.01))
		{
			lastDist = distance;
			vertices[1] = new Vector2(0f, distance - 0.1f);
			LineRenderer.SetPosition(1, new Vector2(0f, distance));
			EdgeCollider.SetPoints(vertices);
		}
	}
}
