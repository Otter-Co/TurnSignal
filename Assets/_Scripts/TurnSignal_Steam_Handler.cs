using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Steamworks;

public class TurnSignal_Steam_Handler : MonoBehaviour 
{
	public bool enableSteamworks = true;
	public bool connectedToSteam = false;

	public bool StartUp() 
	{
		if(!enableSteamworks || connectedToSteam)
			return true;

		if (!Packsize.Test()) {
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
			return false;
		}

		if (!DllCheck.Test()) {
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
			return false;
		}

		connectedToSteam = SteamAPI.Init();

		if(!connectedToSteam)
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
		else
		{
			SteamFriends.ClearRichPresence();
			
			Debug.Log("SteamWorks Started up!");
		}
			

		return connectedToSteam;
	}

	public void ShutDown()
	{
		if(!connectedToSteam)
			return;

		SteamAPI.Shutdown();
		connectedToSteam = false;

		Debug.Log("SteamWorks Shutting Down!");
	}

	void Update()
	{
		if(connectedToSteam)
			SteamAPI.RunCallbacks();
	}
}
