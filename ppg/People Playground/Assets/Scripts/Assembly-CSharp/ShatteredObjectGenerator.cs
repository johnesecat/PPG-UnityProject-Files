using System;
using System.Buffers;
using NaughtyAttributes;
using UnityEngine;

public class ShatteredObjectGenerator : MonoBehaviour
{
	[SkipSerialisation]
	public string Name;

	[SkipSerialisation]
	public float PartMass = 0.05f;

	[SkipSerialisation]
	public Sprite Sprite;

	[SkipSerialisation]
	public int PartCount = 5;

	[SkipSerialisation]
	public float PartSizeRatio = 1f;

	[SkipSerialisation]
	[Layer]
	public int PartsLayer;

	[SkipSerialisation]
	public PhysicalProperties PartsPhysicalProperties;

	[SkipSerialisation]
	public float RandomForceIntensity = 0.1f;

	[SkipSerialisation]
	public float RandomTorqueIntensity = 0.1f;

	[SkipSerialisation]
	public float GenerateSoftJointsChance;

	public bool GenerateOnStart;

	public Rigidbody2D ConnectTo;

	public float ConnectToRange = 0.1f;

	public Vector2 SoftJointStrengthRange = new Vector2(25f, 50f);

	[NonSerialized]
	[SkipSerialisation]
	public float Brightness = 1f;

	public void Start()
	{
		if (GenerateOnStart)
		{
			Generate(base.transform);
		}
	}

	public int Generate(Transform toReplace, Rigidbody2D source = null, GameObject[] creationBuffer = null)
	{
		Material material = ProceduralShatterMaterialPool.Get(Sprite);
		float num = 1f / (float)PartCount;
		float y = PartSizeRatio * num;
		Rigidbody2D[] array = ArrayPool<Rigidbody2D>.Shared.Rent(PartCount);
		Texture2D texture = material.GetTexture(ShaderProperties.Get("_ShardIDMask")) as Texture2D;
		Vector2 vector = new Vector2(Sprite.rect.width / Sprite.pixelsPerUnit, Sprite.rect.height / Sprite.pixelsPerUnit);
		int num2 = 0;
		for (int i = 0; i < PartCount; i++)
		{
			GameObject gameObject = new GameObject(source ? source.name : base.name, typeof(SpriteRenderer), typeof(AudioSourceTimeScaleBehaviour), typeof(Optout));
			if (creationBuffer != null && i < creationBuffer.Length)
			{
				creationBuffer[i] = gameObject;
			}
			gameObject.layer = PartsLayer;
			Vector2 vector2 = new Vector2(num * (float)i, y);
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			component.color = new Color(vector2.x, vector2.y, Brightness, 1f);
			component.sprite = Sprite;
			component.material = material;
			gameObject.AddComponent<ShatteredShardBehaviour>();
			gameObject.AddComponent<DebrisComponent>();
			PolygonCollider2D polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
			Vector2[] polygon = ShatteredColliderShapeCache.GetPolygon(texture, (byte)(vector2.x * 255f), (byte)(vector2.y * 255f));
			Vector2[] array2 = new Vector2[polygon.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				Vector2 vector3 = polygon[j];
				array2[j] = new Vector2(vector3.x * vector.x, vector3.y * vector.y);
			}
			polygonCollider2D.SetPath(0, array2);
			Rigidbody2D rigidbody2D = (array[i] = gameObject.AddComponent<Rigidbody2D>());
			rigidbody2D.mass = PartMass;
			rigidbody2D.AddForce(UnityEngine.Random.insideUnitCircle * RandomForceIntensity, ForceMode2D.Impulse);
			rigidbody2D.AddTorque(RandomTorqueIntensity, ForceMode2D.Impulse);
			PhysicalBehaviour physicalBehaviour = gameObject.AddComponent<PhysicalBehaviour>();
			physicalBehaviour.Properties = PartsPhysicalProperties;
			physicalBehaviour.SpawnSpawnParticles = false;
			physicalBehaviour.HasOutline = false;
			physicalBehaviour.DisplayBloodDecals = false;
			physicalBehaviour.PlaySliderSound = false;
			gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
			physicalBehaviour.OverrideShotSounds = Array.Empty<AudioClip>();
			physicalBehaviour.OverrideImpactSounds = Array.Empty<AudioClip>();
			if ((bool)toReplace)
			{
				gameObject.transform.SetPositionAndRotation(toReplace.position, toReplace.rotation);
				gameObject.transform.localScale = toReplace.lossyScale;
			}
			if ((bool)source)
			{
				rigidbody2D.velocity += source.velocity;
				rigidbody2D.angularVelocity += source.angularVelocity;
			}
			num2++;
		}
		Vector2 b = default(Vector2);
		if ((bool)ConnectTo)
		{
			b = ((!Global.main.PhysicalObjectsInWorldByTransform.TryGetValue(ConnectTo.transform, out var value)) ? ((ConnectTo.position + (Vector2)toReplace.position) * 0.5f) : ((Vector2)value.spriteRenderer.bounds.ClosestPoint(toReplace.position)));
		}
		if (GenerateSoftJointsChance > float.Epsilon)
		{
			for (int k = 0; k < PartCount; k++)
			{
				Rigidbody2D rigidbody2D2 = array[k];
				for (int l = k + 1; l < PartCount; l++)
				{
					Rigidbody2D rigidbody2D3 = array[l];
					if (!(UnityEngine.Random.value > GenerateSoftJointsChance))
					{
						SpringJoint2D springJoint2D = rigidbody2D2.gameObject.AddComponent<SpringJoint2D>();
						Vector2 point = (rigidbody2D2.position + rigidbody2D3.position) * 0.5f;
						Vector2 vector4 = rigidbody2D2.ClosestPointFix(point);
						Vector2 vector5 = rigidbody2D3.ClosestPointFix(point);
						springJoint2D.connectedBody = rigidbody2D3;
						springJoint2D.frequency = 16f;
						springJoint2D.dampingRatio = 1f;
						springJoint2D.breakForce = Mathf.Lerp(SoftJointStrengthRange.x, SoftJointStrengthRange.y, UnityEngine.Random.value) * 0.2f;
						springJoint2D.anchor = (Vector2)rigidbody2D2.transform.InverseTransformPoint(vector4) + UnityEngine.Random.insideUnitCircle * 0.04f;
						springJoint2D.connectedAnchor = (Vector2)rigidbody2D3.transform.InverseTransformPoint(vector5) + UnityEngine.Random.insideUnitCircle * 0.04f;
						springJoint2D.distance *= 0.95f;
					}
				}
				if (UnityEngine.Random.value < GenerateSoftJointsChance && (bool)ConnectTo && (bool)source && Vector2.Distance(rigidbody2D2.worldCenterOfMass, b) < ConnectToRange)
				{
					FixedJoint2D fixedJoint2D = rigidbody2D2.gameObject.AddComponent<FixedJoint2D>();
					fixedJoint2D.connectedBody = ConnectTo;
					fixedJoint2D.frequency = 8f;
					fixedJoint2D.breakForce = Mathf.Lerp(SoftJointStrengthRange.x, SoftJointStrengthRange.y, UnityEngine.Random.value);
					fixedJoint2D.anchor = fixedJoint2D.attachedRigidbody.centerOfMass;
					fixedJoint2D.autoConfigureConnectedAnchor = true;
				}
			}
		}
		ArrayPool<Rigidbody2D>.Shared.Return(array, clearArray: true);
		return num2;
	}
}
