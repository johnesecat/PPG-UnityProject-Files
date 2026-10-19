using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	public class ConstantBuffer
	{
		internal abstract class ConstantBufferBase
		{
			public abstract void Release();
		}

		private class TypedConstantBuffer<CBType> : ConstantBufferBase where CBType : struct
		{
			private HashSet<int> m_GlobalBindings = new HashSet<int>();

			private CBType[] m_Data = new CBType[1];

			private static TypedConstantBuffer<CBType> s_Instance;

			private ComputeBuffer m_GPUConstantBuffer;

			internal static TypedConstantBuffer<CBType> instance
			{
				get
				{
					if (s_Instance == null)
					{
						s_Instance = new TypedConstantBuffer<CBType>();
					}
					return s_Instance;
				}
				set
				{
					s_Instance = value;
				}
			}

			private TypedConstantBuffer()
			{
				m_GPUConstantBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<CBType>(), ComputeBufferType.Constant);
				Register(this);
			}

			public void UpdateData(CommandBuffer cmd, in CBType data)
			{
				m_Data[0] = data;
				cmd.SetComputeBufferData(m_GPUConstantBuffer, m_Data);
			}

			public void SetGlobal(CommandBuffer cmd, int shaderId)
			{
				m_GlobalBindings.Add(shaderId);
				cmd.SetGlobalConstantBuffer(m_GPUConstantBuffer, shaderId, 0, m_GPUConstantBuffer.stride);
			}

			public void Set(CommandBuffer cmd, ComputeShader cs, int shaderId)
			{
				cmd.SetComputeConstantBufferParam(cs, shaderId, m_GPUConstantBuffer, 0, m_GPUConstantBuffer.stride);
			}

			public void Set(Material mat, int shaderId)
			{
				mat.SetConstantBuffer(shaderId, m_GPUConstantBuffer, 0, m_GPUConstantBuffer.stride);
			}

			public override void Release()
			{
				foreach (int globalBinding in m_GlobalBindings)
				{
					Shader.SetGlobalConstantBuffer(globalBinding, (ComputeBuffer)null, 0, 0);
				}
				m_GlobalBindings.Clear();
				CoreUtils.SafeRelease(m_GPUConstantBuffer);
				s_Instance = null;
			}
		}

		private static List<ConstantBufferBase> m_RegisteredConstantBuffers = new List<ConstantBufferBase>();

		public static void PushGlobal<CBType>(CommandBuffer cmd, in CBType data, int shaderId) where CBType : struct
		{
			TypedConstantBuffer<CBType> instance = TypedConstantBuffer<CBType>.instance;
			instance.UpdateData(cmd, in data);
			instance.SetGlobal(cmd, shaderId);
		}

		public static void Push<CBType>(CommandBuffer cmd, in CBType data, ComputeShader cs, int shaderId) where CBType : struct
		{
			TypedConstantBuffer<CBType> instance = TypedConstantBuffer<CBType>.instance;
			instance.UpdateData(cmd, in data);
			instance.Set(cmd, cs, shaderId);
		}

		public static void Push<CBType>(CommandBuffer cmd, in CBType data, Material mat, int shaderId) where CBType : struct
		{
			TypedConstantBuffer<CBType> instance = TypedConstantBuffer<CBType>.instance;
			instance.UpdateData(cmd, in data);
			instance.Set(mat, shaderId);
		}

		public static void UpdateData<CBType>(CommandBuffer cmd, in CBType data) where CBType : struct
		{
			TypedConstantBuffer<CBType>.instance.UpdateData(cmd, in data);
		}

		public static void SetGlobal<CBType>(CommandBuffer cmd, int shaderId) where CBType : struct
		{
			TypedConstantBuffer<CBType>.instance.SetGlobal(cmd, shaderId);
		}

		public static void Set<CBType>(CommandBuffer cmd, ComputeShader cs, int shaderId) where CBType : struct
		{
			TypedConstantBuffer<CBType>.instance.Set(cmd, cs, shaderId);
		}

		public static void Set<CBType>(Material mat, int shaderId) where CBType : struct
		{
			TypedConstantBuffer<CBType>.instance.Set(mat, shaderId);
		}

		public static void ReleaseAll()
		{
			foreach (ConstantBufferBase registeredConstantBuffer in m_RegisteredConstantBuffers)
			{
				registeredConstantBuffer.Release();
			}
			m_RegisteredConstantBuffers.Clear();
		}

		internal static void Register(ConstantBufferBase cb)
		{
			m_RegisteredConstantBuffers.Add(cb);
		}
	}
}
