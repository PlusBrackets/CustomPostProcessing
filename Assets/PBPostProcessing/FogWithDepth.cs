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
        [Range(0f,1f)]
        public FloatParameter fogWeight = new FloatParameter { value = 0 };
        [Range(0f,3f)]
        public FloatParameter fogDensity = new FloatParameter { value = 0f };
        public FloatParameter fogStart = new FloatParameter { value = 0f };
        public FloatParameter fogEnd = new FloatParameter { value = 1f };
        public ColorParameter fogColor = new ColorParameter { value = Color.white };

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

            Camera camera = context.camera;

            float fov = camera.fieldOfView;
            float near = camera.nearClipPlane;
            float aspect = camera.aspect;

            float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

            Vector3 toRight = camera.transform.right * halfHeight * aspect;
            Vector3 toTop = camera.transform.up * halfHeight;
            Vector3 topLeft = camera.transform.forward * near + toTop - toRight;
            float scale = topLeft.magnitude / near;

            topLeft.Normalize();
            topLeft *= scale;

            Vector3 topRight = camera.transform.forward * near + toRight + toTop;
            topRight.Normalize();
            topRight *= scale;

            Vector3 bottomLeft = camera.transform.forward * near - toTop - toRight;
            bottomLeft.Normalize();
            bottomLeft *= scale;

            Vector3 bottomRight = camera.transform.forward * near + toRight - toTop;
            bottomRight.Normalize();
            bottomRight *= scale;

            Matrix4x4 frustumCorners = Matrix4x4.identity;
            frustumCorners.SetRow(0, bottomLeft);
            frustumCorners.SetRow(1, bottomRight);
            frustumCorners.SetRow(2, topRight);
            frustumCorners.SetRow(3, topLeft);
           
            PropertySheet sheet = context.propertySheets.Get(Shader.Find(Constants.Shaders.FogWithDepth));
            sheet.properties.SetMatrix(Constants.ShaderParams.FrustumCornersRay, frustumCorners);

            sheet.properties.SetFloat(Constants.ShaderParams.FogDensity, settings.fogDensity);
            sheet.properties.SetFloat(Constants.ShaderParams.FogStart, settings.fogStart);
            sheet.properties.SetFloat(Constants.ShaderParams.FogEnd, settings.fogEnd);
            sheet.properties.SetColor(Constants.ShaderParams.FogColor, settings.fogColor);

            sheet.properties.SetFloat(Constants.ShaderParams.Weight, settings.fogWeight);
            
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            cmd.EndSample("FogWithDepth");
        }
    }
}