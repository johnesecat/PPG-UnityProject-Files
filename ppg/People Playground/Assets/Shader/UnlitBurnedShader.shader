Shader "Unlit/BurnedShader" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_BurnProgress ("Burn Progress Map", 2D) = "white" {}
		_Progress ("Progress", Range(0, 1)) = 0
		[HideInInspector] _RendererColor ("RendererColor", Vector) = (1,1,1,1)
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