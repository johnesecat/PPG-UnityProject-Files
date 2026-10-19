Shader "Jaap/BodyShader" {
	Properties {
		_MainTex ("Skin", 2D) = "white" {}
		_FleshTex ("Flesh", 2D) = "white" {}
		_BoneTex ("Skeleton", 2D) = "white" {}
		_DamageTex ("Damage", 2D) = "white" {}
		_DamageMultiplier ("Damage Multiplier", Float) = 1
		_BurnProgressInfluence ("Burn Progress Influence", Float) = 1
		_BurnProgress ("Burn Progress", Range(0, 1)) = 0
		_AcidProgress ("Acid Progress", Range(0, 1)) = 0
		_RottenProgress ("Rotten Progress", Range(0, 1)) = 0
		_ElectrocutionIntensity ("Electrocution Intensity", Range(0, 1)) = 0
		_BruiseColor ("Bruise Color", Vector) = (1,0,1,1)
		_SecondBruiseColor ("Second bruise Color", Vector) = (1,0,1,1)
		_ThirdBruiseColor ("Third bruise Color", Vector) = (1,0,1,1)
		_BloodColor ("Blood Color", Vector) = (1,0,0,1)
		_Zombie ("Zombie Color", Vector) = (1,0,0,1)
		[HDR] _ElectroBoneColour ("Electrified Bone Colour", Vector) = (1,1,1,1)
		_FreezeColour ("Freeze Colour", Vector) = (1,1,1,1)
		[HDR] _GlowColour ("Heat Colour", Vector) = (1,0,1,1)
		_Temperature ("Temperature", Range(-1, 1)) = 1
		_DamagePointCount ("debug damage point count", Float) = 1
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