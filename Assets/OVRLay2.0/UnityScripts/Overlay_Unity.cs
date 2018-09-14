using System;
using UnityEngine;
using Valve.VR;

public class Overlay_Unity : MonoBehaviour
{
    public static readonly VRTextureBounds_t TextureBounds = new VRTextureBounds_t()
    {
        uMin = 0,
        vMin = 1,
        uMax = 1,
        vMax = 0
    };

    [Header("Overlay State")]
    public bool overlayCreated = false;
    [Space(10)]
    [Header("Overlay Setup Info")]
    public string overlayName = "Unity Overlay";
    public string overlayKey = "unity-overlay";
    [Space(10)]
    [Header("Overlay Creation Settings")]
    public bool createOnStart = true;
    [Space(10)]
    [Header("Overlay General Settings")]
    public Overlay_Unity_Settings settings = new Overlay_Unity_Settings()
    {
        WidthInMeters = 1,
        Color = Color.white,
        Alpha = 1f,
        TexelAspect = 0f,
        Visible = true,
        HighQuality = false,
    };
    [Space(10)]

    public OVRLay.OVRLay.D_OnDashboardChange OnDashboardChange = (active) => { };
    public OVRLay.OVRLay.D_OnFocusChange OnFocusChange = (hasFocus) => { };
    public OVRLay.OVRLay.D_OnVisibilityChange OnVisibilityChange = (visible) => { };
    public OVRLay.OVRLay.D_OnKeyboardDone OnKeyboardDone = () => { };
    public OVRLay.OVRLay.D_OnKeyboardClose OnKeyboardClose = () => { };
    public OVRLay.OVRLay.D_OnKeyboardInput OnKeyboardInput = (m, f) => { };


    [HideInInspector]
    public OVRLay.OVRLay overlay;
    private Overlay_Unity_Settings lastSettings = new Overlay_Unity_Settings()
    {
        TexelAspect = 0f,
        Visible = true,
        HighQuality = false,
    };

    void Start()
    {
        overlay = new OVRLay.OVRLay(overlayName, overlayKey, !createOnStart);

        overlay.OnDashboardChange += OnDashboardChange;
        overlay.OnFocusChange += OnFocusChange;
        overlay.OnVisibilityChange += OnVisibilityChange;

        overlay.OnKeyboardDone += OnKeyboardDone;
        overlay.OnKeyboardClose += OnKeyboardClose;
        overlay.OnKeyboardInput += OnKeyboardInput;

        overlayCreated = overlay.Created;
    }

    void OnDestroy()
    {
        if (overlay.Created)
            overlay.DestroyOverlay();

        overlay = null;
    }

    void OnEnable()
    {
        if (overlay.Created)
            overlay.Visible = true;
    }

    void OnDisable()
    {
        if (overlay.Created)
            overlay.Visible = false;
    }

    void Update()
    {
        if (overlay.Created && !lastSettings.Equals(settings))
        {
            ApplySettings();
            lastSettings = settings;
            Debug.Log("Settings Changed!");
        }
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