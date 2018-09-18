using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using OVRLay;

public class Floor_Handler : MonoBehaviour
{
    // Methods to make UI easier;
    public void ResetRotationTracking() => currentTurnValue = 0f;
    public void AddMaxTurn() => maxTurns += 1;
    public void SubMaxTurn() => maxTurns = Mathf.Max(1, maxTurns - 1);
    public void SetMaxTurns(float t) => maxTurns = (int)t;

    public Transform hmd;
    public Transform refT;
    [Space(10)]
    public Camera floorRigCamera;
    [Space(10)]
    public Twister turnObj;
    [Space(10)]
    public bool autoUpdate = true;
    public bool reversed = false;
    [Space(10)]
    public float followSpeed = 5f;
    public int maxTurns = 10;
    [Space(10)]
    public float turnProgress = 0f;
    public float currentTurnValue = 0f;
    [Space(10)]
    public Vector3 curProg = Vector3.zero;

    private Quaternion lastRot;
    private OVRLay.Utility.RigidTransform lastRigidT;

    void Update()
    {
        hmd.rotation = GetHMDRotation(hmd);
        var ea = hmd.eulerAngles;

        curProg.x += Mathf.Abs(ea.x) < 180 ? ea.x : -(360f - ea.x);
        curProg.y += Mathf.Abs(ea.y) < 180 ? ea.y : -(360f - ea.y);
        curProg.z += Mathf.Abs(ea.z) < 180 ? ea.z : -(360f - ea.z);

        currentTurnValue = curProg.x + curProg.y + curProg.z;

        float adjustedTurnValue = currentTurnValue / (360f * maxTurns);
        turnProgress = Mathf.Abs(adjustedTurnValue);
        turnObj.twist = (reversed) ? -adjustedTurnValue : adjustedTurnValue;
    }

    float GetAdjustedDiff(float newP, float oldP)
    {
        float baseDiff = Mathf.Abs(newP) - Mathf.Abs(oldP);
        float absBD = Mathf.Abs(baseDiff);

        if (absBD > 350)
            absBD = Mathf.Abs(360 - absBD);
        else if (absBD > 170)
            absBD = Mathf.Abs(180 - absBD);
        else if (absBD > 70)
            absBD = Mathf.Abs(90 - absBD);

        return (baseDiff > 0f) ? absBD : -(absBD);
    }

    Quaternion GetHMDRotation(Transform t)
    {
        var newHmdRot = OVRLay.Pose.GetDeviceRotation(OVRLay.Pose.HmdPoseIndex);
        var pose = OVRLay.Pose.GetRigidT(OVRLay.Pose.HmdPoseIndex);

        var relDiff = pose.GetInverse() * lastRigidT;

        lastRot = newHmdRot;
        lastRigidT = pose;

        return relDiff.rot.normalized;
    }
}
