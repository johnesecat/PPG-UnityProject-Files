Shader "Unlit/RotaryFanShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_FanFrequency ("Blade count", Float) = 4
		_Speed ("Rotation speed", Float) = 1
		_Pixelation ("Pixelation", Float) = 35
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