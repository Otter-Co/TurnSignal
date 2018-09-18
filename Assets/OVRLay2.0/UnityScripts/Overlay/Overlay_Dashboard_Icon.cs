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
            overlay = u_overlay.overlay;
            return;
        }

        if (overlay.Created)
        {
            if (overlay.TextureType != ETextureType.DirectX)
                overlay.TextureType = ETextureType.DirectX;

            if (overlay.TextureBounds.Equals(Overlay_Unity.TextureBounds))
                overlay.TextureBounds = Overlay_Unity.TextureBounds;

            overlay.Texture = currentTexture;
        }
    }
}
