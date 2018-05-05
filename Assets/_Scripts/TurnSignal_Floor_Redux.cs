using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Valve.VR;

public class TurnSignal_Floor_Redux : MonoBehaviour
{
    public Camera floorRigCamera;

    [Space(10)]

    public Twister_Redux turnObj;

    [Space(10)]

    public bool autoUpdate = true;
    public bool reversed = false;

    [Space(10)]

    public Unity_Overlay overlay;

    [Space(10)]

    public Unity_SteamVR_Handler vrHandler;
    public GameObject hmd;

    [Space(10)]

    public float minDownXBeforeSwitch = 55f;
    public float maxUpXBeforeSwitch = 295f;

    [Space(10)]

    public int maxTurns = 10;

    public float turnProgress = 0f;
    public float rawTurnProgress = 0f;

    [Space(10)]

    public int turns = 0;
    public float rawCombinedTurns = 0f;

    [Space(10)]

    public int angVelTurns = 0;
    public int angPosTurns = 0;

    [Space(5)]

    public float rawAngVelTurns = 0f;
    public float rawAngPosTurns = 0f;

    [Space(10)]

    public float debugAngPos = 0f;
    public float debugAngVel = 0f;

    public Quaternion debugTwist;
    public Quaternion debugSwing;


    [Space(10)]

    public float lastDot = 0f;
    [Space()]

    public bool usingAngularVel = false;
    public float debugX = 0f;


    private float lastAngPos = 0f;

    private Vector3 lastForward;
    private Quaternion lastRot;
    private bool lastRotSet = false;


    // Methods to make UI easier;
    public void ResetRotationTracking()
    {
        rawCombinedTurns = 0f;
    }

    public void AddMaxTurn()
    {
        maxTurns += 1;
    }

    public void SubMaxTurn()
    {
        if (maxTurns - 1 > 0)
            maxTurns -= 1;
    }

    public void SetMaxTurns(float t)
    {
        int turns = (int)t;
        maxTurns = turns;
    }

    void Update()
    {
        if (autoUpdate)
            UpdateFloor();
    }

    public void UpdateFloor()
    {
        UpdateHMDRotation();
        UpdateTurnObj();

        if (overlay)
            overlay.UpdateOverlay();
    }

    void UpdateHMDRotation()
    {
        if (!hmd || !vrHandler || !vrHandler.connectedToSteam)
            return;

        var tp = vrHandler.poseHandler.GetRawTrackedMatrix(vrHandler.poseHandler.hmdIndex);

        if (!tp.bPoseIsValid)
            return;

        // Start Y-Euler Tracking (Old Algorithm) 
        // Don't not touch, but tread carfully.

        Quaternion hmdQ = hmd.transform.rotation;
        Vector3 hmdR = hmd.transform.eulerAngles;

        if (!lastRotSet)
        {
            lastRot = hmdQ;
            lastRotSet = true;
        }

        hmdQ *= Quaternion.Euler(0, hmdR.y, 0);

        lastDot = Quaternion.Dot(hmdQ, lastRot);
        lastRot = hmdQ;

        float angPosY = 180f * (
            -Mathf.Atan2(
                tp.mDeviceToAbsoluteTracking.m2,
                tp.mDeviceToAbsoluteTracking.m10
            ) / (Mathf.PI)
        );

        if (lastAngPos == 0)
            lastAngPos = angPosY;

        var angPosDiff = angPosY - lastAngPos;
        lastAngPos = angPosY;

        if (Mathf.Abs(angPosDiff) >= 340f)
            angPosDiff += angPosDiff > 0f ? -360f : 360f;


        // Start Angular Velocity Tracking

        Vector3 angVelVec = new Vector3(tp.vAngularVelocity.v0, tp.vAngularVelocity.v1, tp.vAngularVelocity.v2);

        // Prepping Magic
        float angPos = angPosDiff;
        float angVel = ((angVelVec.y * Time.deltaTime) / (Mathf.PI * 2f)) * -360f;

        // Writing Some History
        rawAngPosTurns += (angPosDiff);
        rawAngVelTurns += (angVel);

        // -- > Where the Magic Starts
        angPosTurns = (int)(rawAngPosTurns / 360f);
        angVelTurns = (int)(rawAngVelTurns / 360f);

        // Get Dem Bugs
        debugX = hmdR.x;

        // If at breaking X angle, use new algo, if not, use old.
        if (hmdR.x < minDownXBeforeSwitch || hmdR.x > maxUpXBeforeSwitch)
        {
            rawCombinedTurns += angPos;

            if (usingAngularVel)
                usingAngularVel = false;
        }
        else
        {
            rawCombinedTurns += angVel;

            if (!usingAngularVel)
                usingAngularVel = true;
        }

        // < -- Then it Ends.

        rawTurnProgress = (rawCombinedTurns / (360f)) / maxTurns;

        turnProgress = Mathf.Abs(rawTurnProgress);

        turns = (int)((rawCombinedTurns) / (360f));

        // Get ALL Dem Bugs.
        debugAngPos = angPosDiff;
        debugAngVel = (angVelVec.y);

        hmdQ.ToAngleAxis(out decomp_angle, out decomp_dir);
    }

    public Vector3 decomp_dir = Vector3.zero;
    public float decomp_angle = 0f;


    void UpdateTurnObj()
    {
        float raw = rawCombinedTurns / (360f * maxTurns);

        if (reversed)
            raw *= -1f;

        turnObj.twist = raw;
    }

    public Tuple<Quaternion, Quaternion> decomp_swing_twist(Quaternion rot, Vector3 dir)
    {
        Vector3 ra = new Vector3()
        {
            x = rot.x,
            y = rot.y,
            z = rot.z
        };

        Vector3 p = Vector3.Project(ra, dir);

        Quaternion twist = new Quaternion(), swing = new Quaternion();

        twist.Set(p.x, p.y, p.z, rot.w);
        swing = rot * twist;

        return new Tuple<Quaternion, Quaternion>(twist, swing);
    }
}
