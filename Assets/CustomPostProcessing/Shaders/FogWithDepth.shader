Shader "Hidden/PBCustom/FogWithDepth"
{
	HLSLINCLUDE

		#include "PostProcessing/Shaders/StdLib.hlsl"
		#include "PBLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_CameraDepthTexture,sampler_CameraDepthTexture);
		
		float4x4 _FrustumCornersRay;
		float4 _MainTex_TexelSize;

		half _FogDensity;
		float4 _FogColor;
		float _FogStart;
		float _FogEnd; 
		half _Weight;

		float4 FragFog(VaryingsDefault i) :SV_Target
		{
			float3 worldPos = GetWorldPos(TEXTURE2D_PARAM(_CameraDepthTexture, sampler_CameraDepthTexture), i.texcoord, _FrustumCornersRay);

			float fogDensity = (_FogEnd - worldPos.y) / (_FogEnd - _FogStart);
			fogDensity = saturate(fogDensity*_FogDensity)*_Weight;
			
			float4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
			finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogDensity);

			return finalColor;
		}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest ON

		Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment FragFog

			ENDHLSL
		}
	}
}
