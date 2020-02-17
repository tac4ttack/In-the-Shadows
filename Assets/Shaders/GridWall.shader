Shader "Custom/Grid Wall"
{
	Properties
	{
		_GridTexture("Grid Texture", 2D) = "white" {}
		_GridColor("Grid Color", Color) = (0.6690548,0.8737041,0.9150943,1)
		_BaseColor("Base Color", Color) = (0.9150943,0.8997345,0.7899163,0)
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 

		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _GridTexture;
		uniform float4 _GridTexture_ST;
		uniform float4 _GridColor;
		uniform float4 _BaseColor;

		void surf(Input i , inout SurfaceOutputStandard o)
		{
			float2 uv_GridTexture = i.uv_texcoord * _GridTexture_ST.xy + _GridTexture_ST.zw;
			o.Albedo = ((tex2D(_GridTexture, uv_GridTexture).a * _GridColor) + _BaseColor).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}