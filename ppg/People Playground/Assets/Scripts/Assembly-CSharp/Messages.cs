using System.Collections.Generic;
using UnityEngine;

public class Messages
{
	public interface IShot
	{
		void Shot(Shot shot);
	}

	public interface IUse
	{
		void Use(ActivationPropagation activation);
	}

	public interface IUseContinuous
	{
		void UseContinuous(ActivationPropagation activation);
	}

	public interface IOnFragmentHit
	{
		void OnFragmentHit(float fragmentForce);
	}

	public interface ILasered
	{
		public struct LaserArgs
		{
			public LaserBehaviour Laser;

			public Utils.LaserHit Hit;

			public LaserArgs(LaserBehaviour laser, Utils.LaserHit hit)
			{
				Laser = laser;
				Hit = hit;
			}
		}

		void OnLasered(LaserArgs args);
	}

	public interface IBreak
	{
		void Break(Vector2 impactVelocity);
	}

	public interface IDamage
	{
		void Damage(float damage);
	}

	public interface ISlice
	{
		void Slice();
	}

	public interface IOnUserFreeze
	{
		void OnUserFreeze();
	}

	public interface IOnUserUnfreeze
	{
		void OnUserUnfreeze();
	}

	public interface IExitShot
	{
		void ExitShot(Shot shot);
	}

	public interface IDecal
	{
		void Decal(DecalInstruction decalInstruction);
	}

	public interface IIsolatedContinuousActivation
	{
		void IsolatedContinuousActivation(ActivationPropagation activation);
	}

	public interface IIsolatedActivation
	{
		void IsolatedActivation(ActivationPropagation activation);
	}

	public interface IRepair
	{
		void Repair();
	}

	public interface IOnDrop
	{
		void OnDrop(GripBehaviour formerGripper);
	}

	public interface IOnGripped
	{
		void OnGripped(GripBehaviour gripper);
	}

	public interface IStabbed
	{
		void Stabbed(Stabbing stabbing);
	}

	public interface IUnstabbed
	{
		void Unstabbed(Stabbing stabbing);
	}

	public interface ILodged
	{
		void Lodged(Stabbing stabbing);
	}

	public interface IDislodged
	{
		void Dislodged(PhysicalBehaviour.Penetration penetration);
	}

	public interface IStunImpact
	{
		void StunImpact(Shot shot);
	}

	public interface IWaterImpact
	{
		void WaterImpact(float impactMagnitude);
	}

	public interface IOnEMPHit
	{
		void OnEMPHit();
	}

	public interface IOnAfterDeserialise
	{
		void OnAfterDeserialise(List<GameObject> gameObjects);
	}

	public interface IOnBeforeSerialise
	{
		void OnBeforeSerialise();
	}

	public interface IOnUserDelete
	{
		void OnUserDelete();
	}

	public interface IOnPoolableInitialised
	{
		void OnPoolableInitialised(ObjectPoolBehaviour pool);
	}

	public interface IOnPoolableReinitialised
	{
		void OnPoolableReinitialised(ObjectPoolBehaviour pool);
	}

	public interface IOnImpactCreated
	{
		void OnImpactCreated(GameObject gm);
	}

	public interface IOnInsideVacuum
	{
		void OnInsideVacuum();
	}
}
