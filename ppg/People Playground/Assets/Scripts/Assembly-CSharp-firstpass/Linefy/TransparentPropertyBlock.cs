using System;
using UnityEngine;

namespace Linefy
{
	[Obsolete("TransparentPropertyBlock is Obsolete , use proper cserializable class from Linefy.Serialization")]
	public class TransparentPropertyBlock
	{
		public Color colorMuliplier;

		public TransparentPropertyBlock(float a, Color b, float c, float d)
		{
			colorMuliplier = b;
		}
	}
}
