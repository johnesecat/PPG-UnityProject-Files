using System;
using ImmersiveVRTools.Runtime.Common.Variable;

[Serializable]
public class Reference<T, G> : Reference where G : Variable<T>
{
	public bool UseConstant = true;

	public T ConstantValue;

	public G Variable;

	public T Value
	{
		get
		{
			if (!UseConstant)
			{
				return Variable.Value;
			}
			return ConstantValue;
		}
		set
		{
			if (UseConstant)
			{
				ConstantValue = value;
			}
			else
			{
				Variable.Value = value;
			}
		}
	}

	public Reference()
	{
	}

	public Reference(T value)
	{
		UseConstant = true;
		ConstantValue = value;
	}

	public static implicit operator T(Reference<T, G> Reference)
	{
		return Reference.Value;
	}

	public static implicit operator Reference<T, G>(T Value)
	{
		return new Reference<T, G>(Value);
	}
}
