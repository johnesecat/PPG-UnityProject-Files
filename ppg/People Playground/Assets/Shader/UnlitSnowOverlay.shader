Shader "Unlit/SnowOverlay" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Noise ("Noise", 2D) = "white" {}
		_SnowDirection ("Snow Direction", Vector) = (0,1,0,0)
		_NoiseDirection ("Noise Direction", Vector) = (1,0,0,0)
		_Intensity ("Intensity", Float) = 1
		_Tiling ("Snow Tiling", Float) = 1
		_TilingN ("Noise Tiling", Float) = 1
		_BlackIsAlpha ("Black is Alpha", Range(0, 1)) = 0
		_Visibility ("Visibility", Range(0, 4)) = 1
		_Density ("Density", Range(0, 1)) = 1
		[HDR] _Tint ("Tint", Vector) = (1,0,1,1)
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