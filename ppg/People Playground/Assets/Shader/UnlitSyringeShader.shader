Shader "Unlit/SyringeShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_CutoffA ("Cutoff A", Range(0, 1)) = 0.1
		_CutoffB ("Cutoff B", Range(0, 1)) = 0.1
		_LiquidColour ("Liquid colour", Vector) = (1,0,1,1)
		_Direction ("Flow direction", Range(-1, 1)) = 0
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