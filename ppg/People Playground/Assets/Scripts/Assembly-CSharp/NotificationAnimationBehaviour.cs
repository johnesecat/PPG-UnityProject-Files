using TMPro;
using UnityEngine;

public class NotificationAnimationBehaviour : MonoBehaviour
{
	private float t;

	private TextMeshProUGUI text;

	public AnimationCurve OffsetCurve;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		t += Time.unscaledDeltaTime;
		float num = OffsetCurve.Evaluate(t / NotificationControllerBehaviour.GetNotificationLifetime());
		base.transform.localPosition = Vector3.zero + (text.renderedWidth + 100f) * num * Vector3.right;
	}
}
