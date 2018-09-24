using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class Old_Twister : MonoBehaviour
{
    public bool autoUpdate = true;

    [Space(10)]
    public int armCount = 6;
    public int armPointCount = 15;
    public int armResolution = 25;


    [Space(10)]
    public float outerDiameter = 1f;
    public float innerDiameter = 0.12f;

    public float coilStrength = 0.15f;

    [Space(10)]
    public float twist = 0f;


    private int lastArmCount = 0;
    private int lastArmPoints = 0;
    private int lastArmRes = 0;

    private float lastOD = 0f;
    private float lastID = 0f;
    private float lastCoilS = 0f;
    private float lastTwist = 0f;

    private GameObject baseLine;
    private GameObject lineHolder;

    private LineRenderer[] armLines = new LineRenderer[0];
    private GameObject[] armObjs = new GameObject[0];

    private Vector3[][] points = new Vector3[0][];
    private Vector3[][] linePoints = new Vector3[0][];

    public void SetPetals(float p)
    {
        int petals = (int)p;
        armCount = petals;
    }
    void Update()
    {
        if (!baseLine)
            baseLine = transform.Find("Line").gameObject;

        if (!lineHolder)
        {
            Transform t = transform.Find("LineHolder");
            if (!t)
            {
                lineHolder = new GameObject("LineHolder");
                lineHolder.transform.parent = transform;
                lineHolder.transform.localPosition = Vector3.zero;

                lastArmCount = 0;
            }
            else
                lineHolder = t.gameObject;
        }

        if (lastArmCount != armCount ||
            lastArmPoints != armPointCount ||
            lastArmRes != armResolution)
        {
            UpdateArmCount();
            UpdateArmPointCount();
        }

        if (Changed())
        {
            SaveChange();
            UpdateArms();
        }
    }

    bool Changed()
    {
        return (lastOD != outerDiameter) ||
            (lastID != innerDiameter) ||
            (lastCoilS != coilStrength) ||
            (lastTwist != twist);
    }

    void SaveChange()
    {
        lastArmCount = armCount;
        lastArmPoints = armPointCount;
        lastArmRes = armResolution;

        lastOD = outerDiameter;
        lastID = innerDiameter;
        lastCoilS = coilStrength;
        lastTwist = twist;
    }

    void UpdateArms()
    {
        for (int i = 0; i < armLines.Length; i++)
        {
            if (i >= points.Length)
            {
                lastArmPoints = 0;
                return;
            }

            GameObject armO = armObjs[i];
            LineRenderer arm = armLines[i];
            Vector3[] armPoints = points[i];
            Vector3[] linePoint = linePoints[i];

            float prog = (float)i / (float)armLines.Length;

            armO.transform.localPosition = GetPosOnCircle(Vector3.zero, outerDiameter, prog);
            armPoints[armPoints.Length - 1] = -armO.transform.localPosition;

            UpdateArmPoints(armPoints, prog, -armO.transform.localPosition);
            UpdateLinePoints(linePoint, armPoints);

            arm.positionCount = armResolution;
            arm.SetPositions(linePoint);
        }
    }

    void UpdateArmPoints(Vector3[] armPoints, float prog, Vector3 origin)
    {
        armPoints[0] = Vector3.zero;
        for (int x = 1; x < armPoints.Length - 1; x++)
        {
            float armProg = (float)x / (float)armPoints.Length;

            float cirRadius;
            cirRadius = ((outerDiameter - innerDiameter) * (1f - armProg)) + innerDiameter;
            cirRadius += (cirRadius * (coilStrength * twist)) * twist;

            float cirProg = prog + (armProg * twist);

            GetPosOnCircle(origin, cirRadius, cirProg, ref armPoints[x]);
        }
    }

    void UpdateLinePoints(Vector3[] linePoint, Vector3[] armPoints)
    {
        for (int i = 0; i < linePoint.Length; i++)
            linePoint[i] = Bezier(armPoints, ((float)i / (float)linePoint.Length));
    }

    void UpdateArmCount()
    {
        GameObject[] oldLineA = armObjs;

        for (int i = 0; i < oldLineA.Length; i++)
            DestroyImmediate(oldLineA[i]);

        oldLineA = null;

        GameObject[] newLineOA = new GameObject[armCount];
        LineRenderer[] newLineA = new LineRenderer[armCount];

        if (Application.isEditor)
            DestroyImmediate(lineHolder);
        else
            Destroy(lineHolder);

        lineHolder = new GameObject("LineHolder");
        lineHolder.transform.parent = transform;
        lineHolder.transform.localPosition = Vector3.zero;

        baseLine.SetActive(true);

        for (int i = 0; i < armCount; i++)
        {
            var go = newLineOA[i] = Instantiate(baseLine, lineHolder.transform);

            go.name = "Arm " + i;
            go.transform.localPosition = Vector3.zero;
            go.transform.LookAt(transform);

            newLineA[i] = go.GetComponent<LineRenderer>();
        }

        baseLine.SetActive(false);

        armLines = newLineA;
        armObjs = newLineOA;
    }

    void UpdateArmPointCount()
    {
        points = new Vector3[armCount][];
        linePoints = new Vector3[armCount][];

        for (int i = 0; i < armLines.Length; i++)
        {
            var armA = points[i] = new Vector3[armPointCount];
            var lineA = linePoints[i] = new Vector3[armResolution];

            for (int x = 0; x < armA.Length; x++)
                armA[x] = Vector3.zero;

            for (int y = 0; y < lineA.Length; y++)
                lineA[y] = Vector3.zero;
        }
    }

    Vector3 GetPosOnCircle(Vector3 origin, float radius, float progress)
    {
        float prog = progress * (Mathf.PI * 2f);
        return new Vector3(
            origin.x + Mathf.Sin(prog) * radius,
            0f,
            origin.z + Mathf.Cos(prog) * radius
        );
    }

    void GetPosOnCircle(Vector3 origin, float radius, float progress, ref Vector3 saveVec)
    {
        float prog = progress * (Mathf.PI * 2f);
        saveVec.x = origin.x + Mathf.Sin(prog) * radius;
        saveVec.z = origin.z + Mathf.Cos(prog) * radius;
    }

    Vector3 BezierNOpt(Vector3[] pN, float prog, Vector3[] wrkCpy = null, int indOff = -1)
    {
        if (wrkCpy == null)
        {
            indOff = pN.Length;
            wrkCpy = new Vector3[pN.Length];
            pN.CopyTo(wrkCpy, 0);
        }

        if (indOff == 1)
            return wrkCpy[0];

        for (int i = 0; i < indOff - 1; i++)
            wrkCpy[i] = Vector3.Lerp(wrkCpy[i], wrkCpy[i + 1], prog);

        return BezierNOpt(null, prog, wrkCpy, indOff - 1);
    }

    Vector3 Bezier(Vector3[] points, float prog)
    {
        var workCopy = new List<Vector3>(points).ToArray();
        int indOffset = workCopy.Length;

        while ((indOffset -= 1) > 1)
            for (int i = 0; i < indOffset; i++)
                workCopy[i] = Vector3.Lerp(
                    workCopy[i],
                    workCopy[i + 1],
                    prog
                );

        return workCopy[0];
    }
}
