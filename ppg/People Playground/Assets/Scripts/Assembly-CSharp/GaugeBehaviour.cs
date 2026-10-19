using System;
using TMPro;
using UnityEngine;

public abstract class GaugeBehaviour : MonoBehaviour
{
	[SkipSerialisation]
	public GameObject GaugeLabelPrefab;

	[SkipSerialisation]
	public Transform Arm;

	[SkipSerialisation]
	public Transform GaugeCenter;

	private DamagableMachineryBehaviour damagableMachinery;

	private float armVel;

	private float armPos;

	public float MinAngle = 162.105f;

	public float MaxAngle = -162.105f;

	public float GaugeRadius = 0.1571429f;

	public float LabelOffset = 0.02f;

	[Min(2f)]
	public int LabelCount = 5;

	protected abstract float GetProgress();

	protected abstract string GetDisplayValueFor(float progress);

	private void Awake()
	{
		damagableMachinery = GetComponent<DamagableMachineryBehaviour>();
	}

	private void Start()
	{
		CreateLabels();
	}

	private void CreateLabels()
	{
		for (int i = 0; i < LabelCount; i++)
		{
			float num = (float)i / ((float)LabelCount - 1f);
			float f = (Mathf.Lerp(MinAngle, MaxAngle, num) + 90f) * ((float)Math.PI / 180f);
			Vector2 vector = (GaugeRadius - LabelOffset) * new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			GameObject obj = UnityEngine.Object.Instantiate(GaugeLabelPrefab, GaugeCenter);
			obj.GetComponent<TextMeshPro>().text = GetDisplayValueFor(num);
			obj.transform.localPosition = vector;
		}
	}

	private void Update()
	{
		float num = (damagableMachinery.Destroyed ? 0f : GetProgress());
		armVel += (num - armPos) * Time.deltaTime;
		armPos += armVel * Time.deltaTime * 150f;
		armVel *= Mathf.Pow(0.1f, Time.deltaTime);
		armPos = Mathf.Clamp01(armPos);
		SetArm(armPos);
	}

	protected void SetArm(float i)
	{
		i = Mathf.Clamp01(i);
		Arm.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(MinAngle, MaxAngle, i));
	}
}
