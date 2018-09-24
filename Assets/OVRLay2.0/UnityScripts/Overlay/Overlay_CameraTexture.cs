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
    public bool autoDisableCamera = true;
    public bool dontForceRenderTexture = false;


    private RenderTexture targetTex;
    private Overlay_Unity u_overlay;
    private Overlay_Texture u_oTex;
    private OVRLay.OVRLay overlay;

    void Start()
    {
        targetTex = new RenderTexture(targetCamera.pixelWidth, targetCamera.pixelHeight, 24);
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