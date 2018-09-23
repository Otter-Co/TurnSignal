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
    public float cameraPixelSizeMultiplier = 1.0f;
    [Space(10)]
    public bool autoDisableCamera = true;
    public bool dontForceRenderTexture = false;
    [Space(10)]

    public int pwidth = 0;
    public int pheight = 0;


    private RenderTexture targetTex;
    private Overlay_Unity u_overlay;
    private Overlay_Texture u_oTex;
    private OVRLay.OVRLay overlay;

    void Start()
    {
        int width = pwidth = (int)(targetCamera.pixelWidth * cameraPixelSizeMultiplier);
        int height = pheight = (int)(targetCamera.pixelHeight * cameraPixelSizeMultiplier);

        targetTex = new RenderTexture(width, height, 24);

        targetTex.antiAliasing = antialiasing;
        targetTex.filterMode = filterMode;

        if (!dontForceRenderTexture)
            targetCamera.targetTexture = targetTex;

        if (autoDisableCamera)
            targetCamera.enabled = false;
    }

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();
            u_oTex = GetComponent<Overlay_Texture>();

            if (u_overlay.overlay != null)
                overlay = u_overlay.overlay;

            u_oTex.currentTexture = targetTex;

            return;
        }

        if (overlay.Created)
        {
            var oldTex = targetCamera.targetTexture;

            if (targetCamera.targetTexture != targetTex)
                targetCamera.targetTexture = targetTex;

            targetCamera.Render();

            if (dontForceRenderTexture)
                targetCamera.targetTexture = oldTex;

            if (autoDisableCamera)
                targetCamera.enabled = false;
        }
    }
}