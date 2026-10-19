using System;
using System.Linq;
using UnityEngine;

public class DragTool : ToolBehaviour
{
	private Vector2 offset;

	private HingeJoint2D dragJoint;

	private bool controlRotation;

	private Rigidbody2D selectedRigidbody;

	private float rotationSpeed = 1f;

	private float currentRotation;

	private static DragTool main;

	private Vector3 snappedMouseDelta;

	private Vector3 prevMousePos;

	private bool initialSnappingFinished;

	private bool isActive;

	private ContraptionOutline outline;

	private bool shouldSendContinuousUseMessage;

	private Vector3 initialTranslationSnapOffset;

	protected bool IsSnapping => InputSystem.Held("snap");

	protected Vector3 MousePosWithSnap => SnapToGrid(Global.main.MousePosition);

	public override bool UsesEmptyDrag => false;

	public override bool HideHeldOutline => true;

	public override bool ShowOutlineOnlyOnHover => false;

	private void Awake()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.AddListener(OnContinuousUpdate);
	}

	private void OnContinuousUpdate(float dt)
	{
		if (!shouldSendContinuousUseMessage)
		{
			return;
		}
		foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
		{
			if ((bool)selectedObject)
			{
				ActivationPropagation activationPropagation = new ActivationPropagation(direct: true, 0);
				Utils.Activations.SendContinuous(activationPropagation, selectedObject);
				selectedObject.gameObject.BroadcastMessage("UseContinuous", activationPropagation, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	protected virtual void Update()
	{
		if (!isActive || DialogBox.IsAnyDialogboxOpen || Global.ActiveUiBlock || TriggerEditorBehaviour.IsBeingEdited || Global.main.GetPausedMenu())
		{
			return;
		}
		if (InputSystem.Down("copy") && SelectionController.Main.SelectedObjects.Count > 0)
		{
			Vector3 mousePosition = Global.main.MousePosition;
			NotificationControllerBehaviour.Show($"Copied {SelectionController.Main.SelectedObjects.Count} objects");
			ClipboardControllerBehaviour.Main.ObjectStateInClipboard = ObjectStateConverter.Convert(from c in SelectionController.Main.SelectedObjects
				where (bool)c && (bool)c.gameObject
				select c.gameObject, mousePosition);
			outline = ContraptionOutlineSerialiser.GenerateOutline(SelectionController.Main.SelectedObjects, mousePosition);
		}
		if (InputSystem.Down("paste") && ClipboardControllerBehaviour.Main.ObjectStateInClipboard != null && ClipboardControllerBehaviour.Main.ObjectStateInClipboard.Length != 0)
		{
			SpawnableOutlineBehaviour instance = SpawnableOutlineBehaviour.Instance;
			instance.SetOutline(outline);
			instance.transform.localScale = new Vector3(1f, 1f, 1f);
			instance.gameObject.SetActive(value: true);
		}
		if (InputSystem.Up("paste") && ClipboardControllerBehaviour.Main.ObjectStateInClipboard != null && ClipboardControllerBehaviour.Main.ObjectStateInClipboard.Length != 0)
		{
			Vector3 mousePosition2 = Global.main.MousePosition;
			NotificationControllerBehaviour.Show($"Pasted {ClipboardControllerBehaviour.Main.ObjectStateInClipboard.Length} objects");
			UndoControllerBehaviour.RegisterAction(new PasteLoadAction(ObjectStateConverter.Convert(ClipboardControllerBehaviour.Main.ObjectStateInClipboard, mousePosition2), "Paste"));
			ContraptionOutlineSerialiser.GenerateOutline(SelectionController.Main.SelectedObjects, mousePosition2);
			SpawnableOutlineBehaviour.Instance.gameObject.SetActive(value: false);
		}
	}

	protected virtual float GetDragFunction()
	{
		return 0.9f;
	}

	[Obsolete]
	public static PhysicalBehaviour GetHeldObject()
	{
		return Global.main.ToolControllerBehaviour.CurrentTool.ActiveSingleSelected;
	}

	public override void OnSelect()
	{
		if ((bool)ActiveSingleSelected && !ActiveSingleSelected.GetComponentInChildren<Undraggable>())
		{
			controlRotation = false;
			base.transform.position = Global.main.MousePosition;
			Rigidbody.position = Global.main.MousePosition;
			Rigidbody.velocity = Vector2.zero;
			offset = ActiveSingleSelected.transform.InverseTransformPoint(Global.main.MousePosition);
			Rigidbody.mass = GetDraggerMassFunction();
			currentRotation = ActiveSingleSelected.transform.eulerAngles.z;
			initialSnappingFinished = !IsSnapping;
			Rigidbody.drag = 150f;
			Rigidbody.angularDrag = 150f;
			if (!Global.main.Paused)
			{
				UnityEngine.Object.Destroy(dragJoint);
				dragJoint = base.gameObject.AddComponent<HingeJoint2D>();
				dragJoint.useMotor = false;
				dragJoint.autoConfigureConnectedAnchor = false;
				dragJoint.connectedAnchor = offset;
				dragJoint.connectedBody = ActiveSingleSelected.GetComponent<Rigidbody2D>();
			}
			selectedRigidbody = ActiveSingleSelected.GetComponent<Rigidbody2D>();
			prevMousePos = MousePosWithSnap;
		}
	}

	protected virtual float GetDraggerMassFunction()
	{
		return ActiveSingleSelected.rigidbody.mass * 10f;
	}

	public static bool IsAnyParentSelected(PhysicalBehaviour phys)
	{
		for (int i = 0; i < SelectionController.Main.SelectedObjects.Count; i++)
		{
			PhysicalBehaviour physicalBehaviour = SelectionController.Main.SelectedObjects[i];
			if (!(phys == null) && !(phys == physicalBehaviour) && phys.transform.IsChildOf(physicalBehaviour.transform))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsUndraggableInSelection()
	{
		for (int i = 0; i < SelectionController.Main.SelectedObjects.Count; i++)
		{
			if (SelectionController.Main.SelectedObjects[i].gameObject.HasComponent<Undraggable>())
			{
				return true;
			}
		}
		return false;
	}

	public override void OnHold()
	{
		shouldSendContinuousUseMessage = false;
		if (!ActiveSingleSelected || !selectedRigidbody)
		{
			return;
		}
		if (InputSystem.Down("snap"))
		{
			initialSnappingFinished = false;
		}
		bool flag = !initialSnappingFinished;
		if (flag)
		{
			initialTranslationSnapOffset = SnapToGrid(ActiveSingleSelected.transform.position) - ActiveSingleSelected.transform.position;
			snappedMouseDelta = initialTranslationSnapOffset;
			initialSnappingFinished = true;
		}
		else
		{
			snappedMouseDelta = MousePosWithSnap - prevMousePos;
		}
		prevMousePos = MousePosWithSnap;
		if (InputSystem.Down("right") || InputSystem.Down("left"))
		{
			rotationSpeed = 1f;
		}
		if (InputSystem.Held("right") || InputSystem.Held("left"))
		{
			rotationSpeed += Time.unscaledDeltaTime * 15f;
		}
		if (Global.main.Paused)
		{
			float num = 0f;
			if (InputSystem.Held("right"))
			{
				num -= 250f;
			}
			if (InputSystem.Held("left"))
			{
				num += 250f;
			}
			if (InputSystem.Held("fast"))
			{
				num *= 4f;
			}
			num *= Time.unscaledDeltaTime;
			num *= rotationSpeed * 0.1f;
			int num2 = (IsSnapping ? UserPreferenceManager.Current.AngleSnap : 0);
			float num3 = Mathf.LerpAngle(currentRotation, currentRotation + num, 1f);
			float angle = Mathf.DeltaAngle(Utils.Snap(currentRotation, num2), Utils.Snap(num3, num2));
			if (flag)
			{
				currentRotation = ActiveSingleSelected.transform.eulerAngles.z;
				num3 = Utils.Snap(currentRotation, num2);
				angle = Mathf.DeltaAngle(currentRotation, num3);
			}
			currentRotation = num3;
			Vector3 vector = (IsSnapping ? (SnapToGrid(ActiveSingleSelected.transform.position) - ActiveSingleSelected.transform.position) : Vector3.zero);
			_ = Global.main.MousePosition;
			for (int i = 0; i < SelectionController.Main.SelectedObjects.Count; i++)
			{
				PhysicalBehaviour physicalBehaviour = SelectionController.Main.SelectedObjects[i];
				if ((bool)physicalBehaviour && !IsAnyParentSelected(physicalBehaviour) && !physicalBehaviour.gameObject.HasComponent<Undraggable>())
				{
					physicalBehaviour.transform.position += (IsSnapping ? (snappedMouseDelta + vector) : ((Vector3)Global.MouseDelta));
					physicalBehaviour.transform.RotateAround(Global.main.MousePosition, Vector3.forward, angle);
				}
			}
		}
		else
		{
			MoveToMouse();
		}
		if (Global.main.UILock)
		{
			return;
		}
		if (InputSystem.Down("activateDirect"))
		{
			foreach (PhysicalBehaviour selectedObject in SelectionController.Main.SelectedObjects)
			{
				if ((bool)selectedObject)
				{
					ActivationPropagation activationPropagation = new ActivationPropagation(direct: true, 0, selectedObject.gameObject);
					activationPropagation.Target = selectedObject.gameObject;
					Utils.Activations.SendOnce(activationPropagation, selectedObject);
					selectedObject.gameObject.BroadcastMessage("Use", activationPropagation, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		shouldSendContinuousUseMessage = InputSystem.Held("activateDirect");
		if (InputSystem.Held("delete"))
		{
			SelectionController.Main.UserDeleteSelection();
		}
	}

	private void Rotate()
	{
		if ((bool)dragJoint)
		{
			JointMotor2D motor = dragJoint.motor;
			if (controlRotation)
			{
				motor.maxMotorTorque = 1000f;
				motor.motorSpeed = 0f;
			}
			if (InputSystem.Held("left"))
			{
				float num = (InputSystem.Held("fast") ? 1000 : 250);
				controlRotation = true;
				dragJoint.useMotor = true;
				num = (motor.motorSpeed = num * (rotationSpeed * 0.1f));
			}
			if (InputSystem.Held("right"))
			{
				float num3 = (InputSystem.Held("fast") ? (-1000) : (-250));
				controlRotation = true;
				dragJoint.useMotor = true;
				num3 = (motor.motorSpeed = num3 * (rotationSpeed * 0.1f));
			}
			dragJoint.motor = motor;
			if (!controlRotation)
			{
				dragJoint.useMotor = false;
			}
		}
	}

	private void OnDestroy()
	{
		ContinuousActivationBehaviour.Instance.OnContinuousUpdate.RemoveListener(OnContinuousUpdate);
	}

	protected virtual void MoveToMouse()
	{
		Vector3 vector = (IsSnapping ? MousePosWithSnap : Global.main.MousePosition) - base.transform.position;
		Rigidbody.AddForce(Time.deltaTime * 60f * 1950f * DragStrengthFunction() * vector);
	}

	private Vector3 SnapToGrid(Vector3 input)
	{
		return Utils.Snap(input + WorldGridOffset.Offset, 1.2136364f * UserPreferenceManager.Current.GridSnap) - WorldGridOffset.Offset;
	}

	public override void OnDeselect()
	{
		shouldSendContinuousUseMessage = false;
		controlRotation = false;
		if ((bool)dragJoint)
		{
			UnityEngine.Object.Destroy(dragJoint);
		}
		Rigidbody.mass = 9500f;
	}

	public override void OnFixedHold()
	{
		if (!Global.main.Paused && (bool)ActiveSingleSelected && !ActiveSingleSelected.GetComponentInChildren<Undraggable>() && (bool)selectedRigidbody)
		{
			if (!Global.main.Paused && Vector3.Distance(Global.main.MousePosition, base.transform.position) < 1f)
			{
				Rigidbody.velocity *= GetDragFunction();
			}
			Rotate();
		}
	}

	public override void OnToolChosen()
	{
		isActive = true;
	}

	public override void OnToolUnchosen()
	{
		isActive = false;
		shouldSendContinuousUseMessage = false;
	}

	protected virtual float DragStrengthFunction()
	{
		return Rigidbody.mass;
	}
}
