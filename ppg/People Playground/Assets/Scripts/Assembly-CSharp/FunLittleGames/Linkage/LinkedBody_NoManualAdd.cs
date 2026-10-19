using UnityEngine;

namespace FunLittleGames.Linkage
{
	[NotDocumented]
	public class LinkedBody_NoManualAdd
	{
		public class LinkedBody : MonoBehaviour
		{
			private static ContactPoint2D[] contactCache = new ContactPoint2D[20];

			public Joint2D[] joints;

			public Rigidbody2D rigid;

			public Vector2 summedForce;

			public float summedTorque;

			public bool lockX;

			public int mechanismX;

			public float leverageX;

			public bool lockY;

			public int mechanismY;

			public float leverageY;

			public bool lockR;

			public int mechanismR;

			public float leverageR;

			public float referenceAngle;

			public float trackedAngle;

			public Vector2 referencePos;

			public Vector2 mechX;

			public Vector2 mechY;

			public Vector3 forcesApplied;

			public Vector3 forcesRedistributed;

			public Linkage.Link.Axis linkedAxes;

			private void OnValidate()
			{
				rigid = GetComponent<Rigidbody2D>();
				joints = GetComponents<Joint2D>();
			}

			private void OnCollisionStay2D(Collision2D collision)
			{
				SumForcesAndTorques(collision);
			}

			private void OnCollisionEnter2D(Collision2D collision)
			{
				SumForcesAndTorques(collision);
			}

			private void FixedUpdate()
			{
			}

			private void SumForcesAndTorques(Collision2D collision)
			{
				int contacts = collision.GetContacts(contactCache);
				for (int i = 0; i < contacts; i++)
				{
					Vector2 normal = contactCache[i].normal;
					Vector2 vector = new Vector2(normal.y, 0f - normal.x);
					float num = contactCache[i].normalImpulse;
					if (float.IsNaN(num) || Mathf.Abs(num) > 1000f)
					{
						num = 0f;
					}
					float num2 = contactCache[i].tangentImpulse;
					if (float.IsNaN(num2) || Mathf.Abs(num2) > 1000f)
					{
						num2 = 0f;
					}
					Vector2 vector2 = normal * num + vector * num2;
					Vector2 lhs = contactCache[i].point - rigid.position;
					summedTorque += Vector2.Dot(lhs, new Vector2(vector2.y, 0f - vector2.x));
					summedForce += vector2;
				}
			}

			public void UpdateMechanismAxes()
			{
				Quaternion quaternion = Quaternion.Euler(0f, 0f, rigid.rotation);
				mechY = quaternion * Vector2.up;
				mechX.Set(mechY.y, 0f - mechY.x);
			}

			public void TransferConstraints(RigidbodyConstraints2D mask)
			{
				if ((mask & rigid.constraints & RigidbodyConstraints2D.FreezePositionX) != 0)
				{
					lockX = true;
				}
				if ((mask & rigid.constraints & RigidbodyConstraints2D.FreezePositionY) != 0)
				{
					lockY = true;
				}
				if ((mask & rigid.constraints & RigidbodyConstraints2D.FreezeRotation) != 0)
				{
					lockR = true;
				}
				rigid.constraints &= -1 - mask;
			}
		}
	}
}
