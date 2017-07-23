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
	public EVRApplicationType appType = EVRApplicationType.VRApplication_Overlay;

    public OpenVR_Pose_Handler pose_handler;
	public ETextureType textureType;

	public bool openVRInit = false;

	private VREvent_t pEvent;

	public bool Setup()
	{
		if(openVRInit)
			return true;

		if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
			textureType = ETextureType.OpenGL;
		else
			textureType = ETextureType.DirectX;

		pose_handler = new OpenVR_Pose_Handler();

		if( !(openVRInit = OpenVR_Setup()) )
		{
			SafeDispose();
			return false;
		}
		else return true;
	}
	bool OpenVR_Setup()
	{
		EVRInitError error = EVRInitError.None;

		OpenVR.Init(ref error, appType);

		if(CheckErr(error))
			return false;

		// OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
		// if(CheckErr(error)) return false;

		// OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
		// if(CheckErr(error)) return false;

		Debug.Log("OpenVR - Startup!");

		return true;
	}
	
	public void FullUpdate()
	{
		while(PollNextEvent(ref pEvent))
			EventHandler(ref pEvent);
		
		pose_handler.UpdatePoses();
	}

	void EventHandler(ref VREvent_t pEvent)
	{
		// Debug.Log((EVREventType) pEvent.eventType);

		switch((EVREventType) pEvent.eventType)
		{
			case EVREventType.VREvent_QuitAcknowledged:
				ShutdownOpenVR();
			break;
		}
	}

	public bool PollNextEvent(ref VREvent_t pEvent)
	{
		var system = OpenVR.System;
		if (system == null)
			return false;

		var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
		return system.PollNextEvent(ref pEvent, size);
	}

	// Returns True if Error
	bool CheckErr(EVRInitError err)
	{
		if(err != EVRInitError.None)
		{
			ReportError(err);
			return true;
		}
		else
			return false;
	}

	static void ReportError(EVRInitError error)
	{
		Debug.Log(OpenVR.GetStringForHmdError(error));
	}

	public void ShutdownOpenVR()
	{
		OpenVR.Shutdown();
		openVRInit = false;

		Debug.Log("OpenVR - Shutdown!");
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
		ShutdownOpenVR();
		_instance = null;
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

public struct OpenVR_Stats 
{
	public bool hasOpenVRConnection;
	public ETextureType textureType;
}