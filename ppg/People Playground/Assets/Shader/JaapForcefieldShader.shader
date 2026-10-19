Shader "Jaap/ForcefieldShader" {
	Properties {
		_MainTex ("Mask", 2D) = "white" {}
		_NormalTexture ("Normal", 2D) = "white" {}
		_Gradient ("Colour gradient", 2D) = "white" {}
		[HDR] _Left ("Left colour", Vector) = (1,1,1,1)
		[HDR] _Right ("Right colour", Vector) = (1,1,1,1)
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