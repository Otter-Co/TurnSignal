/*
	Large Chunks copied from SteamVR.cs, and others, credit to Valve for base work.
 */

using System.Collections;
using UnityEngine;

using System.Runtime.InteropServices;
using Valve.VR;

public class OpenVR_Handler : System.IDisposable
{
	static private OpenVR_Handler _instance;
	static public OpenVR_Handler instance { 
		get 
		{ 
			if(_instance == null)
				_instance = new OpenVR_Handler();
			
			return _instance;
		} 
	}

    public OpenVR_Pose_Handler pose_handler;
	public ETextureType textureType;

	public bool openVRInit = false;

	public EVRApplicationType appType = EVRApplicationType.VRApplication_Overlay;

	public void Setup()
	{
		if( !(openVRInit = OpenVR_Setup()) )
		{
			SafeDispose();
			return;
		}

        pose_handler = new OpenVR_Pose_Handler();

		if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
			textureType = ETextureType.OpenGL;
		else
			textureType = ETextureType.DirectX;
	}

	bool OpenVR_Setup()
	{
		EVRInitError error = EVRInitError.None;

		OpenVR.Init(ref error, appType);

		if(!CheckErr(error))
			return false;

		//OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);

		if(!CheckErr(error))
			return false;

		//OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);

		if(!CheckErr(error))
			return false;

		return true;
	}
	
	bool CheckErr(EVRInitError err)
	{
		if(err != EVRInitError.None)
		{
			ReportError(err);
			return false;
		}
		else
			return true;
	}

	static void ReportError(EVRInitError error)
	{
		switch (error)
		{
			case EVRInitError.None:
				break;
			case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
				Debug.Log("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
				break;
			case EVRInitError.Init_VRClientDLLNotFound:
				Debug.Log("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
				break;
			case EVRInitError.Driver_RuntimeOutOfDate:
				Debug.Log("SteamVR Initialization Failed!  Make sure device's runtime is up to date.");
				break;
			default:
				Debug.Log(OpenVR.GetStringForHmdError(error));
				break;
		}
	}

    ~OpenVR_Handler()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		System.GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		_instance = null;
		OpenVR.Shutdown();
	}

	// Use this interface to avoid accidentally creating the instance in the process of attempting to dispose of it.
	public static void SafeDispose()
	{
		if (_instance != null)
			_instance.Dispose();
	}

	void OnApplicationExit()
	{
		Dispose(true);
	}
}

public class OpenVR_Pose_Handler
{
    public ETrackingUniverseOrigin trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
    public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
    public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

	public uint hmdIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;
	public uint rightIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
	public uint leftIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

    public void UpdatePoses()
    {
        var compositor = OpenVR.Compositor;
		var system = OpenVR.System;

        if(compositor == null)
            return;

        compositor.GetLastPoses(poses, gamePoses);

		rightIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
		leftIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
    }
}