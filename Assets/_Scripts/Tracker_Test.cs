using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker_Test : MonoBehaviour
{
    public enum zSides
    {
        A,
        B
    };

    public float backDistance;
    public zSides curSide = zSides.A;

    [Space(10)]

    public float turn = 0f;

    [Space(10)]

    public Vector3 startPos;
    public Vector3 localStartPos;

    public Vector3 lastPos;
    public float distanceFromCenter = 0f;
    public float lossRatio = 0f;


    private float startLocalDist = 0f;

    public LineRenderer lineR;
    private Vector3[] vecs = new Vector3[2];

    void Start()
    {
        startPos = transform.position;
        localStartPos = transform.localPosition;

        startLocalDist = Vector3.Distance(localStartPos, Vector3.zero);

        lineR = gameObject.AddComponent<LineRenderer>();

        var lineW = 0.2f;
        lineR.startWidth = lineR.endWidth = lineW;
        lineR.positionCount = vecs.Length;
    }

    void Update()
    {
        distanceFromCenter = Vector3.Distance(transform.localPosition, Vector3.zero);
        lossRatio = startLocalDist / distanceFromCenter;

        var pos = transform.position;

        pos.y = startPos.y;
        pos.x *= lossRatio;
        
        transform.position = pos;        

        UpdateLine();

        // UpdateSide(newPos);
        // UpdateTurn(newPos);
    }

    void UpdateLine()
    {
        vecs[0] = startPos;
        vecs[1] = transform.position;

        lineR.SetPositions(vecs);
    }

    void UpdateSide(Vector3 newPos)
    {
        if (newPos.z < 0)
            curSide = zSides.A;
        else
            curSide = zSides.B;
    }

    void UpdateTurn(Vector3 newPos)
    {
        var scale = Mathf.Abs(backDistance) / Mathf.Abs(newPos.z);

        var xDiff = lastPos.x - newPos.x;
        var sXDiff = xDiff * scale;

        if (curSide == zSides.A)
            turn += sXDiff;
        else
            turn -= sXDiff;

        lastPos = newPos;
    }
}
