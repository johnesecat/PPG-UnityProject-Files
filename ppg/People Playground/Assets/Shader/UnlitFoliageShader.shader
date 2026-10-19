Shader "Unlit/FoliageShader" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_LeafTint ("Leaf Tint", Vector) = (1,0,1,1)
		_SwayIntensity ("Sway Intensity", Float) = 0.05
		_SwaySpeed ("Sway Speed", Float) = 1
		_BurnProgress ("Burn Progress", Range(0, 1)) = 1
		[HDR] _FireColour ("Fire Colour", Vector) = (1,0,0,1)
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