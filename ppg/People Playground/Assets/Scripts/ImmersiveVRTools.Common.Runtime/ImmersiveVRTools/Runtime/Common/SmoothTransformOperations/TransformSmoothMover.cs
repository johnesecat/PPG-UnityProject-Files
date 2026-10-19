using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.SmoothTransformOperations
{
	public class TransformSmoothMover
	{
		public static TrackableCoroutine MoveConstantSpeed(Transform transformToMove, Vector3 end, float speedPerSecond)
		{
			float secondsForTransition;
			return MoveConstantSpeed(transformToMove, end, speedPerSecond, out secondsForTransition);
		}

		public static TrackableCoroutine MoveConstantSpeed(Transform transformToMove, Vector3 end, float speedPerSecond, out float secondsForTransition)
		{
			secondsForTransition = CalculateDurationNeededToCompleteAtConstantSpeed(end, transformToMove.position, speedPerSecond);
			return MoveOverSeconds(transformToMove, end, secondsForTransition);
		}

		public static float CalculateDurationNeededToCompleteAtConstantSpeed(Vector3 end, Vector3 start, float speedPerSecond)
		{
			return Vector3.Distance(start, end) / speedPerSecond;
		}

		public static TrackableCoroutine MoveOverSeconds(Transform transformToMove, Vector3 end, float seconds)
		{
			TrackableCoroutine trackableCoroutine = new TrackableCoroutine();
			return trackableCoroutine.Init(MoveOverSecondsInternal(transformToMove, end, seconds, trackableCoroutine));
		}

		public static IEnumerator MoveOverSecondsInternal(Transform transformToMove, Vector3 end, float seconds, TrackableCoroutine trackableCoroutine)
		{
			float elapsedTime = 0f;
			Vector3 position = transformToMove.position;
			Vector3 vectorToCover = end - position;
			float alreadyCompleted = 0f;
			while (elapsedTime < seconds && !trackableCoroutine.IsForceStopRequested)
			{
				float num = elapsedTime / seconds - alreadyCompleted;
				Vector3 vector = Vector3.Lerp(Vector3.zero, vectorToCover, num);
				transformToMove.position += vector;
				alreadyCompleted += num;
				elapsedTime += Time.deltaTime;
				trackableCoroutine.OnBeforeYieldReturn();
				yield return new WaitForEndOfFrame();
			}
			if (!trackableCoroutine.IsForceStopRequested)
			{
				Vector3 vector2 = Vector3.Lerp(Vector3.zero, vectorToCover, 1f - alreadyCompleted);
				transformToMove.position += vector2;
			}
			trackableCoroutine.OnFinished();
		}
	}
}
