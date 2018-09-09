using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Floor_Handler : MonoBehaviour
{
    [HideInInspector]
    public readonly float INC = Mathf.PI * (3 - Mathf.Sqrt(5f));

    public GameObject hmdObject;
    [Space(10)]
    public GameObject pointTemplate;
    [Space(10)]
    public float centerRadius = 0.2f;
    [Space(10)]
    public int pointCount = 36;
    public float activatorDistance = 0.1f;
    [Space(10)]
    public bool visualDebug = true;
    [Space(10)]
    public GameObject pointHolder;
    [Space(10)]
    public int activePoints = 0;
    public float averageDiff = 0;

    private float lastRadius = 0f;
    private int lastCount = 0;

    private List<Floor_Handler_Point> fhp;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!pointHolder || lastCount != pointCount)
        {
            if (pointHolder)
                DestroyImmediate(pointHolder);

            pointHolder = CreatePoints(pointCount);
            pointHolder.transform.position = hmdObject.transform.position;
            pointHolder.transform.parent = hmdObject.transform;

            fhp = GetChildren(pointHolder).ConvertAll(g => g.GetComponent<Floor_Handler_Point>());

            lastCount = pointCount;
            lastRadius = -centerRadius;
        }

        if (lastRadius != centerRadius)
        {
            PositionPointsOnSphere(
                centerRadius,
                GetChildren(pointHolder)
            );

            lastRadius = centerRadius;
        }
        var selected = fhp.FindAll(f => f.selected);
        var activeP = fhp.FindAll(f => f.activated);

        activePoints = activeP.Count;
        averageDiff = GetAverage(activeP.ConvertAll(f => f.diffTest));
    }

    float GetAverage(List<float> l)
    {
        float combo = 0f;

        foreach (float diff in l)
            combo += diff;

        return combo / (float)l.Count;
    }

    void PositionPointsOnSphere(float radius, List<GameObject> points)
    {
        float offset = 2f / points.Count;

        int i = 0;
        foreach (GameObject obj in points)
        {
            float x = 0,
                y = 0,
                z = 0,
                r = Mathf.Sqrt(1 - Mathf.Pow(((i * offset) - 1) + (offset / 2), 2)),
                p = i * INC;

            x = radius * (Mathf.Cos(p) * r);
            y = radius * ((i * offset) - 1) + (offset / 2);
            z = radius * (Mathf.Sin(p) * r);

            obj.transform.localPosition = new Vector3(x, y, z);
            i++;
        }
    }

    GameObject CreatePoints(int count)
    {
        var ret = new GameObject("PointHolder");

        for (int i = 0; i < count; i++)
        {
            var point = Instantiate(pointTemplate);
            point.name = "Point";
            point.transform.parent = ret.transform;

            Floor_Handler_Point flp = !!point.GetComponent<Floor_Handler_Point>() ? point.GetComponent<Floor_Handler_Point>() : point.AddComponent<Floor_Handler_Point>();
            flp.pointIndex = i;
        }

        return ret;
    }

    List<GameObject> GetChildren(GameObject par)
    {
        var c = par.transform.childCount;
        var ret = new List<GameObject>();

        for (int i = 0; i < c; i++)
            ret.Add(par.transform.GetChild(i).gameObject);

        return ret;
    }
}

public enum TurnDirection
{
    Unset,
    Right,
    Left
}