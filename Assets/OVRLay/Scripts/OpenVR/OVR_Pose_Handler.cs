using System.Collections;
using UnityEngine;

using System.Runtime.InteropServices;
using Valve.VR;

public class OVR_Pose_Handler 
{
    static private OVR_Pose_Handler _instance;
    static public OVR_Pose_Handler instance 
    {
        get {
            if(_instance == null)
                _instance = new OVR_Pose_Handler();

            return _instance;
        }
    }
}