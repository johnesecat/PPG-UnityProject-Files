using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("UI/Extensions/UI_Knob")]
	public class UI_Knob : Selectable, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IInitializePotentialDragHandler
	{
		public enum Direction
		{
			CW,
			CCW
		}

		[Tooltip("Direction of rotation CW - clockwise, CCW - counterClockwise")]
		public Direction direction;

		[HideInInspector]
		public float KnobValue;

		[Tooltip("Max value of the knob, maximum RAW output value knob can reach, overrides snap step, IF set to 0 or higher than loops, max value will be set by loops")]
		public float MaxValue;

		[Tooltip("How many rotations knob can do, if higher than max value, the latter will limit max value")]
		public int Loops;

		[Tooltip("Clamp output value between 0 and 1, useful with loops > 1")]
		public bool ClampOutput01;

		[Tooltip("snap to position?")]
		public bool SnapToPosition;

		[Tooltip("Number of positions to snap")]
		public int SnapStepsPerLoop = 10;

		[Tooltip("Parent touch area to extend the touch radius")]
		public RectTransform ParentTouchMask;

		[Tooltip("Default background color of the touch mask. Defaults as transparent")]
		public Color MaskBackground = new Color(0f, 0f, 0f, 0f);

		[Space(30f)]
		public KnobFloatValueEvent OnValueChanged;

		private float _currentLoops;

		private float _previousValue;

		private float _initAngle;

		private float _currentAngle;

		private Vector2 _currentVector;

		private Quaternion _initRotation;

		private bool _canDrag;

		private bool _screenSpaceOverlay;

		protected override void Awake()
		{
			_screenSpaceOverlay = GetComponentInParent<Canvas>().rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
		}

		protected override void Start()
		{
			CheckForParentTouchMask();
		}

		private void CheckForParentTouchMask()
		{
			if ((bool)ParentTouchMask)
			{
				ParentTouchMask.gameObject.GetOrAddComponent<Image>().color = MaskBackground;
				EventTrigger orAddComponent = ParentTouchMask.gameObject.GetOrAddComponent<EventTrigger>();
				orAddComponent.triggers.Clear();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerDown((PointerEventData)data);
				});
				orAddComponent.triggers.Add(entry);
				EventTrigger.Entry entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerUp;
				entry2.callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerUp((PointerEventData)data);
				});
				orAddComponent.triggers.Add(entry2);
				EventTrigger.Entry entry3 = new EventTrigger.Entry();
				entry3.eventID = EventTriggerType.PointerEnter;
				entry3.callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerEnter((PointerEventData)data);
				});
				orAddComponent.triggers.Add(entry3);
				EventTrigger.Entry entry4 = new EventTrigger.Entry();
				entry4.eventID = EventTriggerType.PointerExit;
				entry4.callback.AddListener(delegate(BaseEventData data)
				{
					OnPointerExit((PointerEventData)data);
				});
				orAddComponent.triggers.Add(entry4);
				EventTrigger.Entry entry5 = new EventTrigger.Entry();
				entry5.eventID = EventTriggerType.Drag;
				entry5.callback.AddListener(delegate(BaseEventData data)
				{
					OnDrag((PointerEventData)data);
				});
				orAddComponent.triggers.Add(entry5);
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			_canDrag = false;
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			_canDrag = true;
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			_canDrag = false;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			_canDrag = true;
			base.OnPointerDown(eventData);
			_initRotation = base.transform.rotation;
			if (_screenSpaceOverlay)
			{
				_currentVector = eventData.position - (Vector2)base.transform.position;
			}
			else
			{
				_currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(base.transform.position);
			}
			_initAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * 57.29578f;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_canDrag)
			{
				return;
			}
			if (_screenSpaceOverlay)
			{
				_currentVector = eventData.position - (Vector2)base.transform.position;
			}
			else
			{
				_currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(base.transform.position);
			}
			_currentAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * 57.29578f;
			Quaternion quaternion = Quaternion.AngleAxis(_currentAngle - _initAngle, base.transform.forward);
			quaternion.eulerAngles = new Vector3(0f, 0f, quaternion.eulerAngles.z);
			Quaternion rotation = _initRotation * quaternion;
			if (direction == Direction.CW)
			{
				KnobValue = 1f - rotation.eulerAngles.z / 360f;
				if (SnapToPosition)
				{
					SnapToPositionValue(ref KnobValue);
					rotation.eulerAngles = new Vector3(0f, 0f, 360f - 360f * KnobValue);
				}
			}
			else
			{
				KnobValue = rotation.eulerAngles.z / 360f;
				if (SnapToPosition)
				{
					SnapToPositionValue(ref KnobValue);
					rotation.eulerAngles = new Vector3(0f, 0f, 360f * KnobValue);
				}
			}
			UpdateKnobValue();
			base.transform.rotation = rotation;
			InvokeEvents(KnobValue + _currentLoops);
			_previousValue = KnobValue;
		}

		private void UpdateKnobValue()
		{
			if (Mathf.Abs(KnobValue - _previousValue) > 0.5f)
			{
				if (KnobValue < 0.5f && Loops > 1 && _currentLoops < (float)(Loops - 1))
				{
					_currentLoops += 1f;
				}
				else if (KnobValue > 0.5f && _currentLoops >= 1f)
				{
					_currentLoops -= 1f;
				}
				else
				{
					if (KnobValue > 0.5f && _currentLoops == 0f)
					{
						KnobValue = 0f;
						base.transform.localEulerAngles = Vector3.zero;
						InvokeEvents(KnobValue + _currentLoops);
						return;
					}
					if (KnobValue < 0.5f && _currentLoops == (float)(Loops - 1))
					{
						KnobValue = 1f;
						base.transform.localEulerAngles = Vector3.zero;
						InvokeEvents(KnobValue + _currentLoops);
						return;
					}
				}
			}
			if (MaxValue > 0f && KnobValue + _currentLoops > MaxValue)
			{
				KnobValue = MaxValue;
				float z = ((direction == Direction.CW) ? (360f - 360f * MaxValue) : (360f * MaxValue));
				base.transform.localEulerAngles = new Vector3(0f, 0f, z);
				InvokeEvents(KnobValue);
			}
		}

		public void SetKnobValue(float value, int loops = 0)
		{
			Quaternion identity = Quaternion.identity;
			KnobValue = value;
			_currentLoops = loops;
			if (SnapToPosition)
			{
				SnapToPositionValue(ref KnobValue);
			}
			if (direction == Direction.CW)
			{
				identity.eulerAngles = new Vector3(0f, 0f, 360f - 360f * KnobValue);
			}
			else
			{
				identity.eulerAngles = new Vector3(0f, 0f, 360f * KnobValue);
			}
			UpdateKnobValue();
			base.transform.rotation = identity;
			InvokeEvents(KnobValue + _currentLoops);
			_previousValue = KnobValue;
		}

		private void SnapToPositionValue(ref float knobValue)
		{
			float num = 1f / (float)SnapStepsPerLoop;
			float num2 = (knobValue = Mathf.Round(knobValue / num) * num);
		}

		private void InvokeEvents(float value)
		{
			if (ClampOutput01)
			{
				value /= (float)Loops;
			}
			OnValueChanged.Invoke(value);
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}
	}
}
