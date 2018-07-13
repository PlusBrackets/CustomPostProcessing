Shader "Hidden/PBCustom/FogWithDepth"
{
	HLSLINCLUDE

		#include "PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_CameraDepthTexture,sampler_CameraDepthTexture);

		float4 FragFog(VaryingsDefault i) :SV_Target
		{
			 float4 col = 1;
			 float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,sampler_CameraDepthTexture,i.texcoord);
			 return col*depth;
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
