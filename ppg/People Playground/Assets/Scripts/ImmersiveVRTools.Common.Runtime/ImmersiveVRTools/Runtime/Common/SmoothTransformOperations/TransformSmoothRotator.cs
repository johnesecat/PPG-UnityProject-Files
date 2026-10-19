using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.SmoothTransformOperations
{
	public static class TransformSmoothRotator
	{
		public static TrackableCoroutine RotateConstantSpeed(Transform transformToRotate, Quaternion end, float anglesPerSecond)
		{
			float seconds = CalculateDurationNeededToCompleteAtConstantSpeed(end, transformToRotate.rotation, anglesPerSecond);
			return RotateOverSeconds(transformToRotate, end, seconds);
		}

		public static float CalculateDurationNeededToCompleteAtConstantSpeed(Quaternion end, Quaternion start, float anglesPerSecond)
		{
			return Quaternion.Angle(start, end) / anglesPerSecond;
		}

		public static TrackableCoroutine RotateOverSeconds(Transform transformToRotate, Quaternion end, float seconds)
		{
			TrackableCoroutine trackableCoroutine = new TrackableCoroutine();
			return trackableCoroutine.Init(RotateOverSecondsInternal(transformToRotate, end, seconds, trackableCoroutine));
		}

		private static IEnumerator RotateOverSecondsInternal(Transform transformToRotate, Quaternion end, float seconds, TrackableCoroutine trackableCoroutine)
		{
			float elapsedTime = 0f;
			Quaternion startingRotation = transformToRotate.rotation;
			while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
			{
				float t = elapsedTime / seconds;
				Quaternion quaternion2 = (transformToRotate.rotation = Quaternion.Lerp(startingRotation, end, t));
				elapsedTime += Time.deltaTime;
				trackableCoroutine.OnBeforeYieldReturn();
				yield return new WaitForEndOfFrame();
			}
			if (!trackableCoroutine.IsForceStopRequested)
			{
				Quaternion quaternion4 = (transformToRotate.rotation = Quaternion.Lerp(startingRotation, end, 1f));
			}
			trackableCoroutine.OnFinished();
		}
	}
}
