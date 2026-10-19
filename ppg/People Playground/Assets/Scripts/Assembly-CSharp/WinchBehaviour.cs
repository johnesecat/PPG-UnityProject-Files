using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WinchBehaviour : MonoBehaviour, Messages.IUse
{
	[SkipSerialisation]
	public Transform Wheel;

	[SkipSerialisation]
	public SpriteRenderer NegativeSpeedLight;

	[SkipSerialisation]
	public SpriteRenderer PositiveSpeedLight;

	[SkipSerialisation]
	public PhysicalBehaviour PhysicalBehaviour;

	[SkipSerialisation]
	public LineRenderer LimitGizmo;

	[SkipSerialisation]
	public SliderJoint2D SliderJoint;

	[SkipSerialisation]
	public AudioSource WinchAudio;

	[SkipSerialisation]
	public AudioClip StartClip;

	[SkipSerialisation]
	public AudioClip Loop;

	[SkipSerialisation]
	public AudioClip EndClip;

	public float BaseSpeed;

	[HideInInspector]
	public float InternalBattery;

	private const float InternalBatteryPower = 5f;

	public float LowerLimit;

	public float UpperLimit = 25f;

	private float previousSpeed;

	private float Speed => BaseSpeed * Mathf.Min(20f, Mathf.Max(InternalBattery, PhysicalBehaviour.Charge)) * base.transform.root.localScale.x;

	private void Awake()
	{
		SliderJoint.useMotor = true;
		WinchAudio.clip = Loop;
	}

	private void Start()
	{
		Rigidbody2D connectedBody = SliderJoint.connectedBody;
		SliderJoint.connectedBody = null;
		SliderJoint.connectedBody = connectedBody;
		AddContextMenuButtons();
		UpdateWinchLimits();
		SetLights();
	}

	private void AddContextMenuButtons()
	{
		List<ContextMenuButton> buttons = PhysicalBehaviour.ContextMenuOptions.Buttons;
		ContextMenuButton[] array = new ContextMenuButton[2];
		ContextMenuButton contextMenuButton = new ContextMenuButton("winchSetLower", "Set lower limit", "Set the lower translation limit for the winch", delegate
		{
			SetLimitFromContextMenu("lower", LowerLimit, delegate(WinchBehaviour w, float v)
			{
				w.LowerLimit = Mathf.Min(w.UpperLimit - 0.01f, v);
				w.UpdateWinchLimits();
			});
		})
		{
			LabelWhenMultipleAreSelected = "Set lower winch limit"
		};
		array[0] = contextMenuButton;
		contextMenuButton = new ContextMenuButton("winchSetUpper", "Set upper limit", "Set the upper translation limit for the winch", delegate
		{
			SetLimitFromContextMenu("upper", UpperLimit, delegate(WinchBehaviour w, float v)
			{
				w.UpperLimit = Mathf.Max(v, w.LowerLimit + 0.01f);
				w.UpdateWinchLimits();
			});
		})
		{
			LabelWhenMultipleAreSelected = "Set upper winch limit"
		};
		array[1] = contextMenuButton;
		buttons.AddRange(array);
	}

	private void UpdateWinchLimits()
	{
		bool flag = base.transform.root.localScale.x < 0f;
		SliderJoint.limits = new JointTranslationLimits2D
		{
			min = (flag ? (0f - UpperLimit) : LowerLimit) / (220f / 267f),
			max = (flag ? (0f - LowerLimit) : UpperLimit) / (220f / 267f)
		};
		LimitGizmo.SetPosition(0, new Vector3(LowerLimit / (220f / 267f), 0f, 0f));
		LimitGizmo.SetPosition(1, new Vector3(UpperLimit / (220f / 267f), 0f, 0f));
	}

	private void SetLimitFromContextMenu(string limit, float startValue, Action<WinchBehaviour, float> setValueFunction)
	{
		if (SelectionController.Main.SelectedObjects.Count > 1)
		{
			if (!(SelectionController.Main.SelectedObjects.Where((PhysicalBehaviour a) => a.TryGetComponent<WinchBehaviour>(out var _)).First().transform.root == base.transform.root))
			{
				return;
			}
			PhysicalBehaviour[] old = SelectionController.Main.SelectedObjects.ToArray();
			DialogBox dialog2 = null;
			dialog2 = DialogBoxManager.TextEntry("Set winch " + limit + " limit", "Translation in meters", new DialogButton("Apply", true, delegate
			{
				for (int i = 0; i < old.Length; i++)
				{
					if (old[i].TryGetComponent<WinchBehaviour>(out var component))
					{
						setLimit(dialog2, component);
					}
				}
			}), new DialogButton("Cancel", true));
			dialog2.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			dialog2.InputField.text = startValue.ToString();
			dialog2.InputField.Select();
		}
		else
		{
			DialogBox dialog = null;
			dialog = DialogBoxManager.TextEntry("Set winch " + limit + " limit", "Translation in meters", new DialogButton("Apply", true, delegate
			{
				setLimit(dialog, this);
			}), new DialogButton("Cancel", true));
			dialog.InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			dialog.InputField.text = startValue.ToString();
			dialog.InputField.Select();
		}
		void setLimit(DialogBox d, WinchBehaviour winch)
		{
			if (float.TryParse(d.InputField.text, out var result))
			{
				setValueFunction(winch, result);
			}
			else
			{
				NotificationControllerBehaviour.Show("Invalid decimal input for winch limit!");
			}
		}
	}

	private void Update()
	{
		TurnWheel();
	}

	private void FixedUpdate()
	{
		AffectSlider();
	}

	private void AffectSlider()
	{
		float num = Speed * 0.01f;
		HandleSound(num);
		previousSpeed = num;
		JointMotor2D motor = SliderJoint.motor;
		bool flag = SliderJoint.limitState == (JointLimitState2D)((num < 0f) ? 1 : 2);
		motor.motorSpeed = (flag ? 0f : num);
		SliderJoint.motor = motor;
		SliderJoint.connectedBody.WakeUp();
	}

	private void HandleSound(float speed)
	{
		speed = Mathf.Abs(speed);
		if (speed < 0.05f && Mathf.Abs(previousSpeed) > 0.05f)
		{
			WinchAudio.Stop();
			WinchAudio.PlayOneShot(EndClip);
		}
		else if (speed > 0.05f && Mathf.Abs(previousSpeed) < 0.05f)
		{
			WinchAudio.PlayOneShot(StartClip);
			WinchAudio.Play();
		}
	}

	private void OnDisable()
	{
		SliderJoint.useMotor = false;
		WinchAudio.Stop();
		previousSpeed = 0f;
		SetLights();
	}

	private void OnEnable()
	{
		SliderJoint.useMotor = true;
		SetLights();
	}

	public void Use(ActivationPropagation activation)
	{
		if (activation.Channel == 1)
		{
			InternalBattery = (Mathf.Approximately(InternalBattery, 5f) ? 0f : 5f);
			return;
		}
		BaseSpeed = 0f - BaseSpeed;
		WinchAudio.PlayOneShot(EndClip);
		SetLights();
	}

	private void SetLights()
	{
		bool flag = BaseSpeed > 0f;
		PositiveSpeedLight.enabled = flag && base.enabled;
		NegativeSpeedLight.enabled = !flag && base.enabled;
	}

	private void TurnWheel()
	{
		Wheel.Rotate(0f, 0f, Time.deltaTime * Speed / 2f);
	}
}
