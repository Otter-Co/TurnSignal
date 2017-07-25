using System.Collections;
using UnityEngine;
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
    public bool OpenVRConnected { get { return (_VRSystem != null); } }

    private CVRSystem _VRSystem;
    public CVRSystem VRSystem { get { return _VRSystem; } }

    private CVRCompositor _Compositor;
    public CVRCompositor Compositor { get { return _Compositor; } }

    private CVRChaperone _Chaperone;
    public CVRChaperone Chaperone { get { return _Chaperone; } }

    private CVRChaperoneSetup _ChaperoneSetup;
    public CVRChaperoneSetup ChaperoneSetup { get { return _ChaperoneSetup; } }

    private CVROverlay _Overlay;
    public CVROverlay Overlay { get { return _Overlay; } }

    private CVRSettings _Settings;
    public CVRSettings Settings { get { return _Settings; } }

    private CVRApplications _Applications;
    public CVRApplications Applications { get { return _Applications; } }


    private EVRApplicationType _applicationType = EVRApplicationType.VRApplication_Background;
    public EVRApplicationType applicationType { get { return _applicationType; } }
    

    private OVR_Pose_Handler _poseHandler;
    public OVR_Pose_Handler poseHandler 
    { 
        get 
        { 
            if(_poseHandler == null)
                _poseHandler = OVR_Pose_Handler.instance;

            return _poseHandler; 
        }
    }

    private OVR_Overlay_Handler _overlayHandler;
    public OVR_Overlay_Handler overlayHandler 
    { 
        get 
        { 
            if(_overlayHandler == null)
                _overlayHandler = OVR_Overlay_Handler.instance;

            return _overlayHandler; 
        } 
    }


    private EVRInitError error = EVRInitError.None;
    private VREvent_t pEvent = new VREvent_t();

    public bool StartupOpenVR()
    {
        _VRSystem = OpenVR.Init(ref error, _applicationType);

        bool result = !ErrorCheck(error);
        
        if(result)
            GetOpenVRExistingInterfaces(); // GetOpenVRInterfaces();

        return result;
    }
    public void GetOpenVRExistingInterfaces()
    {
        // _VRSystem = OpenVR.System;
        _Compositor = OpenVR.Compositor;
        _Chaperone = OpenVR.Chaperone;
        _ChaperoneSetup = OpenVR.ChaperoneSetup;
        _Overlay = OpenVR.Overlay;
        _Settings = OpenVR.Settings;
        _Applications = OpenVR.Applications;

        HmdColor_t chapColor = new HmdColor_t();
        HmdColor_t outputColor = new HmdColor_t();

        chapColor.r = 0f;

        Chaperone.GetBoundsColor(ref chapColor, 6, 0f, ref outputColor);

        Debug.Log(new Color(chapColor.r, chapColor.g, chapColor.b, chapColor.a));
        Debug.Log(new Color(outputColor.r, outputColor.g, outputColor.b, outputColor.a));
    }

    public bool ShutDownOpenVR()
    {
        _VRSystem = null;

        _Compositor = null;
        _Chaperone = null;
        _ChaperoneSetup = null;
        _Overlay = null;
        _Settings = null;
        _Applications = null;

        overlayHandler.DestroyAllOverlays();
        OpenVR.Shutdown();

        return false;
    }

    public void UpdateAll()
    {
        while(PollNextEvent(ref pEvent))
            DigestEvent(pEvent);
        
        poseHandler.UpdatePoses();
        overlayHandler.UpdateOverlays();
    }

    private bool PollNextEvent(ref VREvent_t pEvent)
    {
        if(VRSystem == null)
            return false;

		var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
		return VRSystem.PollNextEvent(ref pEvent, size);
    }

    private void DigestEvent(VREvent_t pEvent) 
    {
        EVREventType type = (EVREventType) pEvent.eventType;
        switch(type)
        {
            case EVREventType.VREvent_QuitAcknowledged:
                // ShutDownOpenVR();
            break;

            default:
                Debug.Log(type);        
            break;
        }
    }

    private bool ErrorCheck(EVRInitError error)
    {
        bool err = (error != EVRInitError.None);

        if(err)
            Debug.Log("VR Error: " + OpenVR.GetStringForHmdError(error));

        return err;
    }

    ~OVR_Handler()
    {
        Dispose();
    }

    public void Dispose()
    {
        ShutDownOpenVR();
        _instance = null;
    }

    public void SafeDispose()
    {
        if(_instance != null)
            return;
        _instance = null;
    }
}