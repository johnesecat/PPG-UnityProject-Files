using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct BaseControlNames
{
	public const string Drag = "drag";

	public const string Pan = "pan";

	public const string PanLeft = "panLeft";

	public const string PanRight = "panRight";

	public const string PanUp = "panUp";

	public const string PanDown = "panDown";

	public const string ZoomIn = "zoomIn";

	public const string ZoomOut = "zoomOut";

	public const string Pause = "pause";

	public const string ToggleDetailView = "toggleLimbStatus";

	public const string ToggleSlowMotion = "slowmo";

	public const string PauseTime = "time";

	public const string Copy = "copy";

	public const string Paste = "paste";

	public const string Snap = "snap";

	public const string SnapToCenter = "snapToCenter";

	public const string RotateRight = "right";

	public const string RotateLeft = "left";

	public const string FastRotation = "fast";

	public const string UniformResizing = "fast";

	public const string Multiselect = "fast";

	public const string Activate = "activateDirect";

	public const string Delete = "delete";

	public const string ContextMenu = "context";

	public const string ToggleToybox = "toybox";

	public const string ToggleUserInterface = "toggleUi";

	public const string ToggleToolPowerTab = "toolPowerToggle";

	public const string Undo = "undo";

	public const string SpawnRight = "spawnRight";

	public const string SpawnLeft = "spawnLeft";

	public const string SelectMultiple = "selectMultiple";

	public const string LocalSpaceTransform = "localSpaceTransform";

	public const string FreezeUnderCursor = "freezeSelection";

	public const string IgniteSelection = "igniteSelection";

	public const string ResizeSelection = "resizeSelection";

	public const string ToggleHoveringHighlights = "toggleHoveringHighlights";

	public const string ToggleThermalVision = "toggleThermalVision";

	public const string ChooseObjectUnderCursor = "eyedrop";

	public const string MoveLayerDown = "arrangeDown";

	public const string MoveLayerUp = "arrangeUp";

	public const string BringToFront = "arrangeFront";

	public const string SendToBack = "arrangeBack";

	public const string ArrangeSelection = "arrangeSelection";

	public const string IncrementGrid = "incrementGrid";

	public const string DecrementGrid = "decrementGrid";

	public const string IncrementAngleSnap = "incrementAngleSnap";

	public const string DecrementAngleSnap = "decrementAngleSnap";
}
