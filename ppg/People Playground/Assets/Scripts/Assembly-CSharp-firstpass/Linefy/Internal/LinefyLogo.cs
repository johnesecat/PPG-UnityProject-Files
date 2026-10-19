using System;
using UnityEngine;

namespace Linefy.Internal
{
	public class LinefyLogo : Drawable
	{
		public DVector3Value logoCenter;

		public DFloatValue linesWidths;

		public DFloatValue shadowWidths;

		public DFloatValue shadowTransparency;

		public DFloatValue rombusRadius;

		public DFloatValue crossRadius;

		public DVector3Value shadowOffset;

		public DFloatValue crossRotation;

		public DFlag d_positions;

		private Polyline rombus;

		private Polyline rombusShadow;

		private Lines cross;

		private Color crossColor00 = Color.yellow;

		private Color crossColor01 = Color.red;

		private Color crossColor10 = Color.green;

		private Color crossColor11 = Color.magenta;

		private Texture _linesTexture;

		private LabelsRenderer text;

		public DVector3Value textOffset;

		public DFloatValue textSize;

		private DotsAtlas _font;

		public Texture linesTexture
		{
			get
			{
				return _linesTexture;
			}
			set
			{
				_linesTexture = value;
				cross.texture = _linesTexture;
				rombus.texture = _linesTexture;
				rombusShadow.texture = _linesTexture;
			}
		}

