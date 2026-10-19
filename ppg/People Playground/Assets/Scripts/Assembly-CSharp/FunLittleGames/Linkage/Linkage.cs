using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FunLittleGames.Linkage
{
	[NotDocumented]
	public class Linkage : MonoBehaviour
	{
		[Serializable]
		public struct Limit
		{
			[Range(0f, 1f)]
			public float min;

			[Range(0f, 1f)]
			public float max;

			public Limit(float min, float max)
			{
				this.min = min;
				this.max = max;
			}
		}

		[Serializable]
		public class Link
		{
			public enum Axis
			{
				Local_X = 1,
				Local_Y = 2,
				Rotation = 4,
				World_X = 8,
				World_Y = 0x10
			}

			public Rigidbody2D rigidBody;

			public Axis axis;

			[Tooltip("In world units for X/Y, degrees for Rotation")]
			public float leverage;
		}

		[Serializable]
		public class Mechanism
		{
			[Tooltip("For your reference. Also accepted as a parameter for some Linkage functions.")]
			public string name;

			[Min(0f)]
			[Tooltip("Increase to make the mechanism stiffer and quicker to settle.")]
			public float drag;

			[Tooltip("Values below 1.0 will make the mechanism more stable under opposing loads.")]
			[Range(0f, 1f)]
			public float externalForceModifier = 1f;

			[Header("Mechanism limits")]
			public float minimumLimit;

			[Range(0f, 1f)]
			[Tooltip("Larger values can lead to instability if the mechanism is heavily loaded.")]
			public float minimumLimitStiffness = 0.5f;

			public float maximumLimit;

			[Range(0f, 1f)]
			[Tooltip("Larger values can lead to instability if the mechanism is heavily loaded.")]
			public float maximumLimitStiffness = 0.5f;

			[Header("Internal mechanism forces")]
			[Tooltip("A one-shot impulse. Resets to 0 automatically each update.")]
			public float nudge;

			[Tooltip("A continuous tensioning force pushing the mechanism.")]
			public float tension;

			[Tooltip("The target speed of the mechanism.")]
			public float drive;

			[Tooltip("The degree to which the drive is engaged.")]
			[Range(0f, 1f)]
			public float clutch;

			[Space(10f)]
			public List<Link> links;

			[HideInInspector]
			public int contributors;

			[HideInInspector]
			public float mass;

			[HideInInspector]
			public float force;

			[HideInInspector]
			public float speed;

			[HideInInspector]
			public float displacement;

			public void PreUpdate()
			{
				force = 0f;
				speed = 0f;
				displacement = 0f;
				mass = 0f;
				contributors = 0;
			}

			public void ScaleForceApplyNudgeAndTension()
			{
				force *= externalForceModifier;
				force += nudge + tension;
				nudge = 0f;
			}

			public void AverageSpeedDisplacementApplyDragLimitsAndDrive()
			{
				speed /= contributors;
				displacement /= contributors;
				if (minimumLimit != 0f || maximumLimit != 0f)
				{
					if (displacement < minimumLimit)
					{
						displacement = Mathf.Lerp(displacement, minimumLimit, minimumLimitStiffness);
						if (speed <= 0f)
						{
							speed = 0f;
						}
					}
					if (displacement > maximumLimit)
					{
						displacement = Mathf.Lerp(displacement, maximumLimit, maximumLimitStiffness);
						if (speed >= 0f)
						{
							speed = 0f;
						}
					}
				}
				speed *= 1f - drag * Time.fixedDeltaTime;
				speed = Mathf.Lerp(speed, drive, clutch);
			}
		}

		private const float twoPI = (float)Math.PI * 2f;

		public List<Mechanism> mechanisms = new List<Mechanism>();

		private List<LinkedBody_NoManualAdd.LinkedBody> bodies = new List<LinkedBody_NoManualAdd.LinkedBody>();

		private Dictionary<string, Mechanism> mechDict = new Dictionary<string, Mechanism>();

		private void OnValidate()
		{
			if (Application.isPlaying && bodies.Count > 0)
			{
				CreateLinkages();
			}
		}

		private void Awake()
		{
			foreach (Mechanism mechanism in mechanisms)
			{
				if (mechanism.name != "")
				{
					mechDict[mechanism.name] = mechanism;
				}
			}
			CreateLinkages();
		}

		private void OnEnable()
		{
			StartCoroutine(ProcessForces());
		}

		private void FixedUpdate()
		{
		}

		private IEnumerator ProcessForces()
		{
			while (true)
			{
				yield return new WaitForFixedUpdate();
				PrepareMechanisms();
				GatherMassesAndForces();
				RedistributeForces();
				GatherVelocitiesAndDisplacements();
				RedistributeVelocitiesAndDisplacements();
			}
		}

		public Mechanism GetMechanism(string name)
		{
			if (mechDict.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}

		private void CreateLinkages()
		{
			foreach (LinkedBody_NoManualAdd.LinkedBody body in bodies)
			{
				body.leverageX = 0f;
				body.leverageY = 0f;
				body.leverageR = 0f;
				body.linkedAxes = (Link.Axis)0;
			}
			for (int i = 0; i < mechanisms.Count; i++)
			{
				foreach (Link link in mechanisms[i].links)
				{
					if (link.rigidBody == null)
					{
						Debug.LogError("LINKAGE: Rigid body left blank in channel " + i, this);
						continue;
					}
					if (link.leverage == 0f)
					{
						Debug.LogError("LINKAGE: Leverage of " + link.rigidBody.name + " set to 0 in channel " + i, link.rigidBody);
						continue;
					}
					LinkedBody_NoManualAdd.LinkedBody linkedBody = link.rigidBody.GetComponent<LinkedBody_NoManualAdd.LinkedBody>();
					if (linkedBody == null)
					{
						linkedBody = link.rigidBody.gameObject.AddComponent<LinkedBody_NoManualAdd.LinkedBody>();
						linkedBody.referenceAngle = (linkedBody.trackedAngle = link.rigidBody.rotation * ((float)Math.PI / 180f));
						linkedBody.referencePos = link.rigidBody.position;
						linkedBody.UpdateMechanismAxes();
						bodies.Add(linkedBody);
					}
					linkedBody.linkedAxes |= link.axis;
					if ((link.axis & Link.Axis.Local_X) != 0)
					{
						if ((linkedBody.linkedAxes & (Link.Axis)24) != 0)
						{
							Debug.LogError("LINKAGE: Cannot link both World and Local axes of " + linkedBody.name, linkedBody.gameObject);
							continue;
						}
						if (linkedBody.leverageX != 0f)
						{
							Debug.LogError("LINKAGE: Local X-axis of " + linkedBody.name + " bound to more than one channel.", linkedBody.gameObject);
						}
						else
						{
							linkedBody.leverageX = link.leverage;
							linkedBody.mechanismX = i;
						}
					}
					if ((link.axis & Link.Axis.World_X) != 0)
					{
						if ((linkedBody.linkedAxes & (Link.Axis)3) != 0)
						{
							Debug.LogError("LINKAGE: Cannot link both World and Local axes of " + linkedBody.name, linkedBody.gameObject);
							continue;
						}
						if (linkedBody.leverageX != 0f)
						{
							Debug.LogError("LINKAGE: Local X-axis of " + linkedBody.name + " bound to more than one channel.", linkedBody.gameObject);
						}
						else
						{
							linkedBody.mechX = Vector2.right;
							linkedBody.mechY = Vector2.up;
							linkedBody.leverageX = link.leverage;
							linkedBody.mechanismX = i;
						}
					}
					if ((link.axis & Link.Axis.Local_Y) != 0)
					{
						if ((linkedBody.linkedAxes & (Link.Axis)24) != 0)
						{
							Debug.LogError("LINKAGE: Cannot link both World and Local axes of " + linkedBody.name, linkedBody.gameObject);
							continue;
						}
						if (linkedBody.leverageY != 0f)
						{
							Debug.LogError("LINKAGE: Local Y-axis of " + linkedBody.name + " bound to more than one channel.", linkedBody.gameObject);
						}
						else
						{
							linkedBody.leverageY = link.leverage;
							linkedBody.mechanismY = i;
						}
					}
					if ((link.axis & Link.Axis.World_Y) != 0)
					{
						if ((linkedBody.linkedAxes & (Link.Axis)3) != 0)
						{
							Debug.LogError("LINKAGE: Cannot link both World and Local axes of " + linkedBody.name, linkedBody.gameObject);
							continue;
						}
						if (linkedBody.leverageY != 0f)
						{
							Debug.LogError("LINKAGE: Local Y-axis of " + linkedBody.name + " bound to more than one channel.", linkedBody.gameObject);
						}
						else
						{
							linkedBody.mechX = Vector2.right;
							linkedBody.mechY = Vector2.up;
							linkedBody.leverageY = link.leverage;
							linkedBody.mechanismY = i;
						}
					}
					if ((link.axis & Link.Axis.Rotation) != 0)
					{
						if (linkedBody.leverageR != 0f)
						{
							Debug.LogError("LINKAGE: Rotation of " + linkedBody.name + " bound to more than one channel.", linkedBody.gameObject);
							continue;
						}
						linkedBody.leverageR = link.leverage * ((float)Math.PI / 180f);
						linkedBody.mechanismR = i;
					}
				}
			}
			foreach (LinkedBody_NoManualAdd.LinkedBody body2 in bodies)
			{
				if (body2.linkedAxes == Link.Axis.Rotation)
				{
					body2.mechX = Vector2.right;
					body2.mechY = Vector2.up;
				}
				if ((body2.linkedAxes & Link.Axis.Rotation) != 0)
				{
					if ((body2.rigid.constraints & RigidbodyConstraints2D.FreezeRotation) != 0)
					{
						Debug.LogWarning("LINKAGE: " + body2.name + " has linked rotation, but its rigid body rotation is frozen. Mechanism " + body2.mechanismR + " (" + mechanisms[body2.mechanismR].name + ") will be locked.");
					}
					if (body2.rigid.centerOfMass.sqrMagnitude > 0.001f)
					{
						body2.TransferConstraints(RigidbodyConstraints2D.FreezePosition);
					}
				}
				if ((body2.linkedAxes & (Link.Axis)3) != 0)
				{
					if (Mathf.Abs(body2.mechX.x) > 0.99f)
					{
						Debug.LogWarning("LINKAGE: " + body2.name + " is linked by Local axes, but could by linked by World axes");
					}
					body2.TransferConstraints(RigidbodyConstraints2D.FreezePosition);
				}
				if ((body2.rigid.constraints & RigidbodyConstraints2D.FreezePosition) == RigidbodyConstraints2D.FreezePositionY)
				{
					body2.TransferConstraints(RigidbodyConstraints2D.FreezePositionY);
				}
			}
		}

		private void PrepareMechanisms()
		{
			for (int i = 0; i < mechanisms.Count; i++)
			{
				mechanisms[i].PreUpdate();
			}
		}

		private void GatherMassesAndForces()
		{
			foreach (LinkedBody_NoManualAdd.LinkedBody body in bodies)
			{
				for (int i = 0; i < body.joints.Length; i++)
				{
					Joint2D joint2D = body.joints[i];
					body.summedForce -= joint2D.reactionForce * Time.fixedDeltaTime;
					body.summedTorque -= joint2D.reactionTorque * Time.fixedDeltaTime;
				}
				body.forcesApplied.Set(Vector2.Dot(body.mechX, body.summedForce), Vector2.Dot(body.mechY, body.summedForce), body.summedTorque);
				Vector2 vector = (body.summedForce = Vector2.up * Physics2D.gravity * body.rigid.mass * Time.fixedDeltaTime * body.rigid.gravityScale);
				if (body.leverageX != 0f)
				{
					mechanisms[body.mechanismX].force += body.forcesApplied.x * body.leverageX;
					mechanisms[body.mechanismX].contributors++;
					mechanisms[body.mechanismX].mass += body.rigid.mass;
				}
				else
				{
					body.forcesApplied.x = 0f;
				}
				if (body.leverageY != 0f)
				{
					mechanisms[body.mechanismY].force += body.forcesApplied.y * body.leverageY;
					mechanisms[body.mechanismY].contributors++;
					mechanisms[body.mechanismY].mass += body.rigid.mass;
				}
				else
				{
					body.forcesApplied.y = 0f;
				}
				if (body.leverageR != 0f)
				{
					mechanisms[body.mechanismR].force += body.forcesApplied.z * body.leverageR;
					mechanisms[body.mechanismR].contributors++;
					mechanisms[body.mechanismR].mass += body.rigid.inertia;
				}
				else
				{
					body.forcesApplied.z = 0f;
				}
			}
			for (int j = 0; j < mechanisms.Count; j++)
			{
				mechanisms[j].ScaleForceApplyNudgeAndTension();
			}
		}

		private void RedistributeForces()
		{
			foreach (LinkedBody_NoManualAdd.LinkedBody body in bodies)
			{
				float num = 0f;
				body.forcesRedistributed = Vector3.zero;
				if (body.leverageX != 0f)
				{
					num = mechanisms[body.mechanismX].force / body.leverageX;
					body.rigid.AddForce(body.mechX * num, ForceMode2D.Impulse);
					body.forcesRedistributed.x = num;
				}
				if (body.leverageY != 0f)
				{
					num = mechanisms[body.mechanismY].force / body.leverageY;
					body.rigid.AddForce(body.mechY * num, ForceMode2D.Impulse);
					body.forcesRedistributed.y = num;
				}
				if (body.leverageR != 0f)
				{
					num = mechanisms[body.mechanismR].force / body.leverageR;
					body.rigid.AddTorque(num, ForceMode2D.Impulse);
					body.forcesRedistributed.z = num;
				}
			}
		}

		private void GatherVelocitiesAndDisplacements()
		{
			foreach (LinkedBody_NoManualAdd.LinkedBody body in bodies)
			{
				float num;
				for (num = body.rigid.rotation * ((float)Math.PI / 180f); num < body.trackedAngle - (float)Math.PI; num += (float)Math.PI * 2f)
				{
				}
				while (num > body.trackedAngle + (float)Math.PI)
				{
					num -= (float)Math.PI * 2f;
				}
				body.trackedAngle = num;
				if (body.leverageX != 0f)
				{
					mechanisms[body.mechanismX].speed += Vector2.Dot(body.mechX, body.rigid.velocity) / body.leverageX;
					mechanisms[body.mechanismX].displacement += Vector2.Dot(body.mechX, body.rigid.position - body.referencePos) / body.leverageX;
				}
				if (body.leverageY != 0f)
				{
					mechanisms[body.mechanismY].speed += Vector2.Dot(body.mechY, body.rigid.velocity) / body.leverageY;
					mechanisms[body.mechanismY].displacement += Vector2.Dot(body.mechY, body.rigid.position - body.referencePos) / body.leverageY;
				}
				if (body.leverageR != 0f)
				{
					mechanisms[body.mechanismR].speed += body.rigid.angularVelocity * ((float)Math.PI / 180f) / body.leverageR;
					mechanisms[body.mechanismR].displacement += (body.trackedAngle - body.referenceAngle) / body.leverageR;
				}
			}
			for (int i = 0; i < mechanisms.Count; i++)
			{
				mechanisms[i].AverageSpeedDisplacementApplyDragLimitsAndDrive();
			}
		}

		private void RedistributeVelocitiesAndDisplacements()
		{
			foreach (LinkedBody_NoManualAdd.LinkedBody body in bodies)
			{
				Vector2 vector = new Vector2(Vector2.Dot(body.rigid.velocity, body.mechX), Vector2.Dot(body.rigid.velocity, body.mechY));
				Vector2 vector2 = new Vector2(Vector2.Dot(body.rigid.position - body.referencePos, body.mechX), Vector2.Dot(body.rigid.position - body.referencePos, body.mechY));
				float num = body.trackedAngle - body.referenceAngle;
				float num2 = body.rigid.angularVelocity * ((float)Math.PI / 180f);
				if (body.leverageX != 0f)
				{
					vector.x = mechanisms[body.mechanismX].speed * body.leverageX;
					vector2.x = mechanisms[body.mechanismX].displacement * body.leverageX;
				}
				else if (body.lockX)
				{
					vector2.x = 0f;
					vector.x = 0f;
				}
				if (body.leverageY != 0f)
				{
					vector.y = mechanisms[body.mechanismY].speed * body.leverageY;
					vector2.y = mechanisms[body.mechanismY].displacement * body.leverageY;
				}
				else if (body.lockY)
				{
					vector2.y = 0f;
					vector.y = 0f;
				}
				if (body.leverageR != 0f)
				{
					num2 = mechanisms[body.mechanismR].speed * body.leverageR;
					num = mechanisms[body.mechanismR].displacement * body.leverageR;
				}
				else if (body.lockR)
				{
					num = 0f;
					num2 = 0f;
				}
				body.rigid.velocity = body.mechX * vector.x + body.mechY * vector.y;
				float num3 = (body.referenceAngle + num + num2 * Time.fixedDeltaTime) * 57.29578f;
				Vector2 vector3 = Quaternion.Euler(0f, 0f, num3) * body.rigid.centerOfMass - Quaternion.Euler(0f, 0f, body.rigid.rotation) * body.rigid.centerOfMass;
				body.rigid.MovePosition(vector3 + body.referencePos + body.mechX * vector2.x + body.mechY * vector2.y + body.rigid.velocity * Time.fixedDeltaTime);
				body.rigid.MoveRotation(num3);
				body.rigid.angularVelocity = num2 * 57.29578f;
				Quaternion quaternion = Quaternion.Euler(0f, 0f, num3);
				body.summedTorque = Vector2.Dot(quaternion * body.rigid.centerOfMass, new Vector2(body.summedForce.y, 0f - body.summedForce.x));
			}
		}
	}
}
