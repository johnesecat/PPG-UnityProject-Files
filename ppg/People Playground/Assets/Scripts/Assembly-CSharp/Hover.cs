using UnityEngine;

public abstract class Hover : MonoBehaviour, Messages.IOnUserDelete
{
	internal static Hover CurrentlyHovering;

	[SkipSerialisation]
	public bool IsMouseInsideBounds;

	[SkipSerialisation]
	public bool IsMouseOverlapping;

	protected abstract Bounds GetVisualBounds();

	protected virtual void CheckMouseInput()
	{
		if (DialogBox.IsAnyDialogboxOpen || Global.ActiveUiBlock)
		{
			return;
		}
		IsMouseInsideBounds = GetVisualBounds().IsPointInsideBounds(Global.main.MousePosition);
		bool flag;
		if (!IsMouseInsideBounds)
		{
			flag = false;
			if (CurrentlyHovering == this)
			{
				CurrentlyHovering = null;
			}
		}
		else
		{
			flag = IsMouseInsideCollider();
			if (flag)
			{
				if (UserPreferenceManager.Current.DeleteWireByContextMenu)
				{
					CurrentlyHovering = this;
				}
				if (InputSystem.Held("delete"))
				{
					OnUserDelete();
					Object.Destroy(this);
				}
			}
		}
		if (IsMouseOverlapping != flag)
		{
			IsMouseOverlapping = flag;
			OnMouseOverlapEvent(flag);
		}
	}

	public abstract void OnUserDelete();

	public virtual void OnMouseOverlapEvent(bool overlap)
	{
	}

	protected abstract bool IsMouseInsideCollider();

	protected virtual void OnDestroy()
	{
		if (CurrentlyHovering == this)
		{
			CurrentlyHovering = null;
		}
	}
}
