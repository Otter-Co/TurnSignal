using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointValid : MonoBehaviour
{
    public Color validColor = Color.blue;
    public Color invalidColor = Color.red;
    [Space(10)]

    public float maxYDist = 0.65f;
    private Renderer renderer;

    // Update is called once per frame
    void Update()
    {
        if (!renderer)
            renderer = GetComponent<Renderer>();

        var newColor = Color.Lerp(validColor, invalidColor, Mathf.Abs(transform.position.y) / maxYDist);

        renderer.material.color = newColor;
    }
}
