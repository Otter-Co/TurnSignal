using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_Transform : MonoBehaviour
{
    [Header("Tracked Device Positional Settings")]
    public ETrackingUniverseOrigin transformOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
    public VROverlayTransformType transformType = VROverlayTransformType.VROverlayTransform_Absolute;
    [Space(10)]
    [Header("Settings for Tracking Relative to Device")]
    public OpenVR_DeviceTracker.DeviceType relativeDevice = OpenVR_DeviceTracker.DeviceType.Hmd;
    public int customDeviceIndex;

    private OVRLay.OVRLay overlay;

    void Start()
    {
        overlay = GetComponent<Overlay_Unity>().overlay;
    }

    void Update()
    {
        if (overlay.Created)
        {
            if (overlay.TransformType != transformType)
                overlay.TransformType = transformType;

            if (overlay.TransformAbsoluteTrackingOrigin != transformOrigin)
                overlay.TransformAbsoluteTrackingOrigin = transformOrigin;

            if (overlay.TransformTrackedDeviceRelativeIndex != OpenVR_DeviceTracker.GetDeviceIndex(relativeDevice))
                overlay.TransformTrackedDeviceRelativeIndex = OpenVR_DeviceTracker.GetDeviceIndex(relativeDevice);

            var mat = new OVRLay.Utility.RigidTransform(transform.position, transform.rotation).ToHmdMatrix34();

            switch (transformType)
            {
                default:
                case VROverlayTransformType.VROverlayTransform_Absolute:
                    overlay.TransformAbsolute = mat;
                    break;
                case VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative:
                    overlay.TransformTrackedDeviceRelative = mat;
                    break;
            }
        }
    }
}