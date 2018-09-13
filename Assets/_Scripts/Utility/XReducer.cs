using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XReducer : MonoBehaviour
{
    public float speed = 5f;
    public float xMargin = 30f;

    void LateUpdate()
    {
        var targetRot = OVR_Pose_Handler.instance.GetPosRotation(
            OVR_Pose_Handler.instance.hmdIndex
        ).normalized;

        var newRot = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            speed
        );

        var newRV = newRot.eulerAngles;

        transform.rotation = Quaternion.Euler(
            0, newRV.y, 0
        );
    }
}
