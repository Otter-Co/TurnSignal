using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Texture : MonoBehaviour
{
    [Header("Overlay Texture")]
    public Texture currentTexture;

    private OVRLay.OVRLay overlay;

    void Update()
    {
        if (overlay == null)
            if ((overlay = GetComponent<Overlay_Unity>()?.overlay) == null)
                return;

        if (overlay.Ready && currentTexture != null)
        {
            if (overlay.TextureType != ETextureType.DirectX)
                overlay.TextureType = ETextureType.DirectX;

            overlay.TextureBounds = Overlay_Unity.TextureBounds;
            overlay.Texture = currentTexture;
        }
    }
}