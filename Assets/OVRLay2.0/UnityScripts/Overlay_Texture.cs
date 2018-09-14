using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Texture : MonoBehaviour
{

    [Header("Overlay Texture")]
    public Texture currentTexture;
    [Space(10)]
    [Header("Overlay Texture Settings")]
    public bool isDashboardIcon = false;

    private Overlay_Unity u_overlay;
    private OVRLay.OVRLay overlay;

    void Start()
    {
        u_overlay = GetComponent<Overlay_Unity>();
        overlay = u_overlay.overlay;
    }

    void Update()
    {
        if (overlay.Created)
        {
            if (overlay.TextureType != ETextureType.DirectX)
                overlay.TextureType = ETextureType.DirectX;

            if (!overlay.TextureBounds.Equals(Overlay_Unity.TextureBounds))
                overlay.TextureBounds = Overlay_Unity.TextureBounds;

            overlay.Texture = currentTexture;
        }
    }
}