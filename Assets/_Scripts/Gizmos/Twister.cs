using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Twister : MonoBehaviour
{
    [Header("Twist Settings")]
    public float twist = 0;
    [Space(10)]
    public float coilStrength = -0.15f;

    [Header("Line Settings")]
    public LineRenderer baseLine;

    [Header("Shape Settings")]
    public float radius = 1f;
    public float innerCircleRatio = 0.12f;
    public int petalCount = 6;

    [Header("Performance Settings")]
    public int lineResolution = 25;
    public int curvePointCount = 15;

    private GameObject lineHolder;
    private LineRenderer[] lineRends = new LineRenderer[0];

    private float lastTwist = 0f;
    private float lastCoilStrength = 0f;
    private float lastRadius = 0f;
    private float lastInnerCirRad = 0f;

    void Start()
    {
        CreateLineHolder();
    }

    void Update()
    {
        if (lineRends.Length != petalCount)
        {
            lastTwist = twist + 1;
            GenerateLineRenderers();
        }

        if (lastTwist != twist ||
            lastCoilStrength != coilStrength ||
            lastRadius != radius ||
            lastInnerCirRad != innerCircleRatio)
        {
            lastTwist = twist;
            lastCoilStrength = coilStrength;
            lastRadius = radius;
            lastInnerCirRad = innerCircleRatio;

            UpdateLinePoints();
        }
    }

    void UpdateLinePoints()
    {
        var lineP = GetBezierCirclePoints();

        for (int i = 0; i < lineRends.Length; i++)
        {
            float prog = ((float)i / (float)petalCount);

            lineRends[i].transform.localPosition =
                PositionOnCircle(prog) * radius;

            lineRends[i].transform.LookAt(transform.position);

            lineRends[i].positionCount = lineResolution;
            lineRends[i].SetPositions(lineP);
        }
    }

    Vector3 PositionOnCircle(float prog)
    {
        prog = prog * (Mathf.PI * 2);
        return new Vector3(Mathf.Sin(prog), 0, -Mathf.Cos(prog));
    }

    Vector3[] GetBezierCirclePoints()
    {
        Vector3[] curves = new Vector3[curvePointCount];
        curves[0] = new Vector3(0, 0, 0);

        for (int i = 1; i < curvePointCount; i++)
            curves[i] = GetCurvePoint((float)i / (float)curvePointCount);

        Vector3[] ret = new Vector3[lineResolution];

        for (int i = 0; i < lineResolution; i++)
            ret[i] = Bezier(curves, (float)i / (float)lineResolution);

        return ret;
    }

    Vector3 GetCurvePoint(float prog)
    {
        Vector3 offset = new Vector3(0, 0, (radius));

        float localRadius = (radius - (radius * innerCircleRatio)) * (1f - prog) + (radius * innerCircleRatio);
        localRadius += (localRadius * (coilStrength * twist)) * twist;

        return (PositionOnCircle(prog * twist) * localRadius) + offset;
    }

    List<Vector3> bezierOptArray = new List<Vector3>();
    Vector3 Bezier(Vector3[] points, float prog)
    {
        bezierOptArray.Clear();
        bezierOptArray.AddRange(points);

        int indOffset = bezierOptArray.Count;

        while ((indOffset -= 1) > 1)
            for (int i = 0; i < indOffset; i++)
                bezierOptArray[i] = Vector3.Lerp(
                    bezierOptArray[i],
                    bezierOptArray[i + 1],
                    prog
                );

        return bezierOptArray[0];
    }

    void CreateLineHolder()
    {
        var potentialLH = transform.Find("LineHolder");

        if (potentialLH?.gameObject != null)
            lineHolder = potentialLH.gameObject;
        else
            lineHolder = new GameObject("LineHolder");

        lineHolder.transform.parent = transform;

        lineHolder.transform.localPosition = Vector3.zero;
    }

    void GenerateLineRenderers()
    {
        lineRends = new LineRenderer[petalCount];

        if (Application.isEditor && lineHolder != null)
            DestroyImmediate(lineHolder);
        else if (lineHolder != null)
            Destroy(lineHolder);

        CreateLineHolder();

        for (int i = 0; i < petalCount; i++)
        {
            lineRends[i] = Instantiate(baseLine).GetComponent<LineRenderer>();
            lineRends[i].gameObject.name = "Line " + (i + 1);
            lineRends[i].gameObject.SetActive(true);
            lineRends[i].transform.parent = lineHolder.transform;
        }
    }
}