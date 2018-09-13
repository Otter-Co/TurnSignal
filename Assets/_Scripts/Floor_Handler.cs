using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Valve.VR;

public class Floor_Handler : MonoBehaviour
{
    // Methods to make UI easier;
    public void ResetRotationTracking() => currentTurnValue = 0f;
    public void AddMaxTurn() => maxTurns += 1;
    public void SubMaxTurn() => maxTurns = Mathf.Max(1, maxTurns - 1);
    public void SetMaxTurns(float t) => maxTurns = (int)t;


    public Unity_Overlay overlay;
    public GameObject hmd;
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
    private float lastRotation = 0;

    void Update()
    {
        hmd.transform.rotation = GetHMDRotation(hmd.transform);
        currentTurnValue += GetAdjustedDiff(hmd.transform.eulerAngles.y, lastRotation);
        lastRotation = hmd.transform.eulerAngles.y;

        float adjustedTurnValue = currentTurnValue / (360f * maxTurns);
        turnObj.twist = (reversed) ? -adjustedTurnValue : adjustedTurnValue;

        turnProgress = Mathf.Abs(adjustedTurnValue);

        if (overlay)
            overlay.UpdateOverlay();
    }

    float GetAdjustedDiff(float newP, float oldP)
    {
        float baseDiff = newP - oldP;
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
        var newRot = Quaternion.RotateTowards(
                t.rotation,
                OVR_Pose_Handler.instance.GetPosRotation(
                    OVR_Pose_Handler.instance.hmdIndex
                ).normalized,
                followSpeed
        ).eulerAngles;

        return Quaternion.Euler(
            0, newRot.y, 0
        );
    }
}
