
namespace PB_PostProcessing
{
    public static class Constants
    {
        public static class Shaders
        {
            public const string GaussianBlur = "Hidden/PBCustom/GaussianBlur";
            public const string FogWithDepth = "Hidden/PBCustom/FogWithDepth";
            public const string DigitalGlitch = "Hidden/PBCustom/DigitalGlitch";
            public const string AnalogGlitch = "Hidden/PBCustom/AnalogGlitch";
        }

        public static class ShaderParams
        {
            public const string Intensity = "_Intensity";
            //blur
            public const string BlurSize = "_BlurSize";
            //fog
            public const string FrustumCornersRay = "_FrustumCornersRay";
            public const string FogDensity = "_FogDensity";
            public const string FogStart = "_FogStart";
            public const string FogEnd = "_FogEnd";
            public const string FogColor = "_FogColor";
            //Glitch
            public const string MulColor = "_MulColor";
            public const string AddColor = "_AddColor";
            public const string NoiseTex = "_NoiseTex";
            public const string TrashTex = "_TrashTex";

            public const string ScanLineJitter = "_ScanLineJitter";
            public const string VerticalJump = "_VerticalJump";
            public const string HorizontalShake = "_HorizontalShake";
            public const string ColorDrift = "_ColorDrift";
        }

    }
}