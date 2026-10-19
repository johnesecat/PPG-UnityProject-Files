Shader "Unlit/WaterShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_CausticsTex ("Caustics", 2D) = "white" {}
		_Shimmering ("Distortion Texture", 2D) = "white" {}
		_IceTexture ("Ice texture", 2D) = "white" {}
		_Tint ("Tint", Vector) = (0,0,0,0)
		_DepthColour ("Depth Colour", Vector) = (0,0,0,0)
		_CausticsSpeed ("Caustics speed", Float) = 1
		_Skew ("Skew", Float) = 1
		_DistortIntensity ("Distort Intensity", Float) = 1
		_Brightness ("Brightness", Float) = 1
		_DepthFade ("Depth Fade Factor", Range(0, 1)) = 1
		_DepthMultiplier ("Depth", Float) = 1
		_Temperature ("Temperature", Float) = 18
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