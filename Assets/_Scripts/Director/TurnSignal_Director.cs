﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Valve.VR;
using Steamworks;

public partial class TurnSignal_Director : MonoBehaviour 
{
	public string appKey = "";

	[Space(10)]

	public Unity_Overlay floorOverlay;
	public Unity_Overlay menuOverlay;

	[Space(10)]

	public TurnSignal_Floor_Redux floorRig;
	public TurnSignal_Menu_Redux menuRig;

	[Space(10)]

	public float floorOverlayHandScale = 0.2f;
	public bool flipSides = false;
	public Unity_Overlay.OverlayTrackedDevice floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.None;

	[Space(10)]

	public int windowWidth = 800;
    public int windowHeight = 600;

	[Space(10)]

	public int runningFPS = 90;
	public int idleFPS = 5;

	[Space(10)]

	public GameObject hmdO;
	public float floorOverlayHeight = 0f;
	
	public float floorOverlayFollowSpeed = 1.5f;
	public float floorOverlayFollowSpeedRatio = 1.0f;

	

	private TurnSignal_Prefs_Handler prefs;
	private WindowController winC;
	private OVR_Handler handler;
	
	private bool twistTied = false;

	private int targetFPS = 0;
	private int lastFps = 0;

	void Start () 
	{
		if(SteamManager.Initialized)
			Debug.Log("Starting up SteamWorks!");
		else
			Debug.Log("SteamWorks Init Failed!");
		
		targetFPS = idleFPS;

		prefs = GetComponent<TurnSignal_Prefs_Handler>();
		handler = OVR_Handler.instance;

		winC = GetComponent<WindowController>();

		// Some SteamCloud Stuff

		string prefsPath = Application.dataPath + "/../";
		string prefsFileName = "prefs.json";

		prefs.SetFilePath(prefsPath, prefsFileName);
	}

	public void OnApplicationQuit()	 
	{
		 prefs.Save();
	}

	void Update() 
	{
		UpdateFPS();
		UpdateTwistTie();

		UpdateFloorOverlayRot();
		UpdateFloorOverlayPos();
		
		SetWindowSize();
	}

	public void UpdateFPS()
	{
		if(lastFps != targetFPS)
		{
			lastFps = targetFPS;
			Application.targetFrameRate = targetFPS;
		}
	}

	// Ties Opacity to Twist Progress
	public void UpdateTwistTie()
	{
		if(twistTied)
		{
			var oldOpat = prefs.Opacity;
			floorOverlay.opacity = oldOpat * floorRig.turnProgress;
		}
		else if(floorOverlay.opacity != prefs.Opacity)
			floorOverlay.opacity = prefs.Opacity;

	}

	public void UpdateFloorOverlayRot()
	{
		if(hmdO.transform.position.y < floorOverlayHeight || (floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None && flipSides))
		{
			var foT = floorOverlay.transform;

			if(foT.eulerAngles.x != 270f)
				foT.eulerAngles = new Vector3(270f, foT.eulerAngles.y, foT.eulerAngles.z);

			if(floorOverlayDevice == Unity_Overlay.OverlayTrackedDevice.None)
				floorRig.reversed = true;
			else if(flipSides)
				floorRig.reversed = false;
			else
				floorRig.reversed = true;
		} 
		else
		{
			var foT = floorOverlay.transform;

			if(foT.eulerAngles.x != 90f)
				foT.eulerAngles = new Vector3(90f, foT.eulerAngles.y, foT.eulerAngles.z);

			if(floorRig.reversed)
				floorRig.reversed = false;
		}
	}

	public void UpdateFloorOverlayPos()
	{
		var fot = floorOverlay.transform;

		if(!prefs.FollowPlayerHeadset || floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None)
		{
			if(fot.position.x != 0 || fot.position.y != 0 || fot.position.z != 0)
				fot.position = Vector3.zero;
		}
		else if(prefs.FollowPlayerHeadset)
		{
			var newPos = new Vector3(
				hmdO.transform.position.x, 
				floorOverlayHeight, 
				hmdO.transform.position.z
			);

			fot.position = newPos;
		}
	}
	
	// Recursion DOOMSDAY
	public void SetWindowSize(int lvl = 0, int maxLvl = 5)
    {
		if(Screen.width != windowWidth || Screen.height != windowHeight)
        	Screen.SetResolution(windowWidth, windowHeight, false);

        if(Screen.width != windowWidth || Screen.height != windowHeight)
		{
			if(lvl < maxLvl)
                SetWindowSize(lvl + 1, maxLvl);
		}    
    }

	public void OnSteamVRConnect()
	{
		targetFPS = runningFPS;
	}

	public void OnSteamVRDisconnect()
	{
		if(prefs.StartWithSteamVR)
		{
			Debug.Log("Quitting!");

			Application.Quit();
		}
		else
			targetFPS = idleFPS;
	}

	bool ErrorCheck(EVRApplicationError err)
	{
		bool e = err != EVRApplicationError.None;

		if(e)
			Debug.Log("App Error: " + handler.Applications.GetApplicationsErrorNameFromEnum(err));

		return e;
	}
}
