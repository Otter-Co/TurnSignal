using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Steamworks;

public class TurnSignal_Steam_Handler : MonoBehaviour 
{
	public bool connectedToSteam = false;

	public void StartUp() 
	{
		if(connectedToSteam)
			return;
	}

	public void ShutDown()
	{

	}
}
