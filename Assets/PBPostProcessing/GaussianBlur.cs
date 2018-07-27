using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PB_PostProcessing
{
    [Serializable]
    [PostProcess(typeof(GaussianBlurRenderer), PostProcessEvent.AfterStack, "PBCustom/GaussianBlur")]
    public class GaussianBlur : PostProcessEffectSettings
    {
        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter { value = 0f };
        [Range(1, 10)]
        public IntParameter iterations = new IntParameter { value = 3 };
        [Range(0.02f, 5f)]
        public FloatParameter spread = new FloatParameter { value = 0.5f };
        [Range(1f, 0.1f)]
        public FloatParameter sampleScale = new FloatParameter { value = 0.8f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value && iterations.value > 0 && intensity.value > 0f && spread.value > 0;
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

            cmd.BeginSample("GaussianBlur");

            PropertySheet sheet = context.propertySheets.Get(Shader.Find(Constants.Shaders.GaussianBlur));
            sheet.properties.SetFloat(Constants.ShaderParams.Intensity, settings.intensity);

            float sampleScale = Mathf.Lerp(1, settings.sampleScale, settings.intensity);
            float spread = Mathf.Lerp(0.02f, settings.spread, settings.intensity);

            int rtW = Mathf.FloorToInt(context.screenWidth * sampleScale);
            int rtH = Mathf.FloorToInt(context.screenHeight * sampleScale);

            var lastRT = context.source;

            for (int i = 0; i < Mathf.Min(settings.iterations,m_PassRT.Length); i++)
            {
                sheet.properties.SetFloat(Constants.ShaderParams.BlurSize, 1.0f + i * spread);

                PassRT passRT = m_PassRT[i];
                //释放前一张竖向取样rt
                if (i > 0)
                {
                    cmd.ReleaseTemporaryRT(m_PassRT[i - 1].v);
                }
                context.GetScreenSpaceTemporaryRT(cmd, passRT.v, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
                //vertical pass
                cmd.BlitFullscreenTriangle(lastRT, passRT.v, sheet, 0);
                lastRT = passRT.v;
                //释放前一张横向取样rt
                if (i > 0)
                {
                    cmd.ReleaseTemporaryRT(m_PassRT[i - 1].h);
                }
                context.GetScreenSpaceTemporaryRT(cmd, passRT.h, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, rtW, rtH);
                //horizonal pass
                cmd.BlitFullscreenTriangle(lastRT, passRT.h, sheet, 1);
                lastRT = passRT.h;
            }
            cmd.BlitFullscreenTriangle( lastRT, context.destination);
            int lastPassIndex = Mathf.Min(settings.iterations, m_PassRT.Length)-1;
            if (lastPassIndex > 0)
            {
                cmd.ReleaseTemporaryRT(m_PassRT[lastPassIndex].v);
                cmd.ReleaseTemporaryRT(m_PassRT[lastPassIndex].h);
            }
            cmd.EndSample("GaussianBlur");
        }
    }
}