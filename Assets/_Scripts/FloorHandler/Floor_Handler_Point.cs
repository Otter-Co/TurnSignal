using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Floor_Handler_Point : MonoBehaviour
{
    public Floor_Handler floorHandler;
    [Space(10)]
    public Material inactiveMat;
    public Material activeMat;
    [Space(10)]
    public bool activated = false;
    public bool selected = false;
    [Space(10)]
    public float diffTest = 0;
    public TurnDirection currentDir = TurnDirection.Unset;

    [HideInInspector]
    public int pointIndex = 0;

    private Vector3 lastPos;
    private Renderer rend;



    void Start()
    {
        rend = GetComponent<Renderer>();

        if (!floorHandler.visualDebug)
            rend.enabled = false;
        else
            rend.sharedMaterial = inactiveMat;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPos == Vector3.zero)
            lastPos = transform.position;

        var curLocPos = transform.position - transform.parent.position;
        activated = Mathf.Abs(curLocPos.y) <= floorHandler.activatorDistance;

        var cleanCur = transform.position;
        var cleanLast = lastPos;

        diffTest = GetCurrentChange();
        currentDir = GetDirection(cleanCur, cleanLast);

        lastPos = transform.position;

        if (floorHandler.visualDebug)
            UpdateVisual();
        else if (rend.enabled)
            rend.enabled = false;
    }

    void UpdateVisual()
    {
        if (!rend.enabled)
            rend.enabled = true;

        if (activated && rend.sharedMaterial != activeMat)
            rend.sharedMaterial = activeMat;
        else if (!activated && rend.sharedMaterial != inactiveMat)
            rend.sharedMaterial = inactiveMat;
    }

    public float GetCurrentChange()
    {
        var cleanCur = transform.position;
        var cleanLast = lastPos;

        switch (GetDirection(cleanCur, cleanLast))
        {
            case TurnDirection.Right:
                return Vector3.Distance(cleanCur, cleanLast);
            case TurnDirection.Left:
                return -Vector3.Distance(cleanCur, cleanLast);
            default:
                return 0;
        }
    }

    TurnDirection GetDirection(Vector3 cur, Vector3 old)
    {
        var diff = old - cur;

        TurnDirection q = TurnDirection.Unset;

        float lX = diff.x, lZ = diff.z;

        if (lX > 0 && lZ > 0)
            q = TurnDirection.Left;
        else if (lX > 0 && lZ < 0)
            q = TurnDirection.Right;
        else if (lX < 0 && lZ > 0)
            q = TurnDirection.Right;
        else if (lX < 0 && lZ < 0)
            q = TurnDirection.Left;

        return q;
    }
}
