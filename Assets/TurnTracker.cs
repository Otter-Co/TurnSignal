using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnTracker : MonoBehaviour
{

    public Transform hmd;

    public Vector3 forwardTarget;

    Vector3[] StickProbe = new Vector3[3];
    float[] StickPointDistances = new float[3];
    int indexOfSmallest;

    float currentAngle;
    float currentTwist;

    public float currentTwistValue;


    public int[] TurnTrackerArray = new int[362];
    public int currentIntAngle;
    public int NumberOfTurns;


    public float LastY = 0;

    public Twister twister;

    public float twistOffSet = 0f;

    public float twistRate = 10f;
    public float MaxTwistForOpacity = 2f;

    //  public Text currentTwistText;
    //  public Text currentIntText;
    //  public Text NumberOfTurnsText;
    //  public Text TurnTotal;

    //  public Text OriginalValueDebug;
    //  public Text OriginalValueDebug2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {


        //Debug Text
  //      OriginalValueDebug.text = hmd.localRotation.eulerAngles.ToString();
  //      OriginalValueDebug2.text = hmd.localRotation.ToString();


        //First we calculate the position of the 3 points, one above your head, one in front of you, and one below your head.
        StickProbe[0] = hmd.transform.position + hmd.up;
        StickProbe[1] = hmd.transform.position + hmd.forward;
        StickProbe[2] = hmd.transform.position - hmd.up;

        //Next we calculate the distance between all the probepoints, and the current location of the forward target. 
        for (int j = 0; j < StickPointDistances.Length; j++) {
            StickPointDistances[j] = (forwardTarget - StickProbe[j]).magnitude;
        }

        float smallest = float.MaxValue;

        // We figure out which probe is the closest, as that is the one that will be in charge of moving the forwardTarget around.
        for (int j = 0; j < StickPointDistances.Length; j++) {
            if (StickPointDistances[j] < smallest) {
                smallest = StickPointDistances[j];
                indexOfSmallest = j;
            }
        }

        //Once we know which probe is currently incharge, we find a new location for the forward target, by finding the closest point 
        //on a circle between the target probe, and the forwardTarget.
        forwardTarget = ClosestPointOnCircle(Vector3.zero, 1f, StickProbe[indexOfSmallest]);
        currentAngle = Angle(new Vector2(forwardTarget.x, forwardTarget.z));
        float targetAngle = Mathf.Repeat(currentAngle - twistOffSet,360);


        int targetYint = Mathf.FloorToInt(targetAngle);

        while (currentIntAngle != targetYint) {
            int upOrDown = Mathf.DeltaAngle(currentIntAngle, targetYint)>=0 ? 1 :-1 ;
            if (upOrDown == -1) {
                TurnTrackerArray[currentIntAngle] -= 1;
            }
            currentIntAngle += upOrDown;
            if (currentIntAngle < 0)
                currentIntAngle += 360;
            else if (currentIntAngle >= 360)
                currentIntAngle = 0;
            if (upOrDown == 1) {
                TurnTrackerArray[currentIntAngle] += 1;
            }
        }
        

  //      currentTwistText.text = currentAngle.ToString();
  //      currentIntText.text = currentIntAngle.ToString();
       
      

        int i = currentIntAngle + 1;
        if (i >= 360)
            i = 0;

        currentTwist = (TurnTrackerArray[i] + currentAngle/360f );
        twister.twist = (-currentTwist +twistOffSet)*twistRate/10f;

  //      NumberOfTurnsText.text = TurnTrackerArray[i].ToString();
  //      TurnTotal.text = currentTwist.ToString();

    }

    public void ZeroRotation() {
        twistOffSet = currentTwist;

    }


    public static float Angle(Vector2 p_vector2) {
        if (p_vector2.x < 0) {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        } else {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
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

            if (indexOfSmallest == i) {
                Gizmos.color = Color.blue;
            } else {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawLine(Vector3.zero, StickProbe[i]);
            Gizmos.DrawSphere(new Vector3(StickProbe[i].x, 0, StickProbe[i].z), 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(forwardTarget, Vector3.one/4f);
        }

    }
}
