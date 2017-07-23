using System.Collections;
using UnityEngine;

using System.Runtime.InteropServices;
using Valve.VR;

public class OVR_Handler : System.IDisposable
{
    static private OVR_Handler _instance;
    static public OVR_Handler instance 
    {
        get {
            if(_instance == null)
                _instance = new OVR_Handler();

            return _instance;
        }
    }

    public OVR_Pose_Handler poseHandler;
    public OVR_Overlay_Handler overlayHandler;

    public OVR_Handler()
    {

    }

    public void Dispose()
    {
        
    }

    public void SafeDispose()
    {
        if(_instance != null)
            return;
        _instance = null;
    }
}