using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Facepunch.Steamworks;

public class TurnSignal_Steam_Handler : MonoBehaviour 
{
    [Space(10)]
    public uint appID = 0;

    [Space(10)]
    public bool connectedToSteam = false;

	[HideInInspector]
    public Client steamClient;

	public bool StartUp() 
	{
        if(connectedToSteam)
            return true;

		Facepunch.Steamworks.Config.ForUnity( Application.platform.ToString() );
        steamClient = new Client(appID);

        if(steamClient != null)
            connectedToSteam = true;

        if(connectedToSteam)
            Debug.Log("Connected to Steam!");

		return connectedToSteam;
	}

	public void ShutDown()
	{
		if(!connectedToSteam)
			return;

		steamClient.Dispose();
		steamClient = null;

		connectedToSteam = false;

		Debug.Log("SteamWorks Shutting Down!");
	}

	void Update()
	{
		if(connectedToSteam && steamClient != null)
            steamClient.Update();
	}
}
