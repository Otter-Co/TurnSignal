using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Dashboard_Icon : MonoBehaviour
{
    [Header("Overlay Texture")]
    public Texture currentTexture;
    private OVRLay.OVRLay overlay;
    
    void Update()
    {
        if (overlay == null)
            if ((overlay = GetComponent<Overlay_Unity>()?.overlay) == null)
                return;

        if (overlay.Ready)
        {
            if (overlay.IconTextureType != ETextureType.DirectX)
                overlay.IconTextureType = ETextureType.DirectX;

            overlay.IconTextureBounds = Overlay_Unity.TextureBounds;
            overlay.IconTexture = currentTexture;
        }
    }
}
