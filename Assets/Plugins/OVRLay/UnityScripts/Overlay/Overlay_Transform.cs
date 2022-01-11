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

    private ETrackingUniverseOrigin lastTransformOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
    private VROverlayTransformType lastTransformType = VROverlayTransformType.VROverlayTransform_Absolute;
    private OpenVR_DeviceTracker.DeviceType lastRelativeDevice = OpenVR_DeviceTracker.DeviceType.Hmd;

    private int lastCustomIndex = 0;

    private Vector3 lastPos = Vector3.zero;
    private Vector3 lastRot = Vector3.zero;
    
    private OVRLay.OVRLay overlay;

    void Update()
    {
        if (overlay == null)
            if ((overlay = GetComponent<Overlay_Unity>()?.overlay) == null)
                return;

        if (overlay.Ready && (
            transform.eulerAngles != lastRot ||
            transform.position != lastPos ||
            transformOrigin != lastTransformOrigin ||
            transformType != lastTransformType ||
            relativeDevice != lastRelativeDevice ||
            customDeviceIndex != lastCustomIndex
            ))
        {
            //lastRot = transform.eulerAngles;
            lastPos = transform.position;
            lastTransformOrigin = transformOrigin;
            lastTransformType = transformType;
            lastCustomIndex = customDeviceIndex;
            lastRelativeDevice = relativeDevice;


            overlay.TransformType = transformType;
            overlay.TransformAbsoluteTrackingOrigin = transformOrigin;
            overlay.TransformTrackedDeviceRelativeIndex = OpenVR_DeviceTracker.GetDeviceIndex(
                relativeDevice,
                (uint)customDeviceIndex
            );

            var mat = OVRLay.Utility.RigidTransform.FromLocal(transform).ToHmdMatrix34();
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