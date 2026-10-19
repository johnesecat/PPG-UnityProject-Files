Shader "Jaap/SeismicChargeImplosion" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Pattern ("Pattern", 2D) = "white" {}
		[HDR] _Tint ("Tint", Vector) = (0,0,0,0)
		_CollapseSpeed ("Collapse Speed", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard alpha
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}