using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Texture : MonoBehaviour
{

    [Header("Overlay Texture")]
    public Texture currentTexture;

    private Overlay_Unity u_overlay;
    private OVRLay.OVRLay overlay;

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();

            if (u_overlay.overlay != null)
                overlay = u_overlay.overlay;

            return;
        }

        if (overlay.Created && currentTexture != null)
        {
            if (overlay.TextureType != ETextureType.DirectX)
                overlay.TextureType = ETextureType.DirectX;

            overlay.TextureBounds = Overlay_Unity.TextureBounds;
            overlay.Texture = currentTexture;
        }
    }
}