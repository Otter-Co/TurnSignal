using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
[RequireComponent(typeof(Overlay_Texture))]
public class Overlay_CameraTexture : MonoBehaviour
{
    [Space(10)]
    public Camera targetCamera;
    public int antialiasing = 8;
    public FilterMode filterMode = FilterMode.Trilinear;
    [Space(10)]
    public int textureWidthOverride = 0;
    public int textureHeightOverride = 0;

    private RenderTexture targetTex;
    private Overlay_Unity u_overlay;
    private Overlay_Texture u_oTex;
    private OVRLay.OVRLay overlay;

    void Start()
    {
        int width = (textureWidthOverride > 0) ? textureWidthOverride : targetCamera.pixelWidth;
        int height = (textureHeightOverride > 0) ? textureHeightOverride : targetCamera.pixelHeight;

        targetTex = new RenderTexture(width, height, 24);

        targetTex.antiAliasing = antialiasing;
        targetTex.filterMode = filterMode;

        targetCamera.targetTexture = targetTex;
        targetCamera.enabled = false;
    }

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();
            u_oTex = GetComponent<Overlay_Texture>();
            
            overlay = u_overlay.overlay;
            u_oTex.currentTexture = targetTex;

            return;
        }

        if (overlay.Created)
        {
            if (targetCamera.targetTexture != targetTex)
                targetCamera.targetTexture = targetTex;

            targetCamera.Render();
        }
    }
}