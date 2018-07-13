using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PB_PostProcessing
{
    [Serializable]
    [PostProcess(typeof(FogWithDepthRenderer),PostProcessEvent.BeforeStack,"PBCustom/FogWithDepth")]
    public class FogWithDepth : PostProcessEffectSettings
    {
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value && RuntimeUtilities.supportsDepthNormals;
        }
    }

    public sealed class FogWithDepthRenderer : PostProcessEffectRenderer<FogWithDepth>
    {
        public override void Init()
        {
            base.Init();
            
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;

            cmd.BeginSample("FogWithDepth");

            PropertySheet sheet = context.propertySheets.Get(Shader.Find(Constants.Shaders.FogWithDepth));

            
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            cmd.EndSample("FogWithDepth");
        }
    }
}