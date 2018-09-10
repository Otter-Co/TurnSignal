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
    [Space(10)]
    public float diffTest = 0f;
    public float currentProgress = 0f;

    [HideInInspector] public Vector3 initialPoint = Vector3.zero;
    [HideInInspector] public int pointIndex = 0;
    [HideInInspector] public float interiorRadius = 0f;

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
        var curLocPos = transform.position - transform.parent.position;
        if (lastPos == Vector3.zero)
            lastPos = curLocPos;

        activated = Mathf.Abs((transform.parent.position - initialPoint).y) <= floorHandler.activatorDistance;
        diffTest = GetProgressChange(curLocPos, lastPos, floorHandler.centerRadius);
        currentProgress = GetProgressFromPoint(curLocPos, interiorRadius);

        lastPos = curLocPos;

        UpdatePosition();

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

    void UpdatePosition()
    {
        if (activated)
        {
            var originPos = transform.parent.position + initialPoint;
            var newPos = originPos;
            newPos.y = transform.parent.position.y;

            transform.position = newPos;
        }
        else if (transform.localPosition != initialPoint)
            transform.localPosition = initialPoint;

    }

    static float GetProgressChange(Vector3 p1, Vector3 p2, float radius)
    {
        return (GetProgressFromPoint(p2, radius) - GetProgressFromPoint(p1, radius));
    }

    static float GetProgressFromPoint(Vector3 p1, float radius)
    {
        var cos = Mathf.Acos(p1.x / radius);
        var sin = Mathf.Asin(p1.z / radius);

        return (cos + sin) / 2f;
    }
}
