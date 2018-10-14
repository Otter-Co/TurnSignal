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
    [System.Serializable]
    public struct Overlay_Unity_Flags
    {
        public bool Flag_Curved;
        public bool Flag_RGSS4X;
        public bool Flag_NoDashboardTab;
        public bool Flag_AcceptsGamepadEvents;
        public bool Flag_ShowGamepadFocus;
        public bool Flag_SendVRScrollEvents;
        public bool Flag_SendVRTouchpadEvents;
        public bool Flag_ShowTouchPadScrollWheel;
        public bool Flag_TransferOwnershipToInternalProcess;
        public bool Flag_SideBySide_Parallel;
        public bool Flag_SideBySide_Crossed;
        public bool Flag_Panorama;
        public bool Flag_StereoPanorama;
        public bool Flag_SortWithNonSceneOverlays;
        public bool Flag_VisibleInDashboard;
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

    public Overlay_Unity_Flags flags = new Overlay_Unity_Flags();

    private Overlay_Unity_Settings lastSettings = new Overlay_Unity_Settings();
    private Overlay_Unity_Flags lastFlags = new Overlay_Unity_Flags();

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

            if (!lastFlags.Equals(flags))
                ApplyFlags();

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

    public void ApplyFlags()
    {
        if (overlay == null)
            return;

        overlay.Flag_Curved = flags.Flag_Curved;
        overlay.Flag_RGSS4X = flags.Flag_RGSS4X;
        overlay.Flag_NoDashboardTab = flags.Flag_NoDashboardTab;
        overlay.Flag_AcceptsGamepadEvents = flags.Flag_AcceptsGamepadEvents;
        overlay.Flag_ShowGamepadFocus = flags.Flag_ShowGamepadFocus;
        overlay.Flag_SendVRScrollEvents = flags.Flag_SendVRScrollEvents;
        overlay.Flag_SendVRTouchpadEvents = flags.Flag_SendVRTouchpadEvents;
        overlay.Flag_ShowTouchPadScrollWheel = flags.Flag_ShowTouchPadScrollWheel;
        overlay.Flag_TransferOwnershipToInternalProcess = flags.Flag_TransferOwnershipToInternalProcess;
        overlay.Flag_SideBySide_Parallel = flags.Flag_SideBySide_Parallel;
        overlay.Flag_SideBySide_Crossed = flags.Flag_SideBySide_Crossed;
        overlay.Flag_Panorama = flags.Flag_Panorama;
        overlay.Flag_StereoPanorama = flags.Flag_StereoPanorama;
        overlay.Flag_SortWithNonSceneOverlays = flags.Flag_SortWithNonSceneOverlays;
        overlay.Flag_VisibleInDashboard = flags.Flag_VisibleInDashboard;

        lastFlags = flags;
    }

    public void PollForEvents()
    {
        if (overlay != null && overlay.Ready)
            overlay.UpdateEvents();
    }
}

