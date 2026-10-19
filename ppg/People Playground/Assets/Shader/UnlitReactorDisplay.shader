Shader "Unlit/ReactorDisplay" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[HDR] _Wire1 ("Wire 1", Vector) = (1,1,1,1)
		[HDR] _Wire2 ("Wire 2", Vector) = (1,1,1,1)
		[HDR] _Wire3 ("Wire 3", Vector) = (1,1,1,1)
		[HDR] _Wire4 ("Wire 4", Vector) = (1,1,1,1)
		[HDR] _VacuumChamber ("Vacuum Chamber", Vector) = (1,1,1,1)
		[HDR] _Connector ("Connector", Vector) = (1,1,1,1)
		[HDR] _Base ("Base", Vector) = (1,1,1,1)
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