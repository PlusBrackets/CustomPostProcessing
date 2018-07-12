#include "PostProcessing/Shaders/StdLib.hlsl"

static half kernel3[3] = { 0.4026, 0.2442, 0.0545 };

struct GB_UVSample5
{
    half2 uv[5];
};

GB_UVSample5 GB_SAMPLE_UV5(float2 uv, half2 sampleOffset)
{
    GB_UVSample5 s;
    s.uv[0] = uv;
    s.uv[1] = uv + sampleOffset;
    s.uv[2] = uv - sampleOffset;
    s.uv[3] = uv + sampleOffset * 2.0;
    s.uv[4] = uv - sampleOffset * 2.0;
    return s;
}
 
float4 GB_SUM_COLOR5(TEXTURE2D_ARGS(tex, samplerTex), GB_UVSample5 s)
{
    float4 col = SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[0])) * kernel3[0];
    for (int it = 1; it < 3; it++) {
		col += SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[it * 2 - 1])) * kernel3[it];
		col += SAMPLE_TEXTURE2D(tex, samplerTex, UnityStereoTransformScreenSpaceTex(s.uv[it * 2])) * kernel3[it];
	}
    return col;
}

float4 GAUSSIAN_BLUR_SAMPLE5(TEXTURE2D_ARGS( tex, samplerTex),float2 uv, half2 sampleOffset)
{
    GB_UVSample5 s = GB_SAMPLE_UV5(uv, sampleOffset);
    return GB_SUM_COLOR5(TEXTURE2D_PARAM(tex, samplerTex),s);

}