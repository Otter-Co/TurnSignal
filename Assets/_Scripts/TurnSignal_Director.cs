using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Valve.VR;
public class TurnSignal_Director : MonoBehaviour 
{
	public string appKey = "";

	[Space(10)]

	public Unity_Overlay floorOverlay;
	public Unity_Overlay menuOverlay;

	[Space(10)]

	public TurnSignal_Floor_Redux floorRig;
	public TurnSignal_Menu_Redux menuRig;

	[Space(10)]

	public int windowWidth = 800;
    public int windowHeight = 600;

	[Space(10)]

	public int runningFPS = 90;
	public int idleFPS = 5;
	

	private TurnSignal_Prefs_Handler prefs;

	private OVR_Handler handler;

	private bool twistTied = false;


	private int targetFPS = 0;
	private int lastFps = 0;

	// Methods for Easy UI.
	public void LinkOpacityWithTwist(bool linked)
	{
		twistTied = linked;
	}
	void Start () 
	{
		Application.targetFrameRate = idleFPS;

		prefs = GetComponent<TurnSignal_Prefs_Handler>();
		handler = OVR_Handler.instance;

		// Some SteamCloud Stuff

		string prefsPath = Application.dataPath + "\\..\\";
		string prefsFileName = "prefs.json";

		prefs.SetFilePath(prefsPath, prefsFileName);

		prefs.Load();
	}

	void Update() 
	{
		if(lastFps != targetFPS)
		{
			lastFps = targetFPS;
			Application.targetFrameRate = targetFPS;
		}

		DirectorUpdate();
		SetWindowSize();
	}

	void DirectorUpdate()
	{
		if(twistTied)
		{
			var oldOpat = prefs.Opacity;
			floorOverlay.opacity = oldOpat * floorRig.turnProgress;
		}
		else if(floorOverlay.opacity != prefs.Opacity)
			floorOverlay.opacity = prefs.Opacity;
	}
	public void SetWindowSize(int lvl = 0, int maxLvl = 5)
    {
		if(Screen.width != windowWidth || Screen.height != windowHeight)
        	Screen.SetResolution(windowWidth, windowHeight, false);

        if(Screen.width != windowWidth || Screen.height != windowHeight)
            if(lvl < maxLvl)
                SetWindowSize(lvl + 1, maxLvl);
    }

	public void OnSteamVRConnect()
	{
		prefs.StartWithSteamVR = GetManifestAutoLaunch();
		menuRig.SetUIValues();

		targetFPS = runningFPS;
	}

	public void OnSteamVRDisconnect()
	{
		if(prefs.StartWithSteamVR)
		{
			Debug.Log("SD:LKFJSD:LKFJSL:KDFJS:KLDFJ:l");
			Debug.Log("Quitting!");
			Application.Quit();
		}

		targetFPS = idleFPS;
	}

	public void SetManifestAutoLaunch(bool autoLaunch)
	{
		if(handler != null && handler.Applications != null)
			handler.Applications.SetApplicationAutoLaunch(appKey, autoLaunch);
	}

	public bool GetManifestAutoLaunch()
	{
		return (handler != null && handler.Applications != null) ? handler.Applications.GetApplicationAutoLaunch(appKey) : false; 
	}

	bool ErrorCheck(EVRApplicationError err)
	{
		bool e = err != EVRApplicationError.None;

		if(e)
			Debug.Log("App Error: " + handler.Applications.GetApplicationsErrorNameFromEnum(err));

		return e;
	}
}
