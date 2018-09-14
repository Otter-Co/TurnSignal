using System;
using UnityEngine;
using Valve.VR;
using OVRLay;

public class OpenVR_DeviceTracker : MonoBehaviour
{
    #region Public

    public enum DeviceType
    {
        Hmd,
        RightController,
        LeftController,
        CustomIndex,
    }

    public DeviceType deviceType = DeviceType.Hmd;
    public int customDeviceIndex = 0;
    [Space(10)]
    public bool trackPosition = true;
    public bool trackRotation = true;

    #endregion

    void Update()
    {
        if (OVR.StartedUp && (trackPosition || trackRotation))
        {
            uint devIndex = GetDeviceIndex();

            if (trackPosition)
                transform.position = OVRLay.Pose.GetDevicePosition(devIndex);

            if (trackRotation)
                transform.rotation = OVRLay.Pose.GetDeviceRotation(devIndex);
        }
    }

    uint GetDeviceIndex()
    {
        switch (deviceType)
        {
            default:
            case DeviceType.Hmd:
                return OVRLay.Pose.HmdPoseIndex;
            case DeviceType.RightController:
                return OVRLay.Pose.RightPoseIndex;
            case DeviceType.LeftController:
                return OVRLay.Pose.LeftPoseIndex;
            case DeviceType.CustomIndex:
                return (uint)customDeviceIndex;
        }
    }
}