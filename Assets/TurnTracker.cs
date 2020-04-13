using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnTracker : MonoBehaviour
{

    public Transform hmd;

    public float currentTwistValue;
    public Text currentTwistText;
    public Text currentIntText;
    public Text NumberOfTurnsText;
    public Text TurnTotal;

    public int[] TurnTrackerArray = new int[362];
    public int currentIntAngle;
    public int NumberOfTurns;


    public float LastY = 0;

    public Twister twister;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float targetY = hmd.localRotation.eulerAngles.y;
        int targetYint = Mathf.FloorToInt(targetY);

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
        

        currentTwistText.text = targetY.ToString();
        currentIntText.text = currentIntAngle.ToString();
        NumberOfTurnsText.text = TurnTrackerArray[currentIntAngle].ToString();
        float twistvalue;
        if (TurnTrackerArray[currentIntAngle] >= 0) {
            int i = currentIntAngle + 1;
            if (i >= 360)
                i = 0;

            twistvalue = (TurnTrackerArray[i] + (float)currentIntAngle / 360f);
        } else {
            int i = currentIntAngle + 1;
            if (i >= 360)
                i = 0;
            twistvalue = (TurnTrackerArray[i] + (float)(360 - currentIntAngle) / 360f);
        }

        TurnTotal.text = twistvalue.ToString();
        twister.twist = twistvalue;
    }


}
