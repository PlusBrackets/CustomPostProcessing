Shader "Hidden/PBCustom/DigitalGlitch"
{
	HLSLINCLUDE

		#include "PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_NoiseTex, sampler_NoiseTex);
		TEXTURE2D_SAMPLER2D(_TrashTex, sampler_TrashTex);
		
		half _Intensity;
		float4 _MulColor;
		float4 _AddColor;


		float4 FragGlitch(VaryingsDefault i) :SV_Target
		{
			float4 glitch = SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex,i.texcoord);

			float thresh = 1.0 - _Intensity;
			float w_d = step(thresh, pow(glitch.z, 2.5));// displacement glitch
			float w_f = step(thresh, pow(glitch.w, 2.5));// frame glitch
			float w_c = step(thresh, pow(glitch.z, 3.5));// color glitch

			float2 uv = frac(i.texcoord + glitch.xy * w_d);
			float4 sourceCol = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, uv);

			float4 trashCol = SAMPLE_TEXTURE2D(_TrashTex, sampler_TrashTex, uv);

			float3 color = lerp(sourceCol, trashCol, w_f).rgb;
			
			float3 neg = saturate((color.grb + (1 - dot(color, 1)) * 0.5)*_MulColor.rgb+_AddColor.rbg);
			color = lerp(color, neg, w_c);

			return float4(color, sourceCol.a);
		}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
				
				#pragma vertex VertDefault
				#pragma fragment FragGlitch

			ENDHLSL
		}
	}
}