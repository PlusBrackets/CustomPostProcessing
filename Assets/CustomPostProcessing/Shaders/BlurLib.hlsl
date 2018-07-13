#include "PostProcessing/Shaders/StdLib.hlsl"

#define GAUSSIAN_KERNEL3  { 0.4026, 0.2442, 0.0545 }

struct SampleUV5
{
    half2 uv[5];
};

SampleUV5 GAUSSIAN_UV5(float2 uv, half2 sampleOffset)
{
    SampleUV5 s;
    s.uv[0] = uv;
    s.uv[1] = uv + sampleOffset;
    s.uv[2] = uv - sampleOffset;
    s.uv[3] = uv + sampleOffset * 2.0;
    s.uv[4] = uv - sampleOffset * 2.0;
    return s;
}
 
float4 GAUSSIAN_COLOR5(TEXTURE2D_ARGS(tex, samplerTex), SampleUV5 s)
{
    half kernel3[3] = GAUSSIAN_KERNEL3;
    float4 col = SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[0])) * kernel3[0];
    for (int it = 1; it < 3; it++) {
		col += SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[it * 2 - 1])) * kernel3[it];
		col += SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[it * 2])) * kernel3[it];
	}
    return col;
}

float4 GAUSSIAN_SAMPLE5(TEXTURE2D_ARGS( tex, samplerTex),float2 uv, half2 sampleOffset)
{
    SampleUV5 s = GAUSSIAN_UV5(uv, sampleOffset);
    return GAUSSIAN_COLOR5(TEXTURE2D_PARAM(tex, samplerTex),s);

}