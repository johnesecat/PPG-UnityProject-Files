Shader "Unlit/UvSlideSprite" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_OffsetA ("Offset A", Vector) = (0,0,0,0)
		_OffsetB ("Offset B", Vector) = (1,0,0,0)
		_Transform ("Transform", Vector) = (1,1,0,0)
		_Offset ("Offset", Range(0, 1)) = 0
		[HDR] _Tint ("Tint", Vector) = (1,1,1,0)
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