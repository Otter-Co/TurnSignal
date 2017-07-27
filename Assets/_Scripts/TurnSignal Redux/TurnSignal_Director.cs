using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

	private TurnSignal_Prefs_Handler prefs;

	private OVR_Handler handler;


	private string manifestPath;

	public bool twistTied = false;
	public float twistOpat = 0f;

	void Start () 
	{
		prefs = GetComponent<TurnSignal_Prefs_Handler>();

		handler = OVR_Handler.instance;

		manifestPath = Application.persistentDataPath + "appmanifest.vrmanifest";
	}

	void Update () 
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
		if(handler == null && handler.Applications == null)
			return;

		string manifestText = appManifest.text;

		if(handler.Applications.IsApplicationInstalled(appKey))
			Debug.Log("App Manifest Already Added!");
		else if(Application.isEditor)
			Debug.Log("Added Vr Manifest: " + manifestText);
		else
		{
			if(!System.IO.File.Exists(manifestPath))
				System.IO.File.WriteAllText(manifestPath, manifestText);
			
			handler.Applications.AddApplicationManifest(manifestPath, false);
			handler.Applications.SetApplicationAutoLaunch(appKey, true);
		}
	}

	public void RemVRManifest()
	{
		if(handler.Applications == null)
			return;

		if(Application.isEditor)	
			Debug.Log("Removed Vr Manifest!");
		else 
		{
			handler.Applications.RemoveApplicationManifest(manifestPath);
		}
	}
}
