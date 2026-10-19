Shader "Unlit/Vila" {
	Properties {
		[PerRendererData] _MainTex ("1", 2D) = "white" {}
		_Layer2 ("2", 2D) = "white" {}
		_Layer3 ("3", 2D) = "white" {}
		_Layer4 ("4", 2D) = "white" {}
		_Depth ("Depth", Float) = 0.2
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