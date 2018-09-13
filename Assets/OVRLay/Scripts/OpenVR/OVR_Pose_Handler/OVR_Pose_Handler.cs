using System.Collections;
using UnityEngine;
using Valve.VR;

public class OVR_Pose_Handler
{
    static private OVR_Pose_Handler _instance;
    static public OVR_Pose_Handler instance
    {
        get
        {
            if (_instance == null)
                _instance = new OVR_Pose_Handler();

            return _instance;
        }
    }

    static private OVR_Handler OVR { get { return OVR_Handler.instance; } }

    static private CVRSystem System { get { return OVR.VRSystem; } }
    static private CVRCompositor Compositor { get { return OVR.Compositor; } }

    static private bool CompExists { get { return Compositor != null; } }
    static private bool SysExists { get { return System != null; } }

    public ETrackingUniverseOrigin trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
    public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
    public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

    public uint hmdIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;
    public uint rightIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
    public uint leftIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

    public bool rightActive { get { return rightIndex != OpenVR.k_unTrackedDeviceIndexInvalid; } }
    public bool leftActive { get { return leftIndex != OpenVR.k_unTrackedDeviceIndexInvalid; } }

    public void UpdatePoses()
    {
        if (!CompExists || !SysExists)
            return;

        rightIndex = System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
        leftIndex = System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);

        Compositor.GetLastPoses(poses, gamePoses);
    }

    public Quaternion GetPosRotation(uint ind)
    {
        var pose = new OVR_Utils.RigidTransform(poses[ind].mDeviceToAbsoluteTracking);
        var mat = Matrix4x4.TRS(pose.pos, pose.rot, Vector3.one);
        var rot = OVR_Utils.GetRotation(mat);

        return rot;
    }

    public void SetTransformToTrackedDevice(Transform t, uint ind, bool skipPos = false, bool skipRot = false)
    {
        var pose = new OVR_Utils.RigidTransform(poses[ind].mDeviceToAbsoluteTracking);

        if (!skipPos)
            t.position = pose.pos;

        if (!skipRot)
            t.rotation = pose.rot;
    }

    public TrackedDevicePose_t GetRawTrackedMatrix(uint ind)
    {
        return poses[ind];
    }
}