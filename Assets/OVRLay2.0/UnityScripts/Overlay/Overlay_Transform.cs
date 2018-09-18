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

    private Overlay_Unity u_overlay;
    private OVRLay.OVRLay overlay;

    private Vector3 lastPos = Vector3.zero;
    private Vector3 lastRot = Vector3.zero;

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();
            overlay = u_overlay.overlay;

            return;
        }

        if (overlay.Created && (transform.eulerAngles != lastRot || transform.position != lastPos))
        {
            overlay.TransformType = transformType;
            overlay.TransformAbsoluteTrackingOrigin = transformOrigin;
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

            lastRot = transform.eulerAngles;
            lastPos = transform.position;
        }
    }
}