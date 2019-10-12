using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

public class DepthNormalsFeature : ScriptableRendererFeature
{
    private DepthNormalsPass depthNormalsPass;

    RenderTargetHandle m_DepthNormalsTexture;

    public override void Create()
    {
        if (depthNormalsPass == null)
            depthNormalsPass =
                new DepthNormalsPass(RenderPassEvent.BeforeRenderingPrepasses, RenderQueueRange.opaque, -1);

        m_DepthNormalsTexture.Init("_CameraDepthNormalsTexture");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, m_DepthNormalsTexture);
        renderer.EnqueuePass(depthNormalsPass);
    }

    void OnValidate()
    {
        depthNormalsPass?.SetLayerMask(-1);
    }
}