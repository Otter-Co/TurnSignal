using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public class OVR_Overlay_Handler
{
    private static OVR_Overlay_Handler _instance;
    public static OVR_Overlay_Handler instance 
    {
        get 
        { 
            if(_instance == null)
                _instance = new OVR_Overlay_Handler();

            return _instance;
        }
    }

    private HashSet<OVR_Overlay> overlays = new HashSet<OVR_Overlay>();

    public OVR_Overlay_Handler()
    {

    }

    public bool RegisterOverlay(OVR_Overlay overlay)
    {
        return overlays.Add(overlay);
    }

    public bool DeregisterOverlay(OVR_Overlay overlay)
    {
        return overlays.Remove(overlay);
    }

    public void DestroyAllOverlays()
    {
        foreach(OVR_Overlay overlay in overlays)
        {
            overlay.DestroyOverlay();
        }
    }
}

public class OVR_Overlay 
{    
    public string overlayName 
    {
        get { return _overlayName; }
        set { _overlayName = value; }
    }
    private string _overlayName = "OpenVR Overlay";

    public string overlayKey 
    {
        get {return _overlayKey; }
        set { _overlayKey = value; }
    }
    private string _overlayKey = "open_vr_overlay";

    public ulong overlayHandle { get { return _overlayHandle; } }
    private ulong _overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
    

    public bool created { get { return _created; } }
    private bool _created = false;
    

    public bool focus { get { return _focus; } }
    private bool _focus = false;
    

    public bool hidden { get { return _hidden; } }
    private bool _hidden = false;
    


    private EVROverlayError error;
    private VREvent_t pEvent;

    public OVR_Overlay()
    {
        var ovrOverlayHandler = OVR_Overlay_Handler.instance;
        ovrOverlayHandler.RegisterOverlay(this);
    }

    ~OVR_Overlay()
    {
        var ovrOverlayHandler = OVR_Overlay_Handler.instance;
        ovrOverlayHandler.DeregisterOverlay(this);
        DestroyOverlay();
    }

    public void UpdateOverlay() 
    {
        while(PollNextOverlayEvent(ref pEvent))
            DigestEvent(ref pEvent);
    }

    public bool HideOverlay() 
    {
        var overlay = OpenVR.Overlay;
        if(overlay == null)
            return true;

        error = overlay.HideOverlay(_overlayHandle);
        return !ErrorCheck(error);
        
    }
    public bool ShowOverlay() 
    {
        var overlay = OpenVR.Overlay;
        if(overlay == null)
            return true;

        error = overlay.ShowOverlay(_overlayHandle);
        return !ErrorCheck(error);
    }

    public bool CreateOverlay()
    {
        var overlay = OpenVR.Overlay;

        if(overlay == null)
            return ( _created = false );

        error = overlay.FindOverlay(_overlayKey, ref _overlayHandle);
        bool overlayFound = !ErrorCheck(error);

        if(overlayFound)
            return (_created = true);
        else
        {
            error = overlay.CreateOverlay(_overlayKey, _overlayName, ref _overlayHandle);
            return ( _created = !ErrorCheck(error) );
        }
    }

    public bool DestroyOverlay()
    {
        var overlay = OpenVR.Overlay;
        if(!_created || overlay == null || _overlayHandle == OpenVR.k_ulOverlayHandleInvalid)
            return true;   

        error = overlay.DestroyOverlay(_overlayHandle);

        if(!ErrorCheck(error))
        {
            _created = false;
            return true;
        }
        else
            return false;
    }

    private bool PollNextOverlayEvent(ref VREvent_t pEvent)
    {
        var overlay = OpenVR.Overlay;

		if (overlay == null)
			return false;

		var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));

		return overlay.PollNextOverlayEvent(_overlayHandle, ref pEvent, size);
    }

    private void DigestEvent(ref VREvent_t pEvent)
    {
        EVREventType eventType = (EVREventType) pEvent.eventType;
        switch(eventType)
        {
            default:
            break;
        }
    }

    private bool ErrorCheck(EVROverlayError error)
    {
        if(error != EVROverlayError.None)
            return true;
        else
            return false;
    }
}