using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Overlay_Dashboard_Icon : MonoBehaviour
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

        if (overlay.Created)
        {
            if (overlay.IconTextureType != ETextureType.DirectX)
                overlay.IconTextureType = ETextureType.DirectX;

            overlay.IconTextureBounds = Overlay_Unity.TextureBounds;
            overlay.IconTexture = currentTexture;
        }
    }
}
