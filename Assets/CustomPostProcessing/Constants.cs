
namespace PB_PostProcessing
{
    public static class Constants
    {
        public static class Shaders
        {
            public const string GaussianBlur = "Hidden/PBCustom/GaussianBlur";
            public const string FogWithDepth = "Hidden/PBCustom/FogWithDepth";
        }

        public static class ShaderParams
        {
            public const string Weight = "_Weight";
            public const string BlurSize = "_BlurSize";
            public const string FrustumCornersRay = "_FrustumCornersRay";
            public const string FogDensity = "_FogDensity";
            public const string FogStart = "_FogStart";
            public const string FogEnd = "_FogEnd";
            public const string FogColor = "_FogColor";
        }

    }
}