Shader "Hidden/PBCustom/GaussianBlur"
{
	HLSLINCLUDE

		#include "PostProcessing/Shaders/StdLib.hlsl"
		#include "PBLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		float4 _MainTex_TexelSize;
		float _BlurSize;
		float _Intensity;


		float4 FragVertical(VaryingsDefault i) :SV_Target
		{
			half2 dir = half2(0.0, _MainTex_TexelSize.y);
			float4 col = SampleGaussianBlur5(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), i.texcoord, dir * _BlurSize);
			col = _Intensity * col + (1 - _Intensity) * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
			return col;
		}

		float4 FragHorizontal(VaryingsDefault i) :SV_Target
		{
			half2 dir = half2(_MainTex_TexelSize.x,0.0);
			float4 col = SampleGaussianBlur5(TEXTURE2D_PARAM(_MainTex, sampler_MainTex), i.texcoord, dir * _BlurSize);
			col = _Intensity * col + (1 - _Intensity) * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
			return col;
		}

	ENDHLSL

	SubShader 
	{
		Cull Off ZWrite Off ZTest Always
		//0: Blur Vertical
		Pass
		{	
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment FragVertical

			ENDHLSL
		}
		//1: Blur Horizontal
		Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment FragHorizontal

			ENDHLSL
		}
	}
}
