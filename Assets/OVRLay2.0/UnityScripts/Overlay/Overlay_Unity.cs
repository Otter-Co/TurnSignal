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
    public bool reportDebug = false;
    public bool applySettingsEveryUpdate = false;

    public Overlay_Unity_Settings settings = new Overlay_Unity_Settings()
    {
        WidthInMeters = 1,
        Color = Color.white,
        Alpha = 1f,
        TexelAspect = 1f,
        Visible = true,
        HighQuality = false,
    };

    private Overlay_Unity_Settings lastSettings = new Overlay_Unity_Settings();

    void Update()
    {
        if (overlay == null && createWhenReady && OVRLay.OVR.StartedUp)
        {
            overlay = new OVRLay.OVRLay(overlayName, overlayKey, isDashboardOverlay, !createWhenReady);
            overlayCreated = overlay.Created;

            return;
        }

        if (overlay != null && overlay.Ready)
        {
            if (applySettingsEveryUpdate || !lastSettings.Equals(settings))
                ApplySettings();

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
        if (overlay != null && overlay.Ready)
            overlay.DestroyOverlay();

        overlay = null;
    }

    void OnEnable()
    {
        if (overlay != null && overlay.Ready && settings.Visible)
            overlay.Visible = true;
    }

    void OnDisable()
    {
        if (overlay != null && overlay.Ready)
            overlay.Visible = false;
    }

    public void CreateOverlay()
    {
        if (overlay != null && !overlay.Ready)
        {
            overlay.CreateOverlay();
            overlayCreated = true;
        }
    }

    public void DestroyOverlay()
    {
        if (overlay != null && overlay.Ready)
        {
            overlay.DestroyOverlay();
            overlayCreated = false;
        }
    }

    public void ApplySettings()
    {
        if (overlay == null)
            return;

        overlay.WidthInMeters = settings.WidthInMeters;
        overlay.Color = settings.Color;
        overlay.Alpha = settings.Alpha;
        overlay.TexelAspect = settings.TexelAspect;
        overlay.Visible = settings.Visible;
        overlay.HighQuality = settings.HighQuality;

        lastSettings = settings;
    }

    public void PollForEvents()
    {
        if (overlay != null && overlay.Ready)
            overlay.UpdateEvents();
    }
}