		public DotsAtlas font
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
				text.atlas = _font;
			}
		}

		public LinefyLogo()
		{
			d_positions = new DFlag("positions", initialValue: true);
			logoCenter = new DVector3Value(new Vector3(-20f, 0f, 0f), d_positions);
			linesWidths = new DFloatValue(1.7f, d_positions);
			rombusRadius = new DFloatValue(5f, d_positions);
			crossRadius = new DFloatValue(7f, d_positions);
			crossRotation = new DFloatValue(-45f, d_positions);
			shadowOffset = new DVector3Value(new Vector3(0.25f, -0.25f, 0f), d_positions);
			shadowWidths = new DFloatValue(2.5f, d_positions);
			textOffset = new DVector3Value(new Vector3(6f, 0f, 0f));
			textSize = new DFloatValue(0.04f);
			shadowTransparency = new DFloatValue(0.5f, d_positions);
			rombus = new Polyline(4, isClosed: true);
			rombus.transparent = true;
			rombus[0] = new PolylineVertex(Vector3.zero, Color.red, 1f, 0.2f);
			rombus[1] = new PolylineVertex(Vector3.zero, Color.yellow, 1f, 0.21f);
			rombus[2] = new PolylineVertex(Vector3.zero, Color.blue, 1f, 0.22f);
			rombus[3] = new PolylineVertex(Vector3.zero, Color.cyan, 1f, 0.23f);
			rombus.SetTextureOffset(4, 0.24f);
			rombus.feather = 0.05f;
			rombus.renderOrder = 1;
			rombus.widthMode = WidthMode.WorldspaceXY;
			rombusShadow = new Polyline(4, isClosed: true);
			rombusShadow.widthMode = WidthMode.WorldspaceXY;
			rombusShadow.transparent = true;
			rombusShadow[0] = new PolylineVertex(Vector3.zero, Color.white, 1f, 0.7f);
			rombusShadow[1] = new PolylineVertex(Vector3.zero, Color.white, 1f, 0.71f);
			rombusShadow[2] = new PolylineVertex(Vector3.zero, Color.white, 1f, 0.72f);
			rombusShadow[3] = new PolylineVertex(Vector3.zero, Color.white, 1f, 0.73f);
			rombusShadow.SetTextureOffset(4, 0.74f);
			rombusShadow.renderOrder = 0;
			cross = new Lines(12);
			cross.renderOrder = 2;
			cross.widthMode = WidthMode.WorldspaceXY;
			cross.transparent = true;
			cross.colorMultiplier = Color.white;
			cross.feather = 0.05f;
			text = new LabelsRenderer(font, 1);
			text.transparent = true;
			text.widthMode = WidthMode.WorldspaceXY;
			text[0] = new Label("LINEFY", Vector3.zero, Vector2Int.zero);
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			throw new NotImplementedException();
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			throw new NotImplementedException();
		}

		public override void Draw(Matrix4x4 matrix, Camera cam, int layer)
		{
			PreDraw();
			rombusShadow.Draw(matrix, cam, layer);
			rombus.Draw(matrix, cam, layer);
			cross.Draw(matrix, cam, layer);
			text.Draw(matrix, cam, layer);
		}

		private void PreDraw()
		{
			if ((bool)d_positions)
			{
				Color color = new Color(0f, 0f, 0f, shadowTransparency);
				for (int i = 0; i < 4; i++)
				{
					float f = (float)i * 0.25f * (float)Math.PI * 2f;
					float x = Mathf.Cos(f) * (float)rombusRadius;
					float y = Mathf.Sin(f) * (float)rombusRadius;
					Vector3 vector = new Vector3(x, y) + logoCenter;
					rombus.SetPosition(i, vector);
					rombusShadow.SetPosition(i, vector + shadowOffset);
				}
				float num = (float)crossRotation * ((float)Math.PI / 180f);
				float f2 = num + (float)Math.PI;
				float num2 = (float)Math.PI + (float)crossRotation * ((float)Math.PI / 180f) + (float)Math.PI / 2f;
				float f3 = num2 + (float)Math.PI;
				Vector3 vector2 = new Vector3(Mathf.Cos(num), Mathf.Sin(num), 0f) * crossRadius + logoCenter;
				Vector3 vector3 = new Vector3(Mathf.Cos(f2), Mathf.Sin(f2), 0f) * crossRadius + logoCenter;
				Vector3 vector4 = new Vector3(Mathf.Cos(num2), Mathf.Sin(num2), 0f) * crossRadius + logoCenter;
				Vector3 vector5 = new Vector3(Mathf.Cos(f3), Mathf.Sin(f3), 0f) * crossRadius + logoCenter;
				float num3 = (float)linesWidths / ((float)crossRadius * 2f);
				float t = 1f - num3;
				Vector3 vector6 = Vector3.LerpUnclamped(vector2, vector3, num3);
				Vector3 vector7 = Vector3.LerpUnclamped(vector2, vector3, t);
				Vector3 vector8 = Vector3.LerpUnclamped(vector4, vector5, num3);
				Vector3 vector9 = Vector3.LerpUnclamped(vector4, vector5, t);
				Color color2 = Color.Lerp(crossColor00, crossColor01, num3);
				Color color3 = Color.Lerp(crossColor00, crossColor01, t);
				Color color4 = Color.Lerp(crossColor10, crossColor11, num3);
				Color color5 = Color.Lerp(crossColor10, crossColor11, t);
				cross[0] = new Line(vector2 + shadowOffset, vector6 + shadowOffset, color, color, shadowWidths, shadowWidths, 0.5f, 0.75f);
				cross[1] = new Line(vector6 + shadowOffset, vector7 + shadowOffset, color, color, shadowWidths, shadowWidths, 0.75f, 0.751f);
				cross[2] = new Line(vector7 + shadowOffset, vector3 + shadowOffset, color, color, shadowWidths, shadowWidths, 0.75f, 1f);
				cross[3] = new Line(vector2, vector6, crossColor00, color2, linesWidths, linesWidths, 0f, 0.25f);
				cross[4] = new Line(vector6, vector7, color2, color3, linesWidths, linesWidths, 0.25f, 0.25f);
				cross[5] = new Line(vector7, vector3, color3, crossColor01, linesWidths, linesWidths, 0.25f, 0.5f);
				cross[6] = new Line(vector4 + shadowOffset, vector8 + shadowOffset, color, color, shadowWidths, shadowWidths, 0.5f, 0.75f);
				cross[7] = new Line(vector8 + shadowOffset, vector9 + shadowOffset, color, color, shadowWidths, shadowWidths, 0.75f, 0.751f);
				cross[8] = new Line(vector9 + shadowOffset, vector5 + shadowOffset, color, color, shadowWidths, shadowWidths, 0.75f, 1f);
				cross[9] = new Line(vector4, vector8, crossColor10, color4, linesWidths, linesWidths, 0f, 0.25f);
				cross[10] = new Line(vector8, vector9, color4, color5, linesWidths, linesWidths, 0.25f, 0.25f);
				cross[11] = new Line(vector9, vector5, color5, crossColor11, linesWidths, linesWidths, 0.25f, 0.5f);
				rombus.widthMultiplier = linesWidths;
				rombusShadow.colorMultiplier = color;
				rombusShadow.widthMultiplier = shadowWidths;
				d_positions.Reset();
			}
			text.SetPosition(0, textOffset);
			text.size = textSize;
		}

		public override void Dispose()
		{
			rombus.Dispose();
			rombusShadow.Dispose();
			cross.Dispose();
			text.Dispose();
		}
	}
}
