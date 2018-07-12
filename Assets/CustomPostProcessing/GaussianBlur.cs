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
        [Range(0f, 1f)]
        public FloatParameter weight = new FloatParameter { value = 1f };
        [Range(1, 10)]
        public IntParameter iterations = new IntParameter { value = 3 };
        [Range(0.02f, 5f)]
        public FloatParameter spread = new FloatParameter { value = 0.5f };
        [Range(1f, 0.1f)]
        public FloatParameter sampleScale = new FloatParameter { value = 0.8f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value && iterations.value > 0 && weight.value > 0f && spread.value > 0;
        }

    }

    public sealed class GaussianBlurRenderer : PostProcessEffectRenderer<GaussianBlur>
    {
        struct PassRT
        {
            internal int v;
            internal int h;
        }

        private PassRT[] m_PassRT = new PassRT[10];

        public override void Init()
        {
            for (int i = 0; i < 10; i++)
            {
                m_PassRT[i].v = Shader.PropertyToID("_GaussianBlurV" + i);
                m_PassRT[i].h = Shader.PropertyToID("_GaussianBlurH" + i);
            }
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;

            cmd.BeginSample("GaussianBlurPass");

            PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GaussianBlurEffect"));
            sheet.properties.SetFloat("_BlurWeight", settings.weight);

            float sampleScale = Mathf.Lerp(1, settings.sampleScale, settings.weight);
            float spread = Mathf.Lerp(0.02f, settings.spread, settings.weight);

            int rtW = Mathf.FloorToInt(context.screenWidth * sampleScale);
            int rtH = Mathf.FloorToInt(context.screenHeight * sampleScale);

            var lastRT = context.source;

            for (int i = 0; i < Mathf.Min(settings.iterations,m_PassRT.Length); i++)
            {
                sheet.properties.SetFloat("_BlurSize", 1.0f + i * spread);

                PassRT passRT = m_PassRT[i];               

                context.GetScreenSpaceTemporaryRT(cmd, passRT.v, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
                //vertical pass
                cmd.BlitFullscreenTriangle(lastRT, passRT.v, sheet, 0);
                lastRT = passRT.v;

                context.GetScreenSpaceTemporaryRT(cmd, passRT.h, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
                //horizonal pass
                cmd.BlitFullscreenTriangle(lastRT, passRT.h, sheet, 1);
                lastRT = passRT.h;
            }
            cmd.BlitFullscreenTriangle( lastRT, context.destination);
            for(int i = 0; i < Mathf.Min(settings.iterations, m_PassRT.Length); i++)
            {
                cmd.ReleaseTemporaryRT(m_PassRT[i].v);
                cmd.ReleaseTemporaryRT(m_PassRT[i].h);
            }
            cmd.EndSample("GaussianBlurPass");
        }
    }
}