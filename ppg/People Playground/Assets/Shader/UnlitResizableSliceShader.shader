Shader "Unlit/ResizableSliceShader" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Tint ("Tint", Vector) = (1,1,1,1)
		_NoiseIntensity ("Noise Intensity", Float) = 0.0025
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