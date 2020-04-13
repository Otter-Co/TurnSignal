using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBlock : MonoBehaviour
{

    public Transform hmd;
    public Transform movingCube;
    public float stickLenghts = 3f;
    public float circleRadius = 3f;
    public float TopDistance, FrontDistance, BottonDistance;
    public Vector3[] StickProbe = new Vector3[3];
    public float[] StickPointDistances = new float[3];
    int indexOfSmallest;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StickProbe[0] = hmd.transform.position + hmd.up * stickLenghts;
        StickProbe[1] = hmd.transform.position + hmd.forward * stickLenghts;
        StickProbe[2] = hmd.transform.position - hmd.up * stickLenghts;

        for (int i = 0; i < StickPointDistances.Length; i++) {
            StickPointDistances[i] = (movingCube.position - StickProbe[i] ).magnitude;
        }

        float smallest=float.MaxValue;
 

        for (int i = 0; i < StickPointDistances.Length; i++) {
            if (StickPointDistances[i] < smallest) {
                smallest = StickPointDistances[i];
                indexOfSmallest = i;
            }
        }

        movingCube.transform.position = ClosestPointOnCircle(Vector3.zero, circleRadius, StickProbe[indexOfSmallest]);
    }

    // This is for a 2d circle that is always flat on the ground.
    Vector3 ClosestPointOnCircle(Vector3 circleOrigin, float radius, Vector3 InputPoint) {
        InputPoint.y = circleOrigin.y; // Flatten the input.

        Vector3 Ray = InputPoint - circleOrigin;
        Ray.y = 0;
        Ray = Ray.normalized;

        return circleOrigin + Ray * radius;
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < StickProbe.Length; i++) {

            if(indexOfSmallest == i) {
                Gizmos.color = Color.blue;
            } else {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawLine(Vector3.zero, StickProbe[i]);
            Gizmos.DrawSphere(new Vector3(StickProbe[i].x, 0, StickProbe[i].z), 0.3f);
        }

      
       

    }
}
