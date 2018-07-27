using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace PB_PostProcessing
{
    [Serializable]
    [PostProcess(typeof(DigitalGlitchRenderer), PostProcessEvent.AfterStack, "PBCustom/Digital Glitch")]
    public class DigitalGlitch : PostProcessEffectSettings
    {
        [Range(0, 1)]
        public FloatParameter intenstiy = new FloatParameter { value = 0f };
        [Range(0, 1)]
        public FloatParameter noise = new FloatParameter { value = 0.5f };
        [Range(0, 1)]
        public FloatParameter noiseRate = new FloatParameter { value = 0.2f };
        public IntParameter trashFrameCount1 = new IntParameter { value = 13 };
        public IntParameter trashFrameCount2 = new IntParameter { value = 73 };
        public ColorParameter mulColor = new ColorParameter { value = Color.white };
        public ColorParameter addColor = new ColorParameter { value = Color.black };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return base.IsEnabledAndSupported(context) && intenstiy > 0&&Application.isPlaying;
        }
    }

    public sealed class DigitalGlitchRenderer : PostProcessEffectRenderer<DigitalGlitch>
    {
        private Texture2D m_NoiseTexture;
        private Texture2D noiseTexture
        {
            get
            {
                if (m_NoiseTexture == null)
                {
                    m_NoiseTexture = new Texture2D(64, 64, TextureFormat.ARGB32, false);
                    m_NoiseTexture.wrapMode = TextureWrapMode.Clamp;
                    m_NoiseTexture.filterMode = FilterMode.Point;
                }
                return m_NoiseTexture;
            }
        }

        private RenderTexture m_TrashFrame1;
        private RenderTexture trashFrame1
        {
            get
            {
                if (m_TrashFrame1 == null)
                {
                    m_TrashFrame1 = new RenderTexture(Screen.width, Screen.height, 0);
                }
                return m_TrashFrame1;
            }
        }

        private RenderTexture m_TrashFrame2;
        private RenderTexture trashFrame2
        {
            get
            {
                if (m_TrashFrame2 == null)
                {
                    m_TrashFrame2 = new RenderTexture(Screen.width, Screen.height, 0);
                }
                return m_TrashFrame2;
            }
        }

        private Color RandomColor()
        {
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        private float RandomValue()
        {
            return UnityEngine.Random.value;
        }

        void UpdateNoiseTexture()
        {
            var color = RandomColor();

            for (var y = 0; y < noiseTexture.height; y++)
            {
                for (var x = 0; x < noiseTexture.width; x++)
                {
                    if (RandomValue() > settings.noise)
                        color = RandomColor();
                    noiseTexture.SetPixel(x, y, color);
                }
            }

            noiseTexture.Apply();
#if UNITY_EDITOR
            if (TextureDebug.Ins != null)
                TextureDebug.Ins.target = noiseTexture;
#endif
        }

        public override void Init()
        {
            base.Init();
            UpdateNoiseTexture();
        }

        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer cmd = context.command;

            cmd.BeginSample("DigitalGlitch");

            if (RandomValue() > Mathf.Lerp(1f, 0.5f, settings.noiseRate))
            {
                UpdateNoiseTexture();
            }

            int fcount = Time.frameCount;
            if (fcount % settings.trashFrameCount1 == 0) cmd.BlitFullscreenTriangle(context.source, trashFrame1);
            if (fcount % settings.trashFrameCount2 == 0) cmd.BlitFullscreenTriangle(context.source, trashFrame2);

            PropertySheet sheet = context.propertySheets.Get(Shader.Find(Constants.Shaders.DigitalGlitch));

            sheet.properties.SetFloat(Constants.ShaderParams.Intensity, settings.intenstiy);
            sheet.properties.SetColor(Constants.ShaderParams.MulColor, settings.mulColor);
            sheet.properties.SetColor(Constants.ShaderParams.AddColor, settings.addColor);
            sheet.properties.SetTexture(Constants.ShaderParams.NoiseTex, noiseTexture);
            RenderTexture trashFrame = RandomValue() > 0.5f ? trashFrame1 : trashFrame2;
            sheet.properties.SetTexture(Constants.ShaderParams.TrashTex, trashFrame);

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            cmd.EndSample("DigitalGlitch");

        }

        public override void Release()
        {
            base.Release();
            if (m_TrashFrame1 != null)
                m_TrashFrame1.Release();
            if (m_TrashFrame2 != null)
                m_TrashFrame2.Release();
            m_TrashFrame1 = null;
            m_TrashFrame2 = null;
        }
    }
}