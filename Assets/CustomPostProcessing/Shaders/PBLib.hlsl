#include "PostProcessing/Shaders/StdLib.hlsl"

#define GAUSSIAN_KERNEL3  { 0.4026, 0.2442, 0.0545 }

struct KernelUV5
{
    half2 uv[5];
};

KernelUV5 GetKernelUV5(float2 uv, half2 sampleOffset)
{
    KernelUV5 s;
    s.uv[0] = uv;
    s.uv[1] = uv + sampleOffset;
    s.uv[2] = uv - sampleOffset;
    s.uv[3] = uv + sampleOffset * 2.0;
    s.uv[4] = uv - sampleOffset * 2.0;
    return s;
}
 
float4 GetGaussianBlurColor5(TEXTURE2D_ARGS(tex, samplerTex), KernelUV5 s)
{
    half kernel3[3] = GAUSSIAN_KERNEL3;
    float4 col = SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[0])) * kernel3[0];
    for (int it = 1; it < 3; it++) {
		col += SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[it * 2 - 1])) * kernel3[it];
		col += SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[it * 2])) * kernel3[it];
	}
    return col;
}

float4 SampleGaussianBlur5(TEXTURE2D_ARGS( tex, samplerTex),float2 uv, half2 sampleOffset)
{
    KernelUV5 s = GetKernelUV5(uv, sampleOffset);
    return GetGaussianBlurColor5(TEXTURE2D_PARAM(tex, samplerTex),s);
}

//corner { 0: bottomLeft,1: bottomRight,2:topRight,3:topLeft }
float4 LerpCorner(float4x4 corner, float2 i)
{
    float4 l1 = lerp(corner[0], corner[1], i.x);
    float4 l2 = lerp(corner[3], corner[2], i.x);
    return lerp(l1, l2, i.y);
}

//frustumCornersRay { 0: bottomLeft,1: bottomRight,2:topRight,3:topLeft }
float3 GetWorldPos(float linearDepth,float2 uv, float4x4 frustumCornersRay)
{
    float3 interpolatedRay = LerpCorner(frustumCornersRay,uv).xyz;
    return _WorldSpaceCameraPos + linearDepth * interpolatedRay.xyz;
}

//frustumCornersRay { 0: bottomLeft,1: bottomRight,2:topRight,3:topLeft }
float3 GetWorldPos(TEXTURE2D_ARGS( _dTex, samplerDTex),float2 uv,float4x4 frustumCornersRay)
{
    float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_dTex, samplerDTex, uv));
    return GetWorldPos(linearDepth,uv,frustumCornersRay);
}