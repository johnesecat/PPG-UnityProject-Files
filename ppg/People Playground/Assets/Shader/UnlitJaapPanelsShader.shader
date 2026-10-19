Shader "Unlit/Jaap/PanelsShader" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Panels ("Panels", 2D) = "white" {}
		_PanelSize ("Single panel width and height", Float) = 64
		_ColourVariation ("Colour variation", Float) = 0.01
		_FadeColour ("Fade colour", Vector) = (0.5,0.5,0.5,1)
		_Scale ("Scale", Float) = 1
		_Offset ("Offset", Vector) = (0,0,0,0)
		_ScrollingSpeed ("Scroll speed", Vector) = (0,0,0,0)
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