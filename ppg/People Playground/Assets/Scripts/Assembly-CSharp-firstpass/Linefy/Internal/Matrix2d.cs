using System;
using UnityEngine;

namespace Linefy.Internal
{
	[Serializable]
	public struct Matrix2d
	{
		public static Matrix2d identity = new Matrix2d(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		public static Matrix2d zero = new Matrix2d(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

		public float m00;

		public float m10;

		public float m20;

		public float m01;

		public float m11;

		public float m21;

		public float m02;

		public float m12;

		public float m22;

		public Vector2 Position
		{
			get
			{
				return new Vector2(m02, m12);
			}
			set
			{
				m02 = value.x;
				m12 = value.y;
				m21 = 1f;
			}
		}

		public Vector2 Right => new Vector2(m00, m10);

		public Vector2 Up => new Vector2(m01, m11);

		public float this[int row, int column]
		{
			get
			{
				return this[row + column * 2];
			}
			set
			{
				this[row + column * 2] = value;
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return m00;
				case 1:
					return m10;
				case 2:
					return m01;
				case 3:
					return m11;
				case 4:
					return m02;
				case 5:
					return m12;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					m00 = value;
					break;
				case 1:
					m10 = value;
					break;
				case 2:
					m01 = value;
					break;
				case 3:
					m11 = value;
					break;
				case 4:
					m02 = value;
					break;
				case 5:
					m12 = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		public Matrix2d Inverse
		{
			get
			{
				Matrix2d result = default(Matrix2d);
				float num = this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
				if (Mathf.Approximately(0f, num))
				{
					return zero;
				}
				float num2 = 1f / num;
				result[0, 0] = this[1, 1] * num2;
				result[0, 1] = (0f - this[0, 1]) * num2;
				result[1, 0] = (0f - this[1, 0]) * num2;
				result[1, 1] = this[0, 0] * num2;
				result[0, 2] = 0f - (this[0, 2] * result[0, 0] + this[1, 2] * result[0, 1]);
				result[1, 2] = 0f - (this[0, 2] * result[1, 0] + this[1, 2] * result[1, 1]);
				return result;
			}
		}

		public Matrix2d(Vector2 column0, Vector2 column1, Vector2 column2)
		{
			m00 = column0.x;
			m01 = column1.x;
			m02 = column2.x;
			m10 = column0.y;
			m11 = column1.y;
			m12 = column2.y;
			m20 = 0f;
			m21 = 0f;
			m22 = 1f;
		}

		public Matrix2d(float xDegreeAngle, Vector2 pos)
		{
			float f = xDegreeAngle * ((float)Math.PI / 180f);
			m00 = Mathf.Cos(f);
			m10 = Mathf.Sin(f);
			m20 = 0f;
			m01 = 0f - m10;
			m11 = m00;
			m21 = 0f;
			m02 = pos.x;
			m12 = pos.y;
			m22 = 1f;
		}

		public Matrix2d(Vector2 pos, Vector2 target, bool normalized)
		{
			Vector2 vector = target - pos;
			if (normalized)
			{
				vector = vector.normalized;
			}
			m00 = vector.x;
			m10 = vector.y;
			m20 = 0f;
			m01 = 0f - m10;
			m11 = m00;
			m21 = 0f;
			m02 = pos.x;
			m12 = pos.y;
			m22 = 1f;
		}

		public Matrix2d(Vector2 pos, Vector2 target)
		{
			Vector2 normalized = (target - pos).normalized;
			m00 = normalized.x;
			m10 = normalized.y;
			m20 = 0f;
			m01 = 0f - m10;
			m11 = m00;
			m21 = 0f;
			m02 = pos.x;
			m12 = pos.y;
			m22 = 1f;
		}

		public Matrix2d(float xDegreeAngle, Vector2 pos, Vector2 scale)
		{
			float f = xDegreeAngle * ((float)Math.PI / 180f);
			m00 = Mathf.Cos(f);
			m10 = Mathf.Sin(f);
			m20 = 0f;
			m01 = 0f - m10;
			m11 = m00;
			m21 = 0f;
			m00 *= scale.x;
			m10 *= scale.x;
			m01 *= scale.y;
			m11 *= scale.y;
			m02 = pos.x;
			m12 = pos.y;
			m22 = 1f;
		}

		public Matrix2d(float xRadiansAngle)
		{
			m00 = Mathf.Cos(xRadiansAngle);
			m10 = Mathf.Sin(xRadiansAngle);
			m20 = 0f;
			m01 = 0f - m10;
			m11 = m00;
			m21 = 0f;
			m02 = 0f;
			m12 = 0f;
			m22 = 1f;
		}

		public static Matrix2d DirXDirYPosition(Vector2 dirX, Vector2 dirY, Vector2 pos)
		{
			Matrix2d result = default(Matrix2d);
			result.m00 = dirX.x;
			result.m10 = 0f - dirX.y;
			result.m20 = 0f;
			result.m01 = 0f - dirY.x;
			result.m11 = dirY.y;
			result.m21 = 0f;
			result.m02 = pos.x;
			result.m12 = pos.y;
			result.m22 = 1f;
			return result;
		}

		public Matrix2d(float a00, float a10, float a20, float a01, float a11, float a21, float a02, float a12, float a22)
		{
			m00 = a00;
			m10 = a10;
			m20 = a20;
			m01 = a01;
			m11 = a11;
			m21 = a21;
			m02 = a02;
			m12 = a12;
			m22 = a22;
		}

		public static Matrix2d operator *(Matrix2d lhs, Matrix2d rhs)
		{
			Matrix2d result = default(Matrix2d);
			result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10;
			result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11;
			result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02;
			result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10;
			result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11;
			result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12;
			result.m20 = 0f;
			result.m21 = 0f;
			result.m22 = 1f;
			return result;
		}

		public Vector2 GetColumn(int index)
		{
			switch (index)
			{
			case 0:
				return new Vector2(m00, m10);
			case 1:
				return new Vector2(m01, m11);
			case 2:
				return new Vector2(m02, m12);
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		public Vector3 GetRow(int index)
		{
			switch (index)
			{
			case 0:
				return new Vector3(m00, m01, m02);
			case 1:
				return new Vector3(m10, m11, m12);
			default:
				throw new IndexOutOfRangeException("Invalid row index!");
			}
		}

		public void SetColumn(int index, Vector2 column)
		{
			this[0, index] = column.x;
			this[1, index] = column.y;
		}

		public void SetRow(int index, Vector3 row)
		{
			this[index, 0] = row.x;
			this[index, 1] = row.y;
			this[index, 2] = row.z;
		}

		public static Matrix2d operator *(Matrix2d m, float f)
		{
			m.m00 *= f;
			m.m10 *= f;
			m.m01 *= f;
			m.m11 *= f;
			m.m02 *= f;
			m.m12 *= f;
			return m;
		}

		public static Matrix2d operator *(float f, Matrix2d m)
		{
			return m * f;
		}

		public static Vector2 operator *(Vector2 v, Matrix2d m)
		{
			return m.MultiplyPoint(v);
		}

		public void DrawGizmo()
		{
			Debug.DrawLine(Position, Position + Up, Color.green);
			Debug.DrawLine(Position, Position + Right, Color.red);
		}

		public void DrawGizmoXZ()
		{
			Debug.DrawLine(Position.XYtoXyZ(), (Position + Up).XYtoXyZ(), Color.green);
			Debug.DrawLine(Position.XYtoXyZ(), (Position + Right).XYtoXyZ(), Color.red);
		}

		public Vector2 MultiplyPoint(Vector2 point)
		{
			Vector2 result = default(Vector2);
			result.x = m00 * point.x + m01 * point.y + m02;
			result.y = m10 * point.x + m11 * point.y + m12;
			return result;
		}

		public Vector2 MultiplyVector(Vector2 point)
		{
			Vector2 result = default(Vector2);
			result.x = m00 * point.x + m01 * point.y;
			result.y = m10 * point.x + m11 * point.y;
			return result;
		}

		public Vector3 MultiplyVectorX(Vector3 vec)
		{
			Vector2 vector = MultiplyVector(new Vector2(vec.z, vec.y));
			return new Vector3(vec.x, vector.y, vector.x);
		}

		public Vector3 MultiplyPointX(Vector3 vec)
		{
			Vector2 vector = MultiplyPoint(new Vector2(vec.z, vec.y));
			return new Vector3(vec.x, vector.y, vector.x);
		}

		public Vector3 MultiplyVectorY(Vector3 vec)
		{
			Vector2 vector = MultiplyVector(new Vector2(vec.x, vec.z));
			return new Vector3(vector.x, vec.y, vector.y);
		}

		public Vector3 MultiplyPointY(Vector3 vec)
		{
			Vector2 vector = MultiplyPoint(new Vector2(vec.x, vec.z));
			return new Vector3(vector.x, vec.y, vector.y);
		}

		public Vector3 MultiplyVectorZ(Vector3 vec)
		{
			Vector2 vector = MultiplyVector(new Vector2(vec.x, vec.y));
			return new Vector3(vector.x, vector.y, vec.z);
		}

		public Vector3 MultiplyPointZ(Vector3 vec)
		{
			Vector2 vector = MultiplyPoint(new Vector2(vec.x, vec.y));
			return new Vector3(vector.x, vector.y, vec.z);
		}
	}
}
