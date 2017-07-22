using System.Collections;
using UnityEngine;

using System.Runtime.InteropServices;
using Valve.VR;

public class OpenVR_Pose_Handler
{
    public ETrackingUniverseOrigin trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
    public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
    public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

	public uint hmdIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;
	public uint rightIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
	public uint leftIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

    public void UpdatePoses()
    {
        var compositor = OpenVR.Compositor;
		var system = OpenVR.System;

        if(compositor == null)
            return;

        compositor.GetLastPoses(poses, gamePoses);

		rightIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
		leftIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
    }
}