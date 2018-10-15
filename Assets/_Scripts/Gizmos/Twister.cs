using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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
    private Vector3[] linePoints = new Vector3[0];

    private int lastPetalCount = 0;
    private int lastLineRes = 0;
    private int lastCurveCount = 0;

    void Start()
    {
        DestroyLineHolder();
    }

    void Update()
    {
        if (
            lineHolder == null ||
            lineRends == null ||
            petalCount != lastPetalCount
        )
            SetPetals(petalCount);

        if (
            lineResolution != lastLineRes ||
            curvePointCount != lastCurveCount
        )
            SetLineCurve(lineResolution, curvePointCount);

        UpdateLinePoints();
    }

    public void SetLineCurve(int lineRes, int curveCount)
    {
        SetPetals(petalCount);

        lineResolution = lastLineRes = lineRes;
        curvePointCount = lastCurveCount = curveCount;
    }

    public void SetPetals(int count)
    {
        DestroyLineHolder();
        lineHolder = CreateLineHolder();
        CreateLineRenderers(count, lineHolder.transform);

        petalCount = lastPetalCount = count;
    }

    void UpdateLinePoints()
    {
        linePoints = GetBezierCirclePoints();

        for (int i = 0; i < lineRends.Length; i++)
        {
            float prog = ((float)i / (float)petalCount);

            lineRends[i].gameObject.transform.localPosition =
                PositionOnCircle(prog) * radius;

            lineRends[i].gameObject.transform.LookAt(transform.position);

            lineRends[i].positionCount = lineResolution;
            lineRends[i].SetPositions(linePoints);
        }
    }

    void CreateLineRenderers(int count, Transform parent)
    {
        lineRends = new LineRenderer[count];
        for (int i = 0; i < count; i++)
            lineRends[i] = CreateLineRenderer(baseLine.gameObject, i, parent);
    }

    void DestroyLineHolder()
    {
        var existing = transform.Find("LineHolder")?.gameObject;

        if (Application.isEditor)
        {
            DestroyImmediate(existing);
            DestroyImmediate(lineHolder);
        }
        else
        {
            Destroy(existing);
            Destroy(lineHolder);
        }
    }

    GameObject CreateLineHolder()
    {
        var lineH = new GameObject("LineHolder");
        lineH.transform.parent = transform;
        lineH.transform.localPosition = Vector3.zero;
        return lineH;
    }


    static LineRenderer CreateLineRenderer(GameObject baseLine, int index, Transform parent)
    {
        var lineRend = Instantiate(baseLine).GetComponent<LineRenderer>();

        lineRend.gameObject.name = "Line " + index;
        lineRend.gameObject.SetActive(true);
        lineRend.gameObject.transform.parent = parent;

        return lineRend;
    }

    Vector3[] GetBezierCirclePoints()
    {
        Vector3[] curves = new Vector3[curvePointCount];

        if (bezierOptArray == null || bezierOptArray.Length != curvePointCount)
            bezierOptArray = new Vector3[curvePointCount];

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

    static Vector3 PositionOnCircle(float prog)
    {
        prog = prog * (Mathf.PI * 2);
        return new Vector3(Mathf.Sin(prog), 0, -Mathf.Cos(prog));
    }

    static Vector3[] bezierOptArray = null;
    static Vector3 Bezier(Vector3[] points, float prog)
    {
        points.CopyTo(bezierOptArray, 0);

        int indOffset = bezierOptArray.Length;
        while ((indOffset -= 1) > 1)
            for (int i = 0; i < indOffset; i++)
                bezierOptArray[i] = Vector3.Lerp(
                    bezierOptArray[i],
                    bezierOptArray[i + 1],
                    prog
                );

        return bezierOptArray[0];
    }

    private struct BezierJob : IJob
    {
        public float progress;

        public NativeArray<Vector3> outputs;

        public void Execute()
        {

        }
    }
}