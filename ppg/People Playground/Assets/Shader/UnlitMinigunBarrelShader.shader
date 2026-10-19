Shader "Unlit/MinigunBarrelShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ActualSprite ("Texture", 2D) = "white" {}
		_Mask ("Rotating Mask", 2D) = "white" {}
		_Rotation ("Rotation", Float) = 0
		[HDR] _GlowColour ("Glow Colour", Vector) = (1,0,1,1)
		_Progress ("Glow Intensity", Range(0, 1)) = 1
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