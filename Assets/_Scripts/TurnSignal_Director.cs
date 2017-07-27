using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class TurnSignal_Director : MonoBehaviour 
{
	public string appKey = "";

	public TextAsset appManifest;


	[Space(10)]

	public Unity_Overlay floorOverlay;
	public Unity_Overlay menuOverlay;

	[Space(10)]

	public TurnSignal_Floor_Redux floorRig;
	public TurnSignal_Menu_Redux menuRig;

	[Space(10)]

	public int windowWidth = 800;
    public int windowHeight = 600;

	private TurnSignal_Prefs_Handler prefs;

	private OVR_Handler handler;

	[Space(10)]

	public int targetFPS = 90;

	[Space(10)]
	public UnityEvent onUpdate = new UnityEvent();

	private string manifestPath;

	private bool twistTied = false;

	void Start () 
	{
		prefs = GetComponent<TurnSignal_Prefs_Handler>();

		handler = OVR_Handler.instance;
		manifestPath = Application.dataPath + "\\appmanifest.vrmanifest";

		Debug.Log(manifestPath);

		Application.targetFrameRate = targetFPS;
	}

	void Update() 
	{
		onUpdate.Invoke();
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

	public void LinkOpacityWithTwist(bool linked)
	{
		twistTied = linked;
	}

	public void StartWithSteamVR(bool enableStart)
	{
		if(enableStart)
			AddVRManifest();
		else
			RemVRManifest();
	}

	public void AddVRManifest()
	{
		if(handler == null || handler.Applications == null)
			return;

		if(handler.Applications.IsApplicationInstalled(appKey))
			Debug.Log("App Manifest Already Added!");
		else 
		{
			string manifestText = appManifest.text;

			if(!System.IO.File.Exists(manifestPath))
				System.IO.File.WriteAllText(manifestPath, manifestText);

			EVRApplicationError error = EVRApplicationError.None;
			
			error = handler.Applications.AddApplicationManifest(manifestPath, false);
			ErrorCheck(error);

			error = handler.Applications.SetApplicationAutoLaunch(appKey, true);
			ErrorCheck(error);
		}
	}

	public void RemVRManifest()
	{
		if(handler == null || handler.Applications == null)
			return;

		handler.Applications.RemoveApplicationManifest(manifestPath);
	}

	bool ErrorCheck(EVRApplicationError err)
	{
		bool e = err != EVRApplicationError.None;

		if(e)
			Debug.Log("App Error: " + handler.Applications.GetApplicationsErrorNameFromEnum(err));

		return e;
	}

	public void SetWindowSize(int lvl = 0, int maxLvl = 5)
    {
		if(Screen.width != windowWidth || Screen.height != windowHeight)
        	Screen.SetResolution(windowWidth, windowHeight, false);

        if(Screen.width != windowWidth || Screen.height != windowHeight)
            if(lvl < maxLvl)
                SetWindowSize(lvl + 1, maxLvl);
    }
}
