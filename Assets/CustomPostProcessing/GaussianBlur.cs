using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace CustomPostProcessing
{
    [Serializable]
    [PostProcess(typeof(GaussianBlurRenderer), PostProcessEvent.AfterStack, "Custom/GaussianBlur")]
    public class GaussianBlur : PostProcessEffectSettings
    {
        [Range(1, 10)]
        public IntParameter iterations = new IntParameter { value = 3 };
        [Range(0f, 5f)]
        public FloatParameter spread = new FloatParameter { value = 0.5f };
        [Range(0f, 1f)]
        public FloatParameter weight = new FloatParameter { value = 1f };
        [Range(1f, 0.1f)]
        public FloatParameter sampleScale = new FloatParameter { value = 0.8f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value && iterations.value > 1 && weight.value > 0f && spread.value > 0;
        }

    }

    public sealed class GaussianBlurRenderer : PostProcessEffectRenderer<GaussianBlur>
    {
        private int m_Rt0;
        private int m_Rt1;

        public override void Init()
        {
            m_Rt0 = Shader.PropertyToID("_GaussianBlur0");
            m_Rt1 = Shader.PropertyToID("_GaussianBlur1");
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;

            cmd.BeginSample("GaussianBlurPass");
              
            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GaussianBlurEffect"));
            sheet.properties.SetFloat("_BlurWeight", settings.weight);

            float sampleScale = Mathf.Lerp(1, settings.sampleScale, settings.weight);
            int rtW = Mathf.FloorToInt(context.screenWidth * sampleScale);
            int rtH = Mathf.FloorToInt(context.screenHeight * sampleScale);

            context.GetScreenSpaceTemporaryRT(cmd, m_Rt0, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
            cmd.BlitFullscreenTriangle(context.source, m_Rt0);
            //for (int i = 0; i < settings.iterations; i++)
            //{
            //    sheet.properties.SetFloat("_BlurSize", 1.0f + i * settings.spread);
            //    context.GetScreenSpaceTemporaryRT(cmd, m_Rt1, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
            //    //vertical pass
            //    cmd.BlitFullscreenTriangle(m_Rt0, m_Rt1, sheet, 0);
            //    cmd.ReleaseTemporaryRT(m_Rt0);
            //    m_Rt0 = m_Rt1;

            //    context.GetScreenSpaceTemporaryRT(cmd, m_Rt1, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
            //    //horizonal pass
            //    cmd.BlitFullscreenTriangle(m_Rt0, m_Rt1, sheet, 1);
            //    cmd.ReleaseTemporaryRT(m_Rt0);
            //    m_Rt0 = m_Rt1;
            //}
            cmd.BlitFullscreenTriangle(m_Rt0, context.destination);
            cmd.ReleaseTemporaryRT(m_Rt0);
            cmd.EndSample("GaussianBlurPass");
        }
    }
}