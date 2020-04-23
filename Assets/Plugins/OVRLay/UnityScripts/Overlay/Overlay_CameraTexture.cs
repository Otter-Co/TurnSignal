using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Camera))]
public class Overlay_CameraTexture : MonoBehaviour
{
    [Header("Overlay for Camera")]
    public Overlay_Unity targetOverlay;
    private OVRLay.OVRLay overlay;

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (overlay == null)
        {
            if ((overlay = targetOverlay?.overlay) == null)
                return;

            overlay.TextureType = ETextureType.DirectX;
        }
        else if (overlay.Ready)
        {
            overlay.TextureBounds = Overlay_Unity.TextureBounds;
            overlay.Texture = source;
        }


        Graphics.Blit(source, dest);
    }
}