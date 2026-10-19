using UnityEngine;

public struct RotatedRectangle
{
	public readonly Vector2 Center;

	public readonly Vector2 Size;

	public readonly Vector2 Pivot;

	public readonly float AngleDegrees;

	public RotatedRectangle(Vector2 center, Vector2 size, Vector2 pivot, float angleDegrees)
	{
		Center = center;
		Size = size;
		Pivot = pivot;
		AngleDegrees = angleDegrees;
	}

	public Vector2 GetTopLeft()
	{
		return Utils.Rotate(new Vector3(Center.x - Size.x / 2f, Center.y + Size.y / 2f), AngleDegrees, Pivot);
	}

	public Vector2 GetTopRight()
	{
		return Utils.Rotate(new Vector3(Center.x + Size.x / 2f, Center.y + Size.y / 2f), AngleDegrees, Pivot);
	}

	public Vector2 GetBottomLeft()
	{
		return Utils.Rotate(new Vector3(Center.x - Size.x / 2f, Center.y - Size.y / 2f), AngleDegrees, Pivot);
	}

	public Vector2 GetBottomRight()
	{
		return Utils.Rotate(new Vector3(Center.x + Size.x / 2f, Center.y - Size.y / 2f), AngleDegrees, Pivot);
	}

	public override bool Equals(object obj)
	{
		if (obj is RotatedRectangle)
		{
			RotatedRectangle rotatedRectangle = (RotatedRectangle)obj;
			if (Center.Equals(rotatedRectangle.Center) && Size.Equals(rotatedRectangle.Size))
			{
				return AngleDegrees == rotatedRectangle.AngleDegrees;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 1166163153 * -1521134295;
		Vector2 center = Center;
		int num2 = (num + center.GetHashCode()) * -1521134295;
		center = Size;
		return (num2 + center.GetHashCode()) * -1521134295 + AngleDegrees.GetHashCode();
	}

	public static bool operator ==(RotatedRectangle left, RotatedRectangle right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RotatedRectangle left, RotatedRectangle right)
	{
		return !(left == right);
	}
}
