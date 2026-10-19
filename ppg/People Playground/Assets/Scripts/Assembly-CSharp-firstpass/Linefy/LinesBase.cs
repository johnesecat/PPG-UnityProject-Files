using System;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	public abstract class LinesBase : PrimitivesGroup
	{
		private int id_feather = Shader.PropertyToID("_Feather");

		private int id_textureScale = Shader.PropertyToID("_TextureScale");

		private int id_textureOffset = Shader.PropertyToID("_TextureOffset");

		private float _feather = 1f;

		private float _textureScale = 1f;

		private float _textureOffset;

		protected bool _autoTextureOffset;

		public float feather
		{
			get
			{
				return _feather;
			}
			set
			{
				if (_feather != value)
				{
					_feather = value;
					if (base.transparent)
					{
						base.material.SetFloat(id_feather, _feather);
					}
				}
			}
		}

		public float textureScale
		{
			get
			{
				return _textureScale;
			}
			set
			{
				if (value != _textureScale)
				{
					_textureScale = value;
					base.material.SetFloat(id_textureScale, _textureScale);
				}
			}
		}

		public float textureOffset
		{
			get
			{
				return _textureOffset;
			}
			set
			{
				if (_textureOffset != value)
				{
					_textureOffset = value;
					base.material.SetFloat(id_textureOffset, _textureOffset);
				}
			}
		}

		public virtual bool autoTextureOffset
		{
			get
			{
				return _autoTextureOffset;
			}
			set
			{
				if (_autoTextureOffset != value)
				{
					_autoTextureOffset = value;
				}
			}
		}

		protected virtual void OnAutoTextureOffsetChanged()
		{
		}

		protected override void OnAfterMaterialCreated()
		{
			base.OnAfterMaterialCreated();
			base.material.SetTexture("_MainTex", _texture);
			base.material.SetFloat(id_feather, _feather);
			base.material.SetFloat(id_textureScale, _textureScale);
			base.material.SetFloat(id_textureOffset, _textureOffset);
		}

		[Obsolete("SetVisualPropertyBlock is Obsolete , use LoadSerializationData instead")]
		public override void SetVisualPropertyBlock(VisualPropertiesBlock block)
		{
			base.SetVisualPropertyBlock(block);
			feather = block.feather;
			base.widthMultiplier = block.widthMuliplier;
			base.texture = block.texture;
		}

		public void LoadSerializationData(SerializationData_LinesBase inputData)
		{
			LoadSerializationData((SerializationData_PrimitivesGroup)inputData);
			feather = inputData.feather;
			textureScale = inputData.textureScale;
			textureOffset = inputData.textureOffset;
			autoTextureOffset = inputData.autoTextureOffset;
		}

		public void SaveSerializationData(SerializationData_LinesBase outputData)
		{
			SaveSerializationData((SerializationData_PrimitivesGroup)outputData);
			outputData.feather = feather;
			outputData.textureScale = textureScale;
			outputData.textureOffset = textureOffset;
			outputData.autoTextureOffset = autoTextureOffset;
		}

		public virtual float GetDistanceXY(Vector2 point, ref int segmentIdx, ref float segmentPersentage)
		{
			Debug.LogFormat("Not implemented");
			return float.MaxValue;
		}
	}
}
