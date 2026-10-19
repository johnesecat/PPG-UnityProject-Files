using System;
using UnityEngine;

[Obsolete]
public class ForcefieldGeneratorBehaviour : MonoBehaviour, Messages.IUse
{
	public bool Activated;

	public Transform shield;

	private Rigidbody2D rigidbody;

	private Vector4[] hits = new Vector4[32];

	private Material mat;

	private void Awake()
	{
		mat = GetComponentInChildren<MeshRenderer>().material;
		rigidbody = GetComponent<Rigidbody2D>();
		shield = base.transform.GetChild(0);
		for (int i = 0; i < 32; i++)
		{
			hits[i] = new Vector4(0f, 0f, 100f, 0f);
		}
	}

	public void Use(ActivationPropagation activation)
	{
		Activated = !Activated;
		shield.gameObject.SetActive(Activated);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Rigidbody2D attachedRigidbody = collision.attachedRigidbody;
		if ((bool)attachedRigidbody)
		{
			float magnitude = (rigidbody.velocity - attachedRigidbody.velocity).magnitude;
			Vector2 vector = collision.ClosestPoint(base.transform.position);
			CreateHit(vector, magnitude);
			Vector3 vector2 = (collision.transform.position - base.transform.position).normalized * magnitude * attachedRigidbody.mass * 2f;
			attachedRigidbody.AddForceAtPosition(vector2, vector, ForceMode2D.Impulse);
		}
	}

	private void CreateHit(Vector2 worldPos, float intensity)
	{
		int oldestIndex = GetOldestIndex();
		hits[oldestIndex] = shield.InverseTransformPoint(worldPos);
		hits[oldestIndex].z = 1f / intensity;
	}

	private int GetOldestIndex()
	{
		float num = 0f;
		int result = -1;
		for (int i = 0; i < 32; i++)
		{
			if (num < hits[i].z)
			{
				num = hits[i].z;
				result = i;
			}
		}
		return result;
	}

	private void Update()
	{
		for (int i = 0; i < 32; i++)
		{
			hits[i].z += Time.deltaTime;
		}
		mat.SetVectorArray("_HitPos", hits);
	}
}
