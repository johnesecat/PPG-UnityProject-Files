Shader "Unlit/LavaShader" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_SwirlNoise ("Swirl Noise", 2D) = "white" {}
		_TintGradient ("Tint Gradient", 2D) = "white" {}
		_RockTexture ("Rock Texture", 2D) = "white" {}
		_LavaNoise ("Lava Noise", 2D) = "white" {}
		_RockTint ("Rock Tint", Vector) = (1,1,1,1)
		[HDR] _LavaTint ("Lava Tint", Vector) = (1,1,1,1)
		_CutoffEdge ("Cutoff Edge", Float) = 1
		_Scale ("Scale", Float) = 1
		_Pixelisation ("Pixelisation", Float) = 1
		_Speed ("Speed", Float) = 1
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