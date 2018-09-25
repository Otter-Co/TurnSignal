using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace OVRLay
{
    public static class Pose
    {
        public static TrackedDevicePose_t[] devicesPoses =
            new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        public static TrackedDevicePose_t[] gamePoses =
            new TrackedDevicePose_t[0];
        public static Utility.RigidTransform[] lastPoses =
            new Utility.RigidTransform[OpenVR.k_unMaxTrackedDeviceCount];

        public static uint HmdPoseIndex { get; } = OpenVR.k_unTrackedDeviceIndex_Hmd;
        public static uint RightPoseIndex { get; private set; } = OpenVR.k_unTrackedDeviceIndexInvalid;
        public static uint LeftPoseIndex { get; private set; } = OpenVR.k_unTrackedDeviceIndexInvalid;

        public static Quaternion GetDeviceRotation(uint i) => lastPoses[i].rot;
        public static Vector3 GetDevicePosition(uint i) => lastPoses[i].pos;
        public static TrackedDevicePose_t GetDevicePose(uint i) => devicesPoses[i];
        public static Utility.RigidTransform GetRigidT(uint i) => lastPoses[i];

        public static void UpdatePoses()
        {
            RightPoseIndex = OVR.VR_Sys.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
            LeftPoseIndex = OVR.VR_Sys.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);

            OVR.Compositor.GetLastPoses(devicesPoses, gamePoses);

            for (int i = 0; i < devicesPoses.Length; i++)
                if (devicesPoses[i].bPoseIsValid)
                    lastPoses[i] = new Utility.RigidTransform(devicesPoses[i].mDeviceToAbsoluteTracking);
        }
    }
}