
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
            public const string BlurWeight = "_BlurWeight";
            public const string BlurSize = "_BlurSize";

        }

    }
}