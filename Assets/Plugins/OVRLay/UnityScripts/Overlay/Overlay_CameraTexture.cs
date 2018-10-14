using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class Overlay_CameraTexture : MonoBehaviour
{
    [Header("Overlay for Camera")]
    public Overlay_Unity targetOverlay;

    [Header("Overlay Texture Settings")]
    public bool forceCameraRenderTexture = false;
    public int cameraWidthOverride = 0;
    public int cameraHeightOverride = 0;
    public int cameraAAOveride = 8;

    private OVRLay.OVRLay overlay;
    private RenderTexture renderTex;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cameraWidthOverride != 0 && cameraHeightOverride != 0)
        {
            renderTex = new RenderTexture(cameraWidthOverride, cameraHeightOverride, 24);
            renderTex.antiAliasing = cameraAAOveride;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (renderTex == null)
            renderTex = new RenderTexture(source.descriptor);

        if (forceCameraRenderTexture && cam.targetTexture != renderTex)
            cam.targetTexture = renderTex;
        else
        {
            RenderTexture.active = renderTex;
            Graphics.Blit(source, renderTex);
            RenderTexture.active = dest;
            Graphics.Blit(source, dest);
        }

        if (overlay == null)
        {
            if (targetOverlay.overlay != null)
                overlay = targetOverlay.overlay;
        }
        else
        {
            if (overlay.TextureType != ETextureType.DirectX)
                overlay.TextureType = ETextureType.DirectX;

            overlay.TextureBounds = Overlay_Unity.TextureBounds;

            if (forceCameraRenderTexture)
                overlay.Texture = source;
            else
                overlay.Texture = renderTex;
        }
    }
}