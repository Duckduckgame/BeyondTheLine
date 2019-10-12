using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;


public class DepthNormalsPass : ScriptableRenderPass
{
    int kDepthBufferBits = 32;
    private RenderTargetHandle depthAttachmentHandle { get; set; }
    internal RenderTextureDescriptor descriptor { get; private set; }

    private FilteringSettings m_FilteringSettings;
    string m_ProfilerTag = "DepthNormals Prepass";
    ShaderTagId m_ShaderTagId = new ShaderTagId("DepthOnly");

    private Material depthNormalsMaterial = null;

    public DepthNormalsPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
    {
        m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        renderPassEvent = evt;
    }


    public void Setup(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle)
    {
        this.depthAttachmentHandle = depthAttachmentHandle;
        baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
        baseDescriptor.depthBufferBits = kDepthBufferBits;
        descriptor = baseDescriptor;

        depthNormalsMaterial =
            CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
    }


    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cmd.GetTemporaryRT(depthAttachmentHandle.id, descriptor, FilterMode.Point);
        ConfigureTarget(depthAttachmentHandle.Identifier());
        ConfigureClear(ClearFlag.All, Color.black);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

        using (new ProfilingSample(cmd, m_ProfilerTag))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortFlags);
            drawSettings.perObjectData = PerObjectData.None;


            ref CameraData cameraData = ref renderingData.cameraData;
            Camera camera = cameraData.camera;
            if (cameraData.isStereoEnabled)
                context.StartMultiEye(camera);


            drawSettings.overrideMaterial = depthNormalsMaterial;

            context.DrawRenderers(renderingData.cullResults, ref drawSettings,
                ref m_FilteringSettings);

            cmd.SetGlobalTexture("_CameraDepthNormalsTexture", depthAttachmentHandle.id);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void SetLayerMask(LayerMask mask)
    {
        m_FilteringSettings.layerMask = mask;
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (cmd == null)
            throw new ArgumentNullException("cmd");

        if (depthAttachmentHandle != RenderTargetHandle.CameraTarget)
        {
            cmd.ReleaseTemporaryRT(depthAttachmentHandle.id);
            depthAttachmentHandle = RenderTargetHandle.CameraTarget;
        }
    }
}