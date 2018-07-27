using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PB_PostProcessing
{
    [Serializable]
    [PostProcess(typeof(AnalogGlitchRenderer), PostProcessEvent.AfterStack, "PBCustom/Analog Glitch")]
    public class AnalogGlitch : PostProcessEffectSettings
    {
        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        [Range(0f, 1f)]
        public FloatParameter scanLineJitter = new FloatParameter { value = 0f };

        [Range(0f, 1f)]
        public FloatParameter verticalJump = new FloatParameter { value = 0f };

        [Range(0f, 1f)]
        public FloatParameter horizontalShake = new FloatParameter { value = 0f };

        [Range(0f, 1f)]
        public FloatParameter colorDrift = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return base.IsEnabledAndSupported(context) && intensity > 0;
        }

    }

    public sealed class AnalogGlitchRenderer : PostProcessEffectRenderer<AnalogGlitch>
    {
        float _verticalJumpTime = 0;

        public override void Init()
        {
            base.Init();
            _verticalJumpTime = 0;
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;
            PropertySheet sheet = context.propertySheets.Get(Shader.Find(Constants.Shaders.AnalogGlitch));

            float scanLineJitter = settings.scanLineJitter * settings.intensity;
            float verticalJump = settings.verticalJump * settings.intensity;
            float horizontalShake = settings.horizontalShake * settings.intensity;
            float colorDrift = settings.colorDrift * settings.intensity;

            _verticalJumpTime += Time.deltaTime * verticalJump * 11.3f;

            float sl_thresh = Mathf.Clamp01(1.0f - scanLineJitter * 1.2f);
            float sl_disp = 0.002f + Mathf.Pow(scanLineJitter, 3) * 0.05f;
            sheet.properties.SetVector(Constants.ShaderParams.ScanLineJitter, new Vector2(sl_disp, sl_thresh));

            Vector2 vj = new Vector2(verticalJump, _verticalJumpTime);
            sheet.properties.SetVector(Constants.ShaderParams.VerticalJump, vj);

            sheet.properties.SetFloat(Constants.ShaderParams.HorizontalShake, horizontalShake * 0.2f);

            Vector2 cd = new Vector2(colorDrift * 0.04f, Time.time * 606.11f);
            sheet.properties.SetVector(Constants.ShaderParams.ColorDrift, cd);

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}