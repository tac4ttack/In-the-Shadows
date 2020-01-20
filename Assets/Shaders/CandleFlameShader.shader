/*
	Candle Flame shader
	based on tutorial by Puck Loves Games (https://www.patreon.com/posts/animated-candle-21053825)
*/

Shader "Candle Flame Shader"
{
	Properties
	{
		_BrightnessMultiplier("Brightness Multiplier", Float) = 2.2
		_CoreColor("Core Color", Color) = (0.6149418,0.9056604,0.2947668,0)
		_OuterColor("Outer Color", Color) = (0,0.1070013,1,0)
		_BaseColor("Base Color", Color) = (0,0.7069087,1,0)
		_NoiseScale("Noise Scale", Range(0 , 1)) = 0
		_FlameFlickerSpeed("Flame Flicker Speed", Range(0 , 1)) = 0
		_NoiseSample("Noise Sample", 2D) = "white" {}
		_WindStrength("Wind Strength", Range(0 , 10)) = 0
		_FlameBillboard("Flame Billboard", 2D) = "white" {}
		_FakeGlow("Fake Glow", Range(0 , 1)) = 0
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend One One
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _FlameBillboard_ST;
		uniform sampler2D _FlameBillboard;
		uniform float4 _CoreColor;
		uniform float4 _OuterColor;
		uniform float4 _BaseColor;
		uniform float _FakeGlow;
		uniform float _BrightnessMultiplier;

		uniform float4 _NoiseSample_ST;
		uniform sampler2D _NoiseSample;
		uniform float _FlameFlickerSpeed;
		uniform float _NoiseScale;
		uniform float _WindStrength;

		void vertexDataFunc(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = float3(0, 1, 0);
			float3 forwardCamVec = -normalize (UNITY_MATRIX_V._m20_m21_m22);
			float3 rightCamVec = normalize(UNITY_MATRIX_V._m00_m01_m02);
			float4x4 rotationCamMatrix = float4x4(rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1);
			v.normal = normalize(mul(float4(v.normal , 0), rotationCamMatrix)).xyz;
			v.vertex.x *= length(unity_ObjectToWorld._m00_m10_m20);
			v.vertex.y *= length(unity_ObjectToWorld._m01_m11_m21);
			v.vertex.z *= length(unity_ObjectToWorld._m02_m12_m22);
			v.vertex = mul(v.vertex, rotationCamMatrix);
			v.vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
			//Need to nullify rotation inserted by generated surface shader;
			v.vertex = mul(unity_WorldToObject, v.vertex);
		}

		inline half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4 (0, 0, 0, s.Alpha);
		}

		void surf(Input i , inout SurfaceOutput o)
		{
			float2 appendResult35 = (float2(-_FlameFlickerSpeed , -_FlameFlickerSpeed));
			float3 ase_worldPos = i.worldPos;
			float2 appendResult43 = (float2(ase_worldPos.x , ase_worldPos.y));
			float2 uv0_NoiseSample = i.uv_texcoord * _NoiseSample_ST.xy + _NoiseSample_ST.zw;
			float2 panner36 = (1.0 * _Time.y * appendResult35 + ((appendResult43 * 0.1) + (_NoiseScale * uv0_NoiseSample)));
			float4 tex2DNode4 = tex2D(_FlameBillboard, (i.uv_texcoord + (i.uv_texcoord.y * i.uv_texcoord.y * ((tex2D(_NoiseSample, panner36)).rg - float2(0,0)) * _WindStrength)));
			float2 uv_FlameBillboard = i.uv_texcoord * _FlameBillboard_ST.xy + _FlameBillboard_ST.zw;
			o.Emission = (_BrightnessMultiplier * ((tex2DNode4.r * _CoreColor) + (tex2DNode4.g * _OuterColor) + (tex2DNode4.b * _BaseColor) + (tex2D(_FlameBillboard, uv_FlameBillboard).a * _FakeGlow * _OuterColor))).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}