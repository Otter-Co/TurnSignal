using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTracker : MonoBehaviour
{
    public Transform hmdObject;
    public Transform pointHolder;
    public Transform disc;
    [Space(10)]
    public float currentTurnValue = 0f;
    private float lastRotation = 0;

    void Start()
    {

    }

    void Update()
    {
        currentTurnValue += GetAdjustedDiff(hmdObject.eulerAngles.y, lastRotation);
        lastRotation = hmdObject.eulerAngles.y;
    }

    float GetProgressOfPoint(float radius, float walkVal, float flyVal)
    {
        float sin = (180 / Mathf.PI) * Mathf.Asin(flyVal / radius);
        float cos = (180 / Mathf.PI) * Mathf.Acos(walkVal / radius);

        float outSin = (cos > 90f) ? 180f - sin : (sin < 0) ? 360f + sin : sin;
        float outCos = (sin < 0f) ? 360f - cos : cos;

        return ((outSin + outCos) / 2f);
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
}