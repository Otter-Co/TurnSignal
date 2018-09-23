using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class Overlay_Unity : MonoBehaviour
{
    public static readonly VRTextureBounds_t TextureBounds =
        new VRTextureBounds_t() { uMin = 0, vMin = 1, uMax = 1, vMax = 0 };

    [System.Serializable]
    public struct Overlay_Unity_Settings
    {
        public float WidthInMeters;
        public Color Color;
        public float Alpha;
        public float TexelAspect;
        public bool Visible;
        public bool HighQuality;
    }

    [HideInInspector] public OVRLay.OVRLay overlay;

    [Header("Overlay Creation Settings")]
    public bool createWhenReady = true;
    public bool overlayCreated = false;
    [Header("Overlay Setup Info")]
    public string overlayName = "Unity Overlay";
    public string overlayKey = "unity-overlay";
    public bool isDashboardOverlay = false;
    [Header("Overlay General Settings")]
    public Overlay_Unity_Settings settings = new Overlay_Unity_Settings()
    {
        WidthInMeters = 1,
        Color = Color.white,
        Alpha = 1f,
        TexelAspect = 1f,
        Visible = true,
        HighQuality = false,
    };
    public bool reportDebug = false;
    private Overlay_Unity_Settings lastSettings = new Overlay_Unity_Settings();

    void Update()
    {
        if (overlay == null && createWhenReady && OVRLay.OVR.StartedUp)
        {
            overlay = new OVRLay.OVRLay(overlayName, overlayKey, isDashboardOverlay, !createWhenReady);
            overlayCreated = overlay.Created;

            return;
        }

        if (overlay.Created)
        {
            if (!lastSettings.Equals(settings))
            {
                ApplySettings();
                lastSettings = settings;
            }

            PollForEvents();
        }

        if (reportDebug)
        {
            var curPos = (new OVRLay.Utility.RigidTransform(overlay.TransformAbsolute));

            Debug.Log(curPos.pos);
            Debug.Log(curPos.rot.eulerAngles);
            Debug.Log("Is Visible: " + overlay.Visible);

            reportDebug = false;
        }
    }

    void OnDestroy()
    {
        if (overlay.Created)
            overlay.DestroyOverlay();

        overlay = null;
    }

    void OnEnable()
    {
        if (overlay != null && overlay.Created && settings.Visible)
            overlay.Visible = true;
    }

    void OnDisable()
    {
        if (overlay != null && overlay.Created)
            overlay.Visible = false;
    }

    public void CreateOverlay()
    {
        if (!overlay.Created)
        {
            overlay.CreateOverlay();
            overlayCreated = true;
        }
    }

    public void DestroyOverlay()
    {
        if (overlay.Created)
        {
            overlay.DestroyOverlay();
            overlayCreated = false;
        }
    }

    public void ApplySettings()
    {
        if (lastSettings.WidthInMeters != settings.WidthInMeters)
            overlay.WidthInMeters = settings.WidthInMeters;

        if (lastSettings.Color != settings.Color)
            overlay.Color = settings.Color;

        if (lastSettings.Alpha != settings.Alpha)
            overlay.Alpha = settings.Alpha;

        if (lastSettings.TexelAspect != settings.TexelAspect)
            overlay.TexelAspect = settings.TexelAspect;

        if (lastSettings.Visible != settings.Visible)
            overlay.Visible = settings.Visible;

        if (lastSettings.HighQuality != settings.HighQuality)
            overlay.HighQuality = settings.HighQuality;
    }

    public void PollForEvents()
    {
        if (overlay.Created)
            overlay.UpdateEvents();
    }
}

