Shader "Unlit/LightsTextureShader" {
	Properties {
		_LightsRenderTexture ("Lights RT", 2D) = "white" {}
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity ("Intensity", Float) = 1
		_MinimumBrightness ("Min Brightness", Float) = 0
		_MaximumBrightness ("Max Brightness", Float) = 1
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